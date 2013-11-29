use [VESTALIS3]
go
delete from dbo.ApprovalItem
go
delete from dbo.FormValue
go
delete from dbo.InspectionReportItem
go
delete from dbo.InspectionReport
go
delete from dbo.ValidationRole
go
delete from [dbo].[FormDefinition]
go
delete from dbo.CatalogueValue
go
delete from dbo.Catalogue
go
delete from dbo.VestalisUserApplication
go
delete from dbo.Picture
go
delete from dbo.Document
go
delete from dbo.ServiceOrder
go
delete from [dbo].[BusinessApplication]
go
delete from [dbo].[Country]

declare @creationBy nvarchar(30)
declare @guidId uniqueidentifier
declare @businessApplicationId uniqueidentifier
declare @catalogueValueId uniqueidentifier
declare @countryId uniqueidentifier
declare @catalogueId uniqueidentifier

select @creationBy = 'adminGlobal'

set @businessApplicationId = 'BFE0C1B1-B4B7-4342-BEE7-ED6CC097B6F5'


---Insert into Country

if(select count(*) from [dbo].[Country] where CountryCode='BR')=0
begin
	insert into [dbo].[Country] (CountryCode,CountryName,DefaultLanguage,CreationBy)
	values
	('BR','Brazil','en',@creationBy)
end

---Insert into [BusinessApplication]

if(select count(*) from [dbo].[BusinessApplication] where BusinessApplicationName='Brazil container inspection')=0
begin

	select @countryId = countryId from Country where CountryCode='BR'

	insert into BusinessApplication (BusinessApplicationId,CountryId, BusinessApplicationName,XmlFieldDefinition,CreationBy,Prefix)
	values (@businessApplicationId,@countryId,'Brazil container inspection','<?xml version="1.0" encoding="utf-8" ?>
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
</Fields>',@creationBy,'BC')
print 'INSERT INTO BUSINESSAPPLICATION'
end

--Insert catalogue values for the Catalogue -> FormType

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'FormType')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'FormType', @creationBy,@businessApplicationId)
end

set @catalogueId = null
select @catalogueId= CatalogueId from dbo.Catalogue where CatalogueCategoryName= 'FormType'

if(select count(*) from dbo.CatalogueValue where CatalogueValueData= 'ServiceOrder')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId, CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, 'ServiceOrder', @creationBy)
end

if(select count(*) from dbo.CatalogueValue where CatalogueValueData= 'InspectionReport')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId, CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, 'InspectionReport', @creationBy)
end

--INSERT CATALOGUE PRODUCT

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'Product')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'Product', @creationBy,@businessApplicationId)
end
set @catalogueId = null
select @catalogueId= CatalogueId from dbo.Catalogue where CatalogueCategoryName= 'Product'

--Insert catalogue values for the Catalogue -> Product

if(select count(*) from dbo.CatalogueValue where CatalogueValueData= 'Sugar')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId, CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, 'Sugar', @creationBy)
end


--Insert catalogue: Double Check

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'DoubleCheck')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'DoubleCheck', @creationBy,@businessApplicationId)
end
set @catalogueId = null
select @catalogueId= CatalogueId from dbo.Catalogue where CatalogueCategoryName= 'DoubleCheck'

--Insert catalogue values for the Catalogue -> DoubleCheck

if(select count(*) from dbo.CatalogueValue where CatalogueValueData= 'Yes')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId, CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, 'Yes', @creationBy)
end


if(select count(*) from dbo.CatalogueValue where CatalogueValueData= 'No')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId,CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, 'No', @creationBy)
end

--Insert catalogue: LaboratoryAnalysis

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'LaboratoryAnalysis')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'LaboratoryAnalysis', @creationBy,@businessApplicationId)
end

set @catalogueId = null
select @catalogueId= CatalogueId from dbo.Catalogue where CatalogueCategoryName= 'LaboratoryAnalysis'

--Insert catalogue values for the Catalogue -> LaboratoryAnalysis

if(select count(*) from dbo.CatalogueValue where CatalogueValueData= 'ICUMSA GS9/1/2/3-8')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId, CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, 'ICUMSA GS9/1/2/3-8', @creationBy)
end


if(select count(*) from dbo.CatalogueValue where CatalogueValueData= 'ICUMSGS1/2/3/9-1')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId,CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, 'ICUMSGS1/2/3/9-1', @creationBy)
end


if(select count(*) from dbo.CatalogueValue where CatalogueValueData= 'ICUMSA GS2/1/3/9-15')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId,CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, 'ICUMSA GS2/1/3/9-15', @creationBy)
end


