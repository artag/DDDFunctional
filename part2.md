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

Синтаксис:

```text
type CustomerId = CustomerId of int
     ^type name   ^case label
```

Для нашего domain:

```fsharp
type WidgetCode = WidgetCode of string
type UnitQuantity = UnitQuantity of int
type KilogramQuantity = KilogramQuantity of decimal
```

### Working with Single Case Unions

```text
type CustomerId = CustomerId of int
                  ^this case name will be constructor function
```

Создание типа (использование конструктора):

```fsharp
let customerId = CustomerId 42
```

Два разных типа неэквивалентны:

```fsharp
type CustomerId = CustomerId of int
type OrderId = OrderId of int

let customerId = CustomerId 42
let orderId = OrderId 42

printfn "%b" (orderId = customerId)     // Ошибка компиляции
```

Деконструкция:

```fsharp
let (CustomerId innerValue) = customerId        // innerValue = 42
printfn "%i" innerValue                         // выведет "42"
```

Деконструкция в параметре определяемой функции:

```fsharp
// val processCustomerId: CustomerId -> unit
let processCustomerId (CustomerId innerValue) =
    printfn "innerValue is %i" innerValue
```

### Constrained Values

См. следующую главу 6.

### Avoiding Performance Issues with Simple Types

Такое "обертывание" несколько замедляет работу программ. Если необходимо ускориться, то можно:

1. Использовать type alias. Нет overhead, но теряется type-safety.

```fsharp
type UnitQuantity = int
```

2. Начиная с F# 4.1 можно использовать value type (a struct). Overhead есть, но есть оптимизация
по доступу в памяти.

```fsharp
[<Struct>]
type UnitQuantity = UnitQuantity of int
```

3. При работе с большими массивами, можно рассмотреть определение коллекции примитивных типов
как один тип. Это позволит эффективно работать с внутренними данными (например, перемножение матриц)
и сохранит type-safety.

```fsharp
type UnitQuantities = UnitQuantities of int[]
```

## Modeling Complex Data

### Modeling with Record Types

Используются для моделирования структур данных **AND**.

Такое text-based описание:

```text
data Order =
    CustomerInfo
    AND ShippingAddress
    AND BillingAddress
    AND list of OrderLines
    AND AmountToBill
```

В F# можно записать в виде record:

```fsharp
type Order = {
    CustomerInfo : CustomerInfo
    ShippingAddress : ShippingAddress
    OrderLines : OrderLine list
    AmountToBill : BillingAmount
}
```

### Modeling Unknown Types

Для этапов разработки, когда еще неизвестно внутреннее содержимое некоторых типов
можно этот момент явно указать при описании типа.

**Решение** - использование исключения. В F# это `exn`. Также рекомендуется задать alias, который потом
используется в коде:

```fsharp
type Undefined = exn
```

Использование alias:

```fsharp
type CustomerInfo = Undefined
type ShippingAddress = Undefined
type BillingAddress = Undefined
type OrderLine = Undefined
type BillingAmount = Undefined

type Order = {
    CustomerInfo : CustomerInfo
    ShippingAddress : ShippingAddress
    BillingAddress : BillingAddress
    OrderLines : OrderLine list
    AmountToBill : BillingAmount
}
```

Такой подход позволит описать domain и скомпилировать код. Но при написании функций, которые будут
работать с типами domain надо будет заменить `Undefined` на рабочий код.

### Modeling with Choice Types

Используются для моделирования структур данных **OR**.

Такое text-based описание:

```text
data ProductionCode =
    WidgetCode
    OR GizmoCode

data OrderQuantity =
    UnitQuantity
    OR KilogramQuantity
```

В F# можно записать в виде descriminated union:

```fsharp
type ProductCode =
    | Widget of WidgetCode
    | Gizmo of GizmoCode

type OrderQuantity =
    | Unit of UnitQuantity
    | Kilogram of KilogramQuantity
```

Где

```text
| Widget of WidgetCode
  ^tag      ^type
```

tag - наименование case

type - тип данных, который ассоциируется с tag

## Modeling Workflows with Functions

Описали все "nouns" (существительные) для domain, теперь надо описать "verbs"
(глаголы) - бизнес-процессы.

