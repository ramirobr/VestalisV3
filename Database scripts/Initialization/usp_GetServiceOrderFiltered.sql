alter procedure usp_GetServiceOrdersFiltered	(	@businessApplicationid uniqueidentifier,
													@sortedColumn varchar(30),
													@sortDirection varchar(30),
													@fieldsFilter varchar(200),
													@booleanValue bit = null,
													@decimalValue decimal = null,
													@multiTextValue varchar(2000) = null,
													@singleTextValue varchar(400) = null,
													@regexTextValue varchar(500) = null,
													@timeTextValue varchar(50) = null,
													@intValue int = null,
													@catalogueValue varchar(200) = null,
													@daterangeBegin datetime = null,
													@daterangeEnd datetime = null												
												)
as
	create table #tempTablaFields
	(
		Id int primary key identity,
		FieldName varchar(100),
		FieldType int
	)

	--variables globales
	declare @selectFields  varchar(1000),
			@idFieldsTable integer,
			@id integer,
			@fieldName varchar(100),
			@fieldType int,
			@selectContainer nvarchar(max),
			@firstSelect nvarchar(max)

	--get the list of fields for creating final query
	set @selectFields = 'insert #tempTablaFields (FieldName,FieldType)
	select Distinct(FormValue.FieldName),FieldType from FormValue
	where FormValue.FieldName in ('+@fieldsFilter+')'

	--final query
	set @selectContainer = 'Select ServiceOrderId
							from ('
	--first part of the query
	set @firstSelect = 'SELECT ServiceOrder.ServiceOrderId , 
						isnull(isnull(isnull(isnull(isnull(Isnull(fvalue.TextValue,fvalue.DecimalValue),fvalue.IntegerValue),fvalue.CheckValue),cvalue.CatalogueValueData),fvalue.DateValue),vuapp.UserName)
						as ' +@sortedColumn +'
						FROM ServiceOrder 
						LEFT OUTER JOIN FormValue as fvalue on ServiceOrder.ServiceOrderId = fvalue.ServiceOrderId and fvalue.FieldName = ''' +@sortedColumn +'''
						LEFT OUTER JOIN CatalogueValue as cvalue on fvalue.CatalogueValueId = cvalue.CatalogueValueId
						LEFT OUTER JOIN VestalisUserApplication as vuapp on ServiceOrder.BusinessApplicationId = vuapp.BusinessApplicationId AND fvalue.UserName = vuapp.UserName
						WHERE ServiceOrder.IsDeleted = 0
						AND ServiceOrder.BusinessApplicationId = @businessApplicationId'

	exec(@selectFields)
	set @idFieldsTable = @@ROWCOUNT
	set @id = 1
	set @selectContainer = @selectContainer + @firstSelect
	
	declare @queryBody varchar(max)
	set @queryBody = ''

	while (@id <= @idFieldsTable)
	begin 
		--read the result for creating final query
		select @fieldName= #tempTablaFields.FieldName, @fieldType = #tempTablaFields.FieldType 
		from #tempTablaFields
		where #tempTablaFields.Id = @Id
		set @id = @id + 1

		if(@fieldType = 1) -- catalogues
		begin
			set @queryBody = @queryBody + ' INTERSECT '
			set @queryBody = @queryBody + 'select FormValue.ServiceOrderId, cvalue.CatalogueValueData as '+@sortedColumn+'
			from FormValue 
			LEFT OUTER JOIN FormValue as fvalue on FormValue.ServiceOrderId = fvalue.ServiceOrderId and fvalue.FieldName = '''+@sortedColumn+'''
			LEFT OUTER JOIN CatalogueValue as cvalue on fvalue.CatalogueValueId = cvalue.CatalogueValueId
			where FormValue.IsDeleted = 0 and cvalue.CatalogueValueData like @catalogueValue + ''%'' and cvalue.IsDeleted = 0 and
			FormValue.InspectionReportItemId IS NULL
			and FormValue.FieldName= '''+@fieldName+''''
		end
		if(@fieldType = 2) --single text
		begin
			set @queryBody = @queryBody + ' INTERSECT '
			set @queryBody = @queryBody + 'select FormValue.ServiceOrderId, fvalue.TextValue as '+@sortedColumn+'
			from FormValue
			LEFT OUTER JOIN FormValue as fvalue on FormValue.ServiceOrderId = fvalue.ServiceOrderId and fvalue.FieldName = '''+@sortedColumn+'''
			where FormValue.IsDeleted = 0 AND FormValue.TextValue =  @singleTextValue and FormValue.InspectionReportItemId IS NULL
			and FormValue.FieldName= '''+@fieldName+''''			
		end
		if(@fieldType = 3) --multi text
		begin
			set @queryBody = @queryBody + ' INTERSECT '
			set @queryBody = @queryBody + 'select FormValue.ServiceOrderId, fvalue.TextValue as '+@sortedColumn+'
			from FormValue
			LEFT OUTER JOIN FormValue as fvalue on FormValue.ServiceOrderId = fvalue.ServiceOrderId and fvalue.FieldName = '''+@sortedColumn+'''
			where FormValue.IsDeleted = 0 AND FormValue.TextValue = @multiTextValue  and FormValue.InspectionReportItemId IS NULL
			and FormValue.FieldName= '''+@fieldName+''''			
		end
		if(@fieldType = 4) --boolean 
		begin
			set @queryBody = @queryBody + ' INTERSECT '
			set @queryBody = @queryBody + 'select FormValue.ServiceOrderId, fvalue.TextValue as '+@sortedColumn+'
			from FormValue
			LEFT OUTER JOIN FormValue as fvalue on FormValue.ServiceOrderId = fvalue.ServiceOrderId and fvalue.FieldName = '''+@sortedColumn+'''
			where FormValue.IsDeleted = 0 AND FormValue.CheckValue = @booleanValue  and FormValue.InspectionReportItemId IS NULL
			and FormValue.FieldName= '''+@fieldName+''''			
		end
		if(@fieldType = 5) --date values 
		begin
			set @queryBody = @queryBody + ' INTERSECT '
			set @queryBody = @queryBody + 'select FormValue.ServiceOrderId, convert(varchar(20), fvalue.DateValue, 101) as '+@sortedColumn+'
			from FormValue
			LEFT OUTER JOIN FormValue as fvalue on FormValue.ServiceOrderId = fvalue.ServiceOrderId and fvalue.FieldName = '''+@sortedColumn+'''
			where FormValue.IsDeleted = 0 and FormValue.InspectionReportItemId IS NULL
			AND (FormValue.DateValue >= @daterangeBegin) 
			AND (FormValue.DateValue <= @daterangeEnd) 
			AND (FormValue.FieldName = ''OrderDate'' AND FormValue.InspectionReportItemId IS NULL)'
		end
		if(@fieldType = 6) --time value
		begin
			set @queryBody = @queryBody + ' INTERSECT '
			set @queryBody = @queryBody + 'select FormValue.ServiceOrderId, fvalue.TextValue as '+@sortedColumn+'
			from FormValue
			LEFT OUTER JOIN FormValue as fvalue on FormValue.ServiceOrderId = fvalue.ServiceOrderId and fvalue.FieldName = '''+@sortedColumn+'''
			where FormValue.IsDeleted = 0 AND FormValue.TextValue =  @timeTextValue  and FormValue.InspectionReportItemId IS NULL
			and FormValue.FieldName= '''+@fieldName+''''			
		end
		if(@fieldType = 7) --integer value
		begin
			set @queryBody = @queryBody + ' INTERSECT '
			set @queryBody = @queryBody + 'select FormValue.ServiceOrderId, fvalue.IntegerValue as '+@sortedColumn+'
			from FormValue
			LEFT OUTER JOIN FormValue as fvalue on FormValue.ServiceOrderId = fvalue.ServiceOrderId and fvalue.FieldName = '''+@sortedColumn+'''
			where FormValue.IsDeleted = 0 AND FormValue.IntegerValue = @intValue  and FormValue.InspectionReportItemId IS NULL
			and FormValue.FieldName= '''+@fieldName+''''			
		end
		if(@fieldType = 8) --decimal value
		begin
			set @queryBody = @queryBody + ' INTERSECT ' 
			set @queryBody = @queryBody + 'select FormValue.ServiceOrderId, fvalue.DecimalValue as '+@sortedColumn+'
			from FormValue
			LEFT OUTER JOIN FormValue as fvalue on FormValue.ServiceOrderId = fvalue.ServiceOrderId and fvalue.FieldName = '''+@sortedColumn+'''
			where FormValue.IsDeleted = 0 AND FormValue.DecimalValue = @decimalValue  and FormValue.InspectionReportItemId IS NULL
			and FormValue.FieldName= '''+@fieldName+''''			
		end
		if(@fieldType = 9) --regex value
		begin
			set @queryBody = @queryBody + ' INTERSECT '
			set @queryBody = @queryBody + 'select FormValue.ServiceOrderId, fvalue.TextValue as '+@sortedColumn+'
			from FormValue
			LEFT OUTER JOIN FormValue as fvalue on FormValue.ServiceOrderId = fvalue.ServiceOrderId and fvalue.FieldName = '''+@sortedColumn+'''
			where FormValue.IsDeleted = 0 AND FormValue.TextValue = @regexTextValue and FormValue.InspectionReportItemId IS NULL
			and FormValue.FieldName= '''+@fieldName+''''			
		end
	end

	drop table #tempTablaFields

	
	declare @footerQuery nvarchar(50) = ''

	if(@sortDirection = 'Ascending')
	begin
		set @footerQuery = ') as result order by  result.'+@sortedColumn+' asc '
	end
	if(@sortDirection = 'Descending')
	begin
		set @footerQuery = ') as result order by  result.'+@sortedColumn+' desc '
	end

	set @selectContainer = @selectContainer + @queryBody + @footerQuery

	declare @params nvarchar(max)

	set @params = N'@businessApplicationid uniqueidentifier,
@booleanValue bit = null,
@decimalValue decimal = null,
@multiTextValue varchar(2000) = null,
@singleTextValue varchar(400) = null,
@regexTextValue varchar(500) = null,
@timeTextValue varchar(50) = null,
@intValue int = null,
@catalogueValue varchar(200) = null,
@daterangeBegin datetime = null,
@daterangeEnd datetime = null'

	
EXEC sp_executesql @statement = @selectContainer,
				@params=@params,
				@businessApplicationid = @businessApplicationid,
				@booleanValue = @booleanValue ,
				@decimalValue = @decimalValue,
				@multiTextValue = @multiTextValue,
				@singleTextValue = @singleTextValue,
				@regexTextValue = @regexTextValue,
				@timeTextValue = @timeTextValue,
				@intValue = @intValue,
				@catalogueValue = @catalogueValue,
				@daterangeBegin = @daterangeBegin,
				@daterangeEnd = @daterangeBegin

print @selectContainer
go
