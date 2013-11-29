USE VESTALIS3
GO
------rollback pictures ----------
IF (EXISTS (SELECT COLUMN_NAME
				FROM INFORMATION_SCHEMA.COLUMNS
				WHERE TABLE_NAME = 'Picture' AND COLUMN_NAME = 'PictureFileThumbnail'))
BEGIN

	ALTER TABLE dbo.Picture DROP COLUMN PictureFileThumbnail 

END
--------rollback new roles--------
declare @applicationId uniqueidentifier

select @applicationId =  ApplicationId from aspnet_Applications
where ApplicationName = '/'

if not exists(select 1 from aspnet_Roles where RoleName = 'ApplicationAdministrator')
begin
	insert into aspnet_Roles 
	(ApplicationId,RoleId,RoleName,LoweredRoleName,[Description])
	values 
	(@applicationId,NEWID(),'ApplicationAdministrator','applicationadministrator',null)
end

if exists(select 1 from aspnet_Roles where RoleName='ApplicationAdministrator_BC')
begin
	delete from aspnet_UsersInRoles where RoleId in (select RoleId from aspnet_Roles
	where RoleName = 'ApplicationAdministrator_BC')
	delete from aspnet_Roles where RoleName = 'ApplicationAdministrator_BC'
end

declare @businessApplicationId uniqueidentifier

set @businessApplicationId = 'BFE0C1B1-B4B7-4342-BEE7-ED6CC097B6F5'

--Update Fields

