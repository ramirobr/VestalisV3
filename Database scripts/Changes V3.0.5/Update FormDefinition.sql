USE VESTALIS3
GO

declare @businessApplicationId uniqueidentifier

set @businessApplicationId = 'BFE0C1B1-B4B7-4342-BEE7-ED6CC097B6F5'


update dbo.FormDefinition set FormName = 'Empty Container Inspection' where FormName = 'Empty container inspection' and BusinessApplicationId = @businessApplicationId

update dbo.FormDefinition set FormName = 'Stuffing Inspection' where FormName = 'Stuffing inspection' and BusinessApplicationId = @businessApplicationId

update dbo.FormDefinition set FormName = 'Laboratory Analysis' where FormName = 'Laboratory Report' and BusinessApplicationId = @businessApplicationId


update dbo.InspectionReport set FormName = 'Empty Container Inspection' where FormName = 'Empty container inspection'

update dbo.InspectionReport set FormName = 'Stuffing Inspection' where FormName = 'Stuffing inspection'

update dbo.InspectionReport set FormName = 'Laboratory Analysis' where FormName = 'Laboratory Report'


--Service Order

update dbo.FormDefinition
set XmlFormDefinition = '<?xml version="1.0" encoding="utf-8" ?>
<Form >
  <Sections>
    <Section Caption="General">
      <FormElements>
        <Element Identifier="Client" IsDataGridVisible="true" IsFilterVisible ="true" IsVisibleClient="false" />
        <Element Identifier="BookingNumber" IsDataGridVisible="true" IsFilterVisible ="true" IsInspectionReportVisible = "true"/>
        <Element Identifier="Vessel" />
        <Element Identifier="OrderNumber" IsDataGridVisible="true" IsFilterVisible ="true" IsOrderIdentifier ="true"/>
        <Element Identifier="OrderDate" IsDataGridVisible="true" IsFilterVisible ="true" IsInspectionReportVisible = "true"/>
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
where FormName = 'Service Order' and BusinessApplicationId = @businessApplicationId

--Empty Container Inspection

update dbo.FormDefinition
set XmlFormDefinition = '<?xml version="1.0" encoding="utf-8" ?>
<Form>
  <Sections>
    <Section Caption="Empty container inspection">
      <FormElements>
        <Element Identifier="ContainerNumber" IsDataGridVisible="true" IsFilterVisible ="true" />
        <Element Identifier="InspectionDate" IsDataGridVisible="true" IsFilterVisible ="true"/>
        <Element Identifier="StartTime" IsDataGridVisible="true"/>
        <Element Identifier="EndTime" IsDataGridVisible="true"/>
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
        <Element Identifier="SIContainerNumber" IsDataGridVisible="true" IsFilterVisible ="true" />
        <Element Identifier="InspectionDate" IsDataGridVisible="true" IsFilterVisible ="true"/>
        <Element Identifier="BillNumber" IsDataGridVisible="true"/>
        <Element Identifier="Plate" IsDataGridVisible="true"/>
        <Element Identifier="QuantityOfBagsBill" IsDataGridVisible="true"/>
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
    <Section Caption="Pictures">
      <FormElements>
        <Element Identifier="PictureReport" IsDataGridVisible="true"/>
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
    <Section  Caption="Laboratory analysis result">
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