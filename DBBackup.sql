DECLARE @BackupFileName varchar(Max)

SELECT @BackupFileName = 'D:\Database\backups\velocity_' + replace(replace(CONVERT(NVARCHAR(30),getdate(), 100),':','_'),' ','_') + '.bak'
print @BackupFileName

BACKUP DATABASE velocity TO  DISK = @BackupFileName WITH NOFORMAT, INIT,  NAME = N'velocity', SKIP, REWIND, NOUNLOAD,  STATS = 10
GO



USE [msdb]
GO

/****** Object:  Job [VelocityBackup]    Script Date: 2/22/2016 2:48:23 PM ******/
EXEC msdb.dbo.sp_delete_job @job_id=N'e468b8e7-67d5-49cd-957e-caae6538769a', @delete_unused_schedule=1
GO

/****** Object:  Job [VelocityBackup]    Script Date: 2/22/2016 2:48:23 PM ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]    Script Date: 2/22/2016 2:48:23 PM ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'VelocityBackup', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'No description available.', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'sa', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [DBbackup]    Script Date: 2/22/2016 2:48:24 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'DBbackup', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'DECLARE @BackupFileName varchar(Max)

SELECT @BackupFileName = ''D:\Database\backups\velocity_'' + replace(replace(CONVERT(NVARCHAR(30),getdate(), 100),'':'',''_''),'' '',''_'') + ''.bak''
print @BackupFileName

BACKUP DATABASE velocity TO  DISK = @BackupFileName WITH NOFORMAT, INIT,  NAME = N''velocity'', SKIP, REWIND, NOUNLOAD,  STATS = 10
GO
', 
		@database_name=N'velocity', 
		@database_user_name=N'dbo', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'velocitybackupschedule', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20160112, 
		@active_end_date=99991231, 
		@active_start_time=0, 
		@active_end_time=235959, 
		@schedule_uid=N'277890af-6ba1-4c31-8e05-60b3cc1ae79d'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:

GO

