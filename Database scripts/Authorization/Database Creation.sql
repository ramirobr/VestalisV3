USE [master]
GO

IF EXISTS(SELECT 1 from sys.databases WHERE name =N'CotecnaAuthorization')
BEGIN
	EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'CotecnaAuthorization'
	ALTER DATABASE [CotecnaAuthorization] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP DATABASE [CotecnaAuthorization]
	DROP LOGIN [AuthorizationLogin]
END
GO
CREATE DATABASE [CotecnaAuthorization]
GO
USE [CotecnaAuthorization]
GO
/****** Object:  Schema [Common]    Script Date: 6/18/2012 1:10:00 PM ******/
CREATE SCHEMA [Common]
GO
/****** Object:  Table [Common].[ApplicationAuthorization]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Common].[ApplicationAuthorization](
	[ApplicationId] [smallint] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
 CONSTRAINT [PK_ApplicationAuthorization] PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Common].[ApplicationContract]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Common].[ApplicationContract](
	[ContractId] [smallint] NOT NULL,
	[ApplicationId] [smallint] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
 CONSTRAINT [PK_ApplicationContract] PRIMARY KEY CLUSTERED 
(
	[ContractId] ASC,
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Common].[ApplicationModule]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Common].[ApplicationModule](
	[ApplicationId] [smallint] NOT NULL,
	[ModuleId] [smallint] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
 CONSTRAINT [PK_ApplicationModule] PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC,
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Common].[Contract]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Common].[Contract](
	[ContractId] [smallint] NOT NULL,
	[Code] [char](2) NULL,
	[Name] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
 CONSTRAINT [PK_Contract] PRIMARY KEY CLUSTERED 
(
	[ContractId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Common].[Module]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Common].[Module](
	[ModuleId] [smallint] NOT NULL,
	[Name] [nvarchar](60) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
 CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED 
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Common].[Operation]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Common].[Operation](
	[OperationId] [int] NOT NULL,
	[ModuleId] [smallint] NOT NULL,
	[Name] [nvarchar](60) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
 CONSTRAINT [PK_Operation] PRIMARY KEY CLUSTERED 
(
	[OperationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Common].[Profile]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Common].[Profile](
	[ProfileId] [int] NOT NULL,
	[ApplicationId] [smallint] NOT NULL,
	[Name] [nvarchar](60) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
	[OfficeTypeId] [smallint] NULL,
 CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED 
(
	[ProfileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Common].[ProfileModule]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Common].[ProfileModule](
	[ProfileId] [int] NOT NULL,
	[ModuleId] [smallint] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
 CONSTRAINT [PK_ProfileModule] PRIMARY KEY CLUSTERED 
(
	[ProfileId] ASC,
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Common].[Role]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Common].[Role](
	[RoleId] [int] NOT NULL,
	[ModuleId] [smallint] NOT NULL,
	[Name] [nvarchar](60) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Common].[RoleOperation]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Common].[RoleOperation](
	[RoleId] [int] NOT NULL,
	[OperationId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
 CONSTRAINT [PK_RoleOperation] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[OperationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Common].[RoleProfile]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Common].[RoleProfile](
	[RoleId] [int] NOT NULL,
	[ProfileId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
 CONSTRAINT [PK_RoleProfile] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[ProfileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Common].[User]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Common].[User](
	[UserId] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Code] [nvarchar](10) NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Identifier] [nvarchar](60) NOT NULL,
	[Domain] [nvarchar](20) NOT NULL,
	[CountryId] [smallint] NOT NULL,
	[OfficeOwnerId] [smallint] NOT NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
	[Email] [nvarchar](70) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Common].[UserApplication]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Common].[UserApplication](
	[OfficeId] [smallint] NOT NULL,
	[ProfileId] [int] NOT NULL,
	[ContractId] [smallint] NOT NULL,
	[ApplicationId] [smallint] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[IsDefaultContract] [bit] NOT NULL,
	[DefaultModuleId] [smallint] NOT NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
 CONSTRAINT [PK_UserApplication] PRIMARY KEY CLUSTERED 
(
	[OfficeId] ASC,
	[ProfileId] ASC,
	[ContractId] ASC,
	[ApplicationId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Common].[UserEncrypted]    Script Date: 6/18/2012 1:10:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Common].[UserEncrypted](
	[UserId] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Code] [varbinary](8000) NULL,
	[Name] [varbinary](8000) NOT NULL,
	[Identifier] [varbinary](8000) NOT NULL,
	[Domain] [varbinary](8000) NOT NULL,
	[CountryId] [varbinary](8000) NOT NULL,
	[OfficeOwnerId] [varbinary](8000) NOT NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
 CONSTRAINT [PK_UserEncrypted] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [Common].[ApplicationAuthorization] ADD  CONSTRAINT [DF__ApplicationAuthorization__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[ApplicationAuthorization] ADD  CONSTRAINT [DF__ApplicationAuthorization__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[ApplicationAuthorization] ADD  CONSTRAINT [DF__ApplicationAuthorization__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[ApplicationAuthorization] ADD  CONSTRAINT [DF__ApplicationAuthorization__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[ApplicationContract] ADD  CONSTRAINT [DF__ApplicationContract__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[ApplicationContract] ADD  CONSTRAINT [DF__ApplicationContract__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[ApplicationContract] ADD  CONSTRAINT [DF__ApplicationContract__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[ApplicationContract] ADD  CONSTRAINT [DF__ApplicationContract__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[ApplicationModule] ADD  CONSTRAINT [DF__ApplicationModule__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[ApplicationModule] ADD  CONSTRAINT [DF__ApplicationModule__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[ApplicationModule] ADD  CONSTRAINT [DF__ApplicationModule__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[ApplicationModule] ADD  CONSTRAINT [DF__ApplicationModulei__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[Contract] ADD  CONSTRAINT [DF__Contract__IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [Common].[Contract] ADD  CONSTRAINT [DF__Contract__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[Contract] ADD  CONSTRAINT [DF__Contract__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[Contract] ADD  CONSTRAINT [DF__Contract__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[Contract] ADD  CONSTRAINT [DF__Contract__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[Module] ADD  CONSTRAINT [DF__Module__IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [Common].[Module] ADD  CONSTRAINT [DF__Module__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[Module] ADD  CONSTRAINT [DF__Module__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[Module] ADD  CONSTRAINT [DF__Module__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[Module] ADD  CONSTRAINT [DF__Module__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[Operation] ADD  CONSTRAINT [DF__Operation__IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [Common].[Operation] ADD  CONSTRAINT [DF__Operation__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[Operation] ADD  CONSTRAINT [DF__Operation__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[Operation] ADD  CONSTRAINT [DF__Operation__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[Operation] ADD  CONSTRAINT [DF__Operation__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[Profile] ADD  CONSTRAINT [DF__Profile__IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [Common].[Profile] ADD  CONSTRAINT [DF__Profile__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[Profile] ADD  CONSTRAINT [DF__Profile__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[Profile] ADD  CONSTRAINT [DF__Profile__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[Profile] ADD  CONSTRAINT [DF__Profile__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[ProfileModule] ADD  CONSTRAINT [DF__ProfileModule__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[ProfileModule] ADD  CONSTRAINT [DF__ProfileModule__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[ProfileModule] ADD  CONSTRAINT [DF__ProfileModule__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[ProfileModule] ADD  CONSTRAINT [DF__ProfileModule__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[Role] ADD  CONSTRAINT [DF__Role__IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [Common].[Role] ADD  CONSTRAINT [DF__Role__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[Role] ADD  CONSTRAINT [DF__Role__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[Role] ADD  CONSTRAINT [DF__Role__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[Role] ADD  CONSTRAINT [DF__Role__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[RoleOperation] ADD  CONSTRAINT [DF__RoleOperation__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[RoleOperation] ADD  CONSTRAINT [DF__RoleOperation__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[RoleOperation] ADD  CONSTRAINT [DF__RoleOperation__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[RoleOperation] ADD  CONSTRAINT [DF__RoleOperation__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[RoleProfile] ADD  CONSTRAINT [DF__RoleProfile__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[RoleProfile] ADD  CONSTRAINT [DF__RoleProfile__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[RoleProfile] ADD  CONSTRAINT [DF__RoleProfile__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[RoleProfile] ADD  CONSTRAINT [DF__RoleProfile__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[User] ADD  CONSTRAINT [DF__User__UserId]  DEFAULT (newsequentialid()) FOR [UserId]
GO
ALTER TABLE [Common].[User] ADD  CONSTRAINT [DF__User__IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [Common].[User] ADD  CONSTRAINT [DF__User__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[User] ADD  CONSTRAINT [DF__User__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[User] ADD  CONSTRAINT [DF__User__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[User] ADD  CONSTRAINT [DF__User__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[UserApplication] ADD  CONSTRAINT [DF__UserApplication__IsDefaultContract]  DEFAULT ((0)) FOR [IsDefaultContract]
GO
ALTER TABLE [Common].[UserApplication] ADD  CONSTRAINT [DF__UserApplication__DefaultModuleId]  DEFAULT ((0)) FOR [DefaultModuleId]
GO
ALTER TABLE [Common].[UserApplication] ADD  CONSTRAINT [DF__UserApplication__IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [Common].[UserApplication] ADD  CONSTRAINT [DF__UserApplication__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[UserApplication] ADD  CONSTRAINT [DF__UserApplication__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[UserApplication] ADD  CONSTRAINT [DF__UserApplication__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[UserApplication] ADD  CONSTRAINT [DF__UserApplication__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[UserEncrypted] ADD  CONSTRAINT [DF__UserEncry__UserI__1FB979A9]  DEFAULT (newsequentialid()) FOR [UserId]
GO
ALTER TABLE [Common].[UserEncrypted] ADD  CONSTRAINT [DF__UserEncrypted__IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [Common].[UserEncrypted] ADD  CONSTRAINT [DF__UserEncrypted__IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [Common].[UserEncrypted] ADD  CONSTRAINT [DF__UserEncrypted__CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [Common].[UserEncrypted] ADD  CONSTRAINT [DF__UserEncrypted__ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
GO
ALTER TABLE [Common].[UserEncrypted] ADD  CONSTRAINT [DF__UserEncrypted__LanguageId]  DEFAULT ((1)) FOR [LanguageId]
GO
ALTER TABLE [Common].[ApplicationContract]  WITH CHECK ADD  CONSTRAINT [FK_Application_ApplicationContract] FOREIGN KEY([ApplicationId])
REFERENCES [Common].[ApplicationAuthorization] ([ApplicationId])
GO
ALTER TABLE [Common].[ApplicationContract] CHECK CONSTRAINT [FK_Application_ApplicationContract]
GO
ALTER TABLE [Common].[ApplicationContract]  WITH CHECK ADD  CONSTRAINT [FK_Contract_ApplicationContract] FOREIGN KEY([ContractId])
REFERENCES [Common].[Contract] ([ContractId])
GO
ALTER TABLE [Common].[ApplicationContract] CHECK CONSTRAINT [FK_Contract_ApplicationContract]
GO
ALTER TABLE [Common].[ApplicationModule]  WITH CHECK ADD  CONSTRAINT [FK_Application_ApplicationModule] FOREIGN KEY([ApplicationId])
REFERENCES [Common].[ApplicationAuthorization] ([ApplicationId])
GO
ALTER TABLE [Common].[ApplicationModule] CHECK CONSTRAINT [FK_Application_ApplicationModule]
GO
ALTER TABLE [Common].[ApplicationModule]  WITH CHECK ADD  CONSTRAINT [FK_Module_ApplicationModule] FOREIGN KEY([ModuleId])
REFERENCES [Common].[Module] ([ModuleId])
GO
ALTER TABLE [Common].[ApplicationModule] CHECK CONSTRAINT [FK_Module_ApplicationModule]
GO
ALTER TABLE [Common].[Operation]  WITH CHECK ADD  CONSTRAINT [FK_Module_Operation] FOREIGN KEY([ModuleId])
REFERENCES [Common].[Module] ([ModuleId])
GO
ALTER TABLE [Common].[Operation] CHECK CONSTRAINT [FK_Module_Operation]
GO
ALTER TABLE [Common].[Profile]  WITH CHECK ADD  CONSTRAINT [FK_Application_Profile] FOREIGN KEY([ApplicationId])
REFERENCES [Common].[ApplicationAuthorization] ([ApplicationId])
GO
ALTER TABLE [Common].[Profile] CHECK CONSTRAINT [FK_Application_Profile]
GO
ALTER TABLE [Common].[ProfileModule]  WITH CHECK ADD  CONSTRAINT [FK_Module_ProfileModule] FOREIGN KEY([ModuleId])
REFERENCES [Common].[Module] ([ModuleId])
GO
ALTER TABLE [Common].[ProfileModule] CHECK CONSTRAINT [FK_Module_ProfileModule]
GO
ALTER TABLE [Common].[ProfileModule]  WITH CHECK ADD  CONSTRAINT [FK_Profile_ProfileModule] FOREIGN KEY([ProfileId])
REFERENCES [Common].[Profile] ([ProfileId])
GO
ALTER TABLE [Common].[ProfileModule] CHECK CONSTRAINT [FK_Profile_ProfileModule]
GO
ALTER TABLE [Common].[Role]  WITH CHECK ADD  CONSTRAINT [FK_Module_Role] FOREIGN KEY([ModuleId])
REFERENCES [Common].[Module] ([ModuleId])
GO
ALTER TABLE [Common].[Role] CHECK CONSTRAINT [FK_Module_Role]
GO
ALTER TABLE [Common].[RoleOperation]  WITH CHECK ADD  CONSTRAINT [FK_Operation_RoleOperation] FOREIGN KEY([OperationId])
REFERENCES [Common].[Operation] ([OperationId])
GO
ALTER TABLE [Common].[RoleOperation] CHECK CONSTRAINT [FK_Operation_RoleOperation]
GO
ALTER TABLE [Common].[RoleOperation]  WITH CHECK ADD  CONSTRAINT [FK_Role_RoleOperation] FOREIGN KEY([RoleId])
REFERENCES [Common].[Role] ([RoleId])
GO
ALTER TABLE [Common].[RoleOperation] CHECK CONSTRAINT [FK_Role_RoleOperation]
GO
ALTER TABLE [Common].[RoleProfile]  WITH CHECK ADD  CONSTRAINT [FK_Profile_RoleProfile] FOREIGN KEY([ProfileId])
REFERENCES [Common].[Profile] ([ProfileId])
GO
ALTER TABLE [Common].[RoleProfile] CHECK CONSTRAINT [FK_Profile_RoleProfile]
GO
ALTER TABLE [Common].[RoleProfile]  WITH CHECK ADD  CONSTRAINT [FK_Role_RoleProfile] FOREIGN KEY([RoleId])
REFERENCES [Common].[Role] ([RoleId])
GO
ALTER TABLE [Common].[RoleProfile] CHECK CONSTRAINT [FK_Role_RoleProfile]
GO
ALTER TABLE [Common].[UserApplication]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationContract_UserApplication] FOREIGN KEY([ContractId], [ApplicationId])
REFERENCES [Common].[ApplicationContract] ([ContractId], [ApplicationId])
GO
ALTER TABLE [Common].[UserApplication] CHECK CONSTRAINT [FK_ApplicationContract_UserApplication]
GO
ALTER TABLE [Common].[UserApplication]  WITH CHECK ADD  CONSTRAINT [FK_Profile_UserApplication] FOREIGN KEY([ProfileId])
REFERENCES [Common].[Profile] ([ProfileId])
GO
ALTER TABLE [Common].[UserApplication] CHECK CONSTRAINT [FK_Profile_UserApplication]
GO
ALTER TABLE [Common].[UserApplication]  WITH CHECK ADD  CONSTRAINT [FK_User_UserApplication] FOREIGN KEY([UserId])
REFERENCES [Common].[User] ([UserId])
GO
ALTER TABLE [Common].[UserApplication] CHECK CONSTRAINT [FK_User_UserApplication]
GO
/****************LOGIN CREATION***************************************/
USE [master]
GO
CREATE LOGIN [AuthorizationLogin] WITH PASSWORD=N'AuthorizationLogin', DEFAULT_DATABASE=[Monitoring], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [CotecnaAuthorization]
GO
CREATE USER [AuthorizationLogin] FOR LOGIN [AuthorizationLogin]
GO
USE [CotecnaAuthorization]
GO
ALTER ROLE [db_owner] ADD MEMBER [AuthorizationLogin]
GO