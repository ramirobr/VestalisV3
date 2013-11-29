USE VESTALIS3
GO

declare @businessApplicationId uniqueidentifier

set @businessApplicationId = 'BFE0C1B1-B4B7-4342-BEE7-ED6CC097B6F5'

--Update Fields

update dbo.BusinessApplication 
set XmlFieldDefinition= '<?xml version="1.0" encoding="utf-8" ?>
<Fields>
  <CatalogueField FieldName="Client" Mandatory="true" CatalogueName="Client" EditorType="DropDownList" Caption="Client" IsLikeSearch="true" Width="100" ExcelColumnWidth="27"/>
  <SingleTextLineField FieldName="BookingNumber" Mandatory="true" Caption="Booking" MaxLength="20" IsLikeSearch="true" Width="150" ExcelColumnWidth="25"/>
  <SingleTextLineField FieldName="Vessel" Caption="Vessel:" MaxLength="256" />
  <SingleTextLineField FieldName="OrderNumber" Mandatory="true" MaxLength="20" Caption="Order number"  Width="150" ExcelColumnWidth="25"/>
  <DatepickerField FieldName="OrderDate" Mandatory="true" StartDate="01/01/2012" EndDate="today" Caption="Service order date" StartDateFilter="today-30" EndDateFilter="today" Width="100" ExcelColumnWidth="21"/>
  <SingleTextLineField FieldName="ClientContact" Caption="Client contact" MaxLength="256"/>
  <CatalogueField FieldName="Product" CatalogueName="Product" Mandatory="true" EditorType="DropDownList" Caption="Product"  />
  <IntegerField FieldName="Quantity" MinValue="0" MaxValue="2147483647" Caption="Quantity" CaptionGrid="Quantity appointed" Mandatory="true" Width="100" ExcelColumnWidth="19"/>"
  <IntegerField FieldName="Tonnage" MinValue="0" MaxValue="2147483647" Caption="Tonnage" />
  <DatepickerField FieldName="InspectionDatePlanned" Caption="Inspection date planned" />
  <DatepickerField FieldName="InspectionDeadline" Caption="Inspection deadline" />
  <SingleTextLineField FieldName="Agent" Caption="Agent/Broker" />
  <CatalogueField FieldName="InspectionPort" CatalogueName="InspectionPort" EditorType="DropDownList" Caption="Inspection Port" />
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
  <BooleanField FieldName="RequestedEmpty" Caption="Requested" CaptionGrid="Empty container requested" Width="100" ExcelColumnWidth="26"/>
  <SingleTextLineField FieldName="InspectionPlaceEmpty" Caption="Inspection place" CaptionGrid="Empty container inspection place" MaxLength="256" Width="175" ExcelColumnWidth="31"/>
  <DatepickerField FieldName="InspectionDateEmpty" Caption="Inspection date"/>
  <TimeField FieldName="TimeInspectionDateEmpty" Caption="Inspection time"/>
  <SingleTextLineField FieldName="ContactInspectionPlaceEmpty" Caption="Contact at inspection place" MaxLength="256" />
  <IntegerField FieldName="QuantityInspectedEmpty" Caption="Quantity inspected" CaptionGrid="Empty container quantity inspected" Width="125" ExcelColumnWidth="33"/>
  <IntegerField FieldName="QuantityRemainingEmpty" Caption="Quantity remaining" CaptionGrid="Empty container quantity remaining" Width="125"/>
  <SingleTextLineField FieldName="InspectionPlaceStuffing" Caption="Stuffing place" MaxLength="256" Width="175" ExcelColumnWidth="28"/>
  <DatepickerField FieldName="InspectionDateStuffing" Caption="Inspection date" />
  <TimeField FieldName="TimeInspectionDateStuffing" Caption="Inspection time" />
  <SingleTextLineField FieldName="ContactInspectionPlaceStuffing" Caption="Contact at stuffing place" MaxLength="256" />
  <IntegerField FieldName="QuantityInspectedStuffing" Caption="Quantity inspected" CaptionGrid="Stuffing inspection quantity inspected" Width="125" ExcelColumnWidth="36"/>
  <IntegerField FieldName="QuantityRemainingStuffing" Caption="Quantity remaining" CaptionGrid="Stuffing inspection &lt;br&gt; quantity remaining" Width="110"/>
  <DatepickerField FieldName="InspectionDate" Mandatory="true" Caption="Date" StartDateFilter="today-30" EndDateFilter="today" Width="100" ExcelColumnWidth="17"/>
  <TimeField FieldName="StartTime" Caption="Start time" Width="80" ExcelColumnWidth="18"/>
  <TimeField FieldName="TLStartTime" Caption="Start time" Mandatory="true" Width="100" ExcelColumnWidth="18"/>
  <TimeField FieldName="EndTime" Caption="End time" Width="100" ExcelColumnWidth="18"/>
  <SingleTextLineField FieldName="ContainerNumber" Mandatory="true" Caption="Container Number" MaxLength="20" IsLikeSearch="true" Width="170" ExcelColumnWidth="33"/>
  <AutoCompleteTextField FieldName="SIContainerNumber" Mandatory="true" Caption="Container Number" MaxLength="20" IsLikeSearch="true" Width="170" ExcelColumnWidth="33" ItemSourceCondition="select VALUE fv1.TextValue from VestalisEntities.FormValues as fv1 inner join VestalisEntities.InspectionReportItems as inspecItem on fv1.InspectionReportItemId =  inspecItem.InspectionReportItemId inner join VestalisEntities.InspectionReports as inspectReport on inspecItem.InspectionReportId = inspectReport.InspectionReportId where fv1.Isdeleted = false and fv1.FieldName = ''ContainerNumber'' and fv1.ServiceOrderId = GUID ''SERVICEORDERID'' and inspecItem.IsDeleted = false and inspectReport.FormName = ''Empty Container Inspection'' and not exists(select VALUE fv2.FormValueId from VestalisEntities.FormValues as fv2 where fv2.InspectionReportItemId = fv1.InspectionReportItemId and fv2.FieldName = ''Rejected'' and fv2.CheckValue = true and fv2.IsDeleted = false)"/>
  <IntegerField FieldName="MaxGross" Caption="Max Gross" MinValue="0" MaxValue="2147483647" />
  <IntegerField FieldName="Tare"  Caption="Tare" MinValue="0" MaxValue="2147483647" Width="100" ExcelColumnWidth="19"/>
  <IntegerField FieldName="MaxCargo"  Caption="Max Cargo" MinValue="0" MaxValue="2147483647" />
  <CatalogueField FieldName="Size" CatalogueName="Size" EditorType="DropDownList" Caption="Size"/>
  <CatalogueField FieldName="ContainerType" CatalogueName="ContainerType" EditorType="DropDownList" Caption="Container Type"/>
  <CatalogueField FieldName="Manufacture" CatalogueName="Manufacture" EditorType="DropDownList" Caption="Manufacture"/>
  <SingleTextLineField FieldName="Seals"  Caption="Seals" MaxLength="128"/>
  <MultipleTextLineField FieldName="Remark"  Caption="Remark" MaxLength="2000" Width="300" ExcelColumnWidth="71"/>
  <MultipleTextLineField FieldName="LRRemark"  Caption="Remark" MaxLength="1000" Width="300" ExcelColumnWidth="71"/>
  <MultipleTextLineField FieldName="TLRemark"  Caption="Remark" MaxLength="2000" Mandatory="true" Width="363" ExcelColumnWidth="71"/>
  <BooleanField FieldName="Rejected" Caption="Rejected" />
  <CatalogueField FieldName="Mill" CatalogueName="Mill" EditorType="DropDownList" Caption="Mill"  Width="150" ExcelColumnWidth="23"/>
  <CatalogueField FieldName="BMRMill" CatalogueName="Mill" EditorType="DropDownList" Caption="Mill" Mandatory="true" Width="150" ExcelColumnWidth="23"/>
  <CatalogueField FieldName="MarkingType" CatalogueName="MarkingType" EditorType="DropDownList" Caption="Marking type" Width="150"/>
  <MultipleTextLineField FieldName="MarkingText" Caption="Marking text" MaxLength="4000"  Mandatory="true" Width="200" ExcelColumnWidth="53"/>
  <SingleTextLineField FieldName="BagColor"  Caption="Bag color" MaxLength="64" Width="70" ExcelColumnWidth="31"/>
  <SingleTextLineField FieldName="BagTextOfTheBag"  Caption="Back of the bag" MaxLength="128" Width="243" ExcelColumnWidth="41"/>
  <SingleTextLineField FieldName="BillNumber"  Caption="Invoice" MaxLength="20" Width="200" ExcelColumnWidth="25"/>
  <SingleTextLineField FieldName="Plate"  Caption="Plate" MaxLength="20" Width="170" ExcelColumnWidth="21"/>
  <IntegerField FieldName="QuantityOfBagsBill"  Caption="Quantity of bags (Bill)" MinValue="0" MaxValue="2147483647" Width="150" ExcelColumnWidth="26"/>
  <IntegerField FieldName="QuantityOfBags"  Caption="Quantity of bags" MinValue="0" MaxValue="2147483647" Width="150" ExcelColumnWidth="15"/>
  <SingleTextLineField FieldName="Seal"  Caption="Cotecna Seals" MaxLength="128" Width="200" ExcelColumnWidth="36"/>
  <SingleTextLineField FieldName="OwnersSeal"  Caption="Agent seals" MaxLength="128" Width="200" ExcelColumnWidth="36"/>
  <SingleTextLineField FieldName="OtherSeal"  Caption="Other seals" MaxLength="128" Width="200" ExcelColumnWidth="36"/>
  <IntegerField FieldName="TotalWeightOfBags"  Caption="Total weight of bags" MinValue="0" MaxValue="2147483647" Mandatory="true" Width="120" ExcelColumnWidth="24"/>
  <IntegerField FieldName="TotalOfBags"  Caption="Total of bags" MinValue="0" MaxValue="2147483647" Width="100" ExcelColumnWidth="21"/>
  <IntegerField FieldName="GrossWeightAverage"  Caption="Gross weight average" MinValue="0" MaxValue="2147483647" Width="150" ExcelColumnWidth="20"/>
  <IntegerField FieldName="LessTareAverage"  Caption="Less tare average" MinValue="0" MaxValue="2147483647" Width="120" ExcelColumnWidth="26"/>
  <IntegerField FieldName="NetWeightAverage"  Caption="Net weight average" MinValue="0" MaxValue="2147483647" Width="150" ExcelColumnWidth="31"/>
  <DatepickerField FieldName="LaboratoryDate" Caption="Date" Mandatory="true" StartDateFilter="today-30" EndDateFilter="today" Width="100" ExcelColumnWidth="17"/>
  <SingleTextLineField FieldName="LaboratoryReference"  Caption="Laboratory reference" MaxLength="64" Mandatory="true" Width="250" ExcelColumnWidth="36"/>
  <CatalogueField FieldName="Parameter" CatalogueName="Parameter" EditorType="DropDownList" Caption="Parameter" Mandatory="true" Width="120" ExcelColumnWidth="32"/>
  <CatalogueField FieldName="Unit" CatalogueName="Unit" EditorType="DropDownList" Caption="Unit" Mandatory="true" Width="50" ExcelColumnWidth="32"/>
  <SingleTextLineField FieldName="Result"  Caption="Result" MaxLength="64" Mandatory="true" Width="250" ExcelColumnWidth="28"/>
  <CatalogueField FieldName="Method" CatalogueName="LaboratoryAnalysis" EditorType="DropDownList" Caption="Method" Mandatory="true" Width="150" ExcelColumnWidth="26"/>
  <PictureField FieldName="PictureReport"  Caption="Please choose the images that you need" CaptionGrid="Pictures" Width="100"/>
  <StatusField FieldName="Status"  Caption="Status" CaptionGrid="Status" Width="100" ExcelColumnWidth="24"/>
  <DatepickerField FieldName="BeginDate" Caption="Start date" Width="100" ExcelColumnWidth="21" Mandatory="true" />
  <DatepickerField FieldName="EndDate" Caption="End date" Width="100" ExcelColumnWidth="21"/>
</Fields>'
where BusinessApplicationId = @businessApplicationId