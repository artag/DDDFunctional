# Chapter 4. Understanding Types

## Understanding Functions

### Type Signatures

Описание `apple -> banana` называется *type signature* (подпись типа). Также известна как
*function signature*.

Еще примеры:

```fsharp
let add1 x = x + 1      // signature: int -> int
let add x y = x + y     // signature: int -> int -> int
```

Пример функции в F#. В ней есть подфункция.

```fsharp
// int -> int
let squarePlusOne:
    let square = x * x
    square + 1
```

### Functions with Generic Types

Функция работает с *любым* типом данных:

```fsharp
// 'a -> 'a -> bool
let areEqual x y =
    (x = y)
```

Для сравнения, эта же фукция в C#:

```csharp
static bool AreEqual<T>(T x, T y)
{
    return (x == y);
}
```

## Types and Functions

*Type* (тип) - это имя, данное множеству возможных значений, которые могут быть использованы
в качестве входов или выходов функции.

Типами могут быть любые сущности реальные или виртуальные: примитивные типы, объекты, сами функции.

Сигнатуры функций с input/output различных типов:

```fsharp
int16 -> someOutputType
someInputType -> string
Person -> someOutputType
someInputType -> Fruit
someInputType -> (Fruit -> Fruit)
```

>### Jargon Alert: "Values" vs. "Objects" vs. "Variables"
>
>In a functional programming language, most things are called "*values*." In an object-
>oriented language, most things are called "*objects*". So what is the difference between
>a "value" and an "object"?
>
>A *value* is just a member of a type, something that can be used as an input or an
>output. For example, `1` is a value of type `int` , `"abc"` is a value of type `string`,
>and so on.
>
>*Functions can be values too*. If we define a simple function such as `let add1 x = x + 1`,
>then `add1` is a (function) value of type `int->int`.
>
>*Values are immutable* (which is why they are not called "variables").
>And *values do not have any behavior attached to them*, they are just *data*.
>
>In contrast, an *object is an encapsulation of a data structure and its associated behavior (methods)*.
>In general, *objects are expected to have state* (that is, be *mutable*),
>and *all operations that change the internal state must be provided by the object itself*
>(via "dot" notation).
>
>So in the world of functional programming (where objects don’t exist), you should
>use the term "value" rather than "variable" or "object".

## Composition of Types

В ФП композиция используется для создания новых функций из меньших функций и новых типов из более
мелких типов.

### "AND" Types

Тип record:

```fsharp
type FruitSalad = {
    Apple: AppleVariety
    Banana: BananaVariety
    Cherries: CherryVariety
}
```

### "OR" Types

Choice type - *discriminated union*:

```fsharp
type FruitSnack =
    | Apple of AppleVariety
    | Banana of BananaVariety
    | Cherries of CherryVariety

type AppleVariety =
    | GoldenDelecious
    | GrannySmith
    | Fuji

type BananaVariety =
    | Cavedish
    | GrosMichel
    | Manzano

type CherryVariety =
    | Montmorency
    | Bing
```

>### Jargon Alert: "Product Types" and "Sum Types"
>
>The types that are built using AND are called **product types**.
>
>The types that are built using OR are called **sum types** or **tagged unions** or, in F#
>terminology, **discriminated unions**.

### Simple Types

Choice type могут выражаться только одним типом:

```fsharp
type ProductCode =
    | ProductCode of string
```

или (чаще используется):

```fsharp
type ProductCode = ProductCode of string
```

## Working with F# Types

Определение типа record:

```fsharp
type Person = { First: string; Last: string }
```

Определение value типа `Person`:

```fsharp
let aPerson = { First = "Alex"; Last = "Adams" }
```

Deconstruct:

```fsharp
let { First = first; Last = last } = aPerson
```

или так:

```fsharp
let first = aPerson.First
let last = aPerson.Last
```

Определение типа choice:

```fsharp
type OrderQuantity =
    | UnitQuantity of int
    | KilogramQuantity of decimal
```

Определение value (constructor)

```fsharp
let anOrderQtyInUnits = UnitQuantity 10
let anOrderQtyInKg = KilogramQuantity 2.5
```

Deconstruct:

```fsharp
let printQuantity aOrderQty =
    match aOrderQty with
    | UnionQuantity uQty -> printfn "%i units" uQty
    | KilogramQuantity kgQty -> printfn "%g kg" kgQty
```

