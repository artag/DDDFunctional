module OrderTaking.SimpleTypes

open System
open System.Text.RegularExpressions

/// Not null or empty string.
type NonEmptyString = private NonEmptyString of string

/// Constrained to be 10 chars or less, not null.
type String10 = private String10 of string

/// Constrained to be 50 chars or less, not null.
type String50 = private String50 of string

/// An email address.
type EmailAddress = private EmailAddress of string

/// A zip code.
type ZipCode = private ZipCode of string

/// An Id for Orders. Constrained to be a non-empty string < 10 chars.
type OrderId = private OrderId of string

/// An Id for OrderLines. Constrained to be a non-empty string < 10 chars
type OrderLineId = private OrderLineId of string

/// The codes for Widgets start with a "W" and then four digits.
type WidgetCode = private WidgetCode of string

/// The codes for Widgets start with a "G" and then three digits.
type GizmoCode = private GizmoCode of string

/// A Product code is either a Widget or a Gizmo.
type ProductCode =
| Widget of WidgetCode
| Gizmo of GizmoCode

/// Constrained to be a integer between 1 and 1000.
type UnitQuantity = private UnitQuantity of int

/// Constrained to be a decimal between 0.05 and 100.00
type KilogramQuantity = private KilogramQuantity of decimal

/// A Quantity is either a Unit or a Kilogram.
type OrderQuantity =
| Unit of UnitQuantity
| Kilogram of KilogramQuantity

module NonEmptyString =

    /// Return the value inside a NotEmptyString.
    let value (NonEmptyString str) = str

    /// Validate input string.
    /// Throw Exception if string is null or empty.
    let validate str =
        if String.IsNullOrEmpty(str) then
            failwith "String must not be null or empty"
        else
            str

    /// Create an NonEmptyString from a string.
    /// Throw Exception if string is null or empty.
    let create str =
        validate str
        |> NonEmptyString

module String =

    /// Validate input string.
    /// Throw Exception if string length > 50.
    let validate maximumLength (NonEmptyString str) =
        if str.Length > maximumLength then
            let msg = sprintf "String must not be more than %i chars" maximumLength
            failwith msg
        else
            str

module String10 =

    /// Maximum string length
    let maximumLength = 10

    /// Return the value inside a String10.
    let value (String10 str) = str

    /// Validate input string.
    /// Throw Exception if string length > 10.
    let validate = String.validate maximumLength

    let create str =
        NonEmptyString.create str
        |> validate
        |> String10

module String50 =

    /// Maximum string length
    let maximumLength = 50

    /// Return the value inside a String50.
    let value (String50 str) = str

    /// Validate input string.
    /// Throw Exception if string length > 50.
    let validate = String.validate maximumLength

    /// Create an String50 from a string.
    /// Throw Exception if input is null, empty, or length > 50.
    let create str =
        NonEmptyString.create str
        |> validate
        |> String50

module EmailAddress =

    /// Return the string value inside an EmailAddress
    let value (EmailAddress str) = str

    /// Create an EmailAddress from a non empty string.
    /// Throw Exception if input doesn't have an "@" in it
    let validate (NonEmptyString str) =
        let pattern = ".+@.+"   // anything separated by an "@"
        if Regex.IsMatch(str, pattern) then
            str
        else
            failwith "String must be separated by an '@'"

    /// Create an EmailAddress from a string.
    /// Return Exception if input is null, empty, or doesn't have an "@" in it
    let create str =
        NonEmptyString.create str
        |> validate
        |> EmailAddress

module ZipCode =

    /// Return the string value inside a ZipCode.
    let value (ZipCode str) =
        str

    /// Validate the input.
    /// Throw Excepion if input not match the pattern.
    let validate (NonEmptyString str) =
        let pattern = "\d{5}"
        if Regex.IsMatch(str, pattern) then
            str
        else
            failwith "String must be contain 5 digits"

    /// Create a Zipcode from a string.
    /// Throw Exception if input is null, empty, or not match the pattern.
    let create str =
        NonEmptyString.create str
        |> validate
        |> ZipCode

