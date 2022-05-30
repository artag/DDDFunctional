# Chapter 8. Understanding Functions

## Functions, Functions, Everywhere

Ключевым моментом парадигмы ФП является то, что функции используются *для всего* и *везде*.

Примеры отличия ООП подхода от ФП.

* Основные части/сущности из которых собираются большие программы:

  * ООП - классы и объекты.
  * ФП - функции.

* Иструменты параметризации и инструменты уменьшения coupling (связности) между компонентами:

  * ООП - interfaces и dependency injection.
  * ФП - parameterize with functions (параметризация с помощью функций).

* Don't repeat yourself (DRY):

  * ООП - inheritance или technique like the Decorator pattern.
  * ФП - кладем переиспользуемый код в функции и связываем их посредством composition (композиции).


## Functions Are Things

Функции могут быть передаваться на вход:

<img src="images/ch08_functions_as_input.jpg" alt="Function can be passed as input to other function" width=500 >

Функции могут быть возвращаться как выходные параметры:

<img src="images/ch08_functions_as_output.jpg" alt="Function can be returned as the output of a function" width=500 >

Функции могут быть передаваться как параметр функции для контроля ее поведения:

<img src="images/ch08_functions_as_parameter.jpg" alt="Function can be passed as parameter to a function" width=500 >

>**Higher-Order Functions** (**HOF**s)
>
>Functions that input or output other functions or take functions as parameters are
>called *higher-order functions*, often abbreviated to *HOF*s.

### Treating Functions as Things in F#

4 разных определения функции:

```fsharp
let plus3 x = x + 3             // plus3 : x:int -> int
let times2 x = x * 2            // times2 : x:int -> int
let addThree = plus 3           // addThree : (int -> int)
```

Анонимная (*lambda*) функция:

```fsharp
let square = (fun x -> x * x)   // square : x:int -> int
// аналогична
let square x = x * x            // square : x:int -> int
```

Функции можно put in a list:

```fsharp
// listOfFunctions : (int -> int) list
let listOfFunctions =
    [addThree; times2; square]
```

И можно делать loop:

```fsharp
for fn in listOfFunctions do
    let result = fn 100
    printfn "If 100 is the input, the output is %i" result
```

```text
// Result =>
// If 100 is the input, the output is 103
// If 100 is the input, the output is 200
// If 100 is the input, the output is 10000
```

### Functions as Input

Функция, которая принимает другую функцию в качестве входного параметра:

```fsharp
// evalWhith5ThenAdd2 : fn:(int -> int) -> int
let evalWith5ThenAdd2 fn =
    fn(5) + 2
```

Тест. Входная функция, используемая как параметр для `evalWith5ThenAdd2`:

```fsharp
let add1 x = x + 1          // an int -> int function
evalWith5ThenAdd2 add1      // становится add1(5) + 2
// ответ 8
```

Можно использовать *любую* функцию с сигнатурой `(int -> int)` в качестве входного параметра:

```fsharp
let square x = x * x        // an int -> int function
evalWith5ThenAdd2 square    // становится square(5) + 2
// ответ 27
```

### Function as Output

Функция, возвращающая lambda функцию:

```fsharp
// int -> (int -> int)
let adderGenerator numberToAdd =
    // return a lambda
    fun x -> numberToAdd + x
```

Функция, возвращающая именованную функцию:

```fsharp
let adderGenerator numberToAdd =
    // define a nested inner function
    let innerFn x =
        numberToAdd + x
    // return the inner function
    innerFn
```

Обе функции `adderGenerator` равнозначны.

Использование функции `adderGenerator`:

```fsharp
let add1 = adderGenerator 1
add1 2      // результат 3

let add100 = adderGenerator 100
add100 2    // результат 102
```

### Currying. (Каррирование)

*Currying* (*Каррирование*) - любая функция со множеством параметров может быть представлена
как ряд функций с одним параметром.

Например, функция с двумя параметрами:

```fsharp
// int -> int -> int
let add x y = x + y
```

может быть представлена в виде функции с одним параметром с помощью возвращаемой функции:

```fsharp
// int -> (int -> int)
let adderGenerator x = fun y -> x + y
```

В F# этот прием не надо делать - каждая из функций уже предствлена как curried function.

### Partial Application. (Частичное применение)