if(select count(*) from dbo.CatalogueValue where CatalogueValueData= 'ICUMSA GS1/3/4/8-13')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId,CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, 'ICUMSA GS1/3/4/8-13', @creationBy)
end
--Insert catalogue: Client

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'Client')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'Client', @creationBy,@businessApplicationId)
end


--Insert catalogue: Inspection Port

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'InspectionPort')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'InspectionPort', @creationBy,@businessApplicationId)
end

--Insert catalogue: Shipper

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'Shipper')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'Shipper', @creationBy,@businessApplicationId)
end

--Insert catalogue: Mill

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'Mill')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'Mill', @creationBy,@businessApplicationId)
end

--Insert catalogue: Size

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'Size')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'Size', @creationBy,@businessApplicationId)
end

set @catalogueId = null
select @catalogueId= CatalogueId from dbo.Catalogue where CatalogueCategoryName= 'Size'

if(select count(*) from dbo.CatalogueValue where CatalogueValueData= '20')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId, CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, '20', @creationBy)
end

if(select count(*) from dbo.CatalogueValue where CatalogueValueData= '40')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId, CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, '40', @creationBy)
end

--Insert catalogue: Container Type

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'ContainerType')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'ContainerType', @creationBy,@businessApplicationId)
end

set @catalogueId = null
select @catalogueId= CatalogueId from dbo.Catalogue where CatalogueCategoryName= 'ContainerType'

if(select count(*) from dbo.CatalogueValue where CatalogueValueData= 'Dry')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId, CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, 'Dry', @creationBy)
end

if(select count(*) from dbo.CatalogueValue where CatalogueValueData= 'HC')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId, CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, 'HC', @creationBy)
end

if(select count(*) from dbo.CatalogueValue where CatalogueValueData= 'Reeffer')=0
begin
set @catalogueValueId = NEWID()
insert into [dbo].[CatalogueValue] (CatalogueValueId, CatalogueId, [CatalogueValueData],CreationBy) values
(@catalogueValueId,@catalogueId, 'Reeffer', @creationBy)
end


--Insert catalogue: Manufacture

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'Manufacture')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'Manufacture', @creationBy,@businessApplicationId)
end

--Insert catalogue: Marking type

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'MarkingType')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'MarkingType', @creationBy,@businessApplicationId)
end

--Insert catalogue: Parameter

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'Parameter')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'Parameter', @creationBy,@businessApplicationId)
end

--Insert catalogue: Unit

if(select count(*) from dbo.Catalogue where CatalogueCategoryName= 'Unit')=0
begin
SET @catalogueId = NEWID()
INSERT INTO dbo.Catalogue(CatalogueId,CatalogueCategoryName, CreationBy,BusinessApplicationId) values (@catalogueId,'Unit', @creationBy,@businessApplicationId)
end


set @guidId = NEWID()

--INSERT FORM DEFINITION FOR SERVICE ORDER
select @catalogueValueId = CatalogueValueId from CatalogueValue where CatalogueValueData='ServiceOrder'
if(select count(*) from FormDefinition where FormTypeId= @catalogueValueId)=0
begin
insert into [dbo].[FormDefinition] (FormDefinitionId,BusinessApplicationId,FormTypeId,XmlFormDefinition,IsClientVisible,CreationBy,FormName,FormOrder)
values
(@guidId,@businessApplicationId,@catalogueValueId,
'<?xml version="1.0" encoding="utf-8" ?>
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
</Form>',1,@creationBy,'Service Order',1
)

print 'INSERT INTO FORMDEFINITION (SERVICE ORDER)'
end

set @guidId = NEWID()
--INSERT FORM DEFINITION FOR Empty container inspection
select @catalogueValueId = CatalogueValueId from CatalogueValue where CatalogueValueData='InspectionReport'
if(select count(*) from FormDefinition where FormTypeId= @catalogueValueId and FormName = 'Empty container inspection')=0
begin
insert into [dbo].[FormDefinition] (FormDefinitionId,BusinessApplicationId,FormTypeId,XmlFormDefinition,IsClientVisible,CreationBy,FormName,FormOrder)
values
(@guidId,@businessApplicationId,@catalogueValueId,
'<?xml version="1.0" encoding="utf-8" ?>
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
</Form>',0,@creationBy,'Empty container inspection',1
)
print 'INSERT INTO FORMDEFINITION (EMPTY CONTAINER)'
end

set @guidId = NEWID()
--INSERT FORM DEFINITION FOR STUFFING INSPECTION
select @catalogueValueId = CatalogueValueId from CatalogueValue where CatalogueValueData='InspectionReport'
if(select count(*) from FormDefinition where FormTypeId= @catalogueValueId and FormName = 'Stuffing inspection')=0
begin
insert into [dbo].[FormDefinition] (FormDefinitionId,BusinessApplicationId,FormTypeId,XmlFormDefinition,IsClientVisible,CreationBy,FormName,FormOrder)
values
(@guidId,@businessApplicationId,@catalogueValueId,
'<?xml version="1.0" encoding="utf-8" ?>
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
</Form>',1,@creationBy,'Stuffing inspection',2
)
print 'INSERT INTO FORMDEFINITION (STUFFING INSPECTION)'
end

