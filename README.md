# DDDFunctional

*Materials from book - "Domain Modeling Made Functional" by Scott Wlaschin*

# Chapter 1. Introducing Domain-Driven Design

## The Importance of a Shared Model. (Преимущества общей модели)

<table>
<tr>
<td>

![Shared model version 1](images/ch01_shared_model_1.jpg)

</td>
<td>

![Shared model version 2](images/ch01_shared_model_2.jpg)

</td>
<td>

![Shared model version 3](images/ch01_shared_model_3.jpg)

</td>
</table>

Преимущества общей модели (третий рисунок):

* *Faster time to market*. When the developer and the codebase share the
same model as the person who has the problem, the team is more likely
to develop an appropriate solution quickly.

* *More business value*. A solution that is accurately aligned with the problem
means happier customers and less chance of going offtrack.

* *Less waste*. Clearer requirements means less time wasted in misunderstanding
and rework. Furthermore, this clarity often reveals which components are high
value so that more development effort can be focused on them and less on the
low-value components.

* *Easier maintenance and evolution*. When the model expressed by the code
closely matches the domain expert’s own model, making changes to the
code is easier and less error-prone. Furthermore, new team members are
able to come up to speed faster.

Методические рекомендации по созданию shared model:

* Focus on business events and workflows rather than data structures.

* Partition the problem domain into smaller subdomains.

* Create a model of each subdomain in the solution.

* Develop a common language (known as the "Ubiquitous Language") that
is shared between everyone involved in the project and is used everywhere
in the code.

## Шаг 1. Understanding the Domain Through Business Events

Часто началом работы/запуска какого-либо процесса является:

* *внешний триггер* (a piece of mail arriving or your phone ringing)
* *time-based триггер* (you do something every day at 10 a.m.)
* *наблюдение* (there are no more orders in the inbox to process, so do something else)

>We call these things **Domain Events**.

* Domain Events are the starting point for almost all of the business processes we want to model.

For example, "new order form received" is a Domain Event that will kick off the
order-taking process.

* Domain Events are always written in the past tense (прошедшее время) - something happened.

### Шаг 1.1 Using Event Storming to Discover the Domain

Есть несколько способов discover the domain - один из них *Event Storming*.

Основные положения Event Storming:

* Bring together a variety of people (who understand different parts of the domain).
Собираются все, кто хоть что-то может спросить/сказать по поводу проекта.

* Проводится в помещении, где есть большая доска/стена, на которую можно лепить стикеры и/или
рисовать.

* Одни записывают деловые события на стикеры и размещают их на стене. Другие могут ответить,
разместив заметки с кратким описанием бизнес-процессов, которые запускаются этими событиями.
Эти рабочие процессы, в свою очередь, часто приводят к созданию других бизнес-событий.

* Заметки организованы в виде временной шкалы - распололагаются слева направо.

* Если вопросы не имеют четкого ответа, то они помещаются на доску/стену. Они
  служат триггерами для последующих обсуждений.

### Пример. Discovering the Domain: An Order-Taking System

Первый шаг: Focus on business events - "накидывание" Domain Events.
Получается множество событий наподобие:

* Order form received
* Order placed
* Order shipped
* Order change requested
* Order cancellation requested
* Return requested
* Quote form received
* Quote provided
* New customer request received
* New customer registered

И как оно может выглядеть на доске/стене:

<img src="images/ch01_domain_events.jpg" alt="Domain events after event storming" width=600 >

`Place order` и `Ship order` это business workflows (бизнес-процессы), мы начинаем понимать,
как события объединяются в более крупные рабочие процессы.

Выгоды от использования Event Storming:

* *A shared model of the business*
  * Участники участвуют в разработке общего понимания бизнес-процессов.
  * Все наглядно видят одно и тоже.
  * Многие могут уточнить детали взаимодействия, понимания и т.д.

* *Awareness of all the teams (осведомленность всех команд)*
  * Более полное взаимодействие между разными командами.
  * Может быть обнаружено, что другим командам тоже нужны результаты работы/процессов.

* *Finding gaps (пробелы/недочеты) in the requirements*
  * Требования в начале обсуждения могут быть нечеткими.
  * Наглядность может показать какие-то недочеты в требованиях.
  * Если вопрос не имеет четкого ответа, то он сам должен быть помещен на доску/стену. Он
  послужит триггером для дальнейших обсуждений.
  * Споры или разногласия это проблема, а как возможность улучшения и уточнения бизнес-процессов.

* *Connections between teams*
  * События могут быть сгруппированы по временной шкале (timeline).
  * Часто такие группировки показывают соединения - team's output is another team's input.

<img src="images/ch01_team_connection.jpg" alt="Connections between teams" width=600 >

* *Awareness of reporting requirements*
  * Отчетность (reporting) всегда является частью предметной области.
  * Отчеты и другие доступные только для чтения модели (например, модели представления для
  пользовательского интерфейса) включаются в сеанс анализа событий.

### Шаг 1.2. Expanding (расширение) the Events to the Edges

Рекомендуется проследить цепочку событий как можно дальше, до границ системы.

* Происходят ли какие-либо события перед самым крайним левым событием?

* Какие события происходят после завершения самого крайнего правого события?

```text
Пример:
Мы: "Какое событие происходит перед начальным событием "Order received"?

Ответ: "We open the mail every morning, and the customers send in order forms
on paper, which we open up and classify as orders or quotes."

Мы: "So it looks like we need a "Mail received" event as well".

Мы: "Are there any possible events after you ship the order to the customer?”

Ответ: "If the order is "Signed for delivery" we'll get a notification from the
courier service. So let me add a "Shipment received by customer" event."
```

<img src="images/ch01_events_on_edges.jpg" alt="Expanding the Events to the Edges" width=800 >

