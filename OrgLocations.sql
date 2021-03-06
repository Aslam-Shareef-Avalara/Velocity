/*
   19 December 20162:54:58 PM
   User: sa
   Server: .
   Database: MultiLocationVelocity
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_OrgLocations
	(
	Id int NOT NULL IDENTITY (1, 1),
	OrgId int NOT NULL,
	Address1 nvarchar(200) NULL,
	Address2 nvarchar(200) NULL,
	City nvarchar(50) NULL,
	State nvarchar(50) NULL,
	Country nvarchar(100) NULL,
	Zip nvarchar(50) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_OrgLocations SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_OrgLocations ON
GO
IF EXISTS(SELECT * FROM dbo.OrgLocations)
	 EXEC('INSERT INTO dbo.Tmp_OrgLocations (Id, OrgId, Address1, Address2, City, State, Country, Zip)
		SELECT Id, OrgId, Address1, Address2, City, State, Country, Zip FROM dbo.OrgLocations WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_OrgLocations OFF
GO
DROP TABLE dbo.OrgLocations
GO
EXECUTE sp_rename N'dbo.Tmp_OrgLocations', N'OrgLocations', 'OBJECT' 
GO
ALTER TABLE dbo.OrgLocations ADD CONSTRAINT
	PK_OrgLocations PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.OrgLocations', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.OrgLocations', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.OrgLocations', 'Object', 'CONTROL') as Contr_Per 