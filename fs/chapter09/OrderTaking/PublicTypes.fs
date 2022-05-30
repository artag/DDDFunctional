module OrderTaking.PublicTypes

// Workflow input.

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