>### Workflows, Scenarios, and Use Cases
>
>* A **scenario** describes a goal that a customer (or other user) wants to achieve,
>such as placing an order. It is similar to a "story" in agile development.
>* A **use case** is a more detailed version of a scenario, which describes in general terms
>the user interactions and other steps that the user needs to take to accomplish
>a goal. Both scenario and use case are user-centric concepts, focused on how
>interactions appear from the user's point of view.
>* A **business process** describes a goal that the business (rather than an individual
>user) wants to achieve. It's similar to a scenario but has a business-centric focus
>rather than a user-centric focus.
>* A **workflow** is a detailed description of part of a business process. That is, it lists
>the exact steps that an employee (or software component) needs to do to
>accomplish a business goal or subgoal. We’ll limit a workflow to what a single
>person or team can do, so that when a business process is spread over multiple
>teams (as the ordering process is), we can divide the overall business process
>into a series of smaller workflows, which are then coordinated in some way.

### Шаг 1.3. Documenting Commands

После помещения всех Domain Events на доску/стену необходимо задать вопрос:
"What made these Domain Events happen? (Что было причной появления этих Domain Events?)"

>We call these requests **commands** in DDD terminology.

* Commands are always written in the imperative (повелительное наклонение, неопределенная форма
глагола): "Do this for me."

* Не все команды всегда успешно выполняются (но это рассматривается позже, вначале интересно
успешное выполнение команды).

* Если команда успешно выполнена, она запускает workflow, который создает
соответствующий Domain Events.

```text
Примеры:

1) If the command was "Make X happen" then, if the workflow made X happen,
the corresponding Domain Event would be "X happened."
2) If the command was "Send an order form to Widgets Inc" then, if the workflow sent the order,
the corresponding Domain Event would be "Order form sent."
3) Command: "Place an order"; Domain Event: "Order placed."
4) Command: "Send a shipment to customer ABC"; Domain Event: "Shipment sent."
```

Итого, цепочка такая:

<img src="images/ch01_event_cmd_workflow.jpg" alt="Event-Command-Workflow" width=800 >

Применительно к сценарию Order-taking process:

<img src="images/ch01_order-taking-process.jpg" alt="Order-taking process" width=800 >

## Шаг 2. Partitioning the Domain into Subdomains

После шага 1 есть список команд и событий. Но общая картина пока что хаотична.

Следующий шаг: Partition the problem domain into smaller subdomains.

В примере весь "order-taking process" можно разделить: the order taking, the shipping, the billing,
and so on.

В разделении подобного рода может помочь физическое разделение бизнеса по отделам: отдел продаж,
отдел доставки и т.д. Каждый из этих отделов можно рассматривать как domain.

>**"Domain"** - an area of coherent knowledge (область логически связанных знаний).
>
>Альтернативное определение: **"domain"** is just that which a "domain expert" is
>expert in.

Внути домена могут быть области - subdomains.

>**Subdomains** - a smaller part of a larger domain that has its own specialized knowledge.

Пример subdomain: "web programming" это subdomain of "general programming." И "JavaScript
programming" is a subdomain of web programming.

Внизу, на картинке слева можно увидеть пример programming-related domains.

Домены могут иметь области пересечения. В реальности границы доменов могут быть размытыми,
нечеткими. В наших моделях необходимо выделить subdomains с более четкими границами.

На картинке справа приведен domain-partitioning approach к order-taking process:

<table>
<tr>
<td>

![Programming-related domains](images/ch01_programming-related.jpg)

</td>
<td>

![Domain-partitioning approach](images/ch01_domain-partitioning.jpg)

</td>
</table>

Здесь domains немного перескаются. An order-taker (принимаюзий заказы) должен немного знать
о том как работают отделы billing и shipping. Остальные отделы также должны хоть что-то знать
о своих соседях для взаимодействия.

## Шаг 3. Creating a Solution Using Bounded Contexts. (Создание решения с использованием ограниченных контекстов)

Расмматриваемый домен может содержать много информации. Для решения определенной проблемы бизнеса
необходимо выделить только ту информацию, которая требуется. Все остальное не имеет значения.

Необходимо провести различие между "problem space" ("пространством проблем") и
"solution space" ("пространством решений").

Чтобы построить решение надо создать *модель* проблемной области, извлекая только
те аспекты предметной области, которые имеют отношение к делу:

<img src="images/ch01_problem-solution.jpg" alt="Problem space and solution space" width=800 >

>В solution space видно что domains and subdomains в problem space отображаются на то,
>что в терминах DDD называется **bounded contexts** (ограниченные контексты).

Каждый bounded context это мини модель domain со своими правами.

Почему *context*? Каждый контекст содержит определенные знания. В пределах контекста есть
общий язык и логически-связный дизайн. Но вне контекста информация, взятая из него, может быть
искажена или непригодна к использованию.

Why *bounded* (ограниченный)? В реальном мире домены имеют нечеткие границы и содержат много
информации. Domain model никогда не будет такой богатой, как в реальном мире будет иметь более
четкие границы.

Domain в problem space не всегда имеет однозначную связь с контекстом в solution space.
Иногда domain разбивается на несколько bounded contexts (ограниченных контекстов).
Или несколько domains в problem space моделируется только одним bounded context в
solution space.

Последнее часто встречается, когда необходимо интегрироваться с legacy.

При разделении домена, важно, чтобы каждый bounded context нес четкую ответственность.
Т.к. при реализации модели, каждый ограниченный контекст четко соответствовует какому-то
программному компоненту. Компонентом может быть:

* Отдельная сборка DLL
* Автономный сервис
* Простой namespace (пространство имен)

### Getting the Contexts Right. (Правильное понимание/определение контекстов)

Наибольшая сложность в domain-driven design это правильное определение context boundaries.

Рекомендации:

* *Listen to the domain experts* (слушайте доменных экспертов).

* *Pay attention to existing team and department boundaries* (обратите внимание на существующие
границы команд и отделов). Эти границы могут помочь при выделении domains and subdomains.
Но это может не всегда работать.

