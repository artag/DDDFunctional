module OrderTaking.Tests

open System
open NUnit.Framework
open OrderTaking.SimpleTypes
open OrderTaking.PublicTypes
open OrderTaking.PlaceOrder

[<SetUp>]
let Setup () =
    ()

let createUnvalidatedOrder() : UnvalidatedOrder =
    {
        OrderId = "x12345"
        BillingAddress = {
            AddressLine1 = "Unknown str., 7-42"
            AddressLine2 = String.Empty
            AddressLine3 = String.Empty
            AddressLine4 = String.Empty
            City = "Moscow"
            ZipCode = "109123"
        }
        ShippingAddress = {
            AddressLine1 = "Unknown blv., 1-24"
            AddressLine2 = String.Empty
            AddressLine3 = String.Empty
            AddressLine4 = String.Empty
            City = "Sochi"
            ZipCode = "104576"
        }
        CustomerInfo = {
            FirstName = "Ivan"
            LastName = "Ivanov"
            EmailAddress = "test@mail.com"
        }
        Lines = [
            {
                OrderLineId = "123"
                ProductCode = "W1234"
                Quantity = 10M
            }
        ]
    }


[<Test>]
let ``If product exists, validation succeeds`` () =
    // arrange: set up stub versions of service dependencies
    let checkAddressExists address =
        CheckedAddress address          // succeed

    let checkProductCodeExists productCode =
        true                            // succeed

    // arrange: set up input
    let unvalidatedOrder = createUnvalidatedOrder()

    // act: call vaidateOrder
    let result = validateOrder checkProductCodeExists checkAddressExists unvalidatedOrder

    // assert: check that result is a ValidatedOrder, not an error
    let orderId = result.OrderId |> OrderId.value
    Assert.AreEqual(unvalidatedOrder.OrderId, orderId)


[<Test>]
let ``If product doesn't exists, validation fails`` () =
    // arrange: set up stub versions of service dependencies
    let checkAddressExists address =
        CheckedAddress address          // succeed

    let checkProductCodeExists productCode =
        false                           // fail

    // arrange: set up input
    let unvalidatedOrder = createUnvalidatedOrder()

    // act: call vaidateOrder
    try
        let result = validateOrder checkProductCodeExists checkAddressExists unvalidatedOrder
        Assert.Fail("Exception must be throw")
    with ex ->
        ex |> ignore
