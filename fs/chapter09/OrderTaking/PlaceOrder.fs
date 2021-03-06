module OrderTaking.PlaceOrder

open SimpleTypes
open PublicTypes
open CompoundTypes

// ------------------------------------
// the workflow itself, without effects

type PlaceOrderWorkflow =
    UnvalidatedOrder -> PlaceOrderEvent list


// ======================================================
// Section 1 : Define each step in the workflow using types
// ======================================================

// ---------------------------
// Validation step
// ---------------------------

type CheckedAddress = CheckedAddress of UnvalidatedAddress

type ValidatedOrderLine = {
    OrderLineId : OrderLineId
    ProductCode : ProductCode
    Quantity : OrderQuantity
}

type ValidatedOrder = {
    OrderId : OrderId
    CustomerInfo : CustomerInfo
    ShippingAddress : Address
    BillingAddress : Address
    Lines : ValidatedOrderLine list
}

type CheckProductCodeExists =
    ProductCode -> bool

type CheckAddressExists =
    UnvalidatedAddress -> CheckedAddress

type ValidateOrder =
    CheckProductCodeExists
        -> CheckAddressExists
        -> UnvalidatedOrder
        -> ValidatedOrder

// ---------------------------
// Pricing step
// ---------------------------

type GetProductPrice =
    ProductCode -> Price

type PriceOrder =
    GetProductPrice             // dependency
        -> ValidatedOrder       // input
        -> PricedOrder          // output

// ---------------------------
// Send OrderAcknowledgment
// ---------------------------

type HtmlString = HtmlString of string

type OrderAcknowledgment = {
    EmailAddress : EmailAddress
    Letter : HtmlString
}

type SendResult = Sent | NotSent

type CreateOrderAcknowledgmentLetter =
    PricedOrder -> HtmlString

type SendOrderAcknowledgment =
    OrderAcknowledgment -> SendResult

type AcknowledgeOrder =
    CreateOrderAcknowledgmentLetter             // dependency
        -> SendOrderAcknowledgment              // dependency
        -> PricedOrder                          // input
        -> OrderAcknowledgmentSent option       // output

// ---------------------------
// Create events
// ---------------------------

type CreateEvents =
    PricedOrder                                 // input
        -> OrderAcknowledgmentSent option       // input (event from previous step)
        -> PlaceOrderEvent list                 // output

// ======================================================
// Section 2 : Implementation
// ======================================================

// ---------------------------
// ValidateOrder step
// ---------------------------

// ... UnvalidatedCustomerInfo -> CustomerInfo
let toCustomerInfo (customer : UnvalidatedCustomerInfo) : CustomerInfo =
    let firstName = customer.FirstName |> String50.create
    let lastName = customer.LastName |> String50.create
    let emailAddress = customer.EmailAddress |> EmailAddress.create

    // create a PersonalName
    let name = {
        FirstName = firstName           // ... String50
        LastName = lastName             // ... String50
    }

    // create a CustomerInfo
    let customerInfo = {
        Name = name                     // ... PersonalName
        EmailAddress = emailAddress     // ... EmailAddress
    }

    customerInfo

// ... CheckAddressExists -> UnvalidatedAddress -> Address
let toAddress (checkAddressExists : CheckAddressExists) unvalidatedAddress =
    // call the remote service
    let checkedAddress = checkAddressExists unvalidatedAddress
    // extract the inner value using pattern matching
    let (CheckedAddress inner) = checkedAddress       // inner - UnvalidatedAddress

    // ?????? ???????????????? ?? inner ?????? string
    let addressLine1 = inner.AddressLine1 |> String50.create
    let addressLine2 = inner.AddressLine2 |> String50.createOption
    let addressLine3 = inner.AddressLine3 |> String50.createOption
    let addressLine4 = inner.AddressLine4 |> String50.createOption
    let city = inner.City |> String50.create
    let zipCode = inner.ZipCode |> ZipCode.create

    // create the Address
    let address = {
        AddressLine1 = addressLine1     // ... String50
        AddressLine2 = addressLine2     // ... String50 option
        AddressLine3 = addressLine3     // ... String50 option
        AddressLine4 = addressLine4     // ... String50 option
        City = city                     // ... String50
        ZipCode = zipCode               // ... ZipCode
    }

    address     // ... Address

// Order line validation
// "toProductCode" - helper function.

