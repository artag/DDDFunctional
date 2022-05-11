type UnitQuantity = private UnitQuantity of int

module UnitQuantity =

    let create qty =
        if qty < 1 then
            Error "UnitQuantity can not be negative"            // failure
        else if qty > 1000 then
            Error "UnitQuantity can not be more than 1000"      // failure
        else
            Ok (UnitQuantity qty)             // success - construct value

    let value (UnitQuantity qty) = qty


let unitQtyResult = UnitQuantity.create 1

match unitQtyResult with
| Error msg  ->
    printfn "Failure, Message is %s" msg
| Ok uQty ->
    printfn "Success. Value is %A" uQty
    let innerValue = UnitQuantity.value uQty
    printfn "innerValue is %i" innerValue