Бизнес-процессы описываются с помощью function types:

```fsharp
type ValidateOrder = UnvalidatedOrder -> ValidatedOrder
```

### Working with Complex Inputs and Outputs

#### Результат бизнес-процесса - несколько выходов (связь AND)

1. Надо создать тип record, объединяющий выходы.
2. Описать function type с общим типом в качестве выхода.

Например, результатом бизнес-процесса являются три события. Эти события объединяются в один тип:

```fsharp
type PlaceOrderEvents = {
    AcknowledgmentSent : AcknowledgmentSent
    OrderPlaced : OrderPlaced
    BillableOrderPlaced : BillableOrderPlaced
}
```

Function type, описывающий бизнес-процесс `Place Order`, будет выглядеть так:

```fsharp
type PlaceOrder = UnvalidatedOrder -> PlaceOrderEvents
```

#### Результат бизнес-процесса - один из нескольких выходов (связь OR)

1. Надо создать тип discriminated union (DU), объединяющий выходы.
2. Описать function type с общим типом в качестве выхода.

Пример, есть вот такое text-based описание бизнес-процесса:

```text
workflow "Categorize Inbound Mail" =
    input: Envelope contents
    output:
        QuoteForm (put on appropriate pile)
        OR OrderForm (put on appropriate pile)
        OR ...
```

Его описание в виде кода. Данные:

```fsharp
type EnvelopeContents = EnvelopeContents of string
type CategorizedMail =
    | Quote of QuoteForm
    | Order of OrderForm
    // etc
```

Действие:

```fsharp
type CategorizeInboundMail = EnvelopeContents -> CategorizedMail
```

#### У бизнес-процесса несколько входов. Требуется только один (связь OR)

Надо создать тип discriminated union (DU), объединяющий входы.

#### У бизнес-процесса несколько входов. Требуются все (связь AND)

Бизнес-процесс, например:

```text
"Calculate Prices" =
    input: OrderForm, ProductCatalog
    output: PricedOrder
```

Возможно два варианта реализации.

1. Самый простой. Передать каждый вход как отдельный параметр в функцию:

```fsharp
type CalculatePrices = OrderForm -> ProductCatalog -> PricedOrder
```

Используется если один из параметров это зависимость от другого сервиса.
Функциональный dependency injection.

2. Создать тип record, объединяющий входы. Иногда используются tuple.

```fsharp
type CalculatePriceInput = {
    OrderForm : OrderForm
    ProductCatalog : ProductCatalog
}
```

Function type будет выглядеть так:

```fsharp
type CalculatePrices = CalculatePricesInput -> PricedOrder
```

Используется, если входные параметры логически связаны друг с другом.

### Documenting Effects in the Function Signature

>В ФП используется термин **effects** - побочные эффекты, которые производит функция помимо
>выходных данных.

Функция не всегда может успешно завершиться и возвратить какое-либо значение. Ипользуется тип
`Result` для описания такого effect:

```fsharp
type ValidateOrder =
    UnvalidatedOrder -> Result<ValidatedOrder, ValidationError list>
```

Функция асинхронна и может завершиться с ошибкой:

```fsharp
type ValidateOrder =
    UnvalidatedOrder -> Async<Result<ValidatedOrder, ValidationError list>>
```

Такая сигнатура типа выглядит плохо читаемой, поэтому для нее можно ввести alias:

```fsharp
type ValidationResponse<'a> = Async<Result<'a, ValidationError list>>
```

Функция будет выглядеть так:

```fsharp
type ValidateOrder =
    UnvalidatedOrder -> ValidationResponse<ValidatedOrder>
```

## A Question of Identity: Value Objects

>В DDD объект с явным идентификатором называется **Entity**, без него - **Value Object**.

В большинстве случаев объекты данных без явной идентичности (идентификаторов) взаимозаменяемы,
т.е. являются Value Object'ами.

Например, адреса, имена, почтовые индексы - примеры Value Objects.