Применяя currying можно определить новую функцию: передать "baked in"
параметр и получить новую функцию с меньшим числом параметров.

Такой подход к "baked in" параметров называется *partial application* (*частичным применением*)
и является очень важным функциональным шаблоном.

Пример. Есть функция с двумя параметрами:

```fsharp
// string -> string -> unit
let sayGreetings greeting name =
    printfn "%s %s" greeting name
```

Создание двух новых функций с одним параметром (второй параметр "baked in"):

```fsharp
// string -> unit
let sayHello = sayGreetings "Hello"

// string -> unit
let sayGoodbye = sayGreetings "Goodbye"
```

Применение:

```fsharp
sayHello "Alex"     // результат: "Hello Alex"
sayGoodbye "Alex"   // результат: "Goodbye Alex"
```

## Total Functions

В ФП мы пытаемся сделать функции, чтобы каждому набору входных параметров соответствовал свой
набор выходных параметров (как в математических функциях). Такие функции называются
*total functions* (перевод неясен - полные/целые функции).

Пример. Функция деления 12 на какое-то целое число:

```fsharp
let twelveDivideBy n =
    match n with
    | 6 -> 2
    | 5 -> 2
    | 4 -> 3
    | 3 -> 4
    | 2 -> 6
    | 1 -> 12
    | 0 -> failwith "Can't divide by zero"
```

Один из вариантов деления на ноль - выбрасывание исключения. Но это противоречит сигнатуре
функции `int -> int`: не для каждого входного значения int будет возвращено другое значение int.
Иногда будет выбрасываться исключение.

Как сделать так, чтобы сигнатура `int -> int` была справедлива и не выбрасывались исключения?

### Решение 1. Restrict the input. (Ограничение ввода)

Ограничение диапазона входных значений. Для нашего примера деления:

```fsharp
type NonZeroInteger =
    // Ненулевые целые числа.
    // Сюда добавить smart constructor и т.д.
    private NonZeroInteger of int

// Использование ограниченного ввода

// twelveDivideBy : NonZeroInteger -> int
let twelveDividedBy (NonZeroInteger n) =
    match n with
    | 6 -> 2
    // ...
    // 0 не может быть на входе и его не требуется обрабатывать
```

### Решение 2. Extend the output. (Расширение выходных данных)

Во входных значениях допускается 0, но выходное значение может быть либо *valid int*, либо
*undefined value*.

В F# используется тип `Option` для представления выбора между "something" и "nothing". Пример:

```fsharp
// twelveDivideBy : int -> int option
let twelveDividedBy n =
    match n with
    | 6 -> Some 2       // valid
    | 5 -> Some 2       // valid
    | 4 -> Some 3       // valid
    // ...
    | 0 -> None         // undefined
```

## Composition. (Композиция/составление)

*Function commosition* - комбинирование функций путем присоединения выхода первой ко входу второй:

<img src="images/ch08_functions_compositions.jpg" alt="Two functions composition" width=600>

В итоге получается:

<img src="images/ch08_composed_function.jpg" alt="Composed function" width=600>

Основная фишка composition: *information hiding* (сокрытие информации).

### Composition of Functions in F#

В F# для composition используется "piping" - символы `|>`.

```fsharp
let add1 x = x + 1          // int -> int
let square x = x * x        // int -> int

let add1ThenSquare x =
    x |> add1 |> square

// Тест
add1ThenSquare 5            // Результат: 36
```

Графически это выглядит так:

<img src="images/ch08_functions_pipeline.jpg" alt="Function created with piping" width=400>

Еще пример:

```fsharp
let isEven x =                  // int -> bool
    (x % 2) = 0

let printBool x =               // bool -> string
    sprintf "value is %b" x

let isEvenThePrint x =
    x |> isEven |> printBool    // int -> string

// Тест
isEvenThenPrint 2               // Результат: "value is true"
```

### Building an Entire Application from Functions

Комбинирование функций позволяет из фукнций построить приложение целиком.

Начало:

<img src="images/ch08_lowlevel_function.jpg" alt="Low-level operation" width=550>

Далее:

<img src="images/ch08_service_function.jpg" alt="Service level" width=550>

Далее:

<img src="images/ch08_workflow_function.jpg" alt="Workflow level" width=550>

