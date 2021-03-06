/*
   19 December 20163:09:30 PM
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
ALTER TABLE dbo.OrgLocations SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.OrgLocations', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.OrgLocations', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.OrgLocations', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.EvaluationCycles ADD CONSTRAINT
	FK_EvaluationCycles_Organizations FOREIGN KEY
	(
	OrgId
	) REFERENCES dbo.OrgLocations
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.EvaluationCycles ADD CONSTRAINT
	FK_EvaluationCycles_OrgLocations FOREIGN KEY
	(
	OrgId
	) REFERENCES dbo.OrgLocations
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.EvaluationCycles SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.EvaluationCycles', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.EvaluationCycles', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.EvaluationCycles', 'Object', 'CONTROL') as Contr_Per 