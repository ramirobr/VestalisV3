USE VESTALIS3
GO
exec dbo.aspnet_Roles_CreateRole @ApplicationName=N'/',@RoleName=N'GlobalAdministrator'
GO
exec dbo.aspnet_Roles_CreateRole @ApplicationName=N'/',@RoleName=N'Coordinator_BC'
GO
exec dbo.aspnet_Roles_CreateRole @ApplicationName=N'/',@RoleName=N'ApplicationAdministrator'
GO
exec dbo.aspnet_Roles_CreateRole @ApplicationName=N'/',@RoleName=N'Client'
GO
declare @p12 uniqueidentifier
set @p12='94677EC6-1A08-4FD8-AD9E-E722FF13316F'
declare @dateutc datetime

set @dateutc= getutcdate()

exec dbo.aspnet_Membership_CreateUser @ApplicationName=N'/',@UserName=N'adminGlobal@cotecna.ch',@Password=N'DJf3A5SDpad+ki33xBBaOyxowB4=',@PasswordSalt=N'qaquJEfs6uye2ku6Jt6IHA==',@Email=NULL,@PasswordQuestion=NULL,@PasswordAnswer=NULL,@IsApproved=1,@UniqueEmail=0,@PasswordFormat=1,@CurrentTimeUtc=@dateutc,@UserId=@p12 output
select @p12

exec dbo.aspnet_UsersInRoles_AddUsersToRoles @ApplicationName=N'/',@RoleNames=N'GlobalAdministrator',@UserNames=N'adminGlobal@cotecna.ch',@CurrentTimeUtc=@dateutc

exec dbo.aspnet_Profile_SetProperties @ApplicationName=N'/',@UserName=N'adminGlobal@cotecna.ch',@PropertyNames=N'UserType:S:0:1:UserFullName:S:1:12:',@PropertyValuesString=N'1Admin Global',@PropertyValuesBinary=0x,@IsUserAnonymous=0,@CurrentTimeUtc=@dateutc

