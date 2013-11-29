use VESTALIS3
go
alter table [dbo].[VestalisUserApplication]
add ClientId uniqueidentifier null
print 'The field ClientId has been added to the table [dbo].[VestalisUserApplication]'