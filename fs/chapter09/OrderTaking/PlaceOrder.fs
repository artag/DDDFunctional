module OrderTaking.PlaceOrder

open SimpleTypes
open PublicTypes
open CompoundTypes

// ======================================================
// Section 1 : Define each step in the workflow using types
// ======================================================

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

// ======================================================
// Section 2 : Implementation
// ======================================================

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

    // все значения в inner это string
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