set @guidId = NEWID()
--INSERT FORM DEFINITION FOR BAG MARKING
select @catalogueValueId = CatalogueValueId from CatalogueValue where CatalogueValueData='InspectionReport'
if(select count(*) from FormDefinition where FormTypeId= @catalogueValueId and FormName = 'Bag Marking')=0
begin
insert into [dbo].[FormDefinition] (FormDefinitionId,BusinessApplicationId,FormTypeId,XmlFormDefinition,IsClientVisible,CreationBy,FormName,FormOrder)
values
(@guidId,@businessApplicationId,@catalogueValueId,
'<?xml version="1.0" encoding="utf-8" ?>
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
</Form>
',1,@creationBy,'Bag Marking',3
)
print 'INSERT INTO FORMDEFINITION (BAG MARKING)'
end

set @guidId = NEWID()
--INSERT FORM DEFINITION FOR BAG WEIGHING
select @catalogueValueId = CatalogueValueId from CatalogueValue where CatalogueValueData='InspectionReport'
if(select count(*) from FormDefinition where FormTypeId= @catalogueValueId and FormName = 'Bag Weighing')=0
begin
insert into [dbo].[FormDefinition] (FormDefinitionId,BusinessApplicationId,FormTypeId,XmlFormDefinition,IsClientVisible,CreationBy,FormName,FormOrder)
values
(@guidId,@businessApplicationId,@catalogueValueId,
'<?xml version="1.0" encoding="utf-8" ?>
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
</Form>',1,@creationBy,'Bag Weighing',4
)
print 'INSERT INTO FORMDEFINITION (BAG WEIGHING)'
end

set @guidId = NEWID()
--INSERT FORM DEFINITION FOR LABORATORY REPORT
select @catalogueValueId = CatalogueValueId from CatalogueValue where CatalogueValueData='InspectionReport'
if(select count(*) from FormDefinition where FormTypeId= @catalogueValueId and FormName = 'Laboratory Report')=0
begin
insert into [dbo].[FormDefinition] (FormDefinitionId,BusinessApplicationId,FormTypeId,XmlFormDefinition,IsClientVisible,CreationBy,FormName,FormOrder)
values
(@guidId,@businessApplicationId,@catalogueValueId,
'<?xml version="1.0" encoding="utf-8" ?>
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
</Form>',1,@creationBy,'Laboratory Report',5
)
print 'INSERT INTO FORMDEFINITION (LABORATORY REPORT)'
end


set @guidId = NEWID()
--INSERT FORM DEFINITION FOR TIME LOG
select @catalogueValueId = CatalogueValueId from CatalogueValue where CatalogueValueData='InspectionReport'
if(select count(*) from FormDefinition where FormTypeId= @catalogueValueId and FormName = 'Time Log')=0
begin
insert into [dbo].[FormDefinition] (FormDefinitionId,BusinessApplicationId,FormTypeId,XmlFormDefinition,IsClientVisible,CreationBy,FormName,FormOrder)
values
(@guidId,@businessApplicationId,@catalogueValueId,
'<?xml version="1.0" encoding="utf-8" ?>
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
</Form>',1,@creationBy,'Time Log',6
)
print 'INSERT INTO FORMDEFINITION (TIME LOG)'
end

