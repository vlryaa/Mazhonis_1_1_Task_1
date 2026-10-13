USE [demo09.02.11];
GO
CREATE OR ALTER TRIGGER dbo.TR_DeliveryRequests_SetCreatedAt
ON dbo.DeliveryRequests
AFTER INSERT
AS
BEGIN
 SET NOCOUNT ON;
 UPDATE dr SET CreatedAt=SYSDATETIME()
 FROM dbo.DeliveryRequests dr JOIN inserted i ON i.RequestId=dr.RequestId
 WHERE i.CreatedAt IS NULL;
END;
GO
CREATE OR ALTER TRIGGER dbo.TR_Clients_PreventDeleteWithRequests
ON dbo.Clients
INSTEAD OF DELETE
AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS (SELECT 1 FROM deleted d JOIN dbo.DeliveryRequests r ON r.ClientId=d.ClientId)
 BEGIN
  THROW 50001, N'Нельзя удалить клиента: на него оформлены заявки на доставку.', 1;
 END;
 DELETE c FROM dbo.Clients c JOIN deleted d ON d.ClientId=c.ClientId;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_SearchDeliveryRequests
 @ClientName NVARCHAR(150)=NULL,
 @EmployeeId INT=NULL
AS
BEGIN
 SET NOCOUNT ON;
 SELECT r.RequestId,r.CreatedAt,c.FullName AS ClientName,r.CargoDescription,
        r.OriginAddress,r.DestinationAddress,r.DeliveryDate,e.FullName AS EmployeeName,r.Comment
 FROM dbo.DeliveryRequests r
 JOIN dbo.Clients c ON c.ClientId=r.ClientId
 JOIN dbo.Employees e ON e.EmployeeId=r.EmployeeId
 WHERE (@ClientName IS NULL OR @ClientName=N'' OR c.FullName LIKE N'%'+@ClientName+N'%')
   AND (@EmployeeId IS NULL OR e.EmployeeId=@EmployeeId)
 ORDER BY r.RequestId DESC;
END;
GO
SELECT name AS TriggerName,OBJECT_NAME(parent_id) AS TableName
FROM sys.triggers
WHERE name IN (N'TR_DeliveryRequests_SetCreatedAt',N'TR_Clients_PreventDeleteWithRequests');
GO
