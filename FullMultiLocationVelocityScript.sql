USE [MultiLocationVelocity]
GO
/****** Object:  ForeignKey [FK_EmployeeEducation_Employees]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EmployeeEducation] DROP CONSTRAINT [FK_EmployeeEducation_Employees]
GO
/****** Object:  ForeignKey [FK_EmployeeEvaluation_GoalStatus]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EmployeeEvaluation] DROP CONSTRAINT [FK_EmployeeEvaluation_GoalStatus]
GO
/****** Object:  ForeignKey [FK_Employees_Organizations]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_Employees_Organizations]
GO
/****** Object:  ForeignKey [FK_EmployeeSkills_Employees]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EmployeeSkills] DROP CONSTRAINT [FK_EmployeeSkills_Employees]
GO
/****** Object:  ForeignKey [FK_EmploymentHistory_Employees]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EmploymentHistory] DROP CONSTRAINT [FK_EmploymentHistory_Employees]
GO
/****** Object:  ForeignKey [FK_EvaluationCycles_OrgLocations]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EvaluationCycles] DROP CONSTRAINT [FK_EvaluationCycles_OrgLocations]
GO
/****** Object:  ForeignKey [FK_Goals_OrgLocations]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [dbo].[Goals] DROP CONSTRAINT [FK_Goals_OrgLocations]
GO
/****** Object:  ForeignKey [FK_ManagerEvaluation_GoalStatus]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [dbo].[ManagerEvaluation] DROP CONSTRAINT [FK_ManagerEvaluation_GoalStatus]
GO
/****** Object:  ForeignKey [FK_RatingMaster_Organizations]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [dbo].[RatingMaster] DROP CONSTRAINT [FK_RatingMaster_Organizations]
GO
/****** Object:  Table [dbo].[EmployeeEducation]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EmployeeEducation] DROP CONSTRAINT [FK_EmployeeEducation_Employees]
GO
DROP TABLE [dbo].[EmployeeEducation]
GO
/****** Object:  Table [dbo].[EmployeeSkills]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EmployeeSkills] DROP CONSTRAINT [FK_EmployeeSkills_Employees]
GO
DROP TABLE [dbo].[EmployeeSkills]
GO
/****** Object:  Table [dbo].[EmploymentHistory]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EmploymentHistory] DROP CONSTRAINT [FK_EmploymentHistory_Employees]
GO
DROP TABLE [dbo].[EmploymentHistory]
GO
/****** Object:  StoredProcedure [dbo].[SetGoal]    Script Date: 01/04/2017 11:56:33 ******/
DROP PROCEDURE [dbo].[SetGoal]
GO
/****** Object:  Table [dbo].[EmployeeEvaluation]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EmployeeEvaluation] DROP CONSTRAINT [FK_EmployeeEvaluation_GoalStatus]
GO
ALTER TABLE [dbo].[EmployeeEvaluation] DROP CONSTRAINT [DF_EmployeeEvaluation_GoalRating]
GO
DROP TABLE [dbo].[EmployeeEvaluation]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_Employees_Organizations]
GO
ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [DF_Employees_Active]
GO
ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [DF_Employees_FirstTimer]
GO
DROP TABLE [dbo].[Employees]
GO
/****** Object:  Table [dbo].[Goals]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [dbo].[Goals] DROP CONSTRAINT [FK_Goals_OrgLocations]
GO
ALTER TABLE [dbo].[Goals] DROP CONSTRAINT [DF_Goals_Fixed]
GO
ALTER TABLE [dbo].[Goals] DROP CONSTRAINT [DF_Goals_Weightage]
GO
ALTER TABLE [dbo].[Goals] DROP CONSTRAINT [DF_Goals_Rejected]
GO
DROP TABLE [dbo].[Goals]
GO
/****** Object:  Table [dbo].[ManagerEvaluation]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [dbo].[ManagerEvaluation] DROP CONSTRAINT [FK_ManagerEvaluation_GoalStatus]
GO
ALTER TABLE [dbo].[ManagerEvaluation] DROP CONSTRAINT [DF_ManagerEvaluation_GoalRating]
GO
DROP TABLE [dbo].[ManagerEvaluation]
GO
/****** Object:  Table [dbo].[EvaluationCycles]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EvaluationCycles] DROP CONSTRAINT [FK_EvaluationCycles_OrgLocations]
GO
DROP TABLE [dbo].[EvaluationCycles]
GO
/****** Object:  Table [dbo].[RatingMaster]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [dbo].[RatingMaster] DROP CONSTRAINT [FK_RatingMaster_Organizations]
GO
DROP TABLE [dbo].[RatingMaster]
GO
/****** Object:  Table [dbo].[RejectedMessages]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [dbo].[RejectedMessages]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [dbo].[Roles]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[RunnableInstancesTable]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [System.Activities.DurableInstancing].[RunnableInstancesTable]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[ServiceDeploymentsTable]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [System.Activities.DurableInstancing].[ServiceDeploymentsTable]
GO
/****** Object:  Table [dbo].[EvaluationRatings]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [dbo].[EvaluationRatings]
GO
/****** Object:  Table [dbo].[FeedbackAnswerOptions]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [dbo].[FeedbackAnswerOptions]
GO
/****** Object:  Table [dbo].[FeedbackAnswers]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [dbo].[FeedbackAnswers]
GO
/****** Object:  Table [dbo].[FeedbackQuestions]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [dbo].[FeedbackQuestions]
GO
/****** Object:  Table [dbo].[MessageResources]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [dbo].[MessageResources] DROP CONSTRAINT [DF_MessageResources_Custom]
GO
DROP TABLE [dbo].[MessageResources]
GO
/****** Object:  Table [dbo].[Organizations]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [dbo].[Organizations]
GO
/****** Object:  Table [dbo].[OrgLocations]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [dbo].[OrgLocations]
GO
/****** Object:  Table [dbo].[GoalStatus]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [dbo].[GoalStatus]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[IdentityOwnerTable]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [System.Activities.DurableInstancing].[IdentityOwnerTable]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[InstanceMetadataChangesTable]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [System.Activities.DurableInstancing].[InstanceMetadataChangesTable]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[InstancesTable]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__Primi__4222D4EF]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__Compl__4316F928]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__Write__440B1D61]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__Write__44FF419A]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__Metad__45F365D3]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__DataE__46E78A0C]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__Metad__47DBAE45]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__LastU__48CFD27E]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__Suspe__49C3F6B7]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__Suspe__4AB81AF0]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__Block__4BAC3F29]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__LastM__4CA06362]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__Execu__4D94879B]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__IsIni__4E88ABD4]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__IsSus__4F7CD00D]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__IsRea__5070F446]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] DROP CONSTRAINT [DF__Instances__IsCom__5165187F]
GO
DROP TABLE [System.Activities.DurableInstancing].[InstancesTable]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[KeysTable]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [System.Activities.DurableInstancing].[KeysTable]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[LockOwnersTable]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] DROP CONSTRAINT [DF__LockOwner__Primi__52593CB8]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] DROP CONSTRAINT [DF__LockOwner__Compl__534D60F1]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] DROP CONSTRAINT [DF__LockOwner__Write__5441852A]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] DROP CONSTRAINT [DF__LockOwner__Write__5535A963]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] DROP CONSTRAINT [DF__LockOwner__Encod__5629CD9C]
GO
DROP TABLE [System.Activities.DurableInstancing].[LockOwnersTable]
GO
/****** Object:  Table [dbo].[Log]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [dbo].[Log]
GO
/****** Object:  Table [dbo].[Attachments]    Script Date: 01/04/2017 11:56:31 ******/
DROP TABLE [dbo].[Attachments]
GO
/****** Object:  Table [dbo].[Badges]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[Badges] DROP CONSTRAINT [DF_Badges_Viewed]
GO
DROP TABLE [dbo].[Badges]
GO
/****** Object:  Table [dbo].[BulkNotificationLog]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[BulkNotificationLog] DROP CONSTRAINT [DF_BulkNotificationLog_NumberOfNotifications]
GO
DROP TABLE [dbo].[BulkNotificationLog]
GO
/****** Object:  Table [dbo].[Country]    Script Date: 01/04/2017 11:56:31 ******/
DROP TABLE [dbo].[Country]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[DefinitionIdentityTable]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [System.Activities.DurableInstancing].[DefinitionIdentityTable]
GO
/****** Object:  Table [dbo].[Department]    Script Date: 01/04/2017 11:56:31 ******/
DROP TABLE [dbo].[Department]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 01/04/2017 11:56:31 ******/
DROP TABLE [dbo].[Departments]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[SqlWorkflowInstanceStoreVersionTable]    Script Date: 01/04/2017 11:56:32 ******/
DROP TABLE [System.Activities.DurableInstancing].[SqlWorkflowInstanceStoreVersionTable]
GO
/****** Object:  Table [dbo].[WFQueue]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [dbo].[WFQueue] DROP CONSTRAINT [DF_WFQueue_StartWf]
GO
DROP TABLE [dbo].[WFQueue]
GO
/****** Object:  Table [dbo].[EvaluationConclusion]    Script Date: 01/04/2017 11:56:31 ******/
DROP TABLE [dbo].[EvaluationConclusion]
GO
/****** Object:  Table [dbo].[EvaluationConclusion]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EvaluationConclusion](
	[gid] [uniqueidentifier] NOT NULL,
	[employeeid] [uniqueidentifier] NOT NULL,
	[evalcycleid] [bigint] NOT NULL,
	[training] [nvarchar](2000) NULL,
	[meetingsummary] [nvarchar](2000) NULL,
	[modifiedon] [datetime] NULL,
 CONSTRAINT [PK_EvaluationConclusion] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WFQueue]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WFQueue](
	[Username] [nvarchar](256) NOT NULL,
	[Wfid] [uniqueidentifier] NOT NULL,
	[StartDate] [datetime] NULL,
	[StartWf] [bit] NOT NULL CONSTRAINT [DF_WFQueue_StartWf]  DEFAULT ((1)),
	[WFParam1] [nvarchar](150) NULL,
	[WFParam2] [nvarchar](150) NULL,
	[WFParam3] [nvarchar](150) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[SqlWorkflowInstanceStoreVersionTable]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[SqlWorkflowInstanceStoreVersionTable](
	[Major] [bigint] NULL,
	[Minor] [bigint] NULL,
	[Build] [bigint] NULL,
	[Revision] [bigint] NULL,
	[LastUpdated] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Departments](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[OrgId] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Department]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Department](
	[Id] [bigint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[OrgId] [int] NOT NULL,
	[DepartmentHead] [uniqueidentifier] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [System.Activities.DurableInstancing].[DefinitionIdentityTable]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[DefinitionIdentityTable](
	[SurrogateIdentityId] [bigint] IDENTITY(1,1) NOT NULL,
	[DefinitionIdentityHash] [uniqueidentifier] NOT NULL,
	[DefinitionIdentityAnyRevisionHash] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Package] [nvarchar](max) NULL,
	[Build] [bigint] NULL,
	[Major] [bigint] NULL,
	[Minor] [bigint] NULL,
	[Revision] [bigint] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Country]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Country](
	[Id] [bigint] NOT NULL,
	[Name] [varchar](150) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BulkNotificationLog]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BulkNotificationLog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ReminderType] [varchar](50) NOT NULL,
	[SentOn] [datetime] NOT NULL,
	[NumberOfNotifications] [bigint] NOT NULL CONSTRAINT [DF_BulkNotificationLog_NumberOfNotifications]  DEFAULT ((0)),
 CONSTRAINT [PK_BulkNotificationLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Badges]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Badges](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ToBadge] [uniqueidentifier] NOT NULL,
	[FromBadge] [uniqueidentifier] NULL,
	[BadgeType] [nvarchar](50) NOT NULL,
	[Viewed] [bit] NOT NULL CONSTRAINT [DF_Badges_Viewed]  DEFAULT ((0)),
	[Description] [nvarchar](200) NULL,
 CONSTRAINT [PK_Badges] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Attachments]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Attachments](
	[AttachmentId] [uniqueidentifier] NOT NULL,
	[FileContents] [varbinary](max) NULL,
	[EvaluationGoalId] [uniqueidentifier] NOT NULL,
	[FileType] [nvarchar](50) NULL,
	[FileName] [nvarchar](250) NOT NULL,
	[EvaluationType] [nvarchar](50) NULL,
	[FileSize] [nvarchar](50) NULL,
 CONSTRAINT [PK_Attachments] PRIMARY KEY CLUSTERED 
(
	[AttachmentId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Log]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Thread] [varchar](255) NOT NULL,
	[Level] [varchar](50) NOT NULL,
	[Logger] [varchar](255) NOT NULL,
	[Message] [varchar](4000) NOT NULL,
	[Exception] [varchar](2000) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [System.Activities.DurableInstancing].[LockOwnersTable]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[LockOwnersTable](
	[Id] [uniqueidentifier] NOT NULL,
	[SurrogateLockOwnerId] [bigint] IDENTITY(1,1) NOT NULL,
	[LockExpiration] [datetime] NOT NULL,
	[WorkflowHostType] [uniqueidentifier] NULL,
	[MachineName] [nvarchar](128) NOT NULL,
	[EnqueueCommand] [bit] NOT NULL,
	[DeletesInstanceOnCompletion] [bit] NOT NULL,
	[PrimitiveLockOwnerData] [varbinary](max) NULL DEFAULT (NULL),
	[ComplexLockOwnerData] [varbinary](max) NULL DEFAULT (NULL),
	[WriteOnlyPrimitiveLockOwnerData] [varbinary](max) NULL DEFAULT (NULL),
	[WriteOnlyComplexLockOwnerData] [varbinary](max) NULL DEFAULT (NULL),
	[EncodingOption] [tinyint] NULL DEFAULT ((0)),
	[WorkflowIdentityFilter] [tinyint] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [System.Activities.DurableInstancing].[KeysTable]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[KeysTable](
	[Id] [uniqueidentifier] NOT NULL,
	[SurrogateKeyId] [bigint] IDENTITY(1,1) NOT NULL,
	[SurrogateInstanceId] [bigint] NULL,
	[EncodingOption] [tinyint] NULL,
	[Properties] [varbinary](max) NULL,
	[IsAssociated] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [System.Activities.DurableInstancing].[InstancesTable]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[InstancesTable](
	[Id] [uniqueidentifier] NOT NULL,
	[SurrogateInstanceId] [bigint] IDENTITY(1,1) NOT NULL,
	[SurrogateLockOwnerId] [bigint] NULL,
	[PrimitiveDataProperties] [varbinary](max) NULL DEFAULT (NULL),
	[ComplexDataProperties] [varbinary](max) NULL DEFAULT (NULL),
	[WriteOnlyPrimitiveDataProperties] [varbinary](max) NULL DEFAULT (NULL),
	[WriteOnlyComplexDataProperties] [varbinary](max) NULL DEFAULT (NULL),
	[MetadataProperties] [varbinary](max) NULL DEFAULT (NULL),
	[DataEncodingOption] [tinyint] NULL DEFAULT ((0)),
	[MetadataEncodingOption] [tinyint] NULL DEFAULT ((0)),
	[Version] [bigint] NOT NULL,
	[PendingTimer] [datetime] NULL,
	[CreationTime] [datetime] NOT NULL,
	[LastUpdated] [datetime] NULL DEFAULT (NULL),
	[WorkflowHostType] [uniqueidentifier] NULL,
	[ServiceDeploymentId] [bigint] NULL,
	[SuspensionExceptionName] [nvarchar](450) NULL DEFAULT (NULL),
	[SuspensionReason] [nvarchar](max) NULL DEFAULT (NULL),
	[BlockingBookmarks] [nvarchar](max) NULL DEFAULT (NULL),
	[LastMachineRunOn] [nvarchar](450) NULL DEFAULT (NULL),
	[ExecutionStatus] [nvarchar](450) NULL DEFAULT (NULL),
	[IsInitialized] [bit] NULL DEFAULT ((0)),
	[IsSuspended] [bit] NULL DEFAULT ((0)),
	[IsReadyToRun] [bit] NULL DEFAULT ((0)),
	[IsCompleted] [bit] NULL DEFAULT ((0)),
	[SurrogateIdentityId] [bigint] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable](
	[SurrogateInstanceId] [bigint] NOT NULL,
	[PromotionName] [nvarchar](400) NOT NULL,
	[Value1] [sql_variant] NULL,
	[Value2] [sql_variant] NULL,
	[Value3] [sql_variant] NULL,
	[Value4] [sql_variant] NULL,
	[Value5] [sql_variant] NULL,
	[Value6] [sql_variant] NULL,
	[Value7] [sql_variant] NULL,
	[Value8] [sql_variant] NULL,
	[Value9] [sql_variant] NULL,
	[Value10] [sql_variant] NULL,
	[Value11] [sql_variant] NULL,
	[Value12] [sql_variant] NULL,
	[Value13] [sql_variant] NULL,
	[Value14] [sql_variant] NULL,
	[Value15] [sql_variant] NULL,
	[Value16] [sql_variant] NULL,
	[Value17] [sql_variant] NULL,
	[Value18] [sql_variant] NULL,
	[Value19] [sql_variant] NULL,
	[Value20] [sql_variant] NULL,
	[Value21] [sql_variant] NULL,
	[Value22] [sql_variant] NULL,
	[Value23] [sql_variant] NULL,
	[Value24] [sql_variant] NULL,
	[Value25] [sql_variant] NULL,
	[Value26] [sql_variant] NULL,
	[Value27] [sql_variant] NULL,
	[Value28] [sql_variant] NULL,
	[Value29] [sql_variant] NULL,
	[Value30] [sql_variant] NULL,
	[Value31] [sql_variant] NULL,
	[Value32] [sql_variant] NULL,
	[Value33] [varbinary](max) NULL,
	[Value34] [varbinary](max) NULL,
	[Value35] [varbinary](max) NULL,
	[Value36] [varbinary](max) NULL,
	[Value37] [varbinary](max) NULL,
	[Value38] [varbinary](max) NULL,
	[Value39] [varbinary](max) NULL,
	[Value40] [varbinary](max) NULL,
	[Value41] [varbinary](max) NULL,
	[Value42] [varbinary](max) NULL,
	[Value43] [varbinary](max) NULL,
	[Value44] [varbinary](max) NULL,
	[Value45] [varbinary](max) NULL,
	[Value46] [varbinary](max) NULL,
	[Value47] [varbinary](max) NULL,
	[Value48] [varbinary](max) NULL,
	[Value49] [varbinary](max) NULL,
	[Value50] [varbinary](max) NULL,
	[Value51] [varbinary](max) NULL,
	[Value52] [varbinary](max) NULL,
	[Value53] [varbinary](max) NULL,
	[Value54] [varbinary](max) NULL,
	[Value55] [varbinary](max) NULL,
	[Value56] [varbinary](max) NULL,
	[Value57] [varbinary](max) NULL,
	[Value58] [varbinary](max) NULL,
	[Value59] [varbinary](max) NULL,
	[Value60] [varbinary](max) NULL,
	[Value61] [varbinary](max) NULL,
	[Value62] [varbinary](max) NULL,
	[Value63] [varbinary](max) NULL,
	[Value64] [varbinary](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [System.Activities.DurableInstancing].[InstanceMetadataChangesTable]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[InstanceMetadataChangesTable](
	[SurrogateInstanceId] [bigint] NOT NULL,
	[ChangeTime] [bigint] IDENTITY(1,1) NOT NULL,
	[EncodingOption] [tinyint] NOT NULL,
	[Change] [varbinary](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [System.Activities.DurableInstancing].[IdentityOwnerTable]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[IdentityOwnerTable](
	[SurrogateIdentityId] [bigint] NOT NULL,
	[SurrogateLockOwnerId] [bigint] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoalStatus]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoalStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_GoalStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrgLocations]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrgLocations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LocationName] [nvarchar](100) NULL,
	[OrgId] [int] NOT NULL,
	[Address1] [nvarchar](200) NULL,
	[Address2] [nvarchar](200) NULL,
	[City] [nvarchar](50) NULL,
	[State] [nvarchar](50) NULL,
	[Country] [nvarchar](100) NULL,
	[Zip] [nvarchar](50) NULL,
 CONSTRAINT [PK_OrgLocations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Organizations]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Organizations](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Organizations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MessageResources]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageResources](
	[MessageKey] [nvarchar](100) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[MessageType] [nvarchar](20) NULL,
	[OrgId] [int] NOT NULL,
	[Custom] [bit] NULL CONSTRAINT [DF_MessageResources_Custom]  DEFAULT ((1))
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FeedbackQuestions]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FeedbackQuestions](
	[QuestionId] [bigint] IDENTITY(1,1) NOT NULL,
	[Question] [nvarchar](1000) NULL,
	[OrgId] [bigint] NOT NULL,
 CONSTRAINT [PK_FeedbackQuestions] PRIMARY KEY CLUSTERED 
(
	[QuestionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FeedbackAnswers]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FeedbackAnswers](
	[AnswerId] [uniqueidentifier] NOT NULL,
	[QuestionId] [bigint] NOT NULL,
	[Answer] [nvarchar](2000) NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[EvalCycleId] [bigint] NOT NULL,
 CONSTRAINT [PK_FeedbackAnswers] PRIMARY KEY CLUSTERED 
(
	[AnswerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FeedbackAnswerOptions]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FeedbackAnswerOptions](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[QuestionId] [bigint] NOT NULL,
	[AnswerOption] [nvarchar](255) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EvaluationRatings]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EvaluationRatings](
	[EvalCycleId] [bigint] NOT NULL,
	[EmpId] [uniqueidentifier] NOT NULL,
	[ManagerId] [uniqueidentifier] NULL,
	[SelfOverallRating] [decimal](18, 2) NULL,
	[ManagerOverllRating] [decimal](18, 2) NULL,
	[HrApprovalStage1] [bit] NULL,
	[HrApprovalStage2] [bit] NULL,
	[OverAllReviewComment] [nvarchar](max) NULL,
	[OnGoingFeedBack] [nvarchar](max) NULL,
 CONSTRAINT [PK_EvaluationRatings] PRIMARY KEY CLUSTERED 
(
	[EvalCycleId] ASC,
	[EmpId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[ServiceDeploymentsTable]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[ServiceDeploymentsTable](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ServiceDeploymentHash] [uniqueidentifier] NOT NULL,
	[SiteName] [nvarchar](max) NOT NULL,
	[RelativeServicePath] [nvarchar](max) NOT NULL,
	[RelativeApplicationPath] [nvarchar](max) NOT NULL,
	[ServiceName] [nvarchar](max) NOT NULL,
	[ServiceNamespace] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[RunnableInstancesTable]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[RunnableInstancesTable](
	[SurrogateInstanceId] [bigint] NOT NULL,
	[WorkflowHostType] [uniqueidentifier] NULL,
	[ServiceDeploymentId] [bigint] NULL,
	[RunnableTime] [datetime] NOT NULL,
	[SurrogateIdentityId] [bigint] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] NOT NULL,
	[Role] [nvarchar](50) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RejectedMessages]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RejectedMessages](
	[Id] [uniqueidentifier] NOT NULL,
	[EvalCycleId] [bigint] NOT NULL,
	[ManagerID] [uniqueidentifier] NULL,
	[EmployeeId] [uniqueidentifier] NULL,
	[RejectionReason] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[EvaluationPhase] [nvarchar](50) NULL,
	[GoalId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_RejectedMessages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RatingMaster]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingMaster](
	[Id] [int] NOT NULL,
	[Rating] [int] NOT NULL,
	[Description] [nvarchar](50) NULL,
	[OrgId] [int] NOT NULL,
 CONSTRAINT [PK_RatingMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EvaluationCycles]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EvaluationCycles](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](50) NULL,
	[OrgId] [int] NOT NULL,
	[GoalStartDate] [datetime] NULL,
	[GoalEndDate] [datetime] NULL,
	[EvaluationStart] [datetime] NULL,
	[EvaluationEnd] [datetime] NULL,
 CONSTRAINT [PK_EvaluationCycles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ManagerEvaluation]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ManagerEvaluation](
	[Id] [uniqueidentifier] NOT NULL,
	[GoalId] [uniqueidentifier] NOT NULL,
	[ManagerId] [uniqueidentifier] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[GoalRating] [int] NULL CONSTRAINT [DF_ManagerEvaluation_GoalRating]  DEFAULT ((0)),
	[Status] [int] NULL,
	[EvalCycleId] [bigint] NULL,
	[EmployeeId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ManagerEvaluation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Goals]    Script Date: 01/04/2017 11:56:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Goals](
	[gid] [uniqueidentifier] NOT NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[Goal] [nvarchar](max) NOT NULL,
	[Fixed] [bit] NOT NULL CONSTRAINT [DF_Goals_Fixed]  DEFAULT ((0)),
	[OrgId] [int] NOT NULL,
	[EmployeeId] [uniqueidentifier] NULL,
	[Evalcycleid] [bigint] NULL,
	[Weightage] [int] NULL CONSTRAINT [DF_Goals_Weightage]  DEFAULT ((0)),
	[Title] [nvarchar](250) NULL,
	[Status] [int] NULL,
	[Rejected] [bit] NULL CONSTRAINT [DF_Goals_Rejected]  DEFAULT ((0)),
 CONSTRAINT [PK_Goals] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Employees](
	[gid] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](150) NOT NULL,
	[LastName] [nvarchar](150) NOT NULL,
	[Department] [nvarchar](150) NULL,
	[Manager] [uniqueidentifier] NULL,
	[Email] [nvarchar](150) NOT NULL,
	[Phone] [nvarchar](150) NULL,
	[OrgId] [int] NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_Employees_Active]  DEFAULT ((1)),
	[UserId] [uniqueidentifier] NULL,
	[Designation] [nvarchar](150) NULL,
	[DoJ] [datetime] NULL,
	[ProfilePix] [varbinary](max) NULL,
	[OrgEmpId] [nvarchar](10) NULL,
	[LocationId] [uniqueidentifier] NULL,
	[Mobile] [nvarchar](100) NULL,
	[FirstTime] [bit] NULL CONSTRAINT [DF_Employees_FirstTimer]  DEFAULT ((1)),
	[DoB] [datetime] NULL,
	[Gender] [nvarchar](8) NULL,
	[EmpType] [nvarchar](120) NULL,
	[LastVisit] [datetime] NULL,
	[Reviewer] [uniqueidentifier] NULL,
	[OrgLocationId] [int] NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EmployeeEvaluation]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmployeeEvaluation](
	[Id] [uniqueidentifier] NOT NULL,
	[GoalId] [uniqueidentifier] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[ModifiedBy] [uniqueidentifier] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[GoalRating] [int] NULL CONSTRAINT [DF_EmployeeEvaluation_GoalRating]  DEFAULT ((0)),
	[Status] [int] NULL,
	[Attachment1] [varbinary](max) NULL,
	[Attachment2] [varbinary](max) NULL,
	[Attachment3] [varbinary](max) NULL,
	[Attachment4] [varbinary](max) NULL,
	[Attachment] [varbinary](max) NULL,
	[EvalCycleId] [bigint] NULL,
	[EmployeeId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_EmployeeEvaluation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[SetGoal]    Script Date: 01/04/2017 11:56:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SetGoal] 
	-- Add the parameters for the stored procedure here
	@GoalId UniqueIdentifier, 
	@ModifiedBy UniqueIdentifier,
	@GoalText nvarchar(max),
	@fixed bit=0,
	@OrgId int,
	@EmployeeId UniqueIdentifier,
	@ManagerId UniqueIdentifier,
	@EvaluationCycleId Bigint,
	@Weightage int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
begin tran T1
    -- Insert statements for procedure here
    
    
    
    
	--INSERT INTO [Goals]
 --          ([gid]
 --          ,[ModifiedBy]
 --          ,[ModifiedDate]
 --          ,[Goal]
 --          ,[Fixed]
 --          ,[OrgId],
 --          [EmployeeId],
 --          [Draft]
 --          ,Evalcycleid,
 --          Weightage)
 --    VALUES
 --          (@GoalId
 --          ,@ModifiedBy
 --          ,GETDATE()
 --          ,@GoalText
 --          ,@fixed
 --          ,@OrgId
 --          ,@EmployeeId,
 --          1,
 --          @EvaluationCycleId,
 --          @Weightage)
           if not exists(select * from EmployeeEvaluation where EmployeeId=@EmployeeId and EvalCycleId = @EvaluationCycleId)
           begin
           declare @goal_id uniqueidentifier
				DECLARE fixedgoalsCursor CURSOR -- Declare cursor
					 
					FOR
					 
					Select gid  FROM Goals where OrgId= @OrgId and Fixed=1
					 
					OPEN fixedgoalsCursor -- open the cursor

					FETCH NEXT FROM fixedgoalsCursor INTO @goal_id

					WHILE @@FETCH_STATUS = 0
					 
					BEGIN
					 
						      
							 INSERT INTO [EmployeeEvaluation]
								   ([Id]
								   ,[GoalId]
								   ,[Comment]
								   ,[ModifiedBy]
								   ,[ModifiedDate]
								   ,[GoalRating]
						           , EmployeeId
						           , EvalCycleId
								   )
							 VALUES
								   (NEWID()
								   ,@goal_id
								   ,''
								   ,@ModifiedBy
								   ,GETDATE()
								   ,0
						           ,@EmployeeId
						           ,@EvaluationCycleId
								   )
						           
							 INSERT INTO [ManagerEvaluation]
								   ([Id]
								   ,[GoalId]
								   ,[ManagerId]
								   ,[Comment]
								   ,[ModifiedBy]
								   ,[ModifiedDate]
								   , EmployeeId
						           , EvalCycleId
								   )
							 VALUES
								   (NEWID()
								   ,@goal_id
								   ,@ManagerId
								   ,''
								   ,@EmployeeId
								   ,GETDATE()
								   ,@EmployeeId
						           ,@EvaluationCycleId
									)
					   
						FETCH NEXT FROM fixedgoalsCursor INTO @goal_id
					END
					 
					CLOSE fixedgoalsCursor -- close the cursor

					DEALLOCATE fixedgoalsCursor -- Deallocate the cursor	
           end
			
           
     INSERT INTO [EmployeeEvaluation]
           ([Id]
           ,[GoalId]
           ,[Comment]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[GoalRating]
			, EmployeeId
           , EvalCycleId
           )
     VALUES
           (NEWID()
           ,@GoalId
           ,''
           ,@ModifiedBy
           ,GETDATE()
           ,0
			,@EmployeeId
			,@EvaluationCycleId
           
           )
           
     INSERT INTO [ManagerEvaluation]
           ([Id]
           ,[GoalId]
           ,[ManagerId]
           ,[Comment]
           ,[ModifiedBy]
           ,[ModifiedDate]
           , EmployeeId
           , EvalCycleId

           )
     VALUES
           (NEWID()
           ,@GoalId
           ,@ManagerId
           ,''
           ,@EmployeeId
           ,GETDATE()
,@EmployeeId
,@EvaluationCycleId
           
			)
commit Tran T1

END
GO
/****** Object:  Table [dbo].[EmploymentHistory]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmploymentHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[Company] [nvarchar](150) NOT NULL,
	[FromDate] [datetime] NOT NULL,
	[ToDate] [datetime] NULL,
	[LastPositionHeld] [nvarchar](150) NOT NULL,
	[ReasonForLeaving] [nvarchar](250) NULL,
	[JobDescription] [nvarchar](2000) NOT NULL,
 CONSTRAINT [PK_EmploymentHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeSkills]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeSkills](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[SkillName] [nvarchar](150) NOT NULL,
	[SkillLevel] [smallint] NOT NULL,
 CONSTRAINT [PK_EmployeeSkills] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeEducation]    Script Date: 01/04/2017 11:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeEducation](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[CourseName] [nvarchar](150) NOT NULL,
	[YearOfPassing] [int] NOT NULL,
	[InstitutionName] [nvarchar](250) NOT NULL,
	[LevelDegree] [nvarchar](150) NOT NULL,
	[Marks] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_EmployeeEducation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_EmployeeEducation_Employees]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EmployeeEducation]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeEducation_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([gid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmployeeEducation] CHECK CONSTRAINT [FK_EmployeeEducation_Employees]
GO
/****** Object:  ForeignKey [FK_EmployeeEvaluation_GoalStatus]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EmployeeEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeEvaluation_GoalStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[GoalStatus] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmployeeEvaluation] CHECK CONSTRAINT [FK_EmployeeEvaluation_GoalStatus]
GO
/****** Object:  ForeignKey [FK_Employees_Organizations]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Organizations] FOREIGN KEY([OrgId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Organizations]
GO
/****** Object:  ForeignKey [FK_EmployeeSkills_Employees]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EmployeeSkills]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeSkills_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([gid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmployeeSkills] CHECK CONSTRAINT [FK_EmployeeSkills_Employees]
GO
/****** Object:  ForeignKey [FK_EmploymentHistory_Employees]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EmploymentHistory]  WITH CHECK ADD  CONSTRAINT [FK_EmploymentHistory_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([gid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmploymentHistory] CHECK CONSTRAINT [FK_EmploymentHistory_Employees]
GO
/****** Object:  ForeignKey [FK_EvaluationCycles_OrgLocations]    Script Date: 01/04/2017 11:56:31 ******/
ALTER TABLE [dbo].[EvaluationCycles]  WITH CHECK ADD  CONSTRAINT [FK_EvaluationCycles_OrgLocations] FOREIGN KEY([OrgId])
REFERENCES [dbo].[OrgLocations] ([Id])
GO
ALTER TABLE [dbo].[EvaluationCycles] CHECK CONSTRAINT [FK_EvaluationCycles_OrgLocations]
GO
/****** Object:  ForeignKey [FK_Goals_OrgLocations]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [dbo].[Goals]  WITH CHECK ADD  CONSTRAINT [FK_Goals_OrgLocations] FOREIGN KEY([OrgId])
REFERENCES [dbo].[OrgLocations] ([Id])
GO
ALTER TABLE [dbo].[Goals] CHECK CONSTRAINT [FK_Goals_OrgLocations]
GO
/****** Object:  ForeignKey [FK_ManagerEvaluation_GoalStatus]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [dbo].[ManagerEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_ManagerEvaluation_GoalStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[GoalStatus] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ManagerEvaluation] CHECK CONSTRAINT [FK_ManagerEvaluation_GoalStatus]
GO
/****** Object:  ForeignKey [FK_RatingMaster_Organizations]    Script Date: 01/04/2017 11:56:32 ******/
ALTER TABLE [dbo].[RatingMaster]  WITH CHECK ADD  CONSTRAINT [FK_RatingMaster_Organizations] FOREIGN KEY([OrgId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[RatingMaster] CHECK CONSTRAINT [FK_RatingMaster_Organizations]
GO
