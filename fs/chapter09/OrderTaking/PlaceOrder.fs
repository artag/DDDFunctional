module OrderTaking.PlaceOrder

open SimpleTypes
open PublicTypes
open CompoundTypes

type CheckedAddress = {
    AddressLine1 : String50
    AddressLine2 : String50 option
    AddressLine3 : String50 option
    AddressLine4 : String50 option
    City : String50
    ZipCode : ZipCode
}

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