--INSERT INTO VALIDATION_ROLE
if(select count(*) from ValidationRole where RoleName= 'Coordinator_BC' and BusinessApplicationId=@businessApplicationId)=0
begin
set @guidId = NEWID()
insert into [dbo].ValidationRole (ValidationRoleId,RoleName, BusinessApplicationId, CanPublish, IsReadOnly, RoleLevel,CreationBy)
values
(@guidId,'Coordinator_BC',@businessApplicationId,1,0,1,@creationBy)
print 'INSERT INTO ValidationRole'
end
go
use [VESTALIS3]
GO
GRANT DELETE ON [dbo].[ApprovalItem] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[ApprovalItem] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[ApprovalItem] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[ApprovalItem] TO [db_Vestalis3]
GO
GRANT DELETE ON [dbo].[aspnet_Applications] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[aspnet_Applications] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[aspnet_Applications] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[aspnet_Applications] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[aspnet_Membership] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[aspnet_Membership] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[aspnet_Membership] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[aspnet_Membership] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[aspnet_Paths] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[aspnet_Paths] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[aspnet_Paths] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[aspnet_Paths] TO [db_Vestalis3]
GO
GO
GO
GRANT DELETE ON [dbo].[aspnet_PersonalizationAllUsers] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[aspnet_PersonalizationAllUsers] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[aspnet_PersonalizationAllUsers] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[aspnet_PersonalizationAllUsers] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[aspnet_PersonalizationPerUser] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[aspnet_PersonalizationPerUser] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[aspnet_PersonalizationPerUser] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[aspnet_PersonalizationPerUser] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[aspnet_Profile] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[aspnet_Profile] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[aspnet_Profile] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[aspnet_Profile] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[aspnet_Roles] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[aspnet_Roles] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[aspnet_Roles] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[aspnet_Roles] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[aspnet_SchemaVersions] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[aspnet_SchemaVersions] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[aspnet_SchemaVersions] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[aspnet_SchemaVersions] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[aspnet_Users] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[aspnet_Users] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[aspnet_Users] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[aspnet_Users] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[aspnet_UsersInRoles] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[aspnet_UsersInRoles] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[aspnet_UsersInRoles] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[aspnet_UsersInRoles] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[BusinessApplication] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[BusinessApplication] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[BusinessApplication] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[BusinessApplication] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[Catalogue] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[Catalogue] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[Catalogue] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[Catalogue] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[CatalogueValue] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[CatalogueValue] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[CatalogueValue] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[CatalogueValue] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[Country] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[Country] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[Country] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[Country] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[Document] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[Document] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[Document] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[Document] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[FormDefinition] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[FormDefinition] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[FormDefinition] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[FormDefinition] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[FormValue] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[FormValue] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[FormValue] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[FormValue] TO [db_Vestalis3]
GO

GO
GRANT DELETE ON [dbo].[InspectionReport] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[InspectionReport] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[InspectionReport] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[InspectionReport] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[InspectionReportItem] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[InspectionReportItem] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[InspectionReportItem] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[InspectionReportItem] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[Picture] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[Picture] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[Picture] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[Picture] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[ServiceOrder] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[ServiceOrder] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[ServiceOrder] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[ServiceOrder] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[ValidationRole] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[ValidationRole] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[ValidationRole] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[ValidationRole] TO [db_Vestalis3]
GO
GO
GRANT DELETE ON [dbo].[VestalisUserApplication] TO [db_Vestalis3]
GO
GRANT INSERT ON [dbo].[VestalisUserApplication] TO [db_Vestalis3]
GO
GRANT SELECT ON [dbo].[VestalisUserApplication] TO [db_Vestalis3]
GO
GRANT UPDATE ON [dbo].[VestalisUserApplication] TO [db_Vestalis3]
GO

GRANT EXECUTE ON [dbo].[aspnet_AnyDataInTables] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Applications_CreateApplication] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_CheckSchemaVersion] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_ChangePasswordQuestionAndAnswer] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_CreateUser] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_FindUsersByEmail] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_FindUsersByName] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetAllUsers] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetNumberOfUsersOnline] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetPassword] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetPasswordWithFormat] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetUserByEmail] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetUserByName] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetUserByUserId] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_ResetPassword] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_SetPassword] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_UnlockUser] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_UpdateUser] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Membership_UpdateUserInfo] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Paths_CreatePath] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Personalization_GetApplicationId] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_PersonalizationAdministration_DeleteAllState] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_PersonalizationAdministration_FindState] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_PersonalizationAdministration_GetCountOfState] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_PersonalizationAdministration_ResetSharedState] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_PersonalizationAdministration_ResetUserState] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_PersonalizationAllUsers_GetPageSettings] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_PersonalizationAllUsers_ResetPageSettings] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_PersonalizationAllUsers_SetPageSettings] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_PersonalizationPerUser_GetPageSettings] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_PersonalizationPerUser_ResetPageSettings] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_PersonalizationPerUser_SetPageSettings] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Profile_DeleteInactiveProfiles] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Profile_DeleteProfiles] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Profile_GetNumberOfInactiveProfiles] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Profile_GetProfiles] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Profile_GetProperties] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Profile_SetProperties] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_RegisterSchemaVersion] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Roles_CreateRole] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Roles_DeleteRole] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Roles_GetAllRoles] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Roles_RoleExists] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Setup_RemoveAllRoleMembers] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Setup_RestorePermissions] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_UnRegisterSchemaVersion] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Users_CreateUser] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_Users_DeleteUser] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_UsersInRoles_AddUsersToRoles] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_UsersInRoles_FindUsersInRole] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_UsersInRoles_GetRolesForUser] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_UsersInRoles_IsUserInRole] TO [db_Vestalis3]
GO
GRANT EXECUTE ON [dbo].[aspnet_UsersInRoles_RemoveUsersFromRoles] TO [db_Vestalis3]
GO