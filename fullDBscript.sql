USE [MultiLocationVelocity]
GO
/****** Object:  DatabaseRole [System.Activities.DurableInstancing.InstanceStoreObservers]    Script Date: 12/19/2016 1:53:20 PM ******/
CREATE ROLE [System.Activities.DurableInstancing.InstanceStoreObservers]
GO
/****** Object:  DatabaseRole [System.Activities.DurableInstancing.InstanceStoreUsers]    Script Date: 12/19/2016 1:53:20 PM ******/
CREATE ROLE [System.Activities.DurableInstancing.InstanceStoreUsers]
GO
/****** Object:  DatabaseRole [System.Activities.DurableInstancing.WorkflowActivationUsers]    Script Date: 12/19/2016 1:53:20 PM ******/
CREATE ROLE [System.Activities.DurableInstancing.WorkflowActivationUsers]
GO
/****** Object:  Schema [System.Activities.DurableInstancing]    Script Date: 12/19/2016 1:53:21 PM ******/
CREATE SCHEMA [System.Activities.DurableInstancing]
GO
/****** Object:  UserDefinedFunction [System.Activities.DurableInstancing].[GetExpirationTime]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [System.Activities.DurableInstancing].[GetExpirationTime] (@offsetInMilliseconds bigint)
returns datetime
as
begin

	if (@offsetInMilliseconds is null)
	begin
		return null
	end

	declare @hourInMillisecond bigint
	declare @offsetInHours bigint
	declare @remainingOffsetInMilliseconds bigint
	declare @expirationTimer datetime

	set @hourInMillisecond = 60*60*1000
	set @offsetInHours = @offsetInMilliseconds / @hourInMillisecond
	set @remainingOffsetInMilliseconds = @offsetInMilliseconds % @hourInMillisecond

	set @expirationTimer = getutcdate()
	set @expirationTimer = dateadd (hour, @offsetInHours, @expirationTimer)
	set @expirationTimer = dateadd (millisecond,@remainingOffsetInMilliseconds, @expirationTimer)

	return @expirationTimer

end

GO
/****** Object:  UserDefinedFunction [System.Activities.DurableInstancing].[ParseBinaryPropertyValue]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [System.Activities.DurableInstancing].[ParseBinaryPropertyValue] (@startPosition int, @length int, @concatenatedKeyProperties varbinary(max))
returns varbinary(max)
as
begin
	if (@length > 0)
		return substring(@concatenatedKeyProperties, @startPosition + 1, @length)
	return null
end

GO
/****** Object:  Table [dbo].[Attachments]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Badges]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BulkNotificationLog]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EmployeeEducation]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmployeeEvaluation]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 12/19/2016 1:53:21 PM ******/
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
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EmployeeSkills]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmploymentHistory]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EvaluationConclusion]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EvaluationCycles]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EvaluationRatings]    Script Date: 12/19/2016 1:53:21 PM ******/
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
 CONSTRAINT [PK_EvaluationRatings] PRIMARY KEY CLUSTERED 