```fsharp
let widgetCode1 = WidgetCode "W1234"
let widgetCode2 = WidgetCode "W1234"
printfn "%b" (widgetCode1 = widgetCode2)    // prints "true"

let name1 = {FirstName="Alex"; LastName="Adams"}
let name2 = {FirstName="Alex"; LastName="Adams"}
printfn "%b" (name1 = name2)                // prints "true"

let address1 = {StreetAddress="123 Main St"; City="New York"; Zip="90001"}
let address2 = {StreetAddress="123 Main St"; City="New York"; Zip="90001"}
printfn "%b" (address1 = address2)          // prints "true"
```

### Implementing Equality for Value Objects

В F# по умолчанию все поля алгебраических типов структурно эквивалентны. Все такие типы в F#
можно рассматривать как Value Objects.

## A Question of Identity: Entities

Примеры Entity: orders, quotes, invoices, customer profiles, product sheets, ...

* Различие между Value Object и Enitity зависит от контекста.

* Enitity может содержать в себе один или несколько Value Object.

* При изменении Value Object, Entity, который его содержит остается прежним.

Пример: смартфон (Entity), у которого поменяли экран или батарею.

### Identifiers for Entities

Entity должен иметь стабильный идентификатор, не зависящий от изменений.

* Идентфикатором должен быть уникальный ключ, присущий определенному Entity.
* Под идентфикатор выделяется отдельное поле, типа "Order ID" или "Customer ID".

Пример:

```fsharp
type ContactId = ContactId of int

type Contact = {
    ContactId : ContactId
    PhoneNumner : ...
    EmailAddress : ...
}
```

Источники для идентификаторов:

* Реально существующий domain: бумажные orders, invoices, ...
* Сервис(ы), генерирующий UUIDs.
* Столбцы идентификаторов в БД.

### Adding Identifiers to Data Definitions

В случае типа record добавить идентификатор просто. С discriminated union не все так просто.
Есть два спопоба добавления поля id.

1. **Способ 1**. Идентификатор хранится "снаружи" Entity. Менее распространенный.



```fsharp
// Info for the unpaid case (without id)
type UnpaidInvoiceInfo = ...

// Info for the paid case (without id)
type PaidInvoiceInfo = ...

// Combined information (without id)
type InvoiceInfo =
    | Unpaid of UnpaidInvoiceInfo
    | Paid of PaidInvoiceInfo

// Id for invoice
type InvoiceId =

// Top level invoice type
type Invoice = {
    InvoiceId : InvoiceId           // "outside" the two child cases
    InvoiceInfo : InvoiceInfo
}
```

Недостаток: сложно работать с данными для одного case, данные разделяются между несколькими
компонентами.

1. **Способ 2**. Идентификатор хранится "внутри" Entity. Наиболее популярный способ.

```fsharp
type UnpaidInvoice = {
    InvoiceId : InvoiceId       // id stored "inside"
    // and other info for the unpaid case
}

type PaidInvoice = {
    InvoiceId : InvoiceId       // id stored "inside"
    // and other info for the paid case
}

// top level invoice type
type Invoice =
    | Unpaid of UnpaidInvoice
    | Paid of PaidInvoice
```

Преимущество: все данные доступны в одном месте. Например для pattern matching:

```fsharp
let invoice = Paid { InvoiceId = ... }

match invoice with
    | Unpaid unpaidInvoice -> printfn "The unpaid invoiceId is %A" unpaidInvoice.InvoiceId
    | Paid paidInvoice -> printfn "The paid invoiceId is %A" paidInvoice.InvoiceId
```

### Implementing Equality for Entities

Для Entity эквивалентность определяется по одному полю - полю идентификатора. Т.к. в F#
по умолчанию при эквивалентности рассматриваются все поля типа, то необходимо
переопределить Equality.

**1 Способ**. Надо переопределить следующее:

* Override метод `Equals`.
* Override метод `GetHashCode`.
* Добавить атрибуты `CustomEquality` и `NoComparison` к типу, чтобы сообщить компилятору
о переопределении Equality.

Что получается в итоге:

```fsharp
[<CustomEquality; NoComparison>]
type Contact = {
    ContactId : ContactId
    PhoneNumber : PhoneNumber
    EmailAddress : EmailAddress
    }
    with
    override this.Equals(obj) =
        match obj with
        | :? Contact as c -> this.ContactId = c.ContactId
        | _ -> false
    override this.GetHashCode() =
        hash this.ContactId
```