module OrderId =

    /// Maximum length.
    let maximumLength = 10

    /// Return thestring value inside an OrderId.
    let value (OrderId str) = str

    /// Validate the string input.
    /// Throw Exception if input string length > 10.
    let validate = String10.validate

    /// Create an OrderLineId from a string.
    /// Throw Exception if input is null, empty, or length > 10.
    let create str =
        NonEmptyString.create str
        |> validate
        |> OrderId

module OrderLineId =

    /// Maximum length.
    let maximumLength = 10

    /// Return thestring value inside an OrderId.
    let value (OrderLineId str) = str

    /// Validate the string input.
    /// Throw Exception if input string length > 10.
    let validate = String10.validate

    /// Create an OrderLineId from a string.
    /// Throw Exception if input is null, empty, or length > 10.
    let create str =
        NonEmptyString.create str
        |> validate
        |> OrderLineId

module WidgetCode =

    /// Return the string value inside a WidgetCode.
    let value (WidgetCode code) = code

    /// Validate the input.
    /// Throw Exception if input is not match the pattern.
    let validate (NonEmptyString str) =
        let pattern = "W\d{4}"
        if Regex.IsMatch(str, pattern) then
            str
        else
            failwith "String must be start with \"W\" and contain 4 digits"

    /// Create a WidgetCode from a string.
    /// Throw Exception if input is null, empty, or not match the pattern.
    let create str =
        NonEmptyString.create str
        |> validate
        |> WidgetCode

module GizmoCode =

    /// Return the string value inside a GizmoCode.
    let value (GizmoCode code) = code

    /// Validate the input.
    /// Throw Exception if input is not match the pattern.
    let validate (NonEmptyString str) =
        let pattern = "G\d{3}"
        if Regex.IsMatch(str, pattern) then
            str
        else
            failwith "String must be start with \"W\" and contain 4 digits"

    /// Create a GizmoCode from a string.
    /// Throw Exception if input is null, empty, or not matching pattern.
    let create str =
        NonEmptyString.create str
        |> validate
        |> GizmoCode

module ProductCode =

    /// Return the string value inside a WidgetCode.
    let value = function
        | Widget (WidgetCode wc) -> wc
        | Gizmo (GizmoCode gc) -> gc

    /// Create a ProductCode from a string.
    /// Throw Exception if input is null, empty, or not matching pattern.
    let create str =
        let code = NonEmptyString.validate str
        if code.StartsWith("W") then
            WidgetCode.create code
            |> Widget
        elif code.StartsWith("G") then
            GizmoCode.create code
            |> Gizmo
        else
            let symbol = code[0]
            let msg = sprintf "Code format not recognized: initial symbol is '%c'" symbol
            failwith msg

module UnitQuantity =

    /// The minimum value.
    let minimum = 1

    /// The maximum value.
    let maximum = 1000

    /// Return the value inside a UnitQuantity.
    let value (UnitQuantity qty) = qty

    /// Validate an integer input.
    /// Throw Exception if input is not an integer between 1 and 1000.
    let validate qty =
        if minimum <= qty && qty <= maximum then
            qty
        else
            let msg = sprintf "Wrong quantity value %i. The value must be between %i and %i" qty minimum maximum
            failwith msg

    /// Create a UnitQuantity from a int.
    /// Throw Exception if input is not an integer between 1 and 1000.
    let create qty =
        validate qty
        |> UnitQuantity

module KilogramQuantity =

    /// The minimum value.
    let minimum = 0.05M

    /// The maximum value.
    let maximum = 100M

    /// Return the value inside a KilogramQuantity.
    let value (KilogramQuantity qty) = qty

    /// Validate a decimal input.
    /// Throw Exception if input is not a decimal between 0.05 and 100.00.
    let validate qty =
        if minimum <= qty && qty <= maximum then
            qty
        else
            let msg = sprintf "Wrong quantity value %f. The value must be between %f and %f" qty minimum maximum
            failwith msg

    /// Create a KilogramQuantity from a decimal.
    /// Throw Exception if input is not a decimal between 0.05 and 100.00.
    let create qty =
        validate qty
        |> KilogramQuantity

module OrderQuantity =

    /// Return the value inside a OrderQuantity.
    let value = function
        | Unit uq ->
            uq
            |> UnitQuantity.value
            |> decimal
        | Kilogram kg ->
            kg
            |> KilogramQuantity.value