Использование deconstuct:

```fsharp
printQuantity anOrderQtyInUnits     // "10 units"
printQuantity anOrderQtyInKg        // "2.5 kg"
```

## Building a Domain Model by Composing Types

Пример. Описание домена payments for an e-commerce site.

Документация данных.

```fsharp
type CheckNumber = CheckNumber of int
type CardNumber = CardNumber of string

type CardType =
    Visa | Mastercard

type CreditCardInfo = {
    CardType : CardType
    CardNumber : CardNumber
}

type PaymentMethod =
    | Cash
    | Check of CheckNumber
    | Card of CreditCardInfo

type PaymentAmount = PaymentAmount of decimal
type Currency = EUR | USD

type Payment = {
    Amount : PaymentAmount
    Currency : Currency
    Method : PaymentMethod
}
```

Документация функций.

```fsharp
type PayInvoice =
    UnpaidInvoice -> Payment -> PaidInvoice

type ConvertPaymentCurrency =
    Payment -> Currency -> Payment
```

## Modeling Optional Values, Errors, and Collections

* Optional or missing values
* Errors
* Functions that return no value
* Collections

### Modeling Optional Values

Для отсутствующего значения в F# используется тип `Option`:

```fsharp
type Option<'a> =
    | Some of 'a
    | None
```

Пример использования для описания типа `PersonalName`:

```fsharp
type PersonalName = {
    FirstName : string
    MiddleInitial : Option<string>      // optional
    LastName : string
}
```

Чаще всего записывают в такой форме:

```fsharp
type PersonalName = {
    FirstName : string
    MiddleInitial : string option       // optional
    LastName : string
}
```

### Modeling Errors

В F# предпочтительно документировать возможность ошибки путем использования типа `Result`:

```fsharp
type Result<'Success, 'Failure> =
    | Ok of 'Success
    | Error of 'Failure
```

Тип `Result` входит в стандартные библиотеки F# начиная с версии F# 4.1.

Пример использования:

```fsharp
type PayInvoice =
    UnpaidInvoice -> Payment -> Result<PaidInvoice, PaymentError>

type PaymentError =
    | CardTypeNotRecognized
    | PaymentRejected
    | PaymentProviderOffline
```

### Modeling No Value at All

Отсутствие выходного значения у функции в F#:

```fsharp
type SaveCustomer = Customer -> unit
```

Отсутствие входных значений у функции в F# (без параметров):

```fsharp
type NextRandom = unit -> int
```

Наличие `unit` в сигнатуре функции часто является признаком наличия у фукции side effect.

### Modeling Lists and Collections

Типы коллекций в F#:

* `list` - неизменяемая коллекция, определенного размера. Реализована в виде linked list
(связный список).
* `array` - изменяемая коллекция, определенного размера. Каждый элемент доступен по индексу.
* `ResizeArray` - массив изменяемого размера. Аналогичен по функциональности `List<T>` в C#.
* `seq` - lazy коллекция, каждый элемент возвращается по запросу. Аналогичен по функциональности
`IEnumerable<T>` в C#.
* `Map` (аналогичен `Dictionary`) и `Set`. Редко используются в domain model.

В domain model чаще всего используется `list`.

Пример использования:

```fsharp
type Order = {
    OrderId : OrderId
    Lines : OrderLine list      // a collection
}
```

Создание `list`, добавление элемента к существующему `list`:

```fsharp
let aList = [1; 2; 3]

let aNewList = 0 :: aList       // [0; 1; 2; 3]
```

Deconstruct `list`:

```fsharp
let printList1 aList =
    match aList with
    | [] -> printfn "list is empty"
    | [x] -> printfn "list has one element: %A" x
    | [x; y] -> printfn "list has two elements: %A and %A" x y
    | longerList -> printfn "list has more than two elements"
```

Match с использованием "cons" оператора:

```fsharp
let printList2 aList =
    match aList with
    | [] -> printfn "list is empty"
    | first :: rest -> printfn "list is not empty with the first element being: %A" first
```

## Organizing Types in Files and Projects

В F# порядок файлов важен. Файлы, расположенные выше, не могут ссылаться на файлы,
 расположенные ниже.

