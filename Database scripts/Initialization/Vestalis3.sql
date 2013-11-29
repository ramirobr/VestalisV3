USE [VESTALIS3]
GO

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ApprovalItem') and o.name = 'FK_ApprovalItem_InspectionReportItem')
alter table ApprovalItem
   drop constraint FK_ApprovalItem_InspectionReportItem
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BusinessApplication') and o.name = 'FK_BusinessApplication_Country')
alter table BusinessApplication
   drop constraint FK_BusinessApplication_Country
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Catalogue') and o.name = 'FK_Catalogue_BusinessApplication')
alter table Catalogue
   drop constraint FK_Catalogue_BusinessApplication
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CatalogueValue') and o.name = 'FK_CatalogueValue_Catalogue')
alter table CatalogueValue
   drop constraint FK_CatalogueValue_Catalogue
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Document') and o.name = 'FK_Document_ServiceOrder')
alter table Document
   drop constraint FK_Document_ServiceOrder
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FormDefinition') and o.name = 'FK_FormDefinition_BusinessApplication')
alter table FormDefinition
   drop constraint FK_FormDefinition_BusinessApplication
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FormDefinition') and o.name = 'FK_FormDefinition_CatalogueValue')
alter table FormDefinition
   drop constraint FK_FormDefinition_CatalogueValue
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FormValue') and o.name = 'FK_FormValue_CatalogueValue')
alter table FormValue
   drop constraint FK_FormValue_CatalogueValue
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FormValue') and o.name = 'FK_FormValue_InspectionReportItem')
alter table FormValue
   drop constraint FK_FormValue_InspectionReportItem
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FormValue') and o.name = 'FK_FormValueServiceOrder')
alter table FormValue
   drop constraint FK_FormValueServiceOrder
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('InspectionReport') and o.name = 'FK_InspectionReport_ServiceOrder')
alter table InspectionReport
   drop constraint FK_InspectionReport_ServiceOrder
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('InspectionReportItem') and o.name = 'FK_InspectionReportItem_InspectionReport')
alter table InspectionReportItem
   drop constraint FK_InspectionReportItem_InspectionReport
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Picture') and o.name = 'FK_Picture_InspectionReportItem')
alter table Picture
   drop constraint FK_Picture_InspectionReportItem
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Picture') and o.name = 'FK_PictureServiceOrder')
alter table Picture
   drop constraint FK_PictureServiceOrder
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ServiceOrder') and o.name = 'FK_ServiceOrder_BusinessApplication')
alter table ServiceOrder
   drop constraint FK_ServiceOrder_BusinessApplication
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ValidationRole') and o.name = 'FK_Role_BusinessApplication')
alter table ValidationRole
   drop constraint FK_Role_BusinessApplication
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('VestalisUserApplication') and o.name = 'FK_VestalisUserApplication_BusinessApplication')
alter table VestalisUserApplication
   drop constraint FK_VestalisUserApplication_BusinessApplication
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ApprovalItem')
            and   name  = 'IX_ApprovalItem_InspectionReportItemId'
            and   indid > 0
            and   indid < 255)
   drop index ApprovalItem.IX_ApprovalItem_InspectionReportItemId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ApprovalItem')
            and   type = 'U')
   drop table ApprovalItem
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('BusinessApplication')
            and   name  = 'IX_BusinessApplication_CountryId'
            and   indid > 0
            and   indid < 255)
   drop index BusinessApplication.IX_BusinessApplication_CountryId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('BusinessApplication')
            and   type = 'U')
   drop table BusinessApplication
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('Catalogue')
            and   name  = 'IX_Catalogue_BusinessApplicationId'
            and   indid > 0
            and   indid < 255)
   drop index Catalogue.IX_Catalogue_BusinessApplicationId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Catalogue')
            and   type = 'U')
   drop table Catalogue
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('CatalogueValue')
            and   name  = 'IX_CatalogueValue_CatalogueId'
            and   indid > 0
            and   indid < 255)
   drop index CatalogueValue.IX_CatalogueValue_CatalogueId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CatalogueValue')
            and   type = 'U')
   drop table CatalogueValue
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('Country')
            and   name  = 'IX_Country_CountryCode'
            and   indid > 0
            and   indid < 255)
   drop index Country.IX_Country_CountryCode
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Country')
            and   type = 'U')
   drop table Country
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('Document')
            and   name  = 'IX_Document_ServiceOrderId'
            and   indid > 0
            and   indid < 255)
   drop index Document.IX_Document_ServiceOrderId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Document')
            and   type = 'U')
   drop table Document
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('FormDefinition')
            and   name  = 'IX_FormDefinition_FormTypeId'
            and   indid > 0
            and   indid < 255)
   drop index FormDefinition.IX_FormDefinition_FormTypeId
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('FormDefinition')
            and   name  = 'IX_FormDefinition_BusinessApplicationId'
            and   indid > 0
            and   indid < 255)
   drop index FormDefinition.IX_FormDefinition_BusinessApplicationId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('FormDefinition')
            and   type = 'U')
   drop table FormDefinition
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('FormValue')
            and   name  = 'IX_FormValue_ServiceOrderId'
            and   indid > 0
            and   indid < 255)
   drop index FormValue.IX_FormValue_ServiceOrderId
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('FormValue')
            and   name  = 'IX_FormValue_InspectionReportItemId'
            and   indid > 0
            and   indid < 255)
   drop index FormValue.IX_FormValue_InspectionReportItemId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('FormValue')
            and   type = 'U')
   drop table FormValue
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('InspectionReport')
            and   name  = 'IX_InspectionReport_ServiceOrderId'
            and   indid > 0
            and   indid < 255)
   drop index InspectionReport.IX_InspectionReport_ServiceOrderId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('InspectionReport')
            and   type = 'U')
   drop table InspectionReport
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('InspectionReportItem')
            and   name  = 'IX_InspectionReportItem_InspectionReportId'
            and   indid > 0
            and   indid < 255)
   drop index InspectionReportItem.IX_InspectionReportItem_InspectionReportId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('InspectionReportItem')
            and   type = 'U')
   drop table InspectionReportItem
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('Picture')
            and   name  = 'IX_Picture_ServiceOrderId'
            and   indid > 0
            and   indid < 255)
   drop index Picture.IX_Picture_ServiceOrderId
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('Picture')
            and   name  = 'IX_Picture_InspectionReportItemId'
            and   indid > 0
            and   indid < 255)
   drop index Picture.IX_Picture_InspectionReportItemId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Picture')
            and   type = 'U')
   drop table Picture
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ServiceOrder')
            and   name  = 'IX_ServiceOrder_BusinessApplicationId'
            and   indid > 0
            and   indid < 255)
   drop index ServiceOrder.IX_ServiceOrder_BusinessApplicationId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ServiceOrder')
            and   type = 'U')
   drop table ServiceOrder
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ValidationRole')
            and   name  = 'IX_Role_BusinessApplicationId'
            and   indid > 0
            and   indid < 255)
   drop index ValidationRole.IX_Role_BusinessApplicationId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ValidationRole')
            and   type = 'U')
   drop table ValidationRole
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('VestalisUserApplication')
            and   name  = 'IX_VestalisUserApplication_BusinessApplicationId'
            and   indid > 0
            and   indid < 255)
   drop index VestalisUserApplication.IX_VestalisUserApplication_BusinessApplicationId
go

if exists (select 1
            from  sysobjects
           where  id = object_id('VestalisUserApplication')
            and   type = 'U')
   drop table VestalisUserApplication
go

create table ApprovalItem (
   ApprovalItemId       uniqueidentifier     RowGuidCol not null constraint DF_ApprovalItem_ApprovalItemId default newsequentialid(),
   InspectionReportItemId uniqueidentifier     null,
   RoleName             nvarchar(256)        not null,
   ApprovalLevel        int                  not null,
   CanPublish           bit                  null,
   IsReadOnly           bit                  null,
   ApprovalStatus       int                  null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_ApprovalItem_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_ApprovalItem_IsDeleted default (0),
   constraint PK_ApprovalItem primary key nonclustered (ApprovalItemId)
)
go

create clustered index IX_ApprovalItem_InspectionReportItemId on ApprovalItem (
InspectionReportItemId ASC
)
go

create table BusinessApplication (
   BusinessApplicationId uniqueidentifier     RowGuidCol not null constraint DF_BusinessApplication_BusinessApplicationId default newsequentialid(),
   CountryId            uniqueidentifier     null,
   BusinessApplicationName nvarchar(256)        not null,
   XmlFieldDefinition   nvarchar(Max)        null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_BusinessApplication_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_BusinessApplication_IsDeleted default (0),
   Prefix               char(2)              null,
   constraint PK_BusinessApplication primary key nonclustered (BusinessApplicationId)
)
go

create clustered index IX_BusinessApplication_CountryId on BusinessApplication (
CountryId ASC
)
go

create table Catalogue (
   CatalogueId          uniqueidentifier     RowGuidCol not null constraint DF_Catalogue_CatalogueId default newsequentialid(),
   BusinessApplicationId uniqueidentifier     null,
   CatalogueCategoryName nvarchar(256)        not null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_Catalogue_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_Catalogue_IsDeleted default (0),
   constraint PK_Catalogue primary key nonclustered (CatalogueId)
)
go

create clustered index IX_Catalogue_BusinessApplicationId on Catalogue (
BusinessApplicationId ASC
)
go

create table CatalogueValue (
   CatalogueValueId     uniqueidentifier     RowGuidCol not null constraint DF_CatalogueValue_CatalogueValueId default newsequentialid(),
   CatalogueId          uniqueidentifier     not null,
   CatalogueValueData   nvarchar(1000)       not null,
   CatalogueValueDescription nvarchar(4000)       null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_CatalogueValue_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_CatalogueValue_IsDeleted default (0),
   constraint PK_CatalogueValue primary key nonclustered (CatalogueValueId)
)
go

create clustered index IX_CatalogueValue_CatalogueId on CatalogueValue (
CatalogueId ASC
)
go

create table Country (
   CountryId            uniqueidentifier     RowGuidCol not null constraint DF_Country_CountryId default newsequentialid(),
   CountryCode          nvarchar(2)          not null,
   CountryName          nvarchar(256)        null,
   DefaultLanguage      nvarchar(2)          not null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_Country_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_Country_IsDeleted default (0),
   constraint PK_Country primary key nonclustered (CountryId)
)
go

create clustered index IX_Country_CountryCode on Country (
CountryName ASC
)
go

create table Document (
   DocumentId           uniqueidentifier     RowGuidCol not null constraint DF_Document_DocumentId default newsequentialid(),
   ServiceOrderId       uniqueidentifier     null,
   DocumentDescription  nvarchar(1000)       null,
   DocumentFile         varbinary(Max)       null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_Document_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_Document_IsDeleted default (0),
   DocumentName         nvarchar(256)        null,
   constraint PK_Document primary key nonclustered (DocumentId)
)
go

create clustered index IX_Document_ServiceOrderId on Document (
ServiceOrderId ASC
)
go

create table FormDefinition (
   FormDefinitionId     uniqueidentifier     RowGuidCol not null constraint DF_FormDefinition_FormDefinitionId default newsequentialid(),
   BusinessApplicationId uniqueidentifier     not null,
   FormTypeId           uniqueidentifier     not null,
   XmlFormDefinition    nvarchar(Max)        not null,
   IsClientVisible      bit                  not null,
   FormName             nvarchar(100)        null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_FormDefinition_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_FormDefinition_IsDeleted default (0),
   FormOrder            int                  not null constraint DF__FormDefin__FormO__00DF2177 default (0),
   constraint PK_FormDefinition primary key nonclustered (FormDefinitionId)
)
go

create clustered index IX_FormDefinition_BusinessApplicationId on FormDefinition (
BusinessApplicationId ASC
)
go

create index IX_FormDefinition_FormTypeId on FormDefinition (
FormTypeId ASC
)
go

create table FormValue (
   FormValueId          uniqueidentifier     RowGuidCol not null constraint DF_FormValue_FormValueId default newsequentialid(),
   ServiceOrderId       uniqueidentifier     null,
   InspectionReportItemId uniqueidentifier     null,
   FieldName            nvarchar(256)        not null,
   FieldType            int                  not null,
   CatalogueValueId     uniqueidentifier     null,
   TextValue            nvarchar(Max)        collate SQL_Latin1_General_CP1_CI_AS null,
   CheckValue           bit                  null,
   DateValue            datetime             null,
   IntegerValue         int                  null,
   DecimalValue         decimal(18,5)        null,
   UserName             nvarchar(256)        null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_FormValue_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_FormValue_IsDeleted default (0),
   constraint PK_FormValue primary key nonclustered (FormValueId)
)
go

create index IX_FormValue_InspectionReportItemId on FormValue (
InspectionReportItemId ASC
)
go

create clustered index IX_FormValue_ServiceOrderId on FormValue (
ServiceOrderId ASC
)
go

create table InspectionReport (
   InspectionReportId   uniqueidentifier     RowGuidCol not null constraint DF_InspectionReport_InspectionReportId default newsequentialid(),
   ServiceOrderId       uniqueidentifier     not null,
   XmlFormDefinitionInstance nvarchar(Max)        not null,
   IsClientVisible      bit                  not null,
   StatusCode           nvarchar(2)          null,
   FormName             nvarchar(100)        null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_InspectionReport_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_InspectionReport_IsDeleted default (0),
   FormOrder            int                  not null constraint DF__Inspectio__FormO__7FEAFD3E default (0),
   constraint PK_InspectionReport primary key nonclustered (InspectionReportId)
)
go

create clustered index IX_InspectionReport_ServiceOrderId on InspectionReport (
ServiceOrderId ASC
)
go

create table InspectionReportItem (
   InspectionReportItemId uniqueidentifier     RowGuidCol not null constraint DF_InspectionReportItem_InspectionReportItemId default newsequentialid(),
   InspectionReportId   uniqueidentifier     null,
   CurrentCompletedLevel int                  null,
   StatusCode           nvarchar(2)          null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_InspectionReportItem_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_InspectionReportItem_IsDeleted default (0),
   PublicationDate      datetime             null,
   constraint PK_InspectionReportItem primary key nonclustered (InspectionReportItemId)
)
go

create clustered index IX_InspectionReportItem_InspectionReportId on InspectionReportItem (
InspectionReportId ASC
)
go

create table Picture (
   PictureId            uniqueidentifier     RowGuidCol not null constraint DF_Picture_PictureId default newsequentialid(),
   ServiceOrderId       uniqueidentifier     null,
   InspectionReportItemId uniqueidentifier     null,
   PictureFile          varbinary(Max)       not null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_Picture_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_Picture_IsDeleted default (0),
   constraint PK_Picture primary key nonclustered (PictureId)
)
go

create index IX_Picture_InspectionReportItemId on Picture (
InspectionReportItemId ASC
)
go

create clustered index IX_Picture_ServiceOrderId on Picture (
ServiceOrderId ASC
)
go

create table ServiceOrder (
   ServiceOrderId       uniqueidentifier     RowGuidCol not null constraint DF_ServiceOrder_ServiceOrderId default newsequentialid(),
   BusinessApplicationId uniqueidentifier     not null,
   StatusCode           nvarchar(2)          null,
   XmlFormDefinitionInstance nvarchar(Max)        not null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_ServiceOrder_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_ServiceOrder_IsDeleted default (0),
   constraint PK_ServiceOrder primary key nonclustered (ServiceOrderId)
)
go

create clustered index IX_ServiceOrder_BusinessApplicationId on ServiceOrder (
BusinessApplicationId ASC
)
go

create table ValidationRole (
   ValidationRoleId               uniqueidentifier     RowGuidCol not null constraint DF_Role_RoleId default newsequentialid(),
   RoleName             nvarchar(256)        not null,
   BusinessApplicationId uniqueidentifier     null,
   CanPublish           bit                  null,
   IsReadOnly           bit                  null,
   RoleLevel            int                  not null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_Role_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_Role_IsDeleted default (0),
   constraint PK_ValidationRole primary key nonclustered (ValidationRoleId)
)
go

create clustered index IX_Role_BusinessApplicationId on ValidationRole (
BusinessApplicationId ASC
)
go

create table VestalisUserApplication (
   VestalisUserApplicationId uniqueidentifier     RowGuidCol not null constraint DF_VestalisUserApplication_VestalisUserApplicationId default newsequentialid(),
   BusinessApplicationId uniqueidentifier     null,
   UserName             nvarchar(256)        not null,
   CreationBy           nvarchar(30)         not null,
   CreationDate         datetime             not null constraint DF_VestalisUserApplication_CreationDate default getdate(),
   ModificationBy       nvarchar(30)         null,
   ModificationDate     datetime             null,
   IsDeleted            bit                  not null constraint DF_VestalisUserApplication_IsDeleted default (0),
   constraint PK_VestalisUserApplication primary key nonclustered (VestalisUserApplicationId)
)
go

create clustered index IX_VestalisUserApplication_BusinessApplicationId on VestalisUserApplication (
BusinessApplicationId ASC
)
go

alter table ApprovalItem
   add constraint FK_ApprovalItem_InspectionReportItem foreign key (InspectionReportItemId)
      references InspectionReportItem (InspectionReportItemId)
         
         not for replication
go

alter table BusinessApplication
   add constraint FK_BusinessApplication_Country foreign key (CountryId)
      references Country (CountryId)
         
         not for replication
go

alter table Catalogue
   add constraint FK_Catalogue_BusinessApplication foreign key (BusinessApplicationId)
      references BusinessApplication (BusinessApplicationId)
         
         not for replication
go

alter table CatalogueValue
   add constraint FK_CatalogueValue_Catalogue foreign key (CatalogueId)
      references Catalogue (CatalogueId)
         
         not for replication
go

alter table Document
   add constraint FK_Document_ServiceOrder foreign key (ServiceOrderId)
      references ServiceOrder (ServiceOrderId)
         
         not for replication
go

alter table FormDefinition
   add constraint FK_FormDefinition_BusinessApplication foreign key (BusinessApplicationId)
      references BusinessApplication (BusinessApplicationId)
         
         not for replication
go

alter table FormDefinition
   add constraint FK_FormDefinition_CatalogueValue foreign key (FormTypeId)
      references CatalogueValue (CatalogueValueId)
         
         not for replication
go

alter table FormValue
   add constraint FK_FormValue_CatalogueValue foreign key (CatalogueValueId)
      references CatalogueValue (CatalogueValueId)
         
         not for replication
go

alter table FormValue
   add constraint FK_FormValue_InspectionReportItem foreign key (InspectionReportItemId)
      references InspectionReportItem (InspectionReportItemId)
         
         not for replication
go

alter table FormValue
   add constraint FK_FormValueServiceOrder foreign key (ServiceOrderId)
      references ServiceOrder (ServiceOrderId)
         
         not for replication
go

alter table InspectionReport
   add constraint FK_InspectionReport_ServiceOrder foreign key (ServiceOrderId)
      references ServiceOrder (ServiceOrderId)
         
         not for replication
go

alter table InspectionReportItem
   add constraint FK_InspectionReportItem_InspectionReport foreign key (InspectionReportId)
      references InspectionReport (InspectionReportId)
         
         not for replication
go

alter table Picture
   add constraint FK_Picture_InspectionReportItem foreign key (InspectionReportItemId)
      references InspectionReportItem (InspectionReportItemId)
         
         not for replication
go

alter table Picture
   add constraint FK_PictureServiceOrder foreign key (ServiceOrderId)
      references ServiceOrder (ServiceOrderId)
         
         not for replication
go

alter table ServiceOrder
   add constraint FK_ServiceOrder_BusinessApplication foreign key (BusinessApplicationId)
      references BusinessApplication (BusinessApplicationId)
         
         not for replication
go

alter table ValidationRole
   add constraint FK_Role_BusinessApplication foreign key (BusinessApplicationId)
      references BusinessApplication (BusinessApplicationId)
         
         not for replication
go

alter table VestalisUserApplication
   add constraint FK_VestalisUserApplication_BusinessApplication foreign key (BusinessApplicationId)
      references BusinessApplication (BusinessApplicationId)
         
         not for replication
go

