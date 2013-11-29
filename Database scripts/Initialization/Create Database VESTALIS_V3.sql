
/*==============================================================*/
/* Database name:  VESTALIS_V3                                         */
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     8/7/2012 11:31 PM                         */
/*==============================================================*/

-- *** Default Script Parameter ************************************************************************


:setvar VESTALISV3_Database_DBName VESTALIS3
:setvar VESTALISV3_Database_PrimaryDataName VESTALIS3_data
:setvar VESTALISV3_Database_PrimaryDataLocation "'D:\Program Files\Microsoft SQL Server\MSSQL10.SQL2008\MSSQL\Data\VESTALIS3_data.mdf'"
:setvar VESTALISV3_Database_PrimaryLogName VESTALIS3_log
:setvar VESTALISV3_Database_PrimaryLogLocation "'d:\Program Files\Microsoft SQL Server\MSSQL10.SQL2008\MSSQL\Data\VESTALIS3_log.ldf'"

-- *** Script *****************************************************************************************

/*==============================================================*/
/* Database: VESTALISV3                                               */
/*==============================================================*/
USE MASTER
GO
create database $(VESTALISV3_Database_DBname)
on PRIMARY
	(name = $(VESTALISV3_Database_PrimaryDataName),
		  filename = $(VESTALISV3_Database_PrimaryDataLocation),
          size = 1000MB,
          maxsize = 2GB,
          filegrowth = 15%)
log on
	( name = $(VESTALISV3_Database_PrimaryLogName),
	      filename = $(VESTALISV3_Database_PrimaryLogLocation),
          size = 1500MB,
          maxsize = 2GB,
          filegrowth = 15%)
;
go

USE [master]
IF  EXISTS (SELECT * FROM sys.server_principals WHERE name = N'AppVestalis3')
DROP LOGIN [AppVestalis3]
GO
If not Exists (select loginname from master.dbo.syslogins  where name = 'AppVestalis3' and dbname = 'VESTALIS3') 
Begin 
	EXEC sp_addlogin 'AppVestalis3', 'vestalis@2012','VESTALIS3'	
end
GO
use [$(VESTALISV3_Database_DBname)]
DECLARE @RoleName sysname
set @RoleName = N'db_Vestalis3'
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = @RoleName AND type = 'R')
Begin
	DECLARE @RoleMemberName sysname
	DECLARE Member_Cursor CURSOR FOR
	select [name]
	from sys.database_principals 
	where principal_id in ( 
		select member_principal_id 
		from sys.database_role_members 
		where role_principal_id in (
			select principal_id
			FROM sys.database_principals where [name] = @RoleName  AND type = 'R' ))
		OPEN Member_Cursor;
		FETCH NEXT FROM Member_Cursor
	into @RoleMemberName
		WHILE @@FETCH_STATUS = 0
	BEGIN
	exec sp_droprolemember @rolename=@RoleName, @membername= @RoleMemberName
		FETCH NEXT FROM Member_Cursor
		into @RoleMemberName
	END;

	CLOSE Member_Cursor;
	DEALLOCATE Member_Cursor;
End
GO
	
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'db_Vestalis3' AND type = 'R')
DROP ROLE [db_Vestalis3]
GO
EXEC sp_adduser 'AppVestalis3', 'AppVestalis3'
GO
	
/****** Object:  DatabaseRole [Web]    Script Date: 08/13/2012 08:57:48 ******/
CREATE ROLE [db_Vestalis3] AUTHORIZATION [dbo]
GO
EXEC sp_addrolemember N'db_Vestalis3', N'AppVestalis3'
GO	

