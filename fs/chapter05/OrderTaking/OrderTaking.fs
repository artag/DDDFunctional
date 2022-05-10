namespace OrderTaking.Domain

// Alias for undefined type
type Undefined = exn

// Product code related
// Constraint: starting with "W" then 4 digits
type WidgetCode = WidgetCode of string
// Constraint: starting with "G" then 3 digits
type GizmoCode = GizmoCode of string
type ProductCode =
| Widget of WidgetCode
| Kilos of GizmoCode

// Order Quantity related
type UnitQuantity = UnitQuantity of int
type KilogramQuantity = KilogramQuantity of decimal
type OrderQuantity =
| Unit of UnitQuantity
| Kilos of KilogramQuantity

// Order related
type OrderId = Undefined
type OrderLineId = Undefined
type CustomerId = Undefined

type CustomerInfo = Undefined
type ShippingAddress = Undefined
type BillingAddress = Undefined
type Price = Undefined
type BillingAmount = Undefined

type Order = {
    Id : CustomerId
    CustomerId : CustomerId
    ShippingAddress : ShippingAddress
    BillingAddress : BillingAddress
    OrderLines : OrderLine list
    AmountToBill : BillingAmount
}

and OrderLine = {
    Id : OrderLineId
    OrderId : OrderId
    ProductCode : ProductCode
    OrderQuantity : OrderQuantity
    Price : Price
}

// Workflow related
// Workflow. Unvalidated Order related
type UnvalidatedCustomerInfo = Undefined
type UnvalidatedShippingAddress = Undefined
type UnvalidatedBillingAddress = Undefined
type UnvalidatedOrderLine = Undefined

type UnvalidatedOrder = {
    Id : string
    CustomerInfo : UnvalidatedCustomerInfo
    ShippingAddress : UnvalidatedShippingAddress
    BillingAddress : UnvalidatedBillingAddress
    OrderLines : UnvalidatedOrderLine list
}

// Workflow. PlaceOrderEvents related.Priced Order related
type ValidatedCustomerInfo = Undefined
type ValidatedShippingAddress = Undefined
type ValidatedBillingAddress = Undefined

type PricedOrder = {
    Id : OrderId
    CustomerInfo : ValidatedCustomerInfo
    ShippingAddress : ValidatedShippingAddress
    BillingAddress : ValidatedBillingAddress
    OrderLines : PricedOrderLine list
    AmountOfBill : BillingAmount
}

and PricedOrderLine = {
    Id : OrderLineId
    OrderId : OrderId
    ProductCode : ProductCode
    OrderQuantity : OrderQuantity
    Price : Price
}

// Workflow. PlaceOrderEvents related
type AcknowledgmentLetter = Undefined

type OrderAcknowledgmentSent = {
    PricedOrder : PricedOrder
    AcknowledgmentLetter : AcknowledgmentLetter
}

type OrderPlaced = PricedOrder
type BillableOrderPlaced = PricedOrder

type PlaceOrderEvents = {
    AcknowledgmentSent : OrderAcknowledgmentSent
    OrderPlaced : OrderPlaced
    BillableOrderPlaced : BillableOrderPlaced
}

// Workflow. PlaceOrderError related
type PlaceOrderError =
| ValidationError of ValidationError list

and ValidationError = {
    FieldName : string
    ErrorDescription : string
}

// The "Place Order" process
type PlaceOrder =
    UnvalidatedOrder -> Result<PlaceOrderEvents, PlaceOrderError>