После такого переопределения Equality можно сравнивать между собой `Contact`s:

```fsharp
// Определение двух Contact
let contactId = ContactId 1

let contact1 = {
    ContactId = contactId
    PhoneNumber = PhoneNumber "123-456-7890"
    EmailAddress = EmailAddress "bob@example.com"
}

let contact2 = {
    ContactId = contactId
    PhoneNumber = PhoneNumber "123-456-7890"
    EmailAddress = EmailAddress "robert@example.com"
}

// Сравнение
printfn "%b" (contact1 = contact2)      // true
```

Это распространенный подход в ООП, но такой подход может приводить к ошибкам:

* При изменении Equality потом можно получить результаты отличные от ожидаемых
(забыли о переопределении и рассчитывали на поведение по умолчанию).
* Забыли переопределить Equality.

**2 Способ**. Более простой и предпочтительный поход:

* Добавить атрибуты `NoEquality` и `NoComparison` к типу, чтобы запретить компилятору
прямое сравнение типов.

```fsharp
[<NoEquality; NoComparison>]
type Contact = {
    ContactId : ContactId
    PhoneNumber : PhoneNumber
    EmailAddress : EmailAddress
}
```

Теперь, при попытке напрямую сравнить объекты типа `Contact` друг с другом будет ошибка компиляции:

```fsharp
printfb "%b" (contact1 = contact2)      // ошибка компиляции
```

Можно сравнить объекты `Contact` по id только явным образом:

```fsharp
printfn "%b" (contact1.ContactId = contact2.ContactId)      // true
```

#### Переопределение Equality в Entity для нескольких полей

Добавление дополнительного (синтетического) поля, которое будет содержать поля, по которым
производится сравнение. В примере это дополнительное поле `Key`:

```fsharp
[<NoEquality; NoComparison>]
type OrderLine = {
    OrderId : OrderId
    ProductId : ProductId
    Qty : int
    }
    with
    member this.Key =
        (this.OrderId, this.ProductId)
```

И сравнение можно делать так:

```fsharp
printfn "%b" (line1.Key = line2.Key)
```

### Immutability and Identity

* Для *ValueObject* неизменяемость необходима.
* *Entity* может меняться. В ФП в таком случае создается *копия* с изменениями.

Пример изменения Entity в ФП стиле:

```fsharp
let initialPerson = { PersonId = PersonId 42; Name = "Joseph" }
let updatedPerson = { initialPerson with Name = "Joe" }
```

Преимущества изменений в ФП стиле - явное изменение видно в сигнатуре функции:

```fsharp
type UpdateName = Person -> Name -> Person
```

## Aggregates

Рассмотрим `Order` и `OrderLine`.

1. `Order` является Entity. Т.к. состав в заказе может меняться, при этом заказ остается тот же
самый.
2. `OrderLine` является Entity. У него может меняться количество товара, но при этом `OrderLine`
остается тем же самым.
3. Изменение `OrderLine` влечет за собой иземенение и `Order`.

Pseudocode для обновления цены order line:

```fsharp
// Три параметра:
// 1. order - заказ, в котором меняется строка
// 2. orderLineId - id строки заказа для изменения
// 3. newPrice - новая цена товара
let changeOrderLinePrice order orderLineId newPrice =
    // 1. find the line to change using the orderLineId
    let orderLine = order.OrderLines |> findOrderLine orderLineId

    // 2. make a new version of the OrderLine with the new price
    let newOrderLine = { orderLine with Price = newPrice }

    // 3. create a new list of lines, replacing the old line with the new line
    let newOrderLines =
        order.OrderLines |> replaceOrderLine orderLineId newOrderLine

    // 4. make a new version of the entire order,
    //    replacing all the old lines with the new lines
    let newOrder = { order with OrderLines = newOrderLines }

    // 5. return the new order
    newOrder
```

>В DDD Entity которые содержат другие Entity называются **Aggregate**.
>Или по другому - aggregate это коллекция domain objects, которые могут быть рассмотрены как
>single unit.
>Entity верхнего уровня называется **Aggregate root**.