Файлы проекта могут быть расположены в подобном порядке:

```text
Common.Types.fs
Common.Functions.fs
OrderTaking.Types.fs
OrderTaking.Functions.fs
Shipping.Types.fs
Shipping.Functions.fs
```

В файле, порядок описаний типов и функций также важен:

```fsharp
module Payments =
    type CheckNumber = CheckNumber of int

    type PaymentMethod =
        | Cash
        | Check of CheckNumber      // определен выше
        | Card of ...

    type Payment = {
        Amount : ...
        Currency : ...
        Method : PaymentMethod     // определен выше
    }
```

Когда требуется писать код без ограничения "top down", то можно использовать `rec` (F# 4.1 и выше)
или `and`. Но лучше, когда дизайн "устаканится", писать в стиле "top down".

Пример использования `rec`:

```fsharp
module rec Payments =
    type Payment = {
        Amount : ...
        Currency : ...
        Method : PaymentMethod      // определен ниже
    }

type PaymentMethod =
    | Cash
    | Check of CheckNumber          // определен ниже
    | Card of ...

type CheckNumber = CheckNumber of int
```

Пример использования `and`:

```fsharp
type Payment = {
    Amount : ...
    Currency : ...
    Method : PaymentMethod          // определен ниже
}

and PaymentMethod =
    | Cash
    | Check of CheckNumber          // определен ниже
    | Card of ...

and CheckNumber = CheckNumber of int
```

# Chapter 5. Domain Modeling with Types

## Reviewing the Domain Model

Text-based документация domain model, сделанная ранее:

```text
context: Order-Taking

// --------------------
// Simple types
// --------------------

// Product Codes
data ProductCode = WidgetCode OR GizmoCode
data WidgetCode = string starting with "W" then 4 digits
data GizmoCode = string starting with "G" then 3 digits

// Order Quantity
data OrderQuantity = UnitQuantity OR KilogramQuantity
data UnitQuantity = integer between 1 and 1000
data KilogramQuantity = decimal between 0.05 and 100.0
```

```text
// --------------------
// Order life cycle
// --------------------

// ----- unvalidated state -----
data UnvalidatedOrder =
    UnvalidatedCustomerInfo
    AND UnvalidatedShippingAddress
    AND UnvalidatedBillingAddress
    AND list of UnvalidatedOrderLine

data UnvalidatedOrderLine =
    UnvalidatedProductCode
    AND UnvalidatedOrderQuantity

// ----- validated state -----
data ValidatedOrder =
    ValidatedCustomerInfo
    AND ValidatedShippingAddress
    AND ValidatedBillingAddress
    AND list of ValidatedOrderLine

data ValidatedOrderLine =
    ValidatedProductCode
    AND ValidatedOrderQuantity

// ----- prices state -----
data PricedOrder =
    ValidatedCustomerInfo
    AND ValidatedShippingAddress
    AND list of PricedOrderLine
    AND AmountOfBill

data PricedOrderLine =
    ValidatedOrderLine
    AND LinePrice

// ----- output events -----
data OrderAcknowledgmentSent =
    PricedOrder
    AND AcknowledgementLetter

data OrderPlaced = PricedOrder

data BillableOrderPlaced =
    OrderId
    AND BillingAddress
    AND AmountToBill
```

```text
// --------------------
// Workflows
// --------------------
workflow "Place Order" =
    input: UnvalidatedOrder
    output (on success):
        OrderAcknowledgmentSent
        AND OrderPlaced (to send to shipping)
        AND BillableOrderPlaced (to send to billing)
    output (on error):
        InvalidOrder
// etc
```

## Seeing Patterns in a Domain Model

Из текстового описания domain можно выделить повторяющиеся паттерны:

* *Simple values*. Примитивные типы: `string`, `integer` и т.п. Доменный эксперт не думает в
подобных терминах, а думает в терминах таких как `OrderId` и `ProductCode`.
* *Combinations of values with AND*. Группы связанных данных.
* *Choices with OR*. Выбор данных. `Order` или `Quote`, `UnitQuantity` или `KilogramQuantity`.
* *Workflows*. Бизнес-процессы. Имеют input и output.

## Modeling Simple Values

# Links

* [Understanding type inference in F#](https://fsharpforfunandprofit.com/posts/type-inference/)
