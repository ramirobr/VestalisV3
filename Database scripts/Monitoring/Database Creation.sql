USE [master]
GO
IF EXISTS(SELECT 1 from sys.databases WHERE name ='Monitoring')
BEGIN
	EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'Monitoring'
	ALTER DATABASE [Monitoring] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP DATABASE [Monitoring]
	DROP LOGIN [monitor]
END
CREATE DATABASE [Monitoring]
GO
USE [Monitoring]
GO

/****** Object:  Table [dbo].[Application]    Script Date: 6/18/2012 12:28:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Application](
	[ApplicationId] [smallint] NOT NULL,
	[Description] [nvarchar](60) NOT NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Category]    Script Date: 6/18/2012 12:28:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[CategoryId] [smallint] NOT NULL,
	[Description] [nvarchar](60) NOT NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Counter]    Script Date: 6/18/2012 12:28:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Counter](
	[CounterId] [smallint] NOT NULL,
	[ApplicationId] [smallint] NOT NULL,
	[Value] [int] NOT NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
PRIMARY KEY CLUSTERED 
(
	[CounterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Priority]    Script Date: 6/18/2012 12:28:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Priority](
	[PriorityId] [smallint] NOT NULL,
	[Description] [nvarchar](60) NOT NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
PRIMARY KEY CLUSTERED 
(
	[PriorityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[State]    Script Date: 6/18/2012 12:28:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[State](
	[StateId] [smallint] NOT NULL,
	[Description] [nvarchar](60) NOT NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
PRIMARY KEY CLUSTERED 
(
	[StateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SupportUser]    Script Date: 6/18/2012 12:28:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupportUser](
	[SupportUserId] [uniqueidentifier] NOT NULL,
	[ApplicationId] [smallint] NOT NULL,
	[Email] [nvarchar](70) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
PRIMARY KEY CLUSTERED 
(
	[SupportUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Ticket]    Script Date: 6/18/2012 12:28:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ticket](
	[TicketId] [uniqueidentifier] NOT NULL,
	[PriorityId] [smallint] NOT NULL,
	[TicketTypeId] [smallint] NOT NULL,
	[CategoryId] [smallint] NOT NULL,
	[ApplicationId] [smallint] NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[CreationTicketDate] [datetime] NOT NULL,
	[Number] [nvarchar](50) NOT NULL,
	[UserEmail] [nvarchar](100) NULL,
	[UserModule] [nvarchar](100) NULL,
	[MessageException] [nvarchar](2000) NULL,
	[StackTrace] [nvarchar](max) NULL,
	[ClosedDate] [datetime] NULL,
	[CurrentStateId] [smallint] NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
PRIMARY KEY CLUSTERED 
(
	[TicketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TicketDetail]    Script Date: 6/18/2012 12:28:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicketDetail](
	[TicketDetailId] [uniqueidentifier] NOT NULL,
	[TicketId] [uniqueidentifier] NOT NULL,
	[StateId] [smallint] NOT NULL,
	[SupportUserName] [nvarchar](60) NULL,
	[Comment] [nvarchar](1000) NULL,
	[CommentDate] [datetime] NOT NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
PRIMARY KEY CLUSTERED 
(
	[TicketDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TicketType]    Script Date: 6/18/2012 12:28:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicketType](
	[TicketTypeId] [smallint] NOT NULL,
	[Description] [nvarchar](60) NOT NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreationBy] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModificationBy] [uniqueidentifier] NULL,
	[ModificationDate] [datetime] NULL,
	[WorkstationName] [nvarchar](16) NULL,
	[SmartserverId] [smallint] NULL,
	[LanguageId] [smallint] NULL,
PRIMARY KEY CLUSTERED 
(
	[TicketTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Ticket] ADD  DEFAULT (getdate()) FOR [CreationTicketDate]
GO
ALTER TABLE [dbo].[TicketDetail] ADD  DEFAULT (getdate()) FOR [CommentDate]
GO
ALTER TABLE [dbo].[Counter]  WITH CHECK ADD  CONSTRAINT [FK_Counter_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([ApplicationId])
GO
ALTER TABLE [dbo].[Counter] CHECK CONSTRAINT [FK_Counter_Application]
GO
ALTER TABLE [dbo].[SupportUser]  WITH CHECK ADD  CONSTRAINT [FK_SupportUser_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([ApplicationId])
GO
ALTER TABLE [dbo].[SupportUser] CHECK CONSTRAINT [FK_SupportUser_Application]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([ApplicationId])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_Application]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([CategoryId])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_Category]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Priority] FOREIGN KEY([PriorityId])
REFERENCES [dbo].[Priority] ([PriorityId])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_Priority]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_TicketType] FOREIGN KEY([TicketTypeId])
REFERENCES [dbo].[TicketType] ([TicketTypeId])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_TicketType]
GO
ALTER TABLE [dbo].[TicketDetail]  WITH CHECK ADD  CONSTRAINT [FK_TicketDetail_State] FOREIGN KEY([StateId])
REFERENCES [dbo].[State] ([StateId])
GO
ALTER TABLE [dbo].[TicketDetail] CHECK CONSTRAINT [FK_TicketDetail_State]
GO
ALTER TABLE [dbo].[TicketDetail]  WITH CHECK ADD  CONSTRAINT [FK_TicketDetail_Ticket] FOREIGN KEY([TicketId])
REFERENCES [dbo].[Ticket] ([TicketId])
GO
ALTER TABLE [dbo].[TicketDetail] CHECK CONSTRAINT [FK_TicketDetail_Ticket]
GO
/****** Object:  StoredProcedure [dbo].[usp_Counter_GetByApplication]    Script Date: 6/18/2012 12:28:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE  [dbo].[usp_Counter_GetByApplication]
	@applicationId int,
	@value int OUTPUT
AS
/********************************************************************
**		$Workfile: Monitor.usp_Counter_GetByApplication.StoredProcedure.sql $
**		Name: [dbo].[usp_Counter_GetByApplication]
**		Desc: Gets the identifier counter and updates
**         
**		Parameters:
**		Input
**		----------
**		@applicationId int
**
**		Output
**		----------
**		@Value int OUTPUT
**
**		$Author: ecuiojmarcillo $
**		$Date: 2012-09-02 $ 
**		$Revision:  $
*******************************************************************************
**		Change History
*******************************************************************************
**		Date:			Author:		Description:
**		--------		--------	-----------
** 		23/04/2008		mdavies		Creation
**		24/04/2008		rpurcell	Added creation of new entry for unknown code/officeId
**									Added locking hints to ensure serialized increments
**		05/09/2011		ososa		improve sp to work fine when it is called by several process (not repeat counters)
**		09/02/2012		mgarces		modified sp to be used in Monitoring service
******************************************************************************/

 BEGIN
	SET NOCOUNT ON
	BEGIN TRY

		BEGIN TRANSACTION

		-- Increment counter for this applicationId
		-- HOLDLOCK to ensure serialized increment of Value between transactions
		UPDATE	dbo.Counter
		--WITH (HOLDLOCK, XLOCK)
		SET		[Value] = [Value] + 1
		WHERE	ApplicationId = @applicationId

		-- Return value
		SELECT	@value = [Value] 
		FROM	dbo.Counter 
		WHERE	ApplicationId = @applicationId

		COMMIT TRANSACTION

	END TRY

	BEGIN CATCH

		IF @@TRANCOUNT > 0 
		 BEGIN
			ROLLBACK TRANSACTION
		 END

		--EXEC usp_Error_Rethrow;

	END CATCH
	
	SELECT	@value
	
	SET NOCOUNT OFF	
	SET IMPLICIT_TRANSACTIONS ON

 END


GO

/****************LOGIN CREATION***************************************/
USE [master]
GO
CREATE LOGIN [monitor] WITH PASSWORD=N'monitor', DEFAULT_DATABASE=[Monitoring], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [Monitoring]
GO
CREATE USER [monitor] FOR LOGIN [monitor]
GO
USE [Monitoring]
GO
ALTER ROLE [db_owner] ADD MEMBER [monitor]
GO