Наконец, несколько workflow можно объединить параллельно и создать контроллер/диспетчер для
выбора нужного workflow для соответствующих входных данных:

<img src="images/ch08_application_function.jpg" alt="ApplicationV level" width=450>

### Challenges in Composing Functions. (Проблемы при композиции функций)

Composition функций делать легко при совпадении соответствующих входов и выходов. Как быть
если они не совпадают?

Пример.

Выход одной функции `Option<int>`, а другая требует на вход `int`.
Или наоборот, Первая фукция возвращает `int`, а вторая требует на входе `Option<int>`:

<img src="images/ch08_shapes_different.jpg" alt="Functions compose problems" width=550>

Похожие проблемы возникают при использовании типов `Result`, `async` и т.д.

Наиболее популярный подход к решению подобных проблем, это конвертация обоих сторон
(выхода и входа) к "lowest common multiple" ("наименьшее общее кратное").

Например так:

<img src="images/ch08_lowest_common_multiple.jpg" alt="Lowest common multiple example" width=550>

Пример в коде:

```fsharp
// int -> int
let add1 x = x + 1

let printOption x =
    match x with
    | Some i -> printfn "The int is %i" i
    | None -> printfn "No value"

// Connect
5 |> add1 |> Some |> printOption
```

## Wrapping Up

В этой главе:

* знакомство с основными концепциями функционального программирования в F#
* использование функций в качестве строительных блоков
* проектирование функций таким образом, чтобы они были составными.

# Chapter 9. Implementation: Composing a Pipeline

Напоминание. Stages в проектируемом pipeline:

* Начать с `UnvalidatedOrder`, конвертировать его в `ValidatedOrder`. В случае ошибки валидации
возратить ошибку.
* `ValidatedOrder` конвертировать в `PricedOrder`.
* Из `PricedOrder` создать acknowledgment письмо и послать его.
* Создать набор events (событий) и вернуть их.

Итоговая функция для workflow будет выглядеть примерно так:

```fsharp
let placeOrder unvalidatedOrder =
    unvalidatedOrder
    |> validatedOrder
    |> priceOrder
    |> acknowledgeOrder
    |> createEvents
```

Шаги разработки:

1. Реализация каждого шага workflow в виде отдельной функции. Каждая из функций должна быть
stateless, без side effects.

2. Compose (составление/компоновка/комбинирование) функций.

Со вторым шагом могут возникнуть некоторые сложности - некоторые из функций не могут быть
compose (составлены/скомбинированы) из-за:

* Функции с дополнительными параметрами, которые не являются частью data pipeline,
но необходимы для реализации - "зависимости".

* Функции с явным описанием "эффектов", таких как `Result`, `Async`. Они не могут быть compose
(скомбинированы) с функциями, которые ожидают обычные данные на входе.

В этой главе будет реализован шаг 1. В следующей главе шаг 2.
Вначале, для упрощения, будут созданы функции без "эффектов" - без использования `Result`, `Async`
и т.п.

## Working with Simple Types

Сначала определяются simple types (простые типы), такие как `OrderId` и `ProductCode`.

У каждого simple type должно быть минимум по 2 фунции:

* Функция `create` - smart constructor (умный конструктор), который создает тип с проверкой
входного значения. Если проверка не проходит, то возвращает ошибку.

* Функция `value`. Извлекает внутреннее значение.

Обычно определение simple type и его функции лежат в одном файле.

Пример определения `OrderId` в модуле `Domain`:

```fsharp
module Domain =
    type OrderId = private OrderId of string

    module OrderId =
        /// Define a "Smart constructor" for OrderId
        /// string -> OrderId
        let create str =
            if String.IsNullOrEmpty(str) then
                // use exceptions rather than Result for now
                failwith "OrderId must not be null or empty"
            elif str.Length > 50 then
                failwith "OrderId must not be more than 50 chars"
            else
                OrderId str

        /// Extract the inner value from an OrderId
        /// OrderId -> string
        let value (OrderId str) =       // unwrap in the parameter
            str                         // return the inner value

```

*Замечания*:

* В умном конструкторе при ошибке валидации используется выброс исключений. Это временно и сделано
для упрощения проектирования. Потом будет заменено на возврат `Result`.
* Функция `value` демонстрирует, как можно сделать pattern-matching и извлечь
внутреннее значение за один шаг, непосредственно в параметре.