/// Function adapter to convert a predicate to a passhru.
// ... string -> ('a -> bool) -> 'a -> 'a
let predicateToPassthru errorMsg f x =
    if f x then
        x
    else
        failwith errorMsg

// ... CheckProductCodeExists -> string -> bool
let toProductCode (checkProductCodeExists : CheckProductCodeExists) productCode =

    // create a local ProductCode -> ProductCode function
    // suitable for using in a pipeline
    let checkProduct productCode =
        let errorMsg = sprintf "Invalid: %A" productCode
        predicateToPassthru errorMsg checkProductCodeExists productCode

    productCode                 // ... string
    |> ProductCode.create       // ... ProductCode
    |> checkProduct             // ... ProductCode

// ... CheckProductCodeExists -> UnvalidatedOrderLine -> ValidatedOrderLine
let toValidatedOrderLine checkProductCodeExists
    (unvalidatedOrderLine : UnvalidatedOrderLine) =
    let orderLineId =
        unvalidatedOrderLine.OrderLineId            //...string
        |> OrderLineId.create                       //...OrderLineId
    let productCode =
        unvalidatedOrderLine.ProductCode            //...string
        |> toProductCode checkProductCodeExists     //...ProductCode
    let quantity =
        unvalidatedOrderLine.Quantity               //...decimal
        |> OrderQuantity.create productCode         //...OrderQuantity
    let validatedOrderLine = {
        OrderLineId = orderLineId
        ProductCode = productCode
        Quantity = quantity
    }
    validatedOrderLine          // ... ValidatedOrderLine

// ... CheckProductCodeExists -> CheckAddressExists -> UnvalidatedOrder -> ValidatedOrder
let validateOrder : ValidateOrder =
    fun checkProductCodeExists checkAddressExists unvalidatedOrder ->
        let orderId =
            unvalidatedOrder.OrderId            //...string
            |> OrderId.create                   //...OrderId
        let customerInfo =
            unvalidatedOrder.CustomerInfo       //...UnvalidatedCustomerInfo
            |> toCustomerInfo                   //...CustomerInfo
        let shippingAddress =
            unvalidatedOrder.ShippingAddress    //...UnvalidatedAddress
            |> toAddress checkAddressExists     //...Address
        let billingAddress =
            unvalidatedOrder.BillingAddress     //...UnvalidatedAddress
            |> toAddress checkAddressExists     //...Address
        let lines =
            unvalidatedOrder.Lines              //...UnvalidatedOrderLine list
            |> List.map (toValidatedOrderLine checkProductCodeExists)   //...ValidatedOrderLine list
        let validatedOrder : ValidatedOrder = {
            OrderId = orderId
            CustomerInfo = customerInfo
            ShippingAddress = shippingAddress
            BillingAddress = billingAddress
            Lines = lines
        }
        validatedOrder


// ---------------------------
// PriceOrder step
// ---------------------------

// ... (ProductCode -> Price) -> ValidatedOrderLine -> PricedOrderLine
let toPricedOrderLine getProductPrice (line : ValidatedOrderLine) : PricedOrderLine =
    let qty = line.Quantity |> OrderQuantity.value      //...decimal
    let price = line.ProductCode |> getProductPrice     //...Price
    let linePrice = price |> Price.multiply qty         //...Price

    {                                       //...PricedOrderLine
        OrderLineId = line.OrderLineId      //...OrderLineId
        ProductCode = line.ProductCode      //...ProductCode
        Quantity = line.Quantity            //...OrderQuantity
        LinePrice = linePrice               //...Price
    }

// ... GetProductPrice -> ValidatedOrder -> PricedOrder
let priceOrder : PriceOrder =
    fun getProductPrice validatedOrder ->
        let lines =
            validatedOrder.Lines                                //...ValidatedOrderLine list
            |> List.map (toPricedOrderLine getProductPrice)     //...PricedOrderLine list
        let amountToBill =
            lines                                               //...PricedOrderLine list
            |> List.map (fun line -> line.LinePrice)            //...Price list
            |> BillingAmount.sumPrices                          //...BillingAmount
        let pricedOrder : PricedOrder = {
            OrderId = validatedOrder.OrderId                    //...OrderId
            CustomerInfo = validatedOrder.CustomerInfo          //...CustomerInfo
            ShippingAddress = validatedOrder.ShippingAddress    //...ShippingAddress
            BillingAddress = validatedOrder.BillingAddress      //...BillingAddress
            Lines = lines                                       //...PricedOrderLine list
            AmountToBill = amountToBill                         //...BillingAmount
        }
        pricedOrder

