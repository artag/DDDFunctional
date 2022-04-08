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

We call these things *Domain Events*.

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

`Place order` и `Ship order` это бизнес-процессы, мы начинаем понимать, как свъобытия
объединяются в более крупные рабочие процессы.

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

We call these requests *commands* in DDD terminology

* Commands are always written in the imperative (повелительное наклонение): "Do this for me."

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

# Links

* [EventStorming book by Alberto Brandolini](http://eventstorming.com)

Alberto Brandolini - создатель метода "Event Storming".