* *Don't forget the "bounded" part of a bounded context* (не забывайте о "ограниченной" части
ограниченного контекста). Необходимо следить за изменениями границ при изменениях требований и
условий. Границы должны быть четкими и по возможности неизменяемыми.

* *Design for autonomy* (ориентации при проектировании на автономность). Всегда лучше иметь
независимый и автономный bounded contexts, который можно разрабатывать независимо от остальных.

* *Design for friction-free business workflows* (проектирование с учетом наименьших конфликтов
между различными бизнес-процессами). Если бизнес-процесс взаимодействует с несколькими bounded
contexts и часто блокируется или задерживается ими, то стоит переделать эти contexts.
Отдавать приоритет простоте бизнес-процесса над красотой дизайна.

* *No design is static*. Ни один из дизайнов не является неизменным: все они меняются при
изменениях в бизнес требований.

### Шаг 3.2. Creating Context Maps. (Создание карт контекста)

После определения contexts нужно рассмотреть взаимодействия между ними.
Надо создать общую картину взаимодействий не вдаваясь в детали дизайна.

>В DDD такие диаграммы называются **Context maps** (контекстные карты/карты контекста).

Пример: карта маршрута для путешествий. Она не показывает всех деталей, она фокусируется только
на основных маршрутах. Например, вот набросок карты маршрута авиакомпании:

<img src="images/ch01_airline-route-map.jpg" alt="Airline route map" width=600 >

По данной карте можно узнать, как перемещаться между городами. Если необходимо что-то другое,
например поездка в окрестностях New York, то нужна другая карта.

Context map показывает несколько bounded contexts и их взаимодействия. Цель - не отобразить
все детали, но показать всю систему в целом.

Пример context map для order-taking system:

<img src="images/ch01_context-map.jpg" alt="Context map for order-taking system" width=600 >

* При создании context map нас не интересует внутренняя структура shipping context. Интересует
только то, что shipping context получает данные от order-taking context.
Мы неявно говорим, что shipping context находится *downstream* (нисходящий информационный поток),
а order-taking context в *upstream*.

* Два этих контекста должны будут согласовать общий формат для обмена сообщениями.

  * Как правило, upstream задает формат обмена.

  * Иногда downstream является негибким (legacy system) и либо восходящий контекст должен
  адаптироваться, либо используется вводится translator component (посредник-преобразователь).

* В сложных проектах создается серия context maps, каждая из которых описывает определенную
подсистему.

### Шаг 3.3. Focusing on the Most Important Bounded Contexts. (Фокусирование на наиболее важных ограниченных контекстах)

На данный момент у нас есть несколько bounded contexts. При дальншей работе над domain их может
стать больше. Но все ли они одинаково важны? С чего начать разработку?

Как правило наиболее важны domains, которые обеспечивают бизнес-преимущество. Т.е те,
которые приносят деньги. Другие домены также могут требоваться, но они не являются ключевыми.

>Домены, которые обеспечивают бизнес-преимущество (приносят деньги) называются **core domains**
>(основные домены).
>
>Остальные домены называются **supportive domains** (вспомогательные домены).
>Если они неуникальны для бизнеса, то они называются **generic domains** (общие домены).

Для рассматриваемого примера:

* order-taking и shipping domains являются core (основные) domains, т.к. их бизнес-преимуществом
является превосходное обслуживание клиентов.

* billing domain будет рассматривается как supportive (вспомогательный) domain.

* delivery of the shipments рассматривается как generic (общий) domain. - Можно передать на
outsource.

В реальности может быть все не так просто. Иногда core domain является не тот, что ожидался.
Бизнес электронной коммерции может обнаружить, что наличие товаров на
складе и готовность к отправке имеют решающее значение для удовлетворенности клиентов.
В этом случае inventory management может стать core domain, столь же важной для
успеха бизнеса, как и простой в использовании веб-сайт.

Иногда нет единого мнения о том, что является самой важной областью; каждый отдел может считать,
что его область является наиболее важной. А иногда core domain - это просто чтобы все работало.

Однако во всех случаях важно расставлять приоритеты, и не пытаться реализовать все bounded contexts
одновременно - это часто приводит к неудаче. Вместо этого надо сосредоточиться на bounded contexts,
которые добавляют наибольшую ценность.

## Creating a Ubiquitous Language. (Создание общего языка)

Разработчик и доменный эксперт должны использовать одну и ту же модель.

Это значит, что если доменный эксперт называет что-то `"order"`, то и разработчик должен
использовать в своей работе что-то под названием `Order`, которое ведет себя похожим образом.

Разработчику не следует использовать термины, которые не знакомы доменному эксперту:

* `OrderFactory`
* `OrderManager`
* `OrderHelper`

>Набор понятий и словарного запаса, который является общим для всех членов команды,
>называется **Ubiquitous Language** (вездесущим языком).

Этот язык должен во всех областях проекта:

* в требованиях
* в дизайне
* в исходном коде (самое главное).

Ubiquitous Language постоянно развивается и меняется вместе с дизайином и бизнесом.

В каждом контексте будет свой "диалект" Ubiquitous Language: одно и то же слово может означать
разные вещи. Например, "Customer" или "Product" в различных контекстах могут заметно отличаться.

## Summarizing the Concepts of Domain-Driven Design

* **Domain** (домен/предметная область) это область знаний, связанная (ассоциирующаяся) с
проблемой, которую пытаемся решить или просто, в которой "domain expert" является экспертом.

* **Domain Model** (модель домена/предметной области) это набор упрощений, представляющих те
аспекты domain (домена/предметной области) которые относятся к конкретной проблеме.
Domain model это часть solution space (пространства решений), тогда как domain - это часть
problem space (пространства проблем).

* **Ubiquitous Language** это набор понятий и словарного запаса, которые соответствуют domain.
Это язык, общий для команды и исходного кода.

* **Bounded context** (ограниченный контекст) это подсистема в solution space (пространстве решений)
с четкими границами , которые четко отделяют ее от друших подсистем. A bounded context
часто соответствует subdomain в the problem space (пространтве проблем). A bounded context
также имеет свой набор понятий и словарь, свой диалект в Ubiquitous Language.

