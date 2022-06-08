module OrderTaking.PublicTypes

open SimpleTypes
open CompoundTypes

// ---------------------------
// Workflow input.
// ---------------------------

type UnvalidatedAddress = {
    AddressLine1 : string
    AddressLine2 : string
    AddressLine3 : string
    AddressLine4 : string
    City : string
    ZipCode : string
}

type UnvalidatedCustomerInfo = {
    FirstName : string
    LastName : string
    EmailAddress : string
}

type UnvalidatedOrderLine = {
    OrderLineId : string
    ProductCode : string
    Quantity : decimal
}

type UnvalidatedOrder = {
    OrderId : string
    CustomerInfo : UnvalidatedCustomerInfo
    ShippingAddress : UnvalidatedAddress
    BillingAddress : UnvalidatedAddress
    Lines : UnvalidatedOrderLine list
}

// ---------------------------
// Workflow output.
// ---------------------------

/// Event will be created if the Acknowledgment was successfully posted.
type OrderAcknowledgmentSent = {
    OrderId : OrderId
    EmailAddress : EmailAddress
}

type PricedOrderLine = {
    OrderLineId : OrderLineId
    ProductCode : ProductCode
    Quantity : OrderQuantity
    LinePrice : Price
}

type PricedOrder = {
    OrderId : OrderId
    CustomerInfo : CustomerInfo
    ShippingAddress : Address
    BillingAddress : Address
    AmountToBill : BillingAmount
    Lines : PricedOrderLine list
}