(
	[EvalCycleId] ASC,
	[EmpId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FeedbackAnswerOptions]    Script Date: 12/19/2016 1:53:21 PM ******/
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
/****** Object:  Table [dbo].[FeedbackAnswers]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FeedbackQuestions]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Goals]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GoalStatus]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Log]    Script Date: 12/19/2016 1:53:21 PM ******/
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
/****** Object:  Table [dbo].[ManagerEvaluation]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MessageResources]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageResources](
	[MessageKey] [nvarchar](100) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[MessageType] [nvarchar](20) NULL,
	[OrgId] [int] NOT NULL,
	[Custom] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Organizations]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrgLocations]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrgLocations](
	[Id] [uniqueidentifier] NOT NULL,
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingMaster]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RejectedMessages]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Roles]    Script Date: 12/19/2016 1:53:21 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WFQueue]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WFQueue](
	[Username] [nvarchar](256) NOT NULL,
	[Wfid] [uniqueidentifier] NOT NULL,
	[StartDate] [datetime] NULL,
	[StartWf] [bit] NOT NULL,
	[WFParam1] [nvarchar](150) NULL,
	[WFParam2] [nvarchar](150) NULL,
	[WFParam3] [nvarchar](150) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [System.Activities.DurableInstancing].[DefinitionIdentityTable]    Script Date: 12/19/2016 1:53:21 PM ******/
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
/****** Object:  Table [System.Activities.DurableInstancing].[IdentityOwnerTable]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[IdentityOwnerTable](
	[SurrogateIdentityId] [bigint] NOT NULL,
	[SurrogateLockOwnerId] [bigint] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [System.Activities.DurableInstancing].[InstanceMetadataChangesTable]    Script Date: 12/19/2016 1:53:21 PM ******/
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
/****** Object:  Table [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]    Script Date: 12/19/2016 1:53:21 PM ******/
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
/****** Object:  Table [System.Activities.DurableInstancing].[InstancesTable]    Script Date: 12/19/2016 1:53:21 PM ******/
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
	[PrimitiveDataProperties] [varbinary](max) NULL,
	[ComplexDataProperties] [varbinary](max) NULL,
	[WriteOnlyPrimitiveDataProperties] [varbinary](max) NULL,
	[WriteOnlyComplexDataProperties] [varbinary](max) NULL,
	[MetadataProperties] [varbinary](max) NULL,
	[DataEncodingOption] [tinyint] NULL,
	[MetadataEncodingOption] [tinyint] NULL,
	[Version] [bigint] NOT NULL,
	[PendingTimer] [datetime] NULL,
	[CreationTime] [datetime] NOT NULL,
	[LastUpdated] [datetime] NULL,
	[WorkflowHostType] [uniqueidentifier] NULL,
	[ServiceDeploymentId] [bigint] NULL,
	[SuspensionExceptionName] [nvarchar](450) NULL,
	[SuspensionReason] [nvarchar](max) NULL,
	[BlockingBookmarks] [nvarchar](max) NULL,
	[LastMachineRunOn] [nvarchar](450) NULL,
	[ExecutionStatus] [nvarchar](450) NULL,
	[IsInitialized] [bit] NULL,
	[IsSuspended] [bit] NULL,
	[IsReadyToRun] [bit] NULL,
	[IsCompleted] [bit] NULL,
	[SurrogateIdentityId] [bigint] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [System.Activities.DurableInstancing].[KeysTable]    Script Date: 12/19/2016 1:53:21 PM ******/
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
/****** Object:  Table [System.Activities.DurableInstancing].[LockOwnersTable]    Script Date: 12/19/2016 1:53:21 PM ******/
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
	[PrimitiveLockOwnerData] [varbinary](max) NULL,
	[ComplexLockOwnerData] [varbinary](max) NULL,
	[WriteOnlyPrimitiveLockOwnerData] [varbinary](max) NULL,
	[WriteOnlyComplexLockOwnerData] [varbinary](max) NULL,
	[EncodingOption] [tinyint] NULL,
	[WorkflowIdentityFilter] [tinyint] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [System.Activities.DurableInstancing].[RunnableInstancesTable]    Script Date: 12/19/2016 1:53:21 PM ******/
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
/****** Object:  Table [System.Activities.DurableInstancing].[ServiceDeploymentsTable]    Script Date: 12/19/2016 1:53:21 PM ******/
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
/****** Object:  Table [System.Activities.DurableInstancing].[SqlWorkflowInstanceStoreVersionTable]    Script Date: 12/19/2016 1:53:21 PM ******/
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
/****** Object:  View [System.Activities.DurableInstancing].[InstancePromotedProperties]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [System.Activities.DurableInstancing].[InstancePromotedProperties] with schemabinding as
      select [InstancesTable].[Id] as [InstanceId],
			 [InstancesTable].[DataEncodingOption] as [EncodingOption],
			 [PromotionName],
			 [Value1],
			 [Value2],
			 [Value3],
			 [Value4],
			 [Value5],
			 [Value6],
			 [Value7],
			 [Value8],
			 [Value9],
			 [Value10],
			 [Value11],
			 [Value12],
			 [Value13],
			 [Value14],
			 [Value15],
			 [Value16],
			 [Value17],
			 [Value18],
			 [Value19],
			 [Value20],
			 [Value21],
			 [Value22],
			 [Value23],
			 [Value24],
			 [Value25],
			 [Value26],
			 [Value27],
			 [Value28],
			 [Value29],
			 [Value30],
			 [Value31],
			 [Value32],
			 [Value33],
			 [Value34],
			 [Value35],
			 [Value36],
			 [Value37],
			 [Value38],
			 [Value39],
			 [Value40],
			 [Value41],
			 [Value42],
			 [Value43],
			 [Value44],
			 [Value45],
			 [Value46],
			 [Value47],
			 [Value48],
			 [Value49],
			 [Value50],
			 [Value51],
			 [Value52],
			 [Value53],
			 [Value54],
			 [Value55],
			 [Value56],
			 [Value57],
			 [Value58],
			 [Value59],
			 [Value60],
			 [Value61],
			 [Value62],
			 [Value63],
			 [Value64]
    from [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]
    join [System.Activities.DurableInstancing].[InstancesTable]
    on [InstancePromotedPropertiesTable].[SurrogateInstanceId] = [InstancesTable].[SurrogateInstanceId]

GO
/****** Object:  View [System.Activities.DurableInstancing].[Instances]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [System.Activities.DurableInstancing].[Instances] as
      select [InstancesTable].[Id] as [InstanceId],
             [PendingTimer],
             [CreationTime],
             [LastUpdated] as [LastUpdatedTime],
             [InstancesTable].[ServiceDeploymentId],
             [SuspensionExceptionName],
             [SuspensionReason],
             [BlockingBookmarks] as [ActiveBookmarks],
             case when [LockOwnersTable].[LockExpiration] > getutcdate()
				then [LockOwnersTable].[MachineName]
				else null
				end as [CurrentMachine],
             [LastMachineRunOn] as [LastMachine],
             [ExecutionStatus],
             [IsInitialized],
             [IsSuspended],
             [IsCompleted],
             [InstancesTable].[DataEncodingOption] as [EncodingOption],
             [PrimitiveDataProperties] as [ReadWritePrimitiveDataProperties],
             [WriteOnlyPrimitiveDataProperties],
             [ComplexDataProperties] as [ReadWriteComplexDataProperties],
             [WriteOnlyComplexDataProperties],
             [Name] as [IdentityName],
             [Package] as [IdentityPackage],
             [Build],
             [Major],
             [Minor],
             [Revision]
      from [System.Activities.DurableInstancing].[InstancesTable]
      left outer join [System.Activities.DurableInstancing].[LockOwnersTable]
      on [InstancesTable].[SurrogateLockOwnerId] = [LockOwnersTable].[SurrogateLockOwnerId]
      join [System.Activities.DurableInstancing].[DefinitionIdentityTable]
      on [InstancesTable].[SurrogateIdentityId] = [DefinitionIdentityTable].[SurrogateIdentityId]

GO
/****** Object:  View [System.Activities.DurableInstancing].[ServiceDeployments]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [System.Activities.DurableInstancing].[ServiceDeployments] as
      select [Id] as [ServiceDeploymentId],
             [SiteName],
             [RelativeServicePath],
             [RelativeApplicationPath],
             [ServiceName],
             [ServiceNamespace]
      from [System.Activities.DurableInstancing].[ServiceDeploymentsTable]

GO
ALTER TABLE [dbo].[MessageResources] ADD  CONSTRAINT [DF_MessageResources_Custom]  DEFAULT ((1)) FOR [Custom]
GO
ALTER TABLE [dbo].[WFQueue] ADD  CONSTRAINT [DF_WFQueue_StartWf]  DEFAULT ((1)) FOR [StartWf]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [PrimitiveDataProperties]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [ComplexDataProperties]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [WriteOnlyPrimitiveDataProperties]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [WriteOnlyComplexDataProperties]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [MetadataProperties]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT ((0)) FOR [DataEncodingOption]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT ((0)) FOR [MetadataEncodingOption]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [LastUpdated]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [SuspensionExceptionName]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [SuspensionReason]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [BlockingBookmarks]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [LastMachineRunOn]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [ExecutionStatus]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT ((0)) FOR [IsInitialized]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT ((0)) FOR [IsSuspended]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT ((0)) FOR [IsReadyToRun]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT ((0)) FOR [IsCompleted]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] ADD  DEFAULT (NULL) FOR [PrimitiveLockOwnerData]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] ADD  DEFAULT (NULL) FOR [ComplexLockOwnerData]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] ADD  DEFAULT (NULL) FOR [WriteOnlyPrimitiveLockOwnerData]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] ADD  DEFAULT (NULL) FOR [WriteOnlyComplexLockOwnerData]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] ADD  DEFAULT ((0)) FOR [EncodingOption]
GO
ALTER TABLE [dbo].[EmployeeEducation]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeEducation_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([gid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmployeeEducation] CHECK CONSTRAINT [FK_EmployeeEducation_Employees]
GO
ALTER TABLE [dbo].[EmployeeEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeEvaluation_GoalStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[GoalStatus] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmployeeEvaluation] CHECK CONSTRAINT [FK_EmployeeEvaluation_GoalStatus]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Organizations] FOREIGN KEY([OrgId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Organizations]
GO
ALTER TABLE [dbo].[EmployeeSkills]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeSkills_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([gid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmployeeSkills] CHECK CONSTRAINT [FK_EmployeeSkills_Employees]
GO
ALTER TABLE [dbo].[EmploymentHistory]  WITH CHECK ADD  CONSTRAINT [FK_EmploymentHistory_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([gid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmploymentHistory] CHECK CONSTRAINT [FK_EmploymentHistory_Employees]
GO
ALTER TABLE [dbo].[EvaluationCycles]  WITH CHECK ADD  CONSTRAINT [FK_EvaluationCycles_Organizations] FOREIGN KEY([OrgId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[EvaluationCycles] CHECK CONSTRAINT [FK_EvaluationCycles_Organizations]
GO
ALTER TABLE [dbo].[Goals]  WITH CHECK ADD  CONSTRAINT [FK_Goals_Organization] FOREIGN KEY([OrgId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[Goals] CHECK CONSTRAINT [FK_Goals_Organization]
GO
ALTER TABLE [dbo].[ManagerEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_ManagerEvaluation_GoalStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[GoalStatus] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ManagerEvaluation] CHECK CONSTRAINT [FK_ManagerEvaluation_GoalStatus]
GO
ALTER TABLE [dbo].[RatingMaster]  WITH CHECK ADD  CONSTRAINT [FK_RatingMaster_Organizations] FOREIGN KEY([OrgId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[RatingMaster] CHECK CONSTRAINT [FK_RatingMaster_Organizations]
GO
/****** Object:  StoredProcedure [dbo].[SetGoal]    Script Date: 12/19/2016 1:53:21 PM ******/
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
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[AssociateKeys]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[AssociateKeys]
	@surrogateInstanceId bigint,
	@keysToAssociate xml = null,
	@concatenatedKeyProperties varbinary(max) = null,
	@encodingOption tinyint,
	@singleKeyId uniqueidentifier
as
begin	
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	declare @badKeyId uniqueidentifier
	declare @numberOfKeys int
	declare @result int
	declare @keys table([KeyId] uniqueidentifier, [Properties] varbinary(max))
	
	set @result = 0
	
	if (@keysToAssociate is not null)
	begin
		insert into @keys
		select T.Item.value('@KeyId', 'uniqueidentifier') as [KeyId],
			   [System.Activities.DurableInstancing].[ParseBinaryPropertyValue](T.Item.value('@StartPosition', 'int'), T.Item.value('@BinaryLength', 'int'), @concatenatedKeyProperties) as [Properties]
	    from @keysToAssociate.nodes('/CorrelationKeys/CorrelationKey') as T(Item)
		option (maxdop 1)

		select @numberOfKeys = count(1) from @keys
		
		insert into [KeysTable] ([Id], [SurrogateInstanceId], [IsAssociated])
		select [KeyId], @surrogateInstanceId, 1
		from @keys as [Keys]
		
		if (@@rowcount != @numberOfKeys)
		begin
			select top 1 @badKeyId = [Keys].[KeyId] 
			from @keys as [Keys]
			join [KeysTable] on [Keys].[KeyId] = [KeysTable].[Id]
			where [KeysTable].[SurrogateInstanceId] != @surrogateInstanceId
			
			if (@@rowcount != 0)
			begin
				select 3 as 'Result', @badKeyId
				return 3
			end
		end
		
		update [KeysTable]
		set [Properties] = [Keys].[Properties],
			[EncodingOption] = @encodingOption
		from @keys as [Keys]
		join [KeysTable] on [Keys].[KeyId] = [KeysTable].[Id]
		where [KeysTable].[EncodingOption] is null
	end
	
	if (@singleKeyId is not null)
	begin
InsertSingleKey:
		update [KeysTable]
		set [Properties] = @concatenatedKeyProperties,
			[EncodingOption] = @encodingOption
		where ([Id] = @singleKeyId) and ([SurrogateInstanceId] = @surrogateInstanceId)
			  
		if (@@rowcount != 1)
		begin
			if exists (select [Id] from [KeysTable] where [Id] = @singleKeyId)
			begin
				select 3 as 'Result', @singleKeyId
				return 3
			end
			
			insert into [KeysTable] ([Id], [SurrogateInstanceId], [IsAssociated], [Properties], [EncodingOption])
			values (@singleKeyId, @surrogateInstanceId, 1, @concatenatedKeyProperties, @encodingOption)
			
			if (@@rowcount = 0)
				goto InsertSingleKey
		end
	end
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[CompleteKeys]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[CompleteKeys]
	@surrogateInstanceId bigint,
	@keysToComplete xml = null
as
begin	
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	declare @badKeyId uniqueidentifier
	declare @numberOfKeys int
	declare @result int
	declare @keyIds table([KeyId] uniqueidentifier)
	
	set @result = 0
	
	if (@keysToComplete is not null)
	begin
		insert into @keyIds
		select T.Item.value('@KeyId', 'uniqueidentifier')
		from @keysToComplete.nodes('//CorrelationKey') as T(Item)
		option(maxdop 1)
		
		select @numberOfKeys = count(1) from @keyIds
		
		update [KeysTable]
		set [IsAssociated] = 0
		from @keyIds as [KeyIds]
		join [KeysTable] on [KeyIds].[KeyId] = [KeysTable].[Id]
		where [SurrogateInstanceId] = @surrogateInstanceId
		
		if (@@rowcount != @numberOfKeys)
		begin
			select top 1 @badKeyId = [MissingKeys].[MissingKeyId]
			from
				(select [KeyIds].[KeyId] as [MissingKeyId] 
				 from @keyIds as [KeyIds]
				 except
				 select [Id] from [KeysTable] where [SurrogateInstanceId] = @surrogateInstanceId) as MissingKeys
		
			select 4 as 'Result', @badKeyId
			return 4
		end
	end
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[CreateInstance]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[CreateInstance]
	@instanceId uniqueidentifier,
	@surrogateLockOwnerId bigint,
	@workflowHostType uniqueidentifier,
	@serviceDeploymentId bigint,
	@surrogateInstanceId bigint output,
	@result int output
as
begin
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	set @surrogateInstanceId = 0
	set @result = 0
	declare	@surrogateIdentityId bigint
	
	select	@surrogateIdentityId = [SurrogateIdentityId]
	from	[System.Activities.DurableInstancing].[DefinitionIdentityTable]
	where	[DefinitionIdentityHash] = '00000000-0000-0000-0000-000000000000'
	and		[DefinitionIdentityAnyRevisionHash] = '00000000-0000-0000-0000-000000000000'
	and		[Name] is null
	and		[Package] is null
	and		[Build] is null
	and		[Major] is null
	and		[Minor] is null
	and		[Revision] is null

	if @surrogateIdentityId is null
	 begin
		goto Error
	 end
	else
	 begin
		begin try
			insert into [InstancesTable] ([Id], [SurrogateLockOwnerId], [CreationTime], [WorkflowHostType], [ServiceDeploymentId], [Version], [SurrogateIdentityId])
			values (@instanceId, @surrogateLockOwnerId, getutcdate(), @workflowHostType, @serviceDeploymentId, 1, @surrogateIdentityId)
			
			set @surrogateInstanceId = scope_identity()		
		end try
		begin catch
			if (error_number() != 2601)
			begin
				goto Error
			end
		end catch
	 end
	 
	 goto Done

Error:
	set @result = 99
	select @result as 'Result'
	
Done:

end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[CreateLockOwner]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[CreateLockOwner]
	@lockOwnerId uniqueidentifier,
	@lockTimeout int,
	@workflowHostType uniqueidentifier,
	@enqueueCommand bit,
	@deleteInstanceOnCompletion bit,	
	@primitiveLockOwnerData varbinary(max),
	@complexLockOwnerData varbinary(max),
	@writeOnlyPrimitiveLockOwnerData varbinary(max),
	@writeOnlyComplexLockOwnerData varbinary(max),
	@encodingOption tinyint,
	@machineName nvarchar(128),
	@identityMetadata xml = null
as
begin
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	begin transaction
	
	declare @lockAcquired bigint
	declare @lockExpiration datetime
	declare @now datetime
	declare @result int
	declare @surrogateLockOwnerId bigint
	declare @workflowIdentityFilter tinyint

	set @result = 0
	
	exec @lockAcquired = sp_getapplock @Resource = 'InstanceStoreLock', @LockMode = 'Shared', @LockTimeout = 10000
		
	if (@lockAcquired < 0)
	begin
		select @result as 'Result'
		set @result = 13
	end
	
	if (@result = 0)
	begin
		-- insert the list of identity to the DefinitionIdentityTable
		exec [System.Activities.DurableInstancing].[InsertDefinitionIdentity] @identityMetadata
		
		if (@identityMetadata is not null)
		 begin
			select @workflowIdentityFilter = T.Item.value('WorkflowIdentityFilter[1]', 'tinyint')
			from @identityMetadata.nodes('/IdentityMetadata') as T(Item)
		 end
		
		if @workflowIdentityFilter is null
			set @workflowIdentityFilter = 0

		--Create Lock Owner entry 
		set @now = getutcdate()
		
		if (@lockTimeout = 0)
			set @lockExpiration = '9999-12-31T23:59:59';
		else 
			set @lockExpiration = dateadd(second, @lockTimeout, getutcdate());
		
		insert into [LockOwnersTable] ([Id], [LockExpiration], [MachineName], [WorkflowHostType], [EnqueueCommand], [DeletesInstanceOnCompletion], [PrimitiveLockOwnerData], [ComplexLockOwnerData], [WriteOnlyPrimitiveLockOwnerData], [WriteOnlyComplexLockOwnerData], [EncodingOption], [WorkflowIdentityFilter])
		values (@lockOwnerId, @lockExpiration, @machineName, @workflowHostType, @enqueueCommand, @deleteInstanceOnCompletion, @primitiveLockOwnerData, @complexLockOwnerData, @writeOnlyPrimitiveLockOwnerData, @writeOnlyComplexLockOwnerData, @encodingOption, @workflowIdentityFilter)
		
		set @surrogateLockOwnerId = scope_identity()
		
		-- insert identity collection with its lock owner.
		if (@identityMetadata is not null)
		 begin
			insert into [System.Activities.DurableInstancing].[IdentityOwnerTable] 
			(
						[SurrogateIdentityId], 
						[SurrogateLockOwnerId]
			)
			select		[System.Activities.DurableInstancing].[DefinitionIdentityTable].[SurrogateIdentityId], 
						@surrogateLockOwnerId
			from		[System.Activities.DurableInstancing].[DefinitionIdentityTable]
			where		[System.Activities.DurableInstancing].[DefinitionIdentityTable].[DefinitionIdentityHash] in
						(
							select	T.Item.value('DefinitionIdentityHash[1]', 'uniqueidentifier') 
							from	@identityMetadata.nodes('/IdentityMetadata/IdentityCollection/Identity') as T(Item)
						)
		 end
		else
		 begin
			insert into [System.Activities.DurableInstancing].[IdentityOwnerTable] 
			(
				[SurrogateIdentityId], 
				[SurrogateLockOwnerId]
			)
			select  [SurrogateIdentityId], @surrogateLockOwnerId
			from	[System.Activities.DurableInstancing].[DefinitionIdentityTable]
			where	[DefinitionIdentityHash] = '00000000-0000-0000-0000-000000000000'
		 end 
	end
	
	if (@result != 13)
		exec sp_releaseapplock @Resource = 'InstanceStoreLock'
	
	if (@result = 0)
	begin
		commit transaction
		select 0 as 'Result', @surrogateLockOwnerId
	end
	else
		rollback transaction
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[CreateServiceDeployment]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[CreateServiceDeployment]	
	@serviceDeploymentHash uniqueIdentifier,
	@siteName nvarchar(max),
	@relativeServicePath nvarchar(max),
	@relativeApplicationPath nvarchar(max),
	@serviceName nvarchar(max),
    @serviceNamespace nvarchar(max),
    @serviceDeploymentId bigint output
as
begin
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;	
	
		--Create or select the service deployment id
		insert into [ServiceDeploymentsTable]
			([ServiceDeploymentHash], [SiteName], [RelativeServicePath], [RelativeApplicationPath], [ServiceName], [ServiceNamespace])
			values (@serviceDeploymentHash, @siteName, @relativeServicePath, @relativeApplicationPath, @serviceName, @serviceNamespace)
			
		if (@@rowcount = 0)
		begin		
			select @serviceDeploymentId = [Id]
			from [ServiceDeploymentsTable]
			where [ServiceDeploymentHash] = @serviceDeploymentHash		
		end
		else			
		begin
			set @serviceDeploymentId = scope_identity()		
		end	
		
		select 0 as 'Result', @serviceDeploymentId		
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[DeleteInstance]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[DeleteInstance]
	@surrogateInstanceId bigint = null
as
begin	
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;	
	
	delete [InstancePromotedPropertiesTable]
	where [SurrogateInstanceId] = @surrogateInstanceId
		
	delete [KeysTable]
	where [SurrogateInstanceId] = @surrogateInstanceId
		
	delete [InstanceMetadataChangesTable]
	where [SurrogateInstanceId] = @surrogateInstanceId

	delete [RunnableInstancesTable] 
	where [SurrogateInstanceId] = @surrogateInstanceId

	delete [InstancesTable] 
	where [SurrogateInstanceId] = @surrogateInstanceId

end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[DeleteLockOwner]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[DeleteLockOwner]
	@surrogateLockOwnerId bigint
as
begin
	set nocount on
	set transaction isolation level read committed
	set deadlock_priority low
	set xact_abort on;	
	
	begin transaction
	
	declare @lockAcquired bigint
	declare @result int
	set @result = 0
	
	exec @lockAcquired = sp_getapplock @Resource = 'InstanceStoreLock', @LockMode = 'Shared', @LockTimeout = 10000
		
	if (@lockAcquired < 0)
	begin
		select @result as 'Result'
		set @result = 13
	end
	
	if (@result = 0)
	begin
		update [LockOwnersTable]
		set [LockExpiration] = '2000-01-01T00:00:00'
		where [SurrogateLockOwnerId] = @surrogateLockOwnerId
	end
	
	if (@result != 13)
		exec sp_releaseapplock @Resource = 'InstanceStoreLock' 
	
	if (@result = 0)
	begin
		commit transaction
		select 0 as 'Result'
	end
	else
		rollback transaction
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[DetectRunnableInstances]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[DetectRunnableInstances]
	@workflowHostType uniqueidentifier,
	@surrogateLockOwnerId bigint = null
as
begin
	set nocount on
	set transaction isolation level read committed	
	set xact_abort on;	
	set deadlock_priority low
	
	declare @nextRunnableTime datetime
	declare @workflowIdentityFilter tinyint
	
	if (@surrogateLockOwnerId is not null)
	 begin
		select @workflowIdentityFilter = [WorkflowIdentityFilter]
		from [LockOwnersTable]
		where [LockOwnersTable].SurrogateLockOwnerId = @surrogateLockOwnerId
	 end
	
	if (@workflowIdentityFilter is null)
		set @workflowIdentityFilter = 0
				
	if (@workflowIdentityFilter = 1)
	begin
		-- Any Identity
		select top 1 @nextRunnableTime = [RunnableInstancesTable].[RunnableTime]
				  from [RunnableInstancesTable] with (readpast)
				  where [WorkflowHostType] = @workflowHostType
				  order by [WorkflowHostType], [RunnableTime]
	end
	else if (@workflowIdentityFilter = 2)
	begin
		--AnyRevision
		declare @AnyRevisionFilter table
		(
			DefinitionIdentityAnyRevisionHash uniqueidentifier
			UNIQUE (DefinitionIdentityAnyRevisionHash)
		)

		insert into @AnyRevisionFilter 
		select distinct [DefinitionIdentityAnyRevisionHash]
		from [DefinitionIdentityTable] as IdentityTable, [IdentityOwnerTable] as IdentityOwnerTable
		where IdentityOwnerTable.[SurrogateLockOwnerId] = @surrogateLockOwnerId 
			and 
			IdentityTable.[SurrogateIdentityId] = IdentityOwnerTable.[SurrogateIdentityId]
		
		select top 1 @nextRunnableTime = RunnableInstance.[RunnableTime]
				  from [RunnableInstancesTable] as RunnableInstance with (readpast) 
					inner loop join (
						select IdentityTable.[SurrogateIdentityId] as IdentityId
						from [System.Activities.DurableInstancing].[DefinitionIdentityTable] as IdentityTable, @AnyRevisionFilter as AnyRevision
						where IdentityTable.[DefinitionIdentityAnyRevisionHash] = AnyRevision.DefinitionIdentityAnyRevisionHash
					) as FilteredIdentities
					on RunnableInstance.[SurrogateIdentityId] =  FilteredIdentities.IdentityId
				  where RunnableInstance.[WorkflowHostType] = @workflowHostType
				  order by [WorkflowHostType], [RunnableTime]	
	end 
	else
		begin
			-- default to Exact
			if (@surrogateLockOwnerId is null)
			 begin
				declare	@surrogateIdentityId bigint
	
				select		@surrogateIdentityId = [SurrogateIdentityId]
				from		[System.Activities.DurableInstancing].[DefinitionIdentityTable]
				where		[DefinitionIdentityHash] = '00000000-0000-0000-0000-000000000000'
				and			[DefinitionIdentityAnyRevisionHash] = '00000000-0000-0000-0000-000000000000'
				and			[Name] is null
				and			[Package] is null
				and			[Build] is null
				and			[Major] is null
				and			[Minor] is null
				and			[Revision] is null
				
				select		top 1 @nextRunnableTime = RunnableInstances.[RunnableTime]
				from		[RunnableInstancesTable] as RunnableInstances with (readpast)
				where		[WorkflowHostType] = @workflowHostType
				and			[SurrogateIdentityId] = @surrogateIdentityId
				order by	[WorkflowHostType], [RunnableTime]	
			 end
			else
			 begin
				select top 1 @nextRunnableTime = RunnableInstances.[RunnableTime]
						  from [RunnableInstancesTable] as RunnableInstances with (readpast)
							inner loop join (
								select [SurrogateIdentityId] as IdentityId
								from [IdentityOwnerTable] 
								where [SurrogateLockOwnerId] = @surrogateLockOwnerId
							) as FilteredIdentities
							on RunnableInstances.[SurrogateIdentityId] = FilteredIdentities.IdentityId 
						  where [WorkflowHostType] = @workflowHostType
						  order by [WorkflowHostType], [RunnableTime]	
			 end
		end
	
	
	select 0 as 'Result', @nextRunnableTime, getutcdate()
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[ExtendLock]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[ExtendLock]
	@surrogateLockOwnerId bigint,
	@lockTimeout int	
as
begin
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	begin transaction	
	
	declare @now datetime
	declare @newLockExpiration datetime
	declare @result int
	
	set @now = getutcdate()
	set @result = 0
	
	if (@lockTimeout = 0)
		set @newLockExpiration = '9999-12-31T23:59:59'
	else
		set @newLockExpiration = dateadd(second, @lockTimeout, @now)
	
	update [LockOwnersTable]
	set [LockExpiration] = @newLockExpiration
	where ([SurrogateLockOwnerId] = @surrogateLockOwnerId) and
		  ([LockExpiration] > @now)
	
	if (@@rowcount = 0) 
	begin
		if exists (select * from [LockOwnersTable] where ([SurrogateLockOwnerId] = @surrogateLockOwnerId))
		begin
			exec [System.Activities.DurableInstancing].[DeleteLockOwner] @surrogateLockOwnerId
			set @result = 11
		end
		else
			set @result = 12
	end
	
	select @result as 'Result'
	commit transaction
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[FreeKeys]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[FreeKeys]
	@surrogateInstanceId bigint,
	@keysToFree xml = null
as
begin	
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	declare @badKeyId uniqueidentifier
	declare @numberOfKeys int
	declare @result int
	declare @keyIds table([KeyId] uniqueidentifier)
	
	set @result = 0
	
	if (@keysToFree is not null)
	begin
		insert into @keyIds
		select T.Item.value('@KeyId', 'uniqueidentifier')
		from @keysToFree.nodes('//CorrelationKey') as T(Item)
		option(maxdop 1)
		
		select @numberOfKeys = count(1) from @keyIds
		
		delete [KeysTable]
		from @keyIds as [KeyIds]
		join [KeysTable] on [KeyIds].[KeyId] = [KeysTable].[Id]
		where [SurrogateInstanceId] = @surrogateInstanceId

		if (@@rowcount != @numberOfKeys)
		begin
			select top 1 @badKeyId = [MissingKeys].[MissingKeyId] from
				(select [KeyIds].[KeyId] as [MissingKeyId]
				 from @keyIds as [KeyIds]
				 except
				 select [Id] from [KeysTable] where [SurrogateInstanceId] = @surrogateInstanceId) as MissingKeys
		
			select 4 as 'Result', @badKeyId
			return 4
		end
	end
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[GetActivatableWorkflowsActivationParameters]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[GetActivatableWorkflowsActivationParameters]
	@machineName nvarchar(128)
as
begin
	set nocount on
	set transaction isolation level read committed	
	set xact_abort on;	
	set deadlock_priority low
	
	declare @now datetime
	set @now = getutcdate()
	
	select 0 as 'Result'
	
	select top(1000) serviceDeployments.[SiteName], serviceDeployments.[RelativeApplicationPath], serviceDeployments.[RelativeServicePath]
	from (
		select distinct [ServiceDeploymentId], [WorkflowHostType]
		from [RunnableInstancesTable] with (readpast)
		where [RunnableTime] <= @now
		) runnableWorkflows inner join [ServiceDeploymentsTable] serviceDeployments
		on runnableWorkflows.[ServiceDeploymentId] = serviceDeployments.[Id]
	where not exists (
						select top (1) 1
						from [LockOwnersTable] lockOwners
						where lockOwners.[LockExpiration] > @now
						and lockOwners.[MachineName] = @machineName
						and lockOwners.[WorkflowHostType] = runnableWorkflows.[WorkflowHostType]
					  )
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[GetWorkflowInstanceStoreVersion]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[GetWorkflowInstanceStoreVersion]
as
begin
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;	
		
	select		Major
				,Minor
				,Build
				,Revision
	from		[System.Activities.DurableInstancing].[SqlWorkflowInstanceStoreVersionTable]
	
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[InsertDefinitionIdentity]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[InsertDefinitionIdentity]	
	@identityMetadata xml = null
as
begin
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;
	
	if (@identityMetadata is not null)
	 begin
		insert into [System.Activities.DurableInstancing].[DefinitionIdentityTable]
		(
				[DefinitionIdentityHash],
				[DefinitionIdentityAnyRevisionHash],
				[Name],
				[Package],
				[Build],
				[Major],
				[Minor],
				[Revision]
		)
		select 	T.Item.value('DefinitionIdentityHash[1]', 'uniqueidentifier'),
				T.Item.value('DefinitionIdentityAnyRevisionHash[1]', 'uniqueidentifier'),
				T.Item.value('Name[1]', 'nvarchar(max)'),
				T.Item.value('Package[1]', 'nvarchar(max)'),
				T.Item.value('Build[1]', 'bigint'),
				T.Item.value('Major[1]', 'bigint'),
				T.Item.value('Minor[1]', 'bigint'),
				T.Item.value('Revision[1]', 'bigint')
		from	@identityMetadata.nodes('/IdentityMetadata/IdentityCollection/Identity') as T(Item)
	 end	
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[InsertPromotedProperties]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[InsertPromotedProperties]
	@instanceId uniqueidentifier,
	@promotionName nvarchar(400),
	@value1 sql_variant = null,
	@value2 sql_variant = null,
	@value3 sql_variant = null,
	@value4 sql_variant = null,
	@value5 sql_variant = null,
	@value6 sql_variant = null,
	@value7 sql_variant = null,
	@value8 sql_variant = null,
	@value9 sql_variant = null,
	@value10 sql_variant = null,
	@value11 sql_variant = null,
	@value12 sql_variant = null,
	@value13 sql_variant = null,
	@value14 sql_variant = null,
	@value15 sql_variant = null,
	@value16 sql_variant = null,
	@value17 sql_variant = null,
	@value18 sql_variant = null,
	@value19 sql_variant = null,
	@value20 sql_variant = null,
	@value21 sql_variant = null,
	@value22 sql_variant = null,
	@value23 sql_variant = null,
	@value24 sql_variant = null,
	@value25 sql_variant = null,
	@value26 sql_variant = null,
	@value27 sql_variant = null,
	@value28 sql_variant = null,
	@value29 sql_variant = null,
	@value30 sql_variant = null,
	@value31 sql_variant = null,
	@value32 sql_variant = null,
	@value33 varbinary(max) = null,
	@value34 varbinary(max) = null,
	@value35 varbinary(max) = null,
	@value36 varbinary(max) = null,
	@value37 varbinary(max) = null,
	@value38 varbinary(max) = null,
	@value39 varbinary(max) = null,
	@value40 varbinary(max) = null,
	@value41 varbinary(max) = null,
	@value42 varbinary(max) = null,
	@value43 varbinary(max) = null,
	@value44 varbinary(max) = null,
	@value45 varbinary(max) = null,
	@value46 varbinary(max) = null,
	@value47 varbinary(max) = null,
	@value48 varbinary(max) = null,
	@value49 varbinary(max) = null,
	@value50 varbinary(max) = null,
	@value51 varbinary(max) = null,
	@value52 varbinary(max) = null,
	@value53 varbinary(max) = null,
	@value54 varbinary(max) = null,
	@value55 varbinary(max) = null,
	@value56 varbinary(max) = null,
	@value57 varbinary(max) = null,
	@value58 varbinary(max) = null,
	@value59 varbinary(max) = null,
	@value60 varbinary(max) = null,
	@value61 varbinary(max) = null,
	@value62 varbinary(max) = null,
	@value63 varbinary(max) = null,
	@value64 varbinary(max) = null
as
begin
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;	

	declare @surrogateInstanceId bigint

	select @surrogateInstanceId = [SurrogateInstanceId]
	from [InstancesTable]
	where [Id] = @instanceId

	insert into [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]
	values (@surrogateInstanceId, @promotionName, @value1, @value2, @value3, @value4, @value5, @value6, @value7, @value8,
			@value9, @value10, @value11, @value12, @value13, @value14, @value15, @value16, @value17, @value18, @value19,
			@value20, @value21, @value22, @value23, @value24, @value25, @value26, @value27, @value28, @value29, @value30,
			@value31, @value32, @value33, @value34, @value35, @value36, @value37, @value38, @value39, @value40, @value41,
			@value42, @value43, @value44, @value45, @value46, @value47, @value48, @value49, @value50, @value51, @value52,
			@value53, @value54, @value55, @value56, @value57, @value58, @value59, @value60, @value61, @value62, @value63,
			@value64)
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[InsertRunnableInstanceEntry]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[InsertRunnableInstanceEntry]
	@surrogateInstanceId bigint,
	@workflowHostType uniqueidentifier,
	@serviceDeploymentId bigint, 
	@isSuspended bit,
	@isReadyToRun bit,
	@pendingTimer datetime,
	@surrogateIdentityId bigint
AS
begin    
	set nocount on;
	set transaction isolation level read committed;
	set xact_abort on;	
	
	declare @runnableTime datetime
	
	if (@isSuspended  = 0)
	begin
		if (@isReadyToRun = 1)
		begin
			set @runnableTime = getutcdate()					
		end
		else if (@pendingTimer is not null)
		begin
			set @runnableTime = @pendingTimer
		end
	end
		
	if (@runnableTime is not null and @workflowHostType is not null)
	begin	
		insert into [RunnableInstancesTable]
			([SurrogateInstanceId], [WorkflowHostType], [ServiceDeploymentId], [RunnableTime], [SurrogateIdentityId])
			values( @surrogateInstanceId, @workflowHostType, @serviceDeploymentId, @runnableTime, @surrogateIdentityId)
	end
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[LoadInstance]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[LoadInstance]
	@surrogateLockOwnerId bigint,
	@operationType tinyint,
	@handleInstanceVersion bigint,
	@handleIsBoundToLock bit,
	@keyToLoadBy uniqueidentifier = null,
	@instanceId uniqueidentifier = null,
	@keysToAssociate xml = null,
	@encodingOption tinyint,
	@concatenatedKeyProperties varbinary(max) = null,
	@singleKeyId uniqueidentifier,
	@operationTimeout int
as
begin
	set nocount on
	set transaction isolation level read committed	
	set xact_abort on;		
	set deadlock_priority low
	begin transaction
	
	declare @result int
	declare @lockAcquired bigint
	declare @isInitialized bit
	declare @createKey bit
	declare @createdInstance bit
	declare @keyIsAssociated bit
	declare @loadedByKey bit
	declare @now datetime
	declare @surrogateInstanceId bigint

	set @createdInstance = 0
	set @isInitialized = 0
	set @keyIsAssociated = 0
	set @result = 0
	set @surrogateInstanceId = null
	
	exec @lockAcquired = sp_getapplock @Resource = 'InstanceStoreLock', @LockMode = 'Shared', @LockTimeout = @operationTimeout
	
	if (@lockAcquired < 0)
	begin
		set @result = 13
		select @result as 'Result'
	end
	
	if (@result = 0)
	begin
		set @now = getutcdate()

		if (@operationType = 0) or (@operationType = 2)
		begin
MapKeyToInstanceId:
			set @loadedByKey = 0
			set @createKey = 0
			
			select @surrogateInstanceId = [SurrogateInstanceId],
				   @keyIsAssociated = [IsAssociated]
			from [KeysTable]
			where [Id] = @keyToLoadBy
			
			if (@@rowcount = 0)
			begin
				if (@operationType = 2)
				begin
					set @result = 4
					select @result as 'Result', @keyToLoadBy 
				end
					set @createKey = 1
			end
			else if (@keyIsAssociated = 0)
			begin
				set @result = 8
				select @result as 'Result', @keyToLoadBy
			end
			else
			begin
				select @instanceId = [Id]
				from [InstancesTable]
				where [SurrogateInstanceId] = @surrogateInstanceId

				if (@@rowcount = 0)
					goto MapKeyToInstanceId

				set @loadedByKey = 1
			end
		end
	end

	if (@result = 0)
	begin
LockOrCreateInstance:
		exec [System.Activities.DurableInstancing].[LockInstance] @instanceId, @surrogateLockOwnerId, @handleInstanceVersion, @handleIsBoundToLock, @surrogateInstanceId output, null, @result output
														  
		if (@result = 0 and @surrogateInstanceId = 0)
		begin
			if (@loadedByKey = 1)
				goto MapKeyToInstanceId
			
			if (@operationType > 1)
			begin
				set @result = 1
				select @result as 'Result', @instanceId as 'InstanceId'
			end
			else
			begin				
				exec [System.Activities.DurableInstancing].[CreateInstance] @instanceId, @surrogateLockOwnerId, null, null, @surrogateInstanceId output, @result output
			
				if (@result = 0 and @surrogateInstanceId = 0)
					goto LockOrCreateInstance
				else if (@surrogateInstanceId > 0)
					set @createdInstance = 1
			end
		end
		else if (@result = 0)
		begin
			delete from [RunnableInstancesTable]
			where [SurrogateInstanceId] = @surrogateInstanceId
		end
	end
		
	if (@result = 0)
	begin
		if (@createKey = 1) 
		begin
			select @isInitialized = [IsInitialized]
			from [InstancesTable]
			where [SurrogateInstanceId] = @surrogateInstanceId
			
			if (@isInitialized = 1)
			begin
				set @result = 5
				select @result as 'Result', @instanceId
			end
			else
			begin													
				insert into [KeysTable] ([Id], [SurrogateInstanceId], [IsAssociated])
				values (@keyToLoadBy, @surrogateInstanceId, 1)
				
				if (@@rowcount = 0)
				begin
					if (@createdInstance = 1)
					begin
						delete [InstancesTable]
						where [SurrogateInstanceId] = @surrogateInstanceId
					end
					else
					begin
						update [InstancesTable]
						set [SurrogateLockOwnerId] = null
						where [SurrogateInstanceId] = @surrogateInstanceId
					end
					
					goto MapKeyToInstanceId
				end
			end
		end
		else if (@loadedByKey = 1 and not exists(select [Id] from [KeysTable] where ([Id] = @keyToLoadBy) and ([IsAssociated] = 1)))
		begin
			set @result = 8
			select @result as 'Result', @keyToLoadBy
		end
		
		if (@operationType > 1 and not exists(select [Id] from [InstancesTable] where ([Id] = @instanceId) and ([IsInitialized] = 1)))
		begin
			set @result = 1
			select @result as 'Result', @instanceId as 'InstanceId'
		end
		
		if (@result = 0)
			exec @result = [System.Activities.DurableInstancing].[AssociateKeys] @surrogateInstanceId, @keysToAssociate, @concatenatedKeyProperties, @encodingOption, @singleKeyId
		
		-- Ensure that this key's data will never be overwritten.
		if (@result = 0 and @createKey = 1)
		begin
			update [KeysTable]
			set [EncodingOption] = @encodingOption
			where [Id] = @keyToLoadBy
		end
	end
	
	if (@result != 13)
		exec sp_releaseapplock @Resource = 'InstanceStoreLock'
		
	if (@result = 0)
	begin
		select @result as 'Result',
			   [Id],
			   [SurrogateInstanceId],
			   [PrimitiveDataProperties],
			   [ComplexDataProperties],
			   [MetadataProperties],
			   [DataEncodingOption],
			   [MetadataEncodingOption],
			   [Version],
			   [IsInitialized],
			   @createdInstance
		from [InstancesTable]
		where [SurrogateInstanceId] = @surrogateInstanceId
		
		if (@createdInstance = 0)
		begin
			select @result as 'Result',
				   [EncodingOption],
				   [Change]
			from [InstanceMetadataChangesTable]
			where [SurrogateInstanceId] = @surrogateInstanceId
			order by([ChangeTime])
			
			if (@@rowcount = 0)
			select @result as 'Result', null, null
				
			select @result as 'Result',
				   [Id],
				   [IsAssociated],
				   [EncodingOption],
				   [Properties]
			from [KeysTable] with (index(NCIX_KeysTable_SurrogateInstanceId))
			where ([KeysTable].[SurrogateInstanceId] = @surrogateInstanceId)
			
			if (@@rowcount = 0)
				select @result as 'Result', null, null, null, null
		end

		commit transaction
	end
	else if (@result = 2 or @result = 14)
		commit transaction
	else
		rollback transaction
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[LockInstance]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[LockInstance]
	@instanceId uniqueidentifier,
	@surrogateLockOwnerId bigint,
	@handleInstanceVersion bigint,
	@handleIsBoundToLock bit,
	@surrogateInstanceId bigint output,
	@lockVersion bigint output,
	@result int output
as
begin
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	declare @isCompleted bit
	declare @currentLockOwnerId bigint
	declare @currentVersion bigint

TryLockInstance:
	set @currentLockOwnerId = 0
	set @surrogateInstanceId = 0
	set @result = 0
	
	update [InstancesTable]
	set [SurrogateLockOwnerId] = @surrogateLockOwnerId,
		@lockVersion = [Version] = case when ([InstancesTable].[SurrogateLockOwnerId] is null or 
											  [InstancesTable].[SurrogateLockOwnerId] != @surrogateLockOwnerId)
									then [Version] + 1
									else [Version]
								  end,
		@surrogateInstanceId = [SurrogateInstanceId]
	from [InstancesTable]
	left outer join [LockOwnersTable] on [InstancesTable].[SurrogateLockOwnerId] = [LockOwnersTable].[SurrogateLockOwnerId]
	where ([InstancesTable].[Id] = @instanceId) and
		  ([InstancesTable].[IsCompleted] = 0) and
		  (
		   (@handleIsBoundToLock = 0 and
		    (
		     ([InstancesTable].[SurrogateLockOwnerId] is null) or
		     ([LockOwnersTable].[SurrogateLockOwnerId] is null) or
			  (
		       ([LockOwnersTable].[LockExpiration] < getutcdate()) and
               ([LockOwnersTable].[SurrogateLockOwnerId] != @surrogateLockOwnerId)
			  )
		    )
		   ) or 
		   (
			(@handleIsBoundToLock = 1) and
		    ([LockOwnersTable].[SurrogateLockOwnerId] = @surrogateLockOwnerId) and
		    ([LockOwnersTable].[LockExpiration] > getutcdate()) and
		    ([InstancesTable].[Version] = @handleInstanceVersion)
		   )
		  )
	
	if (@@rowcount = 0)
	begin
		if not exists (select * from [LockOwnersTable] where ([SurrogateLockOwnerId] = @surrogateLockOwnerId) and ([LockExpiration] > getutcdate()))
		begin
			if exists (select * from [LockOwnersTable] where [SurrogateLockOwnerId] = @surrogateLockOwnerId)
				set @result = 11
			else
				set @result = 12
			
			select @result as 'Result'
			return 0
		end
		
		select @currentLockOwnerId = [SurrogateLockOwnerId],
			   @isCompleted = [IsCompleted],
			   @currentVersion = [Version]
		from [InstancesTable]
		where [Id] = @instanceId
	
		if (@@rowcount = 1)
		begin
			if (@isCompleted = 1)
				set @result = 7
			else if (@currentLockOwnerId = @surrogateLockOwnerId)
			begin
				if (@handleIsBoundToLock = 1)
					set @result = 10
				else
					set @result = 14
			end
			else if (@handleIsBoundToLock = 0)
				set @result = 2
			else
				set @result = 6
		end
		else if (@handleIsBoundToLock = 1)
			set @result = 6
	end

	if (@result != 0 and @result != 2)
		select @result as 'Result', @instanceId, @currentVersion
	else if (@result = 2)
	begin
		select @result as 'Result', @instanceId, [LockOwnersTable].[Id], [LockOwnersTable].[EncodingOption], [PrimitiveLockOwnerData], [ComplexLockOwnerData]
		from [LockOwnersTable]
		join [InstancesTable] on [InstancesTable].[SurrogateLockOwnerId] = [LockOwnersTable].[SurrogateLockOwnerId]
		where [InstancesTable].[SurrogateLockOwnerId] = @currentLockOwnerId and
			  [InstancesTable].[Id] = @instanceId
		
		if (@@rowcount = 0)
			goto TryLockInstance
	end
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[RecoverInstanceLocks]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[RecoverInstanceLocks]
as
begin
	set nocount on;
	set transaction isolation level read committed;
	set xact_abort on;
	set deadlock_priority low;
    
	declare @now as datetime
	set @now = getutcdate()	
	
	insert into [RunnableInstancesTable] ([SurrogateInstanceId], [WorkflowHostType], [ServiceDeploymentId], [RunnableTime], [SurrogateIdentityId])
		select top (1000) instances.[SurrogateInstanceId], instances.[WorkflowHostType], instances.[ServiceDeploymentId], @now, instances.[SurrogateIdentityId]
		from [LockOwnersTable] lockOwners with (readpast) inner loop join
			 [InstancesTable] instances with (readpast)
				on instances.[SurrogateLockOwnerId] = lockOwners.[SurrogateLockOwnerId]
			where 
				lockOwners.[LockExpiration] <= @now and
				instances.[IsInitialized] = 1 and
				instances.[IsSuspended] = 0
	
	delete from [IdentityOwnerTable] with (readpast)
	where [IdentityOwnerTable].[SurrogateLockOwnerId] in
	(
		select [SurrogateLockOwnerId]
		from [System.Activities.DurableInstancing].[LockOwnersTable] lockOwners
		where [LockExpiration] <= @now
		and not exists
		(
			select top (1) 1
			from [System.Activities.DurableInstancing].[InstancesTable] instances with (nolock)
			where instances.[SurrogateLockOwnerId] = lockOwners.[SurrogateLockOwnerId]
		)
	)

	delete from [LockOwnersTable] with (readpast)
	from [LockOwnersTable] lockOwners
	where [LockExpiration] <= @now
	and not exists
	(
		select top (1) 1
		from [InstancesTable] instances with (nolock)
		where instances.[SurrogateLockOwnerId] = lockOwners.[SurrogateLockOwnerId]
	)
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[SaveInstance]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[SaveInstance]
	@instanceId uniqueidentifier,
	@surrogateLockOwnerId bigint,
	@handleInstanceVersion bigint,
	@handleIsBoundToLock bit,
	@primitiveDataProperties varbinary(max),
	@complexDataProperties varbinary(max),
	@writeOnlyPrimitiveDataProperties varbinary(max),
	@writeOnlyComplexDataProperties varbinary(max),
	@metadataProperties varbinary(max),
	@metadataIsConsistent bit,
	@encodingOption tinyint,
	@timerDurationMilliseconds bigint,
	@suspensionStateChange tinyint,
	@suspensionReason nvarchar(max),
	@suspensionExceptionName nvarchar(450),
	@keysToAssociate xml,
	@keysToComplete xml,
	@keysToFree xml,
	@concatenatedKeyProperties varbinary(max),
	@unlockInstance bit,
	@isReadyToRun bit,
	@isCompleted bit,
	@singleKeyId uniqueidentifier,
	@lastMachineRunOn nvarchar(450),
	@executionStatus nvarchar(450),
	@blockingBookmarks nvarchar(max),
	@workflowHostType uniqueidentifier,
	@serviceDeploymentId bigint,
	@operationTimeout int,
	@identityMetadata xml = null
as
begin
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;	

	declare @currentInstanceVersion bigint
	declare @deleteInstanceOnCompletion bit
	declare @enqueueCommand bit
	declare @isSuspended bit
	declare @lockAcquired bigint
	declare @metadataUpdateOnly bit
	declare @now datetime
	declare @result int
	declare @surrogateInstanceId bigint
	declare @pendingTimer datetime
	declare @definitionIdentityHash uniqueidentifier
	declare @surrogateIdentityId bigint

	set @result = 0
	set @metadataUpdateOnly = 0
	
	exec @lockAcquired = sp_getapplock @Resource = 'InstanceStoreLock', @LockMode = 'Shared', @LockTimeout = @operationTimeout
		
	if (@lockAcquired < 0)
	begin
		select @result as 'Result'
		set @result = 13
	end
	
	set @now = getutcdate()
	
	if (@primitiveDataProperties is null and @complexDataProperties is null and @writeOnlyPrimitiveDataProperties is null and @writeOnlyComplexDataProperties is null)
		set @metadataUpdateOnly = 1

LockOrCreateInstance:
	if (@result = 0)
	begin
		exec [System.Activities.DurableInstancing].[LockInstance] @instanceId, @surrogateLockOwnerId, @handleInstanceVersion, @handleIsBoundToLock, @surrogateInstanceId output, @currentInstanceVersion output, @result output
															  
		if (@result = 0 and @surrogateInstanceId = 0)
		begin
			exec [System.Activities.DurableInstancing].[CreateInstance] @instanceId, @surrogateLockOwnerId, @workflowHostType, @serviceDeploymentId, @surrogateInstanceId output, @result output
			
			if (@result = 0 and @surrogateInstanceId = 0)
				goto LockOrCreateInstance
			
			set @currentInstanceVersion = 1
		end
	end
	
	if (@result = 0)
	begin
		select @enqueueCommand = [EnqueueCommand],
			   @deleteInstanceOnCompletion = [DeletesInstanceOnCompletion]
		from [LockOwnersTable]
		where ([SurrogateLockOwnerId] = @surrogateLockOwnerId)
		
		if (@isCompleted = 1 and @deleteInstanceOnCompletion = 1)
		begin
			exec [System.Activities.DurableInstancing].[DeleteInstance] @surrogateInstanceId
			goto Finally
		end
		
		if @identityMetadata is not null
		begin
			select @definitionIdentityHash = T.Item.value('DefinitionIdentityHash[1]', 'uniqueidentifier') 
			from @identityMetadata.nodes('/IdentityMetadata/IdentityCollection/Identity') as T(Item)
			
			if (@definitionIdentityHash is null)
			begin
				set @result = 15
				goto Finally 
			end
			else
			begin
				if not exists (
					select 1
					from [DefinitionIdentityTable]
					where [DefinitionIdentityHash] = @definitionIdentityHash
					)
				begin
					--insert the new identity
					exec [System.Activities.DurableInstancing].[InsertDefinitionIdentity] @identityMetadata
				end

                select @surrogateIdentityId = [SurrogateIdentityId]
                from [DefinitionIdentityTable]
                where ([DefinitionIdentityHash] = @definitionIdentityHash)
			end
		end
		
		update [InstancesTable] 
		set @instanceId = [InstancesTable].[Id],
			@workflowHostType = [WorkflowHostType] = 
					case when (@workflowHostType is null)
						then [WorkflowHostType]
						else @workflowHostType 
					end,
			@serviceDeploymentId = [ServiceDeploymentId] = 
					case when (@serviceDeploymentId is null)
						then [ServiceDeploymentId]
						else @serviceDeploymentId 
					end,
			@pendingTimer = [PendingTimer] = 
					case when (@metadataUpdateOnly = 1)
						then [PendingTimer]
						else [System.Activities.DurableInstancing].[GetExpirationTime](@timerDurationMilliseconds)
					end,
			@isReadyToRun = [IsReadyToRun] = 
					case when (@metadataUpdateOnly = 1)
						then [IsReadyToRun]
						else @isReadyToRun
					end,
			@isSuspended = [IsSuspended] = 
					case when (@suspensionStateChange = 0) then [IsSuspended]
						 when (@suspensionStateChange = 1) then 1
						 else 0
					end,
			[SurrogateLockOwnerId] = case when (@unlockInstance = 1 or @isCompleted = 1)
										then null
										else @surrogateLockOwnerId
									 end,
			[PrimitiveDataProperties] = case when (@metadataUpdateOnly = 1)
										then [PrimitiveDataProperties]
										else @primitiveDataProperties
									   end,
			[ComplexDataProperties] = case when (@metadataUpdateOnly = 1)
										then [ComplexDataProperties]
										else @complexDataProperties
									   end,
			[WriteOnlyPrimitiveDataProperties] = case when (@metadataUpdateOnly = 1)
										then [WriteOnlyPrimitiveDataProperties]
										else @writeOnlyPrimitiveDataProperties
									   end,
			[WriteOnlyComplexDataProperties] = case when (@metadataUpdateOnly = 1)
										then [WriteOnlyComplexDataProperties]
										else @writeOnlyComplexDataProperties
									   end,
			[MetadataProperties] = case
									 when (@metadataIsConsistent = 1) then @metadataProperties
									 else [MetadataProperties]
								   end,
			[SuspensionReason] = case
									when (@suspensionStateChange = 0) then [SuspensionReason]
									when (@suspensionStateChange = 1) then @suspensionReason
									else null
								 end,
			[SuspensionExceptionName] = case
									when (@suspensionStateChange = 0) then [SuspensionExceptionName]
									when (@suspensionStateChange = 1) then @suspensionExceptionName
									else null
								 end,
			[IsCompleted] = @isCompleted,
			[IsInitialized] = case
								when (@metadataUpdateOnly = 0) then 1
								else [IsInitialized]
							  end,
			[DataEncodingOption] = case
									when (@metadataUpdateOnly = 0) then @encodingOption
									else [DataEncodingOption]
								   end,
			[MetadataEncodingOption] = case
									when (@metadataIsConsistent = 1) then @encodingOption
									else [MetadataEncodingOption]
								   end,
			[BlockingBookmarks] = case
									when (@metadataUpdateOnly = 0) then @blockingBookmarks
									else [BlockingBookmarks]
								  end,
			[LastUpdated] = @now,
			[LastMachineRunOn] = case
									when (@metadataUpdateOnly = 0) then @lastMachineRunOn
									else [LastMachineRunOn]
								 end,
			[ExecutionStatus] = case
									when (@metadataUpdateOnly = 0) then @executionStatus
									else [ExecutionStatus]
								end,
			@surrogateIdentityId = [SurrogateIdentityId] = 
								case
									when (@identityMetadata is null) then [SurrogateIdentityId]
									else @surrogateIdentityId
								end
		from [InstancesTable]		
		where ([InstancesTable].[SurrogateInstanceId] = @surrogateInstanceId)
	
		if (@@rowcount = 0)
		begin
			set @result = 99
			select @result as 'Result' 
		end
		else
		begin
			if (@keysToAssociate is not null or @singleKeyId is not null)
				exec @result = [System.Activities.DurableInstancing].[AssociateKeys] @surrogateInstanceId, @keysToAssociate, @concatenatedKeyProperties, @encodingOption, @singleKeyId
			
			if (@result = 0 and @keysToComplete is not null)
				exec @result = [System.Activities.DurableInstancing].[CompleteKeys] @surrogateInstanceId, @keysToComplete
			
			if (@result = 0 and @keysToFree is not null)
				exec @result = [System.Activities.DurableInstancing].[FreeKeys] @surrogateInstanceId, @keysToFree
			
			if (@result = 0) and (@metadataUpdateOnly = 0)
			begin
				delete from [InstancePromotedPropertiesTable]
				where [SurrogateInstanceId] = @surrogateInstanceId
			end
			
			if (@result = 0)
			begin
				if (@metadataIsConsistent = 1)
				begin
					delete from [InstanceMetadataChangesTable]
					where [SurrogateInstanceId] = @surrogateInstanceId
				end
				else if (@metadataProperties is not null)
				begin
					insert into [InstanceMetadataChangesTable] ([SurrogateInstanceId], [EncodingOption], [Change])
					values (@surrogateInstanceId, @encodingOption, @metadataProperties)
				end
			end
			
			if (@result = 0 and @unlockInstance = 1 and @isCompleted = 0)
				exec [System.Activities.DurableInstancing].[InsertRunnableInstanceEntry] @surrogateInstanceId, @workflowHostType, @serviceDeploymentId, @isSuspended, @isReadyToRun, @pendingTimer, @surrogateIdentityId	
		end
	end

Finally:
	if (@result != 13)
		exec sp_releaseapplock @Resource = 'InstanceStoreLock'
	
	if (@result = 0)
		select @result as 'Result', @currentInstanceVersion

	return @result
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[TryLoadRunnableInstance]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[TryLoadRunnableInstance]
	@surrogateLockOwnerId bigint,
	@workflowHostType uniqueidentifier,
	@operationType tinyint,
	@handleInstanceVersion bigint,
	@handleIsBoundToLock bit,
	@encodingOption tinyint,	
	@operationTimeout int
as
begin
	set nocount on
	set transaction isolation level read committed	
	set xact_abort on;	
	set deadlock_priority -6
	begin tran
	
	declare @instanceId uniqueIdentifier
	declare @now datetime
	set @now = getutcdate()
	
	declare @workflowIdentityFilter tinyint
	
	select @workflowIdentityFilter = [WorkflowIdentityFilter]
	from [LockOwnersTable]
	where [LockOwnersTable].SurrogateLockOwnerId = @surrogateLockOwnerId
	
	if (@workflowIdentityFilter = null)
		set @workflowIdentityFilter = 0
		
	if (@workflowIdentityFilter = 1)
	begin
		-- Load any Runnable instances of specified WorkflowHostType
		select top (1) @instanceId = instances.[Id]
		from [RunnableInstancesTable] runnableInstances with (readpast, updlock)
			inner loop join [InstancesTable] instances with (readpast, updlock)
			on runnableInstances.[SurrogateInstanceId] = instances.[SurrogateInstanceId]
		where runnableInstances.[WorkflowHostType] = @workflowHostType
			  and 
			  runnableInstances.[RunnableTime] <= @now	
	end
	else if (@workflowIdentityFilter = 2)
	begin
		declare @AnyRevisionFilter table
		(
			DefinitionIdentityAnyRevisionHash uniqueidentifier
			UNIQUE (DefinitionIdentityAnyRevisionHash)
		)

		insert into @AnyRevisionFilter 
		select distinct [DefinitionIdentityAnyRevisionHash]
		from [DefinitionIdentityTable] as IdentityTable, [IdentityOwnerTable] as IdentityOwnerTable
		where IdentityOwnerTable.[SurrogateLockOwnerId] = @surrogateLockOwnerId 
			and 
			IdentityTable.[SurrogateIdentityId] = IdentityOwnerTable.[SurrogateIdentityId]
	
		-- Load Runnables instances of specified WorkflowHostType and ignore revision field on supported identities. 
		select top (1) @instanceId = instances.[Id]
		from [RunnableInstancesTable] runnableInstances with (readpast, updlock)
			inner loop join [InstancesTable] instances with (readpast, updlock)
			on runnableInstances.[SurrogateInstanceId] = instances.[SurrogateInstanceId]
			inner loop join (
						select IdentityTable.[SurrogateIdentityId] as IdentityId
						from [System.Activities.DurableInstancing].[DefinitionIdentityTable] as IdentityTable, @AnyRevisionFilter as AnyRevision
						where IdentityTable.[DefinitionIdentityAnyRevisionHash] = AnyRevision.DefinitionIdentityAnyRevisionHash
					) as FilteredIdentities 
			on runnableInstances.SurrogateIdentityId = FilteredIdentities.IdentityId
		where runnableInstances.[WorkflowHostType] = @workflowHostType
			  and 
			  runnableInstances.[RunnableTime] <= @now	  			
	end
	else
	begin
		-- Load Runnable instances of specified WorkflowHostType and the supported identities
		select top (1) @instanceId = instances.[Id]
		from [RunnableInstancesTable] runnableInstances with (readpast, updlock)
			inner loop join [InstancesTable] instances with (readpast, updlock)
			on runnableInstances.[SurrogateInstanceId] = instances.[SurrogateInstanceId]
			inner loop join (
				select [SurrogateIdentityId] as IdentityId 
				from [IdentityOwnerTable] 
				where [SurrogateLockOwnerId] = @surrogateLockOwnerId
			) as FilteredIdentities  
			on runnableInstances.[SurrogateIdentityId] = FilteredIdentities.IdentityId
		where runnableInstances.[WorkflowHostType] = @workflowHostType
			  and 
			  runnableInstances.[RunnableTime] <= @now
	end	       

    if (@@rowcount = 1)
    begin
		select 0 as 'Result', cast(1 as bit)				
		exec [System.Activities.DurableInstancing].[LoadInstance] @surrogateLockOwnerId, @operationType, @handleInstanceVersion, @handleIsBoundToLock, null, @instanceId, null, @encodingOption, null, null, @operationTimeout
    end	
    else
    begin
		select 0 as 'Result', cast(0 as bit)
    end
    
    if (@@trancount > 0)
    begin
		commit tran
    end
end

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[UnlockInstance]    Script Date: 12/19/2016 1:53:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[UnlockInstance]
	@surrogateLockOwnerId bigint,
	@instanceId uniqueidentifier,
	@handleInstanceVersion bigint
as
begin
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;	
	begin transaction
	
	declare @pendingTimer datetime
	declare @surrogateInstanceId bigint
	declare @workflowHostType uniqueidentifier
	declare @serviceDeploymentId bigint
	declare @enqueueCommand bit	
	declare @isReadyToRun bit	
	declare @isSuspended bit
	declare @now datetime
	declare @surrogateIdentityId bigint
	
	set @now = getutcdate()
		
	update [InstancesTable]
	set [SurrogateLockOwnerId] = null,
	    @surrogateInstanceId = [SurrogateInstanceId],
	    @workflowHostType = [WorkflowHostType],
   	    @serviceDeploymentId = [ServiceDeploymentId],
	    @pendingTimer = [PendingTimer],
	    @isReadyToRun =  [IsReadyToRun],
	    @isSuspended = [IsSuspended],
	    @surrogateIdentityId = [SurrogateIdentityId]
	where [Id] = @instanceId and
		  [SurrogateLockOwnerId] = @surrogateLockOwnerId and
		  [Version] = @handleInstanceVersion
	
	exec [System.Activities.DurableInstancing].[InsertRunnableInstanceEntry] @surrogateInstanceId, @workflowHostType, @serviceDeploymentId, @isSuspended, @isReadyToRun, @pendingTimer, @surrogateIdentityId    
	
	commit transaction
end

GO