* **Context Map** (карта контекста) это высокоуровневая диаграмма, показывающая bounded contexts
и взаимодействия между ними.

* **Domain Event** (доменное событие/событие предметной области) - запись о чем-то, что произошло
в системе. Обычно описывается в прошедшем времени. Событие часто является триггером для
других действий.

* **Command** - это запрос на выполнение какого-либо действия/процесса, который инициируется
человеком или другим событием. Если дествие/процесс завершается успешно, состояние системы
изменяется и записывается одно или несколько Domain Events (событий домена).

## Wrapping Up. (Подведение итогов)

Важносто создать общую модель предметной области и решения. Модель, одинаковую для команды
разработчиков и экспертов предметной области.

Четыре принципа, которые помогут сделать это:

* Focus on events and processes rather than data.
* Partition the problem domain into smaller subdomains.
* Create a model of each subdomain in the solution.
* Develop an "everywhere language" that can be shared between everyone involved in the project.

### Events and Processes

The event-storming session quickly revealed all the major Domain Events in the domain.

### Subdomains and Bounded Contexts

We have discovered three subdomains so far: "Order Taking", "Shipping" and "Billing".

We then defined three bounded contexts to correspond with these subdomains
and created a context map that shows how these three contexts interact.

Which one is the core domain that we should focus on?

### The Ubiquitous Language

We have terms like "order form", "quote" and "order".

It would be a good idea to create a living document or wiki page that lists these terms
and their definitions.

# Chapter 2. Understanding the Domain

## Interview with a Domain Expert

Начинается более подробное выяснение деталей о каждом из domain. Это может быть серия коротких
совещаний с domain expert'ом(ами).

Здесь важно:

* Отбросить все свои предположения и домыслы (могут быть неверными).
* Уметь слушать.
* Исследовать область домена, задавая вопросы доменному эксперту.

### Understanding the Non-functional Requirements

В примере рассматривается выяснение деталей по the order-placing process.

У domain expert выяснено:

1. Заказчик знает, что он хочет заказать. Для заказа используется простая форма.
2. Заказчик вводит коды продуктов и их количество.
3. Наименований продуктов может быть несколько сотен, поэтому коды вводятся заказчиком "напрямую",
без выбора из страниц.

Нефункциональные требования:

1. Порядка 200 заказов каждый день. (Нет большой нагрузки).
2. Нагрузка постоянна в течение всего года. (Нет пиковых нагрузок).
3. Заказчики не новички. (Дизайн/бизнес-процесс для чайников отличается от дизайна/бизнес-процесса
для экспертов).
4. Скорость для заказчика не важна. Подтверждение ожидается в течение дня.
5. Важна надежность: обратная связь, аудит на каждом из этапов обработки заказа.

### Understanding the Rest of the Workflow

Функциональные требования:

1.1 После получения формы коды товаров на форме сверяются со справочником product catalog.

1.2 Product catalog обновляется раз в месяц.

1.3 Открытие **новой детали** в системе: Product catalog - это плюс еще один bounded context.
В будущем по нему необходимо выяснить более подробные сведения.

2.1 После проверки кодов товаров считается суммарная стоимость всех products.

2.2 Сумма  записывается в поле "Total".

3.1 Order сканируется и делаются 2 копии.

3.2 Оригинал остается в департаменте.

3.3 Одна копия идет в shipping department (доставка), другая - в billing department (выставление
счетов).

3.4 Скан order отправляется заказчику, чтобы он увидел окончательные цены. Это называется
"order acknoledgment" (подтверждение заказа).

4.1 У формы есть два флажка для заполнения "Quote" или "Order".

4.2 Для "Quote" просто считаются цены на продукты и форма отправляется обратно заказчику.
Никакие копии не отправляются в другие отделы shipping и billing.

### Thinking About Inputs and Outputs

Итак **Input** (вход) - это **order form** (форма заказа).

>**Output** для workflow должен быть всегда events (события), которые генерируются.
>Events являются триггерами действий, которые запускаются в других bounded contexts.

В нашем случае **Output** - это "**Order Placed**" (заказ размещен). Это событие посылается
в shipping и billing департаменты.

<img src="images/ch02_place_order_event.jpg" alt="Place Order event" width=600 >

## Fighting the Impulse to Do Database-Driven Design

Если у разработчика большой опыт работы с БД, то он может начать разработку приложения с
таблиц БД.

На данном этапе можно запланировать создание следующих таблиц их relationships:

* `Order`
* `OrderLine`
* `Customer`
* `Address`
* `Product`

<img src="images/ch02_domain_dd.jpg" alt="Database-driven design" width=600 >

**Это ошибка**. В domain-driven design, *domain* задает design, а не база данных:

1. В реальном мире нет баз данных.
2. Конепция "база данных" не часть ubiquitous language. Пользователям не важно как обрабатываются
данные.

>В терминах DDD такое игнорирование обработки данных называется **persistence ignorance**.

3. Если проектировать дизайн с точки зрения базы данных, то дизайн будет искажен.
4. Не все бизнес правила можно просто и точно отразить в структуре БД.
5. Дизайн может (и будет) меняться. В БД сложно менять уже существующую структуру.

## Fighting the Impulse to Do Class-Driven Design

Если у разработчика большой опыт работы с ООП, то проектирование домена с точки зрения
классов может также исказить дизайн.

Например, для разделения заказов на `Order` и `Quote` разработчик может создать побочный класс
`OrderBase`, которого нет в реальности. Это искажение домена. Попробуйте попросить
объяснить domain expert что такое `OrderBase`.

<img src="images/ch02_class_dd.jpg" alt="Class-driven design" width=600 >

**Итоговые выводы** после этих двух подразделов: во время сбора требований держите разум
открытым и не навязывайте домену свои технические мысли, идеи и реализации.

## Documenting the Domain

Как задокументировать требования без "технического" влияния?

Можно использовать визуальные диаграммы (такие как UML), но:

1. С ними часто сложно работать.
2. Они недостаточно подробны, чтобы охватить некоторые тонкости предметной области.

Решение: использование простого text-based языка (простое текстовое описание).
Преимущества такого описания в том, что его можно показать domain expert.

Используется text-based язык со следующими правилами:

* Для workflows (бизнес процессов), документируем входы и выходы, для бизнес логики используем
простой псевдокод.
* Для структур данных используются:
  * **AND** - требуются все виды/типы данных. Например:

  ```text
  Name AND Address
  ```

  * **OR** - требуется либо одни данные, либо другие. Например:

  ```text
  Email OR PhoneNumber
  ```

### Документирование `Place Order`

Вот как можно задокументировать `Place Order` workflow:

```text
Bounded context: Order-Taking

Workflow: "Place order"
    triggered by:
        "Order form received" event (where Quote is not checked)
    primary input:
        An order form
    other input:
        Product catalog
    output events:
        "Order Placed" event
    side-effects:
        An acknowledgement is sent to the customer, along with the placed order
```

Документация структур данных для этого бизнес-процесса:

```text
Bounded context: Order-Taking

data Order =
    CustomerInfo =
    AND ShippingAddress
    AND BillingAddress
    AND list of OrderLines
    AND AmountOfBill

data OrderLine =
    Product
    AND Quantity
    AND Price

data CustomerInfo = ??      // don't know yet
data BillingAddress = ??    // don't know yet
```

Аналогично делается для заказа вида Quote и соответствующих структур данных.

## Diving Deeper into the Order-Taking Workflow

Продолжение выяснения деталей по the order-placing process.

1.1 Каждый день с утра, после получения писем, идет их сортировка.

1.2 Формы заказа Order помещаются в одну стопку, с Quote - в другую.

1.3 Order'ы более важны, т.к. именно они приносят деньги.

1.4 Обработка Order'ов идет в первую очередь, Quote обрабатываются позже.

**Итог**. Здесь видно, что приоритет при разработке следует отдать обработке заказов в виде `Order`, т.к.
именно это приносит деньги.

2.1 После сортировки проверяется валидность customer's name, email, shipping address,
billing address.

2.2 Адреса проверяются через сторонний сервис (приложение).

2.3 После проверки адреса преобразуются в стандартный формат, который принят у delivery service.

3.1 Если имя и адрес not valid, то они помечаются красным цветом на форме заказа.

3.2 Not valid форма заказа помещается в специальную стопку с not valid заказами.

3.3 Потом человек обзванивает заказчиков с not valid заказами и просит их скорректировать свои
заказы.

**Итог**. Здесь видно, что есть три очереди из форм заказов, одна из которых наиболее приоритетна.

4.1 После идет валидация кодов продуктов, указанных в форме заказа.

4.2 Коды продуктов Widgets начинаются с `W`, далее идут 4 цифры.

4.3 Коды продуктов Gizmos начинаются с `G`, далее идут 3 цифры.

4.4 Коды продуктов не меняются. Новые виды кодов не добавляются.

4.5 Валидные коды продуктов есть у проверяющего в справочнике.

4.6 Если код продукта из формы не найден в справочнике, то форма помечается как invalid и
кладется в стопку с not valid заказами.

4.7 Работа проверяющего должна быть автономной и не зависеть от других продуктовых команд.

**Итог**. Здесь видно:

* Проверка формата кодов продуктов чисто синтактическая.
* Справочник в виде книги может быть сделан в виде БД.
* Служба, обрабатывающая заказы должная быть автономной и не зависеть от других служб и команд.

5.1 Проверка количества товара

5.2 Widget'ы продаются по unit'ам (поштучно). Количество указывается в виде целых чисел.

5.3 Gizmos продаются в килограммах. Килограммы представлены в виде десятичных чисел.

6.1 Для каждого продукта вычисляется его суммарная цена (цена x количество).

6.2 Цены всех товаров суммируются и вычисляется total amount to bill (общая выставляемая сумма).

7.1 Делается две копии формы заказа.

7.2 Первая копия заказа идет в shipping outbox.

7.3 Вторая копия заказа идет в billing outbox.

7.4 Скан формы заказа прикрепляется к email с подтверждением и отправляется обратно клиенту.

## Representing Complexity in Our Domain Model

После еще одного выяснения деталей, бизнес процесс (workflow) усложнился еще сильнее, что очень
хорошо. Чем раньше будут учтены все детали, тем лучше.

<img src="images/ch02_workflow_diagram.jpg" alt="Workflow diagram" width=600 >

Эта диаграмма не учитывает всех деталей, поэтому в документировании лучше использовать
text-based язык (как это было сделано ранее).

### Representing Constraints. (Представление ограничений)

Важно делать описание с точки зрения domain expert. Ограничения валидации являются частью дизайна.

Первыми опишем примитивные значения: коды продуктов и их количество.

```text
Bounded context: Order-Taking

data WidgetCode = string starting with "W" then 4 digits
data GizmoCode = string starting with "G" then 3 digits
data ProductCode = WidgetCode OR GizmoCode

data OrderQuantity = UnitQuantity OR KilogramQuantity
data UnitQuantity = integer between 1 and ?
data KilogramQuantity = decimal between ? and ?
```

Описывая количество продуктов, увидели, что не были уточнены нижний и верхний пределы.
Domain expert сообщил про ограничения на количество продуктов и описание становится таким:

```text
Bounded context: Order-Taking

data WidgetCode = string starting with "W" then 4 digits
data GizmoCode = string starting with "G" then 3 digits
data ProductCode = WidgetCode OR GizmoCode

data OrderQuantity = UnitQuantity OR KilogramQuantity
data UnitQuantity = integer between 1 and 1000
data KilogramQuantity = decimal between 0.05 and 100.0
```

### Representing the Life Cycle of an Order. (Представление жизненного цикла заказа)

