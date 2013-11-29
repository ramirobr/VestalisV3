use VESTALIS3
go

declare @applicationId uniqueidentifier

select @applicationId =  ApplicationId from aspnet_Applications
where ApplicationName = '/'

if not exists(select 1 from aspnet_Roles where RoleName = 'ApplicationAdministrator_BC')
begin
	insert into aspnet_Roles 
	(ApplicationId,RoleId,RoleName,LoweredRoleName,[Description])
	values 
	(@applicationId,NEWID(),'ApplicationAdministrator_BC','applicationadministrator_bc',null)
end

if exists(select 1 from aspnet_Roles where RoleName='ApplicationAdministrator')
begin
	delete from aspnet_UsersInRoles where RoleId in (
		select RoleId from aspnet_Roles where RoleName = 'ApplicationAdministrator')
	delete from aspnet_Roles where RoleName = 'ApplicationAdministrator'
end