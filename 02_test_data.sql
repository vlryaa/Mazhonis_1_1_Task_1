USE [demo09.02.11];
GO
INSERT INTO dbo.Clients (FullName, Phone, Email) VALUES
(N'Иванов Иван Иванович', N'+7 900 111-22-33', N'ivanov@example.com'),
(N'Петрова Анна Сергеевна', N'+7 900 222-33-44', N'petrova@example.com'),
(N'Сидоров Павел Олегович', N'+7 900 333-44-55', N'sidorov@example.com');
INSERT INTO dbo.Employees (FullName, Phone) VALUES
(N'Кузнецов Алексей Игоревич', N'+7 901 111-22-33'),
(N'Смирнова Мария Андреевна', N'+7 901 222-33-44'),
(N'Орлов Дмитрий Сергеевич', N'+7 901 333-44-55');
INSERT INTO dbo.DeliveryRequests
 (ClientId, CargoDescription, OriginAddress, DestinationAddress, DeliveryDate, EmployeeId, Comment)
VALUES
(1, N'Коробки с документами', N'г. Москва, ул. Лесная, д. 5', N'г. Москва, ул. Тверская, д. 10', DATEADD(day,3,CONVERT(date,GETDATE())),1,N'Демонстрационная заявка'),
(2, N'Бытовая техника', N'г. Москва, ул. Мира, д. 15', N'г. Химки, ул. Молодёжная, д. 4', DATEADD(day,5,CONVERT(date,GETDATE())),2,NULL),
(3, N'Запасные части', N'г. Москва, ул. Заводская, д. 8', N'г. Подольск, ул. Кирова, д. 20', DATEADD(day,7,CONVERT(date,GETDATE())),3,N'Позвонить получателю заранее');
GO