Теперь описание заказа. Сделанное ранее описание заказа:

```text
data Order =
    CustomerInfo
    AND ShippingAddress
    AND BillingAddress
    AND list of OrderLines
    AND AmountToBill
```

Такое описание не отображает жизненный цикл заказа, который описал domain expert. Заказ проходит
несколько стадий: unvalidated (после получения письма), validated, priced.

Жизненный цикл заказа можно описать так:

1. Unvalidated Order

```text
data UnvalidatedOrder =
    UnvalidatedCustomerInfo
    AND UnvalidatedShippingAddress
    AND UnvalidatedBillingAddress
    AND list of UnvalidatedOrderLine

data UnvalidatedOrderLine =
    UnvalidatedProductCode
    AND UnvalidatedOrderQuantity
```

*Примечание: unvalidated order не имеет данных о ценах.*

2. Validated Order

```text
data ValidatedOrder =
    ValidatedCustomerInfo
    AND ValidatedShippingAddress
    AND ValidatedBillingAddress
    AND list of ValidatedOrderLine

data ValidatedOrderLine =
    ValidatedProductCode
    AND ValidatedOrderQuantity
```

*Примечание: все линии validated order должны быть проверены, а не только некоторые из них.*

3. Priced Order

* `PricedOrderLine` это `ValidatedOrderLine` плюс `LinePrice`
* `AmountToBill` вычисляется как сумма цен по каждой `ProductOrderLine`

```text
data PricedOrder =
    ValidatedCustomerInfo
    AND ValidatedShippingAddress
    AND list of PricedOrderLine     // different from ValidatedOrderLine
    AND AmountOfBill                // new

data PricedOrderLine =
    ValidatedOrderLine
    AND LinePrice                   // new
```

4. Order acknowledgement

```text
data PlacedOrderAcknowledgement =
    PricedOrder
    AND AcknowledgementLetter
```

### Fleshing out the Steps in the Workflow. (Конкретизация шагов в бизнес-процессе)

Вначале мы думали, что workflow (бизнес-процесс) "Place Order" заканчивается событием
"Order Placed".

Но потом выяснили что все сложнее и возможные итоговые значения для workflow могут быть:

* Посылка события "Order placed" в shipping/bulling. ИЛИ
* Добавление invalid формы заказа в специальную стопку с not valid заказами и пропуск остальных
шагов.

<table>
<tr>
<td>

Первоначальное text-based описание workflow:

```text
Bounded context: Order-Taking

Workflow: "Place order"
    triggered by:
        "Order form received" event
        (where Quote is not checked)
    primary input:
        An order form
    other input:
        Product catalog
    output events:
        "Order Placed" event
    side-effects:
        An acknowledgement is sent to the customer,
        along with the placed order
```

</td>
<td>

Расширенное (уточненное) text-based описание workflow:

```text
workflow "Place Order" =
    input: OrderForm
    output:
        OrderPlaced event
          (put on a pile to send to other teams)
        OR InvalidOrder
          (put on appropriate pile)

    // step 1
    do ValidateOrder
    If order is invalid then:
        add InvalidOrder to pile
        stop

    // step 2
    do PriceOrder

    // step 3
    do SendAcknowledgmentToCustomer

    // step 4
    return OrderPlaced event (if no errors)
```

</td>
</table>

### Более детальное документирование отдельных шагов

После уточнения документации всего workflow можно сделать более подробную документацию для
каждого отдельного шага.

1. ValidateOrder

```text
substep "ValidateOrder" =
    input: UnvalidatedOrder
    output: ValidatedOrder OR ValidationError
    dependencies: CheckProductCodeExists, CheckAddressExists

    validate the customer name
    check that the shipping and billing address exist
    for each line:
        check product code syntax
        check that product code exists in ProductCatalog

    if everything is OK, then:
        return ValidatedOrder
    else:
        return ValidationError
```

*Зависимости: product catalog (`CheckProductCodeExists`) и внешний address checking service*
*(`CheckAddressExists`)*.

2. PriceOrder

```text
substep "PriceOrder" =
    input: ValidatedOrder
    output: PricedOrder
    dependencies: GetProductPrice

    for each line:
        get the price for the product
        set the price for the line
    set amount to bill ( = sum of the line prices)
```

*Зависимости: product catalog (`GetProductPrice`)*.

3. SendAcknowledgementToCustomer

```text
substep "SendAcknowledgementToCustomer" =
    input: PricedOrder
    output: None

    create acknowledgment letter and send it
    and the priced order to the customer
```

## Wrapping Up. (Подведение итогов)

* При документировании domain важно не вносить сюда технические детали (БД, классы).

* Консультация у domain expert выявила некоторые дополнительные моменты, которые не были известны
на первом этапе.

# Chapter 3. A Functional Architecture

