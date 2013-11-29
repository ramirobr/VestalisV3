
use CotecnaAuthorization
go


SET NOCOUNT ON;

-- Inserting Application
print '... Inserting Application'
go

INSERT INTO [Common].[ApplicationAuthorization]
           ([ApplicationId]
           ,[Name]
           ,[Description]
          )
SELECT 1,'Monitoring','Monitoring Service'

go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go


-- Inserting Contract
print '... Inserting Contract'
go

INSERT INTO [Common].[Contract]
           ([ContractId]
           ,[Code]
           ,[Name]
           ,[Description]
           ,[IsActive]
           )
SELECT 0,'MN','Common','Common',1

go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go



-- Inserting ApplicationContract
print '... Inserting ApplicationContract'
go

INSERT INTO [Common].[ApplicationContract]
           ([ContractId]
           ,[ApplicationId]
           )
SELECT 0,1

go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go


-- Inserting Module
print '... Inserting Module'
go

INSERT INTO [Common].[Module]
           ([ModuleId]
           ,[Name]
           ,[Description]
           ,[IsActive]
			)
SELECT 1,'Monitoring','Monitoring',1

go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go

-- Inserting ApplicationModule
print '... Inserting ApplicationModule'
go

INSERT INTO [Common].[ApplicationModule]
           ([ApplicationId]
           ,[ModuleId]
           )
SELECT 1,1

go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go


-- Inserting Profile
print '... Inserting Profile'
go

INSERT INTO [Common].[Profile]
           ([ProfileId]
           ,[ApplicationId]
           ,[Name]
           ,[Description]
           ,[IsActive]           
           ,[OfficeTypeId])
SELECT 1,1,'Support user','Responsible of review and solve the issues of all applications',1,0 

go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go

-- Inserting ProfileModule
print '... Inserting ProfileModule'
go

INSERT INTO [Common].[ProfileModule]
           ([ProfileId]
           ,[ModuleId]
           )
SELECT 1,1 

go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go

-- Inserting Role
print '... Inserting Role'
go

INSERT INTO [Common].[Role]
           ([RoleId]
           ,[ModuleId]
           ,[Name]
           ,[Description]
           ,[IsActive]
           )
SELECT 1,1,'Support user','Help desk',1          

go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go


-- Inserting RoleProfile
print '... Inserting RoleProfile'
go

INSERT INTO [Common].[RoleProfile]
           ([RoleId]
           ,[ProfileId]
           )
SELECT 1,1 

go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go


-- Inserting User
print '... Inserting User'
go

INSERT INTO [Common].[User]
           ([UserId]
           ,[Code]
           ,[Name]
           ,[Identifier]
           ,[Domain]
           ,[CountryId]
           ,[OfficeOwnerId]
           ,[IsActive]
           ,[LanguageId]
           ,[Email])
SELECT NEWID(),'RB','Ramiro Batallas','ecuiorbatallas','AMERICA',30,10892,1,1,'ramiro.batallas@cotecna.com.ec' UNION
SELECT NEWID(),'MP','Mariela Pancho','ecuiompancho','AMERICA',30,10892,1,1,'mariela.pancho@cotecna.com.ec' UNION
SELECT NEWID(),'YG','Yvan Germini','gvagermini','EUROPE',30,10892,1,1,NULL 

go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go


-- Inserting UserApplication
print '... Inserting UserApplication'
go

INSERT INTO [Common].[UserApplication]
           ([OfficeId]
           ,[ProfileId]
           ,[ContractId]
           ,[ApplicationId]
           ,[UserId]
           ,[IsDefaultContract]
           ,[DefaultModuleId]
           ,[IsActive]
           )
SELECT 10892,1,0,1,[UserId],1,0,1
FROM [Common].[USER]     

go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go

-- Inserting Operation
print '... Inserting Operation'
go
INSERT INTO [Common].[Operation]
           ([OperationId]
           ,[ModuleId]
           ,[Name]
           ,[Description]
           ,[IsActive]
           )

SELECT 49,1,'Monitoring Service allow','Allow to use monitoring service',1 --UNION
       

go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go

-- Inserting RoleOperation
print '... Inserting RoleOperation'
go
INSERT INTO [Common].[RoleOperation]
           ([RoleId]
           ,[OperationId]
           )
SELECT 1,49 


go
print '    Done. '+CAST(@@Rowcount AS VARCHAR)+' records inserted'
go



-- *** END ***