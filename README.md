# Мажонис — задание 1: заявки на доставку грузов

Учебный проект для **SSMS + Visual Studio Windows Forms**.

- SQL Server: `WIN-PUURG92IVC5\SQLEXPRESS`
- База данных: `demo09.02.11`

## Структура репозитория

```text
Mazhonis_09.02.11_Task_1/
├── README.md
├── .gitignore
├── storage/
│   ├── 01_schema.sql
│   ├── 02_test_data.sql
│   └── 03_procedures_triggers.sql
├── app/src/
│   ├── Form1.cs
│   └── Form1.Designer.cs
└── config/
    └── App.config.example
```

## Создание базы в SSMS
Подключись к `WIN-PUURG92IVC5\SQLEXPRESS` и выполняй файлы по порядку:
1. `storage/01_schema.sql` — создаёт базу и таблицы `Clients`, `Employees`, `DeliveryRequests`.
2. `storage/02_test_data.sql` — добавляет демонстрационные данные.
3. `storage/03_procedures_triggers.sql` — создаёт процедуру поиска и два триггера.

В конце третьего скрипта должны появиться триггеры `TR_DeliveryRequests_SetCreatedAt` и `TR_Clients_PreventDeleteWithRequests`.

> `01_schema.sql` предназначен для первого запуска. Не запускай повторно поверх уже созданных таблиц без резервной копии/очистки.

## Запуск Windows Forms в Visual Studio
1. Создай проект **Windows Forms App (.NET Framework)** на C# с именем `DeliveryRequestsApp`.
2. Замени `Form1.cs` и `Form1.Designer.cs` файлами из `app/src/`.
3. В `Form1.cs` строка подключения должна указывать на `WIN-PUURG92IVC5\SQLEXPRESS` и базу `demo09.02.11`.
4. Проверь наличие ссылки `System.Data`, затем запусти проект.

Форма позволяет просматривать, искать по ФИО клиента, фильтровать по ответственному сотруднику, добавлять, редактировать и удалять заявки. Клиент и сотрудник выбираются из справочников.

## Реализованные требования
- PK/FK, обязательные поля, проверка даты доставки;
- процедура `usp_SearchDeliveryRequests`;
- триггер автоматического заполнения даты создания;
- триггер запрета удаления клиента при наличии заявок;
- Windows Forms для основных операций с заявками.

## GitHub
Рекомендуемое имя репозитория: `Mazhonis_09.02.11_Task_1`. Загружай содержимое этой папки в корень репозитория, а не ZIP-файл.