update dbo.BusinessApplication set XmlFieldDefinition= '<?xml version="1.0" encoding="utf-8" ?>
<Fields>
  <CatalogueField FieldName="Client" Mandatory="true" CatalogueName="Client" EditorType="DropDownList" Caption="Client"/>
  <SingleTextLineField FieldName="BookingNumber" Mandatory="true" Caption="Booking" MaxLength="20" />
  <SingleTextLineField FieldName="Vessel" Caption="Vessel:" MaxLength="256" />
  <SingleTextLineField FieldName="OrderNumber" Mandatory="true" MaxLength="20" Caption="Order number"  />
  <DatepickerField FieldName="OrderDate" Mandatory="true" StartDate="01/01/2012" EndDate="today" Caption="Service order date" StartDateFilter="today-30" EndDateFilter="today" />
  <SingleTextLineField FieldName="ClientContact" Caption="Client contact" MaxLength="256"/>
  <CatalogueField FieldName="Product" CatalogueName="Product" Mandatory="true" EditorType="DropDownList" Caption="Product"  />
  <IntegerField FieldName="Quantity" MinValue="0" MaxValue="2147483647" Caption="Quantity" CaptionGrid="Quantity &lt;br&gt; appointed" Mandatory="true" />"
  <IntegerField FieldName="Tonnage" MinValue="0" MaxValue="2147483647" Caption="Tonnage" />
  <DatepickerField FieldName="InspectionDatePlanned" Caption="Inspection date planned" />
  <DatepickerField FieldName="InspectionDeadline" Caption="Inspection deadline" />
  <CatalogueField FieldName="Agent" CatalogueName="Agent" EditorType="DropDownList" Caption="Agent/Broker" />
  <CatalogueField FieldName="InspectionPort" CatalogueName="InspectionPort" EditorType="DropDownList" Caption="InspectionPort" />
  <SingleTextLineField FieldName="Destination" Caption="Destination" MaxLength="256" />
  <SingleTextLineField FieldName="Shipper" Caption="Shipper" MaxLength="256"/>
  <CatalogueField FieldName="DoubleCheck" CatalogueName="DoubleCheck" EditorType="DropDownList" Caption="Double Check" />
  <SingleTextLineField FieldName="Service" Caption="Service" MaxLength="256" />
  <MultipleTextLineField FieldName="JobDescription" Caption="Job description" MaxLength="2000" />
  <MultipleTextLineField FieldName="Specifications" Caption="Specifications" MaxLength="2000" />
  <SingleTextLineField FieldName="LaboratoryName" Caption="Laboratory" MaxLength="256" />
  <CatalogueField FieldName="AnalysisColorUI" CatalogueName="LaboratoryAnalysis" EditorType="DropDownList" Caption="Color UI" />
  <CatalogueField FieldName="AnalysisPolarization" CatalogueName="LaboratoryAnalysis" EditorType="DropDownList" Caption="Polarization ºZ" />
  <CatalogueField FieldName="AnalysisMoisture" CatalogueName="LaboratoryAnalysis" EditorType="DropDownList" Caption="Moisture %" />
  <CatalogueField FieldName="AnalysisAsh" CatalogueName="LaboratoryAnalysis" EditorType="DropDownList" Caption="Ash %" />
  <BooleanField FieldName="RequestedEmpty" Caption="Requested" CaptionGrid="Empty container &lt;br&gt; requested" />
  <SingleTextLineField FieldName="InspectionPlaceEmpty" Caption="Inspection place" CaptionGrid="Empty container &lt;br&gt; inspection place" MaxLength="256" />
  <DatepickerField FieldName="InspectionDateEmpty" Caption="Inspection date"/>
  <TimeField FieldName="TimeInspectionDateEmpty" Caption="Inspection time"/>
  <SingleTextLineField FieldName="ContactInspectionPlaceEmpty" Caption="Contact at inspection place" MaxLength="256" />
  <IntegerField FieldName="QuantityInspectedEmpty" Caption="Quantity inspected" CaptionGrid="Empty container &lt;br&gt; quantity inspected"/>
  <IntegerField FieldName="QuantityRemainingEmpty" Caption="Quantity remaining" CaptionGrid="Empty container &lt;br&gt; quantity remaining"/>
  <SingleTextLineField FieldName="InspectionPlaceStuffing" Caption="Stuffing place" MaxLength="256"/>
  <DatepickerField FieldName="InspectionDateStuffing" Caption="Inspection date" />
  <TimeField FieldName="TimeInspectionDateStuffing" Caption="Inspection time" />
  <SingleTextLineField FieldName="ContactInspectionPlaceStuffing" Caption="Contact at stuffing place" MaxLength="256" />
  <IntegerField FieldName="QuantityInspectedStuffing" Caption="Quantity inspected" CaptionGrid="Stuffing inspection &lt;br&gt; quantity inspected" />
  <IntegerField FieldName="QuantityRemainingStuffing" Caption="Quantity remaining" CaptionGrid="Stuffing inspection &lt;br&gt; quantity remaining"/>
  <DatepickerField FieldName="InspectionDate" Mandatory="true" Caption="Date" StartDateFilter="today-30" EndDateFilter="today"/>
  <TimeField FieldName="StartTime" Caption="Start time" />
  <TimeField FieldName="TLStartTime" Caption="Start time" Mandatory="true"/>
  <TimeField FieldName="EndTime" Caption="End time" />
  <SingleTextLineField FieldName="ContainerNumber" Mandatory="true" Caption="Container Number" MaxLength="20"/>
  <IntegerField FieldName="MaxGross" Caption="Max Gross" MinValue="0" MaxValue="2147483647" />
  <IntegerField FieldName="Tare"  Caption="Tare" MinValue="0" MaxValue="2147483647" />
  <IntegerField FieldName="MaxCargo"  Caption="Max Cargo" MinValue="0" MaxValue="2147483647" />
  <CatalogueField FieldName="Size" CatalogueName="Size" EditorType="DropDownList" Caption="Size"/>
  <CatalogueField FieldName="ContainerType" CatalogueName="ContainerType" EditorType="DropDownList" Caption="Container Type"/>
  <CatalogueField FieldName="Manufacture" CatalogueName="Manufacture" EditorType="DropDownList" Caption="Manufacture"/>
  <SingleTextLineField FieldName="Seals"  Caption="Seals" MaxLength="128"/>
  <MultipleTextLineField FieldName="Remark"  Caption="Remark" MaxLength="2000" />
  <MultipleTextLineField FieldName="LRRemark"  Caption="Remark" MaxLength="1000" />
  <MultipleTextLineField FieldName="TLRemark"  Caption="Remark" MaxLength="2000" Mandatory="true" />
  <BooleanField FieldName="Rejected" Caption="Rejected" />
  <CatalogueField FieldName="Mill" CatalogueName="Mill" EditorType="DropDownList" Caption="Mill"/>
  <CatalogueField FieldName="BMRMill" CatalogueName="Mill" EditorType="DropDownList" Caption="Mill" Mandatory="true"/>
  <CatalogueField FieldName="MarkingType" CatalogueName="MarkingType" EditorType="DropDownList" Caption="Marking type"/>
  <MultipleTextLineField FieldName="MarkingText" Caption="Marking text" MaxLength="4000"  Mandatory="true" />
  <SingleTextLineField FieldName="BagColor"  Caption="Bag color" MaxLength="64" />
  <SingleTextLineField FieldName="BagTextOfTheBag"  Caption="Back of the bag" MaxLength="128" />
  <SingleTextLineField FieldName="BillNumber"  Caption="Invoice" MaxLength="20" />
  <SingleTextLineField FieldName="Plate"  Caption="Plate" MaxLength="20" />
  <IntegerField FieldName="QuantityOfBagsBill"  Caption="Quantity of bags (Bill)" MinValue="0" MaxValue="2147483647" />
  <IntegerField FieldName="QuantityOfBags"  Caption="Quantity of bags" MinValue="0" MaxValue="2147483647" />
  <SingleTextLineField FieldName="Seal"  Caption="Cotecna Seals" MaxLength="128" />
  <SingleTextLineField FieldName="OwnersSeal"  Caption="Agent seals" MaxLength="128" />
  <SingleTextLineField FieldName="OtherSeal"  Caption="Other seals" MaxLength="128" />
  <IntegerField FieldName="TotalWeightOfBags"  Caption="Total weight of bags" MinValue="0" MaxValue="2147483647" Mandatory="true" />
  <IntegerField FieldName="TotalOfBags"  Caption="Total of bags" MinValue="0" MaxValue="2147483647" />
  <IntegerField FieldName="GrossWeightAverage"  Caption="Gross weight average" MinValue="0" MaxValue="2147483647" />
  <IntegerField FieldName="LessTareAverage"  Caption="Less tare average" MinValue="0" MaxValue="2147483647" />
  <IntegerField FieldName="NetWeightAverage"  Caption="Net weight average" MinValue="0" MaxValue="2147483647" />
  <DatepickerField FieldName="LaboratoryDate" Caption="Date" Mandatory="true" StartDateFilter="today-30" EndDateFilter="today" />
  <SingleTextLineField FieldName="LaboratoryReference"  Caption="Laboratory reference" MaxLength="64" Mandatory="true"/>
  <CatalogueField FieldName="Parameter" CatalogueName="Parameter" EditorType="DropDownList" Caption="Parameter" Mandatory="true"/>
  <CatalogueField FieldName="Unit" CatalogueName="Unit" EditorType="DropDownList" Caption="Unit" Mandatory="true"/>
  <SingleTextLineField FieldName="Result"  Caption="Result" MaxLength="64" Mandatory="true" />
  <CatalogueField FieldName="Method" CatalogueName="LaboratoryAnalysis" EditorType="DropDownList" Caption="Method" Mandatory="true" />
