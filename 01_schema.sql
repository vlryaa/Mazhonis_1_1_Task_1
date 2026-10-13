/*
 Проект: Мажонис — задание 1
 Сервер: WIN-PUURG92IVC5\SQLEXPRESS
 База: demo09.02.11
*/
IF DB_ID(N'demo09.02.11') IS NULL EXEC(N'CREATE DATABASE [demo09.02.11]');
GO
USE [demo09.02.11];
GO
CREATE TABLE dbo.Clients (
 ClientId INT IDENTITY(1,1) CONSTRAINT PK_Clients PRIMARY KEY,
 FullName NVARCHAR(150) NOT NULL,
 Phone NVARCHAR(30) NULL,
 Email NVARCHAR(150) NULL,
 CONSTRAINT UQ_Clients_FullName UNIQUE (FullName)
);
GO
CREATE TABLE dbo.Employees (
 EmployeeId INT IDENTITY(1,1) CONSTRAINT PK_Employees PRIMARY KEY,
 FullName NVARCHAR(150) NOT NULL,
 Phone NVARCHAR(30) NULL,
 CONSTRAINT UQ_Employees_FullName UNIQUE (FullName)
);
GO
CREATE TABLE dbo.DeliveryRequests (
 RequestId INT IDENTITY(1,1) CONSTRAINT PK_DeliveryRequests PRIMARY KEY,
 CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_DeliveryRequests_CreatedAt DEFAULT (SYSDATETIME()),
 ClientId INT NOT NULL,
 CargoDescription NVARCHAR(500) NOT NULL,
 OriginAddress NVARCHAR(300) NOT NULL,
 DestinationAddress NVARCHAR(300) NOT NULL,
 DeliveryDate DATE NOT NULL,
 EmployeeId INT NOT NULL,
 Comment NVARCHAR(1000) NULL,
 CONSTRAINT FK_DeliveryRequests_Clients FOREIGN KEY (ClientId) REFERENCES dbo.Clients(ClientId),
 CONSTRAINT FK_DeliveryRequests_Employees FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees(EmployeeId),
 CONSTRAINT CK_DeliveryRequests_DeliveryDate CHECK (DeliveryDate >= CONVERT(date, CreatedAt))
);
GO
