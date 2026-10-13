## Структура репозитория

```text
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
Подключитесь к `WIN-PUURG92IVC5\SQLEXPRESS` и выполняйте файлы по порядку:
1. storage/01_schema.sql — создаёт базу и таблицы Clients, Employees, DeliveryRequests.
2. storage/02_test_data.sql — добавляет демонстрационные данные.
3. storage/03_procedures_triggers.sql — создаёт процедуру поиска и два триггера.


## Запуск Windows Forms в Visual Studio
1. Проект **Windows Forms App (.NET Framework)** на C# с именем DeliveryRequestsApp.
2. Замени Form1.cs и Form1.Designer.cs файлами из app/src/.
3. В Form1.cs строка подключения должна указывать на WIN-PUURG92IVC5\SQLEXPRESS и базу demo09.02.11.
4. Проверь наличие ссылки `System.Data`, затем запусти проект.

Форма позволяет просматривать, искать по ФИО клиента, фильтровать по ответственному сотруднику, добавлять, редактировать и удалять заявки. Клиент и сотрудник выбираются из справочников.


