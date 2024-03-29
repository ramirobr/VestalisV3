USE [Monitoring]
GO
INSERT [dbo].[Application] 
([ApplicationId], [Description], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId]) 
VALUES 
(1, N'VestalisV3', 1, 0, NULL, GETDATE(), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[SupportUser] 
([SupportUserId], [ApplicationId], [Email], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId])
VALUES 
(NEWID(), 1, N'nombre.apellido@cotecna.com.ec', 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Counter] 
([CounterId], [ApplicationId], [Value], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId]) 
VALUES 
(1, 1, 0, 1, 0, NULL, GETDATE(), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Category] 
([CategoryId], [Description], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId]) 
VALUES 
(1, N'Client', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Category] 
([CategoryId], [Description], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId]) 
VALUES 
(2, N'Server', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Priority] 
([PriorityId], [Description], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId]) 
VALUES 
(1, N'High', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Priority] 
([PriorityId], [Description], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId]) 
VALUES 
(2, N'Low', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Priority] 
([PriorityId], [Description], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId]) 
VALUES 
(3, N'Medium', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[State] 
([StateId], [Description], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId]) 
VALUES 
(1, N'Closed', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[State] 
([StateId], [Description], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId]) 
VALUES 
(2, N'Created', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[State] 
([StateId], [Description], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId]) 
VALUES 
(3, N'In Progress', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[TicketType] 
([TicketTypeId], [Description], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId]) 
VALUES 
(1, N'Bug', 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[TicketType] 
([TicketTypeId], [Description], [IsActive], [IsDeleted], [CreationBy], [CreationDate], [ModificationBy], [ModificationDate], [WorkstationName], [SmartserverId], [LanguageId]) 
VALUES 
(2, N'Fix', 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
