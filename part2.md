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

# Links

* [Understanding type inference in F#](https://fsharpforfunandprofit.com/posts/type-inference/)