Для описания архитектуры используется терминология из статьи Simon Brown's C4
(см. [Simon Brown's "C4" approach](http://static.codingthearchitecture.com/c4.pdf)).
Согласно C4 архитектура приложений состоит из 4х уровней:

1. "System context" - верхний уровень, представляет всю систему.
2. System context состоит из "containers", каждый из который представляет единицу развертывания
(вебсайт, веб-сервис, база данных, ...).
3. Каждый container включает в себя "components" - основные строительные блоки в коде.
4. Каждый component включает в себя "classes" (или в ФП - "modules") - содержат внутри себя
низкоуровневые методы или функции.

Одна из целей хорошей архитектуры - определить различные boundaries между containers, components,
modules. Чтобы минимизировать "cost of change" (стоимость изменений).

## Bounded Contexts as Autonomous (автономные) Software Components

Важно, чтобы контекст представлял собой autonomous subsystem (автономную подсистему) с
well-defined boundary (четко определенными границами).

Возможные варианты реализации context как autonomous subsystem:

1. Вся система реализована как single monolithic deployable (a single container в терминах C4).

Bounded context может быть:

* отдельный module с well-defined interface
* отдельный .NET assembly (предпочтельней)

2. Все bounded context системы могут быть deployed separately.

Bounded context может быть deployed separately в виде:

* отдельный container как в classic service-oriented architecture
* отдельный container, как в microservice architecture

На ранних этапах разработки не обязательно придерживаться какого-то конкретного
подхода. Перевод с logic дизайна на deployable эквивалент не является критичным, если мы уверены,
что bounded context остаются decoupled и autonomous (расцепленными и автономными).

**Итог**.

* В начале разработки boundaries могут быть определены не совсем точно.
* По мере изучения domain boundaries могут изменяться.
* В начале проще делать монолитное приложение. И только потом, если потребуется, разбить его
на отдельные decoupled containers. Микросервисы вносят дополнительную сложность
(см. ["Martin Fowler - "MicroservicePremium"](https://www.martinfowler.com/bliki/MicroservicePremium.html)).

## Communicating (взаимодействие) Between Bounded Contexts

Для взаимодействия между bounded contexts используются event.

Пример такого взаимодействия:

* The `Place-Order` workflow emits ("кидает") an `OrderPlaced` event.
* The `OrderPlaced` event помещается в очередь или публикуется еще каким-либо способом.
* The shipping context следит за появлением `OrderPlaced` events.
* Когда event получен, создается `ShipOrder`command.
* `ShipOrder` command запускает `Ship-Order` workflow (бизнес-процесс).
* Когда `Ship-Order` workflow успешно завершается, он emits ("кидает") `OrderShipped` event.

<img src="images/ch03_communication.jpg" alt="Communication between bounded contexts" width=700 >

Это decoupled design: the upstream component (the order-taking subsystem) and the downstream component (the
shipping subsystem) не зависят друг от друга и общаются только посредством events.

Механизм передачи events зависит от выбора архитектуры:

* Queues хороши для buffered asynchronous communication. Они выбираются при реализации
microservices or agents.

* В monolithic system можно использовать
  * похожий внутренний queuing подход
  * простую связь между upstream и downstream component через вызов функции

Механизм передачи можно выбрать позднее.

Handler который передает events может быть:

* частью downstream context
* [separate router](http://www.enterpriseintegrationpatterns.com/patterns/messaging/MessageRouter.html)
* [process manager](https://www.slideshare.net/BerndRuecker/long-running-processes-in-ddd)

Два последних являются отдельными элементами инфраструктуры.

### Transferring Data Between Bounded Contexts

* Event может быть в виде
  * simple signal
  * data, которые пересылаются другому downstream компоненту
  * reference to shared data (если объем пересылаемх данных большой)

>Пересылаемые data object называются **Data Transfer Objects** or **DTO**s

DTO могут быть похожи на объекты домена и содержать те же данные, но это другие объекты.
DTO проектируются для сериализации/десериализации.

На границах upstream context, domain objects конвертируются в DTOs, которые сериализуются в
JSON, XML, или другие форматы:

<img src="images/ch03_output_dto.jpg" alt="DTO from upstream context" width=600 >

Для downstream context процесс повторяется в обратном порядке -
JSON или XML десереализуются в DTO, которое конвертируется в domain object:

<img src="images/ch03_input_dto.jpg" alt="DTO to downstream context" width=600 >

### Trust Boundaries (границы доверия) and Validation

Периметр bounded context действует как "trust boundary" (границы доверия).

Все объекты внутри bounded context всегда trusted и valid.

Объекты вне bounded context всегда untrusted и могут быть invalid.

Мы добавляем "gates" ("ворота") в начало и конец workflow (бизнес-процесса), которые действуют
как шлюзы между двумя мирами.

<img src="images/ch03_boundaries.jpg" alt="DTO to downstream context" width=600 >

**Задача входного gate** - валидация входных данные, чтобы они удовлетворяли ограничениям
domain model.

Пример:

* Какое-то свойство `Order` должно быть non-null и менее 50 символов.
* Входящий `OrderDTO` может не иметь таких ограничений.
* После валидации мы будем уверены, что объект домена `Order`не будет содержать ошибок.
* Если валидация закончится ошибкой, то workflow будет bypassed и создана ошибка (сообщение об ошибке).

**Задача выходного gate** - фильтрация выходных данных. Важно чтобы private информация
не вышла за пределы bounded context.

Пример: фильтрация номера кредитной карты, используемой для оплаты заказа.

## Contracts Between Bounded Contexts

Объекты коммуникации (events и DTOs) всегда вносят некоторую связность (контракты) между
Bounded Contexts.

Контракты могут быть:

* *Shared Kernel* - один формат данных для общения разных contexts. Изменение формата требует
согласования с обеих сторон.

* *Customer/Supplier* или [*Consumer Driven Contract*](https://www.infoq.com/articles/consumer-driven-contracts) -
downstream context определяет контракт, который требуется от upstream context.

* *Conformist* - противоположный предыдущему. downstream context подстраивается под контракт
upstream context.

### Anti-Corruption Layers

Очень часто для взаимодействия с внешними системами используется интерфейс. Иногда интерфейс,
который есть в наличии не совпадает с domain model.

В таком случае требуется дополнительная прослойка снаружи bounded context, чтобы domain model
не была "испорчена" знаниями о внешнем мире.

>Такой дополнительный уровень decoupling между contexts называется **Anti-Corruption Layer**
>(**ACL**) в терминологии DDD.

"Input gate", описанный выше, является примером ACL.

Основная задача ACL это не валидация данных, а их трансформация. Например - трансформация данных
из словаря order-taking context в словарь shipping context.

ACL часто используется для взаимодействия с third-party components.

### A Context Map with Relationships

Для проектируемой системы определяем тип взаимодействий между contexts.

Например, для описываемой order-taking системы:

* Между order-taking и shipping context тип взаимодействия "Shared Kernel"
(общий формат обмена - по договоренности).
* Между order-taking и billing context тип взаимодействия "Consumer-Driven Contract"
(формат определяется downstream context).
* Между order-taking и product catalog тип взаимодействия "Conformist"
(формат определяется upstream context).
* Между order-taking и address service тип взаимодействия с использованием ACL
(транформация данных).

<img src="images/ch03_context_map.jpg" alt="Context Map with Relationships" width=450 >

## Workflows Within a Bounded Context

* Бизнес-процессы рассматриваются как мини-процессы, запускаемые command и которые генерируют
одну или несколько Domain Events.

* В функциональной архитектуре каждый такой процесс представлен одной функцией,
где на входе объект command а на выходе list event objects.

* Public workflow - рабочие-процессы, запуск которых инициируется снаружи.

* Workflow должен находиться в пределах одного bounded context. Он никогда не должен реализовывать
сценарий "end-to-end" и проходить через несколько contexts.

<img src="images/ch03_workflow_in_out.jpg" alt="Public Workflow within bounded context" width=450 >

Из одного context в другой должна посылаться только необходимая для работы информация.

Например, `OrderPlaced` event должен содержать только данные, необходимые для работы
billing context. Это значит, что нужно создать новый, специальный для billing
event `BillableOrderPlaced`:

```text
data BillableOrderPlaced =
    OrderId
    AND BillingAddress
    AND AmountToBill
```

И еще нужен отдельный event для подтверждения заказа - `OrderAcknowledgmentSent`.

Диаграмма workflow "Place Order" с несколькими event'ами:

<img src="images/ch03_workflow_placeorder.jpg" alt="Workflow 'Place Order' with events" width=550 >

### Avoid Domain Events Within a Bounded Context

В ООП принят подход, когда все Domain Events генерируются внутри bounded context.

Внутри context один из объектов workflow генерирует событие `OrderPlaced`. Специальные handlers
(listeners) "ловят" это событие и генерируют другие события: `BillableOrderPlaced` и т.д.
Выглядит это так:

<img src="images/ch03_domain_events_1.jpg" alt="OOP style events creation" width=650 >

В ФП предпочтительно не использовать этот подход, поскольку он создает
скрытые зависимости. Вместо этого, если нужен listener для event, то мы просто
добавляем его в конец workflow следующим образом:

<img src="images/ch03_domain_events_2.jpg" alt="FP style events creation" width=650 >

Такой подход более понятен - нет глобальных менеджеров событий с изменяемым состоянием
и поэтому его легче понимать и поддерживать.

## Code Structure Within a Bounded Context

Традиционный "layered approach" (картинка слева).

<table>
<tr>
<td>

<img src="images/ch03_codeflow_vertical_1.jpg" alt="Layered approach workflow 1" width=325 >

</td>
<td>

<img src="images/ch03_codeflow_vertical_2.jpg" alt="Layered approach workflow 2" width=200 >

</td>
</table>

Workflow проходит через все слои - сверху вниз и обратно. Недостаток: если надо поменять что-то
в работе workflow, то это затронет все слои.

Более походящим решением может быть организация workflow в виде "vertical slices" (рисунок справа).
Хотя такое решение все еще неидеально. Представление в горизонтальном виде:

<img src="images/ch03_codeflow_horizontal.jpg" alt="Horizontal workflow" width=500 >

### The Onion Architecture

Приципы [Onion Architecture](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/):

* Domain layer в центре.
* Каждый слой зависит только от внутренних слоев и не видит внешние.
* Все зависимости направлены внутрь.

<img src="images/ch03_onion.jpg" alt="Onion Architecture" width=450 >

Аналогичные подходы:
[Hexagonal Architecture](http://alistair.cockburn.us/Hexagonal+architecture) и
[Clean Architecture](https://8thlight.com/blog/uncle-bob/2012/08/13/the-clean-architecture.html)

### Keep I/O at the Edges

Предсказуемость поведения функций в ФП обеспечивается следующим:

* Использование immutable (неизменяемых) данных где только возможно.
* Пытаться избегать side effects в функциях:
  * Избегать randomness
  * Избегать mutation of variables
  * Избегать операций I/O

Как получать/сохранять данные?

Пытаться поместить все операции I/O ближе к границам onion.
Для workflow - I/O желательно располагать в его начале или конце.

## Wrapping Up

* *Domain Object* - объект, предназначенный для использования только в пределах context.
* *Data Transfer Object* или *DTO* - объект, предназначенный для сериализации и
совместного использования между contexts.
* *Shared Kernel*, *Customer/Supplier*, и *Conformist* - различные виды
отношений между bounded contexts.
* *Anti-Corruption Layer* или *ACL* - компонент, который переводит концепции
из одного domain в другой, чтобы уменьшить coupling (сцепление) и позволить domains
развиваться независимо.
* *Persistence Ignorance* - domain model должна основываться только
на концепциях только domain и не должна содержать какой-либо информации
о базах данных или других механизмах хранения и обработки информации.

# Links

* [EventStorming book by Alberto Brandolini](http://eventstorming.com)

Alberto Brandolini - создатель метода "Event Storming".

* [Simon Brown's "C4" approach](http://static.codingthearchitecture.com/c4.pdf)

* [Martin Fowler - "MicroservicePremium"](https://www.martinfowler.com/bliki/MicroservicePremium.html)

* [Message Router Pattern](http://www.enterpriseintegrationpatterns.com/patterns/messaging/MessageRouter.html)

* [Long running processes in DDD slides](https://www.slideshare.net/BerndRuecker/long-running-processes-in-ddd)

* [Martin Fowler - "Data Transfer Object"](https://martinfowler.com/eaaCatalog/dataTransferObject.html)

* [Service-Oriented Development with Consumer-Driven Contracts](https://www.infoq.com/articles/consumer-driven-contracts)

* [Inverse Conway maneuver](http://bit.ly/InverseConwayManeuver)

* [Onion Architecture](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/)

* [Hexagonal Architecture](http://alistair.cockburn.us/Hexagonal+architecture)

* [Clean Architecture](https://8thlight.com/blog/uncle-bob/2012/08/13/the-clean-architecture.html)