### Aggregates Enforce Consistency and Invariants. (Агрегаты обеспечивают согласованность и инварианты)

Aggregate поддерживает согласованность внутренних данных: когда одна часть aggregate обновляется,
другие части также должны быть обновлены для обспечения согласованности.

Aggregate root - единственный компонент, который "знает" как сохранить согласованность.

Также aggregate поддерживает invariants. Например, может быть правило, что в заказе должна быть
хотя бы одна строка. Удаление этой строки должно вызвать ошибку. Aggregate гарантирует наличие
хотя бы одной строки.

### Aggregate References

Если нужно связать `Order` и `Customer`, то не надо делать так:

```fsharp
type Order = {
    OrderId : OrderId
    Customer : Customer
    OrderLines : OrderLine list
    // etc
}
```

Недостаток: при любом изменении `Customer` потребуется изменение `Order`.


Более грамотный подход - хранение *reference* (ссылки) на `Customer`:

```fsharp
type Order = {
    OrderId : OrderId
    CustomerId : CustomerId     // ссылка на customer
    OrderLines : OrderLine list
    // etc
}
```

Когда нам нужна полная информация о клиенте, мы получаем идентификатор `Customer` из `Order`,
а затем загружаем соответствующую информацию о `Customer`.

Другими словами, `Customer` и `Order` *различные* и *независимые* aggregates.
Каждый из них отвечает за свою собственную внутреннюю согласованность, и
единственная связь между ними осуществляется через их идентификаторы root объектов.

Это приводит к еще одному важному аспекту aggregates: они являются
*basic unit of persistence*. Если вы хотите загрузить или сохранить объекты из базы данных,
вы должны загружать/сохранять aggregates целиком.

Каждая транзакция базы данных должна работать с **одним**  и не должна включать в себя
несколько aggregates или пересекать их границы.

Аналогично для сериализации - допустима сериализация только одного aggregate целиком, а не по
частям.

### Summary of aggregates role

* Aggregate - коллекция domain objects, которые могут быть рассмотрены как single unit,
с Entity верхнего уровня, действующей как "root".
* Все изменения объектов внутри aggregate должны быть применены через
верхний уровень к root. Aggregate действует как согласованный объект, поэтому необходимо убедиться,
что все данные внутри aggregate обновляются правильно и одновременно.
* Aggregate - это атомарная единица хранения, транзакции базы данных и передачи данных.

>## More Domain-Driven Design Vocabulary
>
>Новые термины:
>
>* **Value Object** - domain object без идентификации. Два Value Objects включающие одинаковые
>данные, рассматриваются как идентичные. Value Objects должны быть неизменяемыми.
>Примеры Value Objects: имена, адреса, пункты назначения, деньги, даты.
>
>* **Entity** - domain object у которого есть идентификация, который сохраняется даже при изменении
>его свойств/данных. Entity objects обычно имеют ID или key поле. Два Entities с одинаковыми
>ID/key рассматриваются как один объект. Entities обычно представляют domain objects
>у которыхесть жизненный цикл и история изменений.
>Примеры Entities: customers, orders, products, and invoices.
>
>* **Aggregate** - коллекция objects, которые могут быть рассмотрены как один компонент для
>обеспечения согласованности в domain и использованы как атомарная единица при передачи данных.
>Другие Entities могут только ссылаться на aggregate по его идентификатору, где ID это
>верхний уровень aggregate (иначе называемого "root").

## Putting It All Together

Пример компилируемого кода, в котором есть описание domain model для бизнес-процесса "Place Order".

[Solution](fs/chapter05/chapter05.sln)
[Domain](fs/chapter05/OrderTaking/OrderTaking.fs)

Domain описан не весь, но компилируется.

## Wrapping Up

* Изучили как описывать domain при помощи F#. (И никаких типов вида `Manager` и `Handler`!).
* Ввели термины DDD: "Value Object", "Entity", "Aggregate".
* Описали типы в F#. Описание близко к документации и его можно скомпилировать.

# Links

* [Understanding type inference in F#](https://fsharpforfunandprofit.com/posts/type-inference/)

* [Data-oriented design](https://en.wikipedia.org/wiki/Data-oriented_design)