## Using Function Types to Guide the Implementation

Определение функции обычным способом:

```fsharp
let validateOrder
    checkProductCodeExists          // dependency
    checkAddressExists              // dependency
    unvalidatedOrder =              // input
    ...
```

Определение функции в определенной сигнатуре (предопределенный тип):

```fsharp
// define a function signature
type MyFunctionSignature = Param1 -> Param2 -> Result

// define a function that implements that signature
let myFunc : MyFunctionSignature =
    fun param1 param2 ->
        // ...
```

Определение функции `validateOrder` будет выглядеть так:

```fsharp
let validateOrder : ValidateOrder =
    fun checkProductCodeExists checkAddressExists unvalidatedOrder ->
     // ^dependency            ^dependency        ^input
```

Применение такого подхода позволяет локализовать ошибку реализации функции в ее пределах,
а не на этапе компоновки всех функций.

Компилятор всегда пытается вывести тип входных и выходных параметров функции из контекста их
использования. Но при таком походе, все параметры и возвращаемое значение функции имеют заранее
заданные типы. Поэтому, если будет сделана ошибка в реализации функции, то она будет сразу видна
как локальная ошибка внутри определения функции.

При обычном определении функции подобного рода ошибки обычно появляются на этапе
компоновки нескольких функций.

## Implementing the Validation Step

Реализация validation (проверочного) step.

На вход поступает unvalidated order с полями-примитивами, который преобразуется в правильный,
полностью проверенный объект domain (домена).

Описание функций:

```fsharp
type CheckProductCodeExists =
    ProductCode -> bool

type CheckAddressExists =
    UnvalidatedAddress -> AsyncResult<CheckedAddress, AddressValidationError>

type ValidateOrder =
    CheckProductCodeExists                                      // dependency
        -> CheckAddressExists                                   // AsyncResult dependency
        -> UnvalidatedOrder                                     // input
        -> AsyncResult<ValidatedOrder, ValidationError list>    // output
```

Как говорилось выше, вначале, для упрощения, будут созданы функции без "эффектов" - без
использования `Result`, `Async` и т.п.

Поэтому на данном этапе вместо эффектов в случае ошибки будет кидаться исключение (потом, на
следующих этапах, все будет сделано как надо):

```fsharp
type CheckProductCodeExists =
    ProductCode -> bool

type CheckAddressExists =
    UnvalidatedAddress -> CheckedAddress

type ValidateOrder =
    CheckProductCodeExists      // dependency
        -> CheckAddressExists   // dependency
        -> UnvalidatedOrder     // input
        -> ValidatedOrder       // output
```

Пример описания типов, которые требуются для реализации validation step:

* Файл [SimpleTypes.fs](fs/chapter09/OrderTaking/SimpleTypes.fs). Описание simple types
(простых типов) данных. Сделаны по алгоритму, описанному выше, в разделе
"Working with Simple Types".

  * `NonEmptyString`
  * `String10`
  * `String50`
  * `EmailAddress`
  * `ZipCode`
  * `OrderId`
  * `OrderLineId`
  * `ProductCode`, состоит ("OR") из `WidgetCode` и `GizmoCode`
  * `OrderQuantity`, состоит ("OR") из `UnitQuantity` и `KilogramQuantity`

* Файл [PublicTypes.fs](fs/chapter09/OrderTaking/PublicTypes.fs). Описание типов данных, доступных
"снаружи" domain.

  * `UnvalidatedAddress`
  * `UnvalidatedCustomerInfo`
  * `UnvalidatedOrderLine`
  * `UnvalidatedOrder`

* Файл [CompoundTypes.fs](fs/chapter09/OrderTaking/CompoundTypes.fs). Описание составных типов данных.

  * `PersonalName`
  * `CustomerInfo`
  * `Address`

* Файл [PlaceOrder.fs](fs/chapter09/OrderTaking/PlaceOrder.fs). Описание validation step, функций
и типов данных, необходимых для его реализации.

  Данные:

  * `CheckedAddress`
  * `ValidatedOrderLine`
  * `ValidatedOrder`

  Функции:

  * `CheckProductCodeExists`
  * `CheckAddressExists`
  * `ValidateOrders`