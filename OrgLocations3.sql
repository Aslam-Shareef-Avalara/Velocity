/*
   19 December 20163:16:12 PM
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
EXECUTE sp_rename N'dbo.OrgLocations.Name', N'Tmp_LocationName', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.OrgLocations.Tmp_LocationName', N'LocationName', 'COLUMN' 
GO
ALTER TABLE dbo.OrgLocations SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.OrgLocations', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.OrgLocations', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.OrgLocations', 'Object', 'CONTROL') as Contr_Per 