// ---------------------------
// AcknowledgeOrder step
// ---------------------------

// ... CreateOrderAcknowledgmentLetter : PricedOrder -> HtmlString
// ... SendOrderAcknowledgment : OrderAcknowledgment -> SendResult
// ... CreateOrderAcknowledgmentLetter -> SendOrderAcknowledgment -> PricedOrder -> OrderAcknowledgmentSent option
let acknowledgeOrder : AcknowledgeOrder =
    fun createAcknowledgmentLetter sendAcknowledgment pricedOrder ->
        let letter = createAcknowledgmentLetter pricedOrder             //...HtmlString
        let acknowledgment = {                                          //...OrderAcknowledgment
            EmailAddress = pricedOrder.CustomerInfo.EmailAddress
            Letter = letter
        }

        // if the acknowledgment was successfully sent,
        // return the corresponding event, else return None
        // ... SendOrderAcknowledgment -> OrderAcknowledgment -> OrderAcknowledgmentSent option
        match sendAcknowledgment acknowledgment with
        | Sent ->
            let event = {                           // OrderAcknowledgmentSent
                OrderId = pricedOrder.OrderId
                EmailAddress = pricedOrder.CustomerInfo.EmailAddress
            }
            Some event
        | NotSent ->
            None

// ---------------------------
// Create events
// ---------------------------

// ... PricedOrder -> BillableOrderPlaced option
let createBillingEvent (placedOrder : PricedOrder) : BillableOrderPlaced option =
    let billingAmount = placedOrder.AmountToBill |> BillingAmount.value
    if billingAmount > 0M then
        let order = {                           // BillableOrderPlaced
            OrderId = placedOrder.OrderId
            BillingAddress = placedOrder.BillingAddress
            AmountToBill = placedOrder.AmountToBill
        }
        Some order
    else
        None

/// Helper to convert an Option into a List.
// ... option<'a> -> list<'a>
let listOfOption opt =
    match opt with
    | Some x -> [x]
    | None -> []

// ... PricedOrder -> OrderAcknowledgmentSent -> PlaceOrderEvent list
let createEvents : CreateEvents =
    fun pricedOrder acknowledgmentEventOpt ->
        let events1 =
            pricedOrder                         //...PricedOrder
            |> PlaceOrderEvent.OrderPlaced      //...PlaceOrderEvent
            |> List.singleton                   //...PlaceOrderEvent list
        let events2 =
            acknowledgmentEventOpt                              //...OrderAcknowledgmentSent option
            |> Option.map PlaceOrderEvent.AcknowledgmentSent    //...PlaceOrderEvent option
            |> listOfOption                                     //...PlaceOrderEvent list
        let events3 =
            pricedOrder                                         //...PricedOrder
            |> createBillingEvent                               //...BillableOrderPlaced option
            |> Option.map PlaceOrderEvent.BillableOrderPlaced   //...PlaceOrderEvent option
            |> listOfOption                                     //...PlaceOrderEvent list

        // return all the events
        [
            yield! events1
            yield! events2
            yield! events3
        ]

// ---------------------------
// Overall workflow
// ---------------------------

// ... CheckProductCodeExists -> CheckAddressExists -> GetProductPrice ->
// ...      CreateOrderAcknowledgmentLetter -> SendOrderAcknowledgment ->
// ...      UnvalidatedOrder -> PlaceOrderEvent list
let placeOrder
    checkProductCodeExists          // dependency
    checkAddressExists              // dependency
    getProductPrice                 // dependency
    createAcknowledgmentLetter      // dependency
    sendOrderAcknowledgment         // dependency
        : PlaceOrderWorkflow =      // function definition
    fun unvalidatedOrder ->
        let validatedOrder =            // ValidatedOrder
            unvalidatedOrder            // UnvalidatedOrder
            |> validateOrder checkProductCodeExists checkAddressExists
        let pricedOrder =               // PricedOrder
            validatedOrder              // ValidatedOrder
            |> priceOrder getProductPrice
        let acknowledgementOption =     // OrderAcknowledgmenSent option
            pricedOrder                 // PricedOrder
            |> acknowledgeOrder createAcknowledgmentLetter sendOrderAcknowledgment
        let events =                    // PlaceOrderEvent list
            createEvents pricedOrder acknowledgementOption
        events