</Fields>'
where BusinessApplicationId = @businessApplicationId

--Update Service Order 

update dbo.FormDefinition set XmlFormDefinition = '<?xml version="1.0" encoding="utf-8" ?>
<Form >
  <Sections>
    <Section Caption="General">
      <FormElements>
        <Element Identifier="Client" IsDataGridVisible="true" IsFilterVisible ="true" IsVisibleClient="false" />
        <Element Identifier="BookingNumber" IsDataGridVisible="true" IsFilterVisible ="true" IsInspectionReportVisible = "true"/>
        <Element Identifier="Vessel" />
        <Element Identifier="OrderNumber" IsDataGridVisible="true" IsFilterVisible ="true" IsInspectionReportVisible = "true"/>
        <Element Identifier="OrderDate" IsDataGridVisible="true" IsFilterVisible ="true"/>
        <Element Identifier="ClientContact" />
        <Element Identifier="Product"  />
        <Element Identifier="Quantity" IsDataGridVisible="true"/>
        <Element Identifier="Tonnage" />
        <Element Identifier="InspectionDatePlanned" />
        <Element Identifier="InspectionDeadline" />
        <Element Identifier="Agent" />
        <Element Identifier="InspectionPort" />
        <Element Identifier="Destination" />
        <Element Identifier="Shipper" />
        <Element Identifier="DoubleCheck" />
        <Element Identifier="Service" />
        <Element Identifier="JobDescription" />
        <Element Identifier="Specifications" />
        <Element Identifier="LaboratoryName" />
        <Element Identifier="Remark" />
      </FormElements>
    </Section>
    <Section Caption="Analysis">
      <FormElements>
        <Element Identifier="AnalysisColorUI"/>
        <Element Identifier="AnalysisPolarization"/>
        <Element Identifier="AnalysisMoisture" />
        <Element Identifier="AnalysisAsh" />
      </FormElements>
    </Section>
    <Section Caption="Empty container inspection">
      <FormElements>
        <Element Identifier="RequestedEmpty" IsDataGridVisible="true"/>
        <Element Identifier="InspectionPlaceEmpty" IsDataGridVisible="true" />
        <Element Identifier="InspectionDateEmpty"/>
        <Element Identifier="TimeInspectionDateEmpty" />
        <Element Identifier="ContactInspectionPlaceEmpty" />
        <Element Identifier="QuantityInspectedEmpty" IsDataGridVisible="true" />
        <Element Identifier="QuantityRemainingEmpty" />
      </FormElements>
    </Section>
    <Section Caption="Stuffing inspection">
      <FormElements>
        <Element Identifier="InspectionPlaceStuffing" IsDataGridVisible="true" />
        <Element Identifier="InspectionDateStuffing"/>
        <Element Identifier="TimeInspectionDateStuffing" />
        <Element Identifier="ContactInspectionPlaceStuffing"/>
        <Element Identifier="QuantityInspectedStuffing" IsDataGridVisible="true"/>
        <Element Identifier="QuantityRemainingStuffing" />
      </FormElements>
    </Section>
  </Sections>
</Form>' 
where BusinessApplicationId = @businessApplicationId and FormName ='Service Order'


--Empty Container Inspection

update dbo.FormDefinition
set XmlFormDefinition = '<?xml version="1.0" encoding="utf-8" ?>
<Form>
  <Sections>
    <Section Caption="Empty container inspection">
      <FormElements>
        <Element Identifier="InspectionDate" IsDataGridVisible="true" IsFilterVisible ="true"/>
        <Element Identifier="StartTime" IsDataGridVisible="true"/>
        <Element Identifier="EndTime" IsDataGridVisible="true"/>
        <Element Identifier="ContainerNumber" IsDataGridVisible="true" IsFilterVisible ="true"/>
        <Element Identifier="MaxGross"/>
        <Element Identifier="Tare"/>
        <Element Identifier="MaxCargo"/>
        <Element Identifier="Size"/>
        <Element Identifier="ContainerType"/>
        <Element Identifier="Manufacture"/>
        <Element Identifier="Seals"/>
        <Element Identifier="Remark" IsDataGridVisible="true"/>
        <Element Identifier="Rejected"/>
      </FormElements>
    </Section>
  </Sections>
</Form>'
where FormName = 'Empty Container Inspection' and BusinessApplicationId = @businessApplicationId

--Stuffing Inspection

update dbo.FormDefinition
set XmlFormDefinition = '<?xml version="1.0" encoding="utf-8" ?>
<Form>
  <Sections>
    <Section Caption="Stuffing inspection">
      <FormElements>
        <Element Identifier="InspectionDate" IsDataGridVisible="true" IsFilterVisible ="true"/>
        <Element Identifier="BillNumber" IsDataGridVisible="true"/>
        <Element Identifier="Plate" IsDataGridVisible="true"/>
        <Element Identifier="QuantityOfBagsBill" IsDataGridVisible="true"/>
        <Element Identifier="ContainerNumber" IsDataGridVisible="true" IsFilterVisible ="true"/>
        <Element Identifier="StartTime" IsDataGridVisible="true"/>
        <Element Identifier="EndTime" IsDataGridVisible="true"/>
        <Element Identifier="Tare" IsDataGridVisible="true"/>
        <Element Identifier="QuantityOfBags" IsDataGridVisible="true"/>
        <Element Identifier="Seal" IsDataGridVisible="true"/>
        <Element Identifier="OwnersSeal" IsDataGridVisible="true"/>
        <Element Identifier="OtherSeal" IsDataGridVisible="true"/>
        <Element Identifier="Mill" IsDataGridVisible="true"/>
        <Element Identifier="Remark" IsDataGridVisible="true"/>
      </FormElements>
    </Section>
  </Sections>
</Form>'
where FormName = 'Stuffing Inspection' and BusinessApplicationId = @businessApplicationId

--Bag Marking

update dbo.FormDefinition
set XmlFormDefinition = '<?xml version="1.0" encoding="utf-8" ?>
<Form>
  <Sections>
    <Section Caption="Bag Marking report">
      <FormElements>
        <Element Identifier="BMRMill" IsDataGridVisible="true" Mandatoy="true"/>
        <Element Identifier="MarkingType" />
        <Element Identifier="MarkingText" IsDataGridVisible="true"  Mandatoy="true" />
        <Element Identifier="BagColor" IsDataGridVisible="true"/>
        <Element Identifier="BagTextOfTheBag" IsDataGridVisible="true"/>
      </FormElements>
    </Section>  
  </Sections>
  <Rules>
    <DefaultValueDependentOnCatalogue CatalogueField="MarkingType" TextField="MarkingText" />
  </Rules>
</Form>'
where FormName = 'Bag Marking' and BusinessApplicationId = @businessApplicationId

--Bag Weighing

update dbo.FormDefinition
set XmlFormDefinition = '<?xml version="1.0" encoding="utf-8" ?>
<Form>
  <Sections>
    <Section Caption="Bag weighing report">
      <FormElements >
        <Element Identifier="Mill" IsDataGridVisible="true" IsFilterVisible ="true"/>
        <Element Identifier="InspectionDate" IsDataGridVisible="true" IsFilterVisible ="true"/>
        <Element Identifier="StartTime" IsDataGridVisible="false"/>
        <Element Identifier="EndTime" IsDataGridVisible="false"/>
        <Element Identifier="TotalWeightOfBags" IsDataGridVisible="true"/>
        <Element Identifier="TotalOfBags" IsDataGridVisible="true"/>
        <Element Identifier="GrossWeightAverage" IsDataGridVisible="true"/>
        <Element Identifier="LessTareAverage" IsDataGridVisible="true"/>
        <Element Identifier="NetWeightAverage" IsDataGridVisible="true"/>
      </FormElements>
    </Section>
  </Sections>
</Form>'
where FormName = 'Bag Weighing' and BusinessApplicationId = @businessApplicationId

--Laboratory Analysis

update dbo.FormDefinition
set XmlFormDefinition = '<?xml version="1.0" encoding="utf-8" ?>
<Form>
  <Sections>
    <Section  Caption="Laboratory order result">
      <FormElements>
        <Element Identifier="LaboratoryDate" IsDataGridVisible="true" Mandatory="true" IsFilterVisible ="true"/>
        <Element Identifier="LaboratoryReference" IsDataGridVisible="true" Mandatory="true"/>
        <Element Identifier="Parameter" IsDataGridVisible="true" Mandatory="true"/>
        <Element Identifier="Unit" IsDataGridVisible="true" Mandatory="true"/>
        <Element Identifier="Result" IsDataGridVisible="true" Mandatory="true"/>
        <Element Identifier="Method" IsDataGridVisible="true" Mandatory="true"/>
        <Element Identifier="LRRemark" IsDataGridVisible="true" />
      </FormElements>
    </Section>
  </Sections>
</Form>'
where FormName = 'Laboratory Analysis' and BusinessApplicationId = @businessApplicationId

--Time Log

update dbo.FormDefinition
set XmlFormDefinition = '<?xml version="1.0" encoding="utf-8" ?>
<Form>
  <Sections>
    <Section Caption="Time log report">
      <FormElements>
        <Element Identifier="InspectionDate" IsDataGridVisible="true" IsFilterVisible ="true"/>
        <Element Identifier="TLStartTime" IsDataGridVisible="true"/>
        <Element Identifier="EndTime" IsDataGridVisible="true"/>
        <Element Identifier="TLRemark" IsDataGridVisible="true"/>
      </FormElements>
    </Section>
  </Sections>
</Form>'
where FormName = 'Time Log' and BusinessApplicationId = @businessApplicationId


--Delete current users
delete from VestalisUserApplication

delete from aspnet_Membership

delete from aspnet_Profile

delete from aspnet_UsersInRoles

delete from aspnet_Users

--Create a global admin
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

exec dbo.aspnet_Membership_CreateUser @ApplicationName=N'/',@UserName=N'adminGlobal',@Password=N'DJf3A5SDpad+ki33xBBaOyxowB4=',@PasswordSalt=N'qaquJEfs6uye2ku6Jt6IHA==',@Email=NULL,@PasswordQuestion=NULL,@PasswordAnswer=NULL,@IsApproved=1,@UniqueEmail=0,@PasswordFormat=1,@CurrentTimeUtc=@dateutc,@UserId=@p12 output
select @p12

exec dbo.aspnet_UsersInRoles_AddUsersToRoles @ApplicationName=N'/',@RoleNames=N'GlobalAdministrator',@UserNames=N'adminGlobal',@CurrentTimeUtc=@dateutc

exec dbo.aspnet_Profile_SetProperties @ApplicationName=N'/',@UserName=N'adminGlobal',@PropertyNames=N'UserType:S:0:1:UserFullName:S:1:12:',@PropertyValuesString=N'1Admin Global',@PropertyValuesBinary=0x,@IsUserAnonymous=0,@CurrentTimeUtc=@dateutc


--------------------------------------------------------------------------------

--Remove ClientId field in VestalisUserApplication table
IF (EXISTS (SELECT COLUMN_NAME
				FROM INFORMATION_SCHEMA.COLUMNS
				WHERE TABLE_NAME = 'VestalisUserApplication' AND COLUMN_NAME = 'ClientId'))
BEGIN

alter table [dbo].[VestalisUserApplication]
drop column ClientId
print 'The field ClientId has been removed to the table [dbo].[VestalisUserApplication]'

END 