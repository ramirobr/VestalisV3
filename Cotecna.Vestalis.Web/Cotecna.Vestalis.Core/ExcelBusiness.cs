using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using Cotecna.Vestalis.Core.Resources;
using System.Drawing;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class contains all methods for generating excel reports
    /// </summary>
    public static class ExcelBusiness
    {
        #region public methods


        #region GenerateAllInspectionReports
        /// <summary>
        /// Generate all inspection reports
        /// </summary>
        /// <param name="templatePath">Path of template</param>
        /// <param name="itemsource">Item source</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream GenerateAllInspectionReports(ExportInspectionReportsModel itemsource,string logoPath)
        {
            MemoryStream ms = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms,SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                Workbook workbook = new Workbook();
                workbookPart.Workbook = workbook;

                //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
                WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");

                //get and save the stylesheet
                Stylesheet stylesheet = VestalisStyleSheet();
                workbookStylesPart.Stylesheet = stylesheet;
                workbookStylesPart.Stylesheet.Save();

                int sheetId = 1;
                Sheets sheets = new Sheets();
                if (itemsource.IsSelectedServiceOrder)
                {
                    //Generate service order report
                    GenerateServiceOrder(itemsource, workbookPart, sheets,sheetId,logoPath);
                    sheetId++;
                }
                //Generate inspection reports
                GenerateAllInspectionReports(itemsource, workbookPart, sheets, sheetId, logoPath);
                
                //add the new sheet to the report
                workbook.Append(sheets);
                //save all report
                workbook.Save();
                //close the stream.
                document.Close();

            }
            return ms;
        }

        #region GenerateServiceOrder
        /// <summary>
        /// Generate service order report
        /// </summary>
        /// <param name="itemsource">Item source</param>
        /// <param name="workbookPart">Worbook part</param>
        private static void GenerateServiceOrder(ExportInspectionReportsModel itemsource, WorkbookPart workbookPart, Sheets sheets,int sheetId,string logoPath)
        {
            if (itemsource.IsSelectedServiceOrder)
            {

                // Remove the sheet reference from the workbook.
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                // The SheetData object will contain all the data.
                SheetData sheetData = new SheetData();
                Worksheet worksheet = new Worksheet();

                Form serviceOrder = itemsource.ServiceOrderData;

                Row rowTitle;
                //get the string name of the columns
                string[] excelColumnNamesTitle = new string[9];
                for (int n = 0; n < 9; n++)
                    excelColumnNamesTitle[n] = GetExcelColumnName(n);

                //build the title
                for (int i = 1; i <= 6; i++)
                {
                    rowTitle = new Row() { RowIndex = (UInt32Value)(uint)i };
                    for (int cellval = 0; cellval < 9; cellval++)
                    {
                        AppendTextCell(excelColumnNamesTitle[cellval] + i, string.Empty, rowTitle, 3);
                    }
                    sheetData.Append(rowTitle);
                }

                MergeCells mergeCells = new MergeCells();

                Row currentRowTitle = sheetData.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)2);
                //add the business application name
                UpdateStringCellValue("B2", itemsource.BusinessApplicationName, currentRowTitle, 5);

                //merge all cells in the title
                MergeCell mergeCell = new MergeCell();
                mergeCell.Reference = "B2:E2";
                mergeCells.Append(mergeCell);

                currentRowTitle = sheetData.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)4);
                //add the form name
                UpdateStringCellValue("B4", itemsource.ServiceOrderSheetName, currentRowTitle, 5);

                //merge all cell in the form name
                mergeCell = new MergeCell();
                mergeCell.Reference = "B4:E4";
                mergeCells.Append(mergeCell);
                Drawing drawing = AddLogo(logoPath, worksheetPart);
                Columns columns = new Columns();
                columns.Append(CreateColumnData((UInt32Value)(uint)1, (UInt32Value)(uint)1, 26));
                columns.Append(CreateColumnData((UInt32Value)(uint)2, (UInt32Value)(uint)2, 73));

                worksheet.Append(columns);

                int rowIndex = 8;
                Row sectionRow;

                foreach (var section in serviceOrder.Sections)
                {
                    sectionRow = new Row() { RowIndex = (UInt32Value)(uint)rowIndex }; 
                    mergeCell = new MergeCell();
                    mergeCell.Reference = "A" + rowIndex + ":B" + rowIndex;
                    mergeCells.Append(mergeCell);
                    AppendTextCell("A" + rowIndex, section.Caption, sectionRow, 6);
                    AppendTextCell("B" + rowIndex, string.Empty, sectionRow, 6);
                    sheetData.Append(sectionRow);
                    foreach (var element in section.FormElements)
                    {
                        rowIndex++;
                        //The current row is obtained for updating the value of the cell
                        Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                        switch (element.Field.FieldType)
                        {
                            case FieldType.Catalogue:
                                AppendTextCell("A" + rowIndex.ToString(), element.Field.Caption, rowData, 1);
                                if (!string.IsNullOrEmpty(element.Field.FieldValue))
                                {
                                    string catalogueValue = CatalogueBusiness.GetCatalogueValue(new Guid(element.Field.FieldValue)).CatalogueValueData;
                                    AppendTextCell("B" + rowIndex.ToString(), catalogueValue, rowData, 1);
                                }
                                else
                                {
                                    AppendTextCell("B" + rowIndex.ToString(), string.Empty, rowData, 1);
                                }
                                break;
                            case FieldType.RegularExpressionText:
                            case FieldType.Time:
                            case FieldType.SingleTextLine:
                            case FieldType.MultipleTextLine:
                            case FieldType.Datepicker:
                                AppendTextCell("A" + rowIndex.ToString(), element.Field.Caption, rowData, 1);
                                AppendTextCell("B" + rowIndex.ToString(), element.Field.FieldValue, rowData, 1);
                                break;
                            case FieldType.Boolean:
                                string boolValue = element.Field.FieldValue == "True" ? LanguageResource.Yes : LanguageResource.No;
                                AppendTextCell("A" + rowIndex.ToString(), element.Field.Caption, rowData, 1);
                                AppendTextCell("B" + rowIndex.ToString(), boolValue, rowData, 1);
                                break;
                            case FieldType.Integer:
                            case FieldType.Decimal:
                                AppendTextCell("A" + rowIndex.ToString(), element.Field.Caption, rowData, 1);
                                AppendNumberCell("B" + rowIndex.ToString(), element.Field.FieldValue, rowData, 1);
                                break;
                            default:
                                break;
                        }
                        
                        sheetData.Append(rowData);
                    }
                    rowIndex+=2;
                }

                
                worksheet.Append(sheetData);
                //add merged cells
                worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                worksheet.Append(drawing);
                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();

                //create the new sheet for this report 
                Sheet sheet = new Sheet() { Name = itemsource.ServiceOrderSheetName, SheetId = (UInt32Value)(uint)sheetId, Id = workbookPart.GetIdOfPart(worksheetPart) };
                sheets.Append(sheet);
                sheetId++;
            }
        }
        #endregion

        #region GenerateAllInspectionReports
        /// <summary>
        /// Generate inspection reports
        /// </summary>
        /// <param name="itemsource">Item source</param>
        /// <param name="workbookPart">Worbook part</param>
        private static void GenerateAllInspectionReports(ExportInspectionReportsModel itemsource, WorkbookPart workbookPart, Sheets sheets, int sheetId, string logoPath)
        {
            //  Loop through each of the DataTables in our DataSet, and create a new Excel Worksheet for each.
            foreach (var item in itemsource.InspectionReports)
            {
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                Worksheet worksheet = new Worksheet();
                SheetData sheetData1 = new SheetData();

                //get the number of columns in the report
                Row rowTitle;
                int numberOfColumnsCaption = item.Value.Captions.Count;

                //get the string name of the columns
                string[] excelColumnNamesTitle = new string[numberOfColumnsCaption];
                for (int n = 0; n < numberOfColumnsCaption; n++)
                    excelColumnNamesTitle[n] = GetExcelColumnName(n);

                //build the title
                for (int i = 1; i <= 6; i++)
                {
                    rowTitle = new Row() { RowIndex = (UInt32Value)(uint)i };
                    for (int cellval = 0; cellval < numberOfColumnsCaption; cellval++)
                    {
                        AppendTextCell(excelColumnNamesTitle[cellval] + i, string.Empty, rowTitle, 3);
                    }
                    sheetData1.Append(rowTitle);
                }

                MergeCells mergeCells = new MergeCells();

                Row currentRowTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)2);
                //add the business application name
                UpdateStringCellValue("B2", item.Value.BusinessApplicationName, currentRowTitle, 5);

                string lastColumnName = excelColumnNamesTitle.Last() + "2";
                //merge all cells in the title
                MergeCell mergeCell = new MergeCell();
                mergeCell.Reference = "B2:" + lastColumnName;
                mergeCells.Append(mergeCell);

                currentRowTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)4);
                //add the form name
                UpdateStringCellValue("B4", item.Key, currentRowTitle, 5);

                lastColumnName = lastColumnName.Replace("2", "4");

                //merge all cell in the form name
                mergeCell = new MergeCell();
                mergeCell.Reference = "B4:" + lastColumnName;
                mergeCells.Append(mergeCell);

                Drawing drawing = AddLogo(logoPath, worksheetPart);


                int rowIndex = 7;

                //get the names of the columns
                string[] excelColumnNamesCaptions = new string[numberOfColumnsCaption];
                for (int n = 0; n < numberOfColumnsCaption; n++)
                    excelColumnNamesCaptions[n] = GetExcelColumnName(n);

                Row rowCaption = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                //build column names of the report
                Columns columns = new Columns();
                for (int i = 0; i < item.Value.Captions.Count; i++)
                {
                    var caption = item.Value.Captions[i];
                    AppendTextCell(excelColumnNamesCaptions[i] + rowIndex.ToString(), caption.Caption, rowCaption, 2);
                    columns.Append(CreateColumnData((UInt32Value)(uint)i + 1, (UInt32Value)(uint)i + 1, caption.ExcelColumnWidth));
                }
                sheetData1.Append(rowCaption);
                //add the new row with the name of the columns
                worksheet.Append(columns);
                rowIndex = 8;
                //write the data of the report
                foreach (var row in item.Value.DataRows)
                {
                    int numberOfColumnsData = row.FieldValues.Count;
                    //get column names
                    string[] excelColumnNamesData = new string[numberOfColumnsData];
                    for (int n = 0; n < numberOfColumnsData; n++)
                        excelColumnNamesData[n] = GetExcelColumnName(n);

                    //build the row
                    Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                    for (int colInx = 0; colInx < numberOfColumnsData; colInx++)
                    {
                        DynamicDataRowValue col = row.FieldValues[colInx];
                        switch (col.FieldType)
                        {
                            case (int)FieldType.Catalogue:
                            case (int)FieldType.RegularExpressionText:
                            case (int)FieldType.Time:
                            case (int)FieldType.SingleTextLine:
                            case (int)FieldType.MultipleTextLine:
                            case (int)FieldType.Datepicker:
                            case (int)FieldType.Boolean:
                            case (int)FieldType.AutoComplete:
                            case (int)FieldType.StatusField:
                                AppendTextCell(excelColumnNamesData[colInx] + rowIndex.ToString(), col.FieldValue, rowData, 1);
                                break;
                            case (int)FieldType.Integer:
                            case (int)FieldType.Decimal:
                                AppendNumberCell(excelColumnNamesData[colInx] + rowIndex.ToString(), col.FieldValue, rowData, 1);
                                break;
                            default:
                                break;
                        }
                    }

                    //add the new row to the report
                    sheetData1.Append(rowData);
                    rowIndex++;
                }

                //add the information of the current sheet
                worksheet.Append(sheetData1);
                //add merged cells
                worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                worksheet.Append(drawing);
                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();

                
                //create the new sheet for this report 
                Sheet sheet = new Sheet() { Name = item.Key, SheetId = (UInt32Value)(uint)sheetId, Id = workbookPart.GetIdOfPart(worksheetPart) };
                sheets.Append(sheet);
                sheetId++;
            }
        }
        #endregion

        #endregion

        #region GenerateCatalogueReport
        /// <summary>
        /// Generate the report for catalogue categories
        /// </summary>
        /// <param name="templatePath">Path of the template</param>
        /// <param name="itemSource">Item source</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream GenerateCatalogueReport(List<CatalogueModel> itemSource,string logoPath)
        {
            MemoryStream ms = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms,SpreadsheetDocumentType.Workbook))
            {
                //create the new workbook
                WorkbookPart workbookPart = document.AddWorkbookPart();
                Workbook workbook = new Workbook();
                workbookPart.Workbook = workbook;

                //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
                WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");

                //get and save the stylesheet
                Stylesheet stylesheet = VestalisStyleSheet();
                workbookStylesPart.Stylesheet = stylesheet;
                workbookStylesPart.Stylesheet.Save();

                //add the new workseet
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                Worksheet worksheet = new Worksheet();
                SheetData sheetData1 = new SheetData();

                Sheets sheets = new Sheets();

                //get the number of columns in the report
                Row rowTitle;
                
                //get the string name of the columns
                string[] excelColumnNamesTitle = new string[2];
                for (int n = 0; n < 2; n++)
                    excelColumnNamesTitle[n] = GetExcelColumnName(n);

                //build the title
                for (int i = 1; i <= 6; i++)
                {
                    rowTitle = new Row() { RowIndex = (UInt32Value)(uint)i };
                    for (int cellval = 0; cellval < 2; cellval++)
                    {
                        AppendTextCell(excelColumnNamesTitle[cellval] + i, string.Empty, rowTitle, 3);
                    }
                    sheetData1.Append(rowTitle);
                }

                MergeCells mergeCells = new MergeCells();

                Row currentRowTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)2);
                //add the business application name
                UpdateStringCellValue("B2", LanguageResource.CatalogueReport, currentRowTitle, 5);

                //merge all cells in the title
                MergeCell mergeCell = new MergeCell();
                mergeCell.Reference = "B2:B4";
                mergeCells.Append(mergeCell);
                Drawing drawing = AddLogo(logoPath, worksheetPart);
                int rowIndex = 7;

                Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex.ToString(), LanguageResource.BusinessApplication, rowData, 2);
                AppendTextCell("B" + rowIndex.ToString(), LanguageResource.CategoryName, rowData, 2);
                sheetData1.Append(rowData);


                Columns columns = new Columns();
                columns.Append(CreateColumnData((UInt32Value)(uint)1, (UInt32Value)(uint)1, 50));
                columns.Append(CreateColumnData((UInt32Value)(uint)2, (UInt32Value)(uint)2, 61));
                worksheet.Append(columns);

                rowIndex = 8;
                foreach (var item in itemSource)
                {
                    rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                    AppendTextCell("A" + rowIndex.ToString(), item.BusinessApplicationName, rowData, 1);
                    AppendTextCell("B" + rowIndex.ToString(), item.CatalogueCategoryName, rowData, 1);
                    sheetData1.Append(rowData);
                    rowIndex++;
                }

                //add the information of the current sheet
                worksheet.Append(sheetData1);
                //add merged cells
                worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                worksheet.Append(drawing);
                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();

                //create the new sheet for this report 
                Sheet sheet = new Sheet() { Name = LanguageResource.Report, SheetId = (UInt32Value)1, Id = workbookPart.GetIdOfPart(worksheetPart) };
                sheets.Append(sheet);
                //add the new sheet to the report
                workbook.Append(sheets);
                //save all report
                workbook.Save();
                //close the stream.
                document.Close();
            }

            return ms;
        }
        #endregion

        #region GenerateCatalogueValueReport
        /// <summary>
        /// Generate the report for catalogue categories
        /// </summary>
        /// <param name="templatePath">Path of the template</param>
        /// <param name="itemSource">Item source</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream GenerateCatalogueValueReport(CatalogueValueSearchModel itemSource,string logoPath)
        {
            MemoryStream ms = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                //create the new workbook
                WorkbookPart workbookPart = document.AddWorkbookPart();
                Workbook workbook = new Workbook();
                workbookPart.Workbook = workbook;

                //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
                WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");

                //get and save the stylesheet
                Stylesheet stylesheet = VestalisStyleSheet();
                workbookStylesPart.Stylesheet = stylesheet;
                workbookStylesPart.Stylesheet.Save();

                //add the new workseet
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                Worksheet worksheet = new Worksheet();
                SheetData sheetData1 = new SheetData();

                Sheets sheets = new Sheets();

                //get the number of columns in the report
                Row rowTitle;

                //get the string name of the columns
                string[] excelColumnNamesTitle = new string[2];
                for (int n = 0; n < 2; n++)
                    excelColumnNamesTitle[n] = GetExcelColumnName(n);

                //build the title
                for (int i = 1; i <= 6; i++)
                {
                    rowTitle = new Row() { RowIndex = (UInt32Value)(uint)i };
                    for (int cellval = 0; cellval < 2; cellval++)
                    {
                        AppendTextCell(excelColumnNamesTitle[cellval] + i, string.Empty, rowTitle, 3);
                    }
                    sheetData1.Append(rowTitle);
                }

                MergeCells mergeCells = new MergeCells();

                Row currentRowTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)2);
                //add the business application name
                UpdateStringCellValue("B2", LanguageResource.CatalogueValuesReport, currentRowTitle, 5);

                //merge all cells in the title
                MergeCell mergeCell = new MergeCell();
                mergeCell.Reference = "B2:B4";
                mergeCells.Append(mergeCell);
                Drawing drawing = AddLogo(logoPath, worksheetPart);

                Columns columns = new Columns();
                columns.Append(CreateColumnData((UInt32Value)(uint)1, (UInt32Value)(uint)1, 50));
                columns.Append(CreateColumnData((UInt32Value)(uint)2, (UInt32Value)(uint)2, 71));
                worksheet.Append(columns);
                
                int rowIndex = 8;
                Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex, LanguageResource.BusinessApplicationName, rowData, 2);
                AppendTextCell("B" + rowIndex, itemSource.BusinessApplicatioName, rowData, 1);
                sheetData1.Append(rowData);

                rowIndex = 9;
                rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex, LanguageResource.CatalogueName, rowData, 2);
                AppendTextCell("B" + rowIndex, itemSource.CatalogueSelectedName, rowData, 1);
                sheetData1.Append(rowData);

                rowIndex = 11;

                rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex.ToString(), LanguageResource.Value, rowData, 2);
                AppendTextCell("B" + rowIndex.ToString(), LanguageResource.Description, rowData, 2);
                sheetData1.Append(rowData);

                rowIndex = 12;

                foreach (var item in itemSource.SearchResult.Collection)
                {
                    rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                    AppendTextCell("A" + rowIndex.ToString(), item.CatalogueValueData, rowData, 1);
                    AppendTextCell("B" + rowIndex.ToString(), item.CatalogueValueDescription, rowData, 1);
                    sheetData1.Append(rowData);
                    rowIndex++;
                }

                //add the information of the current sheet
                worksheet.Append(sheetData1);
                //add merged cells
                worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                worksheet.Append(drawing);
                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();

                //create the new sheet for this report 
                Sheet sheet = new Sheet() { Name = LanguageResource.Report, SheetId = (UInt32Value)1, Id = workbookPart.GetIdOfPart(worksheetPart) };
                sheets.Append(sheet);
                //add the new sheet to the report
                workbook.Append(sheets);
                //save all report
                workbook.Save();
                //close the stream.
                document.Close();
            }

            return ms;
        }
        #endregion

        #region GenerateUserReport
        /// <summary>
        /// Genere the report user list
        /// </summary>
        /// <param name="templatePath">Path of the template</param>
        /// <param name="itemSource">Data source</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream GenerateUserReport(List<UserGridModel> itemSource,string logoPath)
        {
            //Put in memory the template file
            MemoryStream ms = new MemoryStream();

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                //create the new workbook
                WorkbookPart workbookPart = document.AddWorkbookPart();
                Workbook workbook = new Workbook();
                workbookPart.Workbook = workbook;

                //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
                WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");

                //get and save the stylesheet
                Stylesheet stylesheet = VestalisStyleSheet();
                workbookStylesPart.Stylesheet = stylesheet;
                workbookStylesPart.Stylesheet.Save();

                //add the new workseet
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                Worksheet worksheet = new Worksheet();
                SheetData sheetData1 = new SheetData();

                Sheets sheets = new Sheets();

                //get the number of columns in the report
                Row rowTitle;

                //get the string name of the columns
                string[] excelColumnNamesTitle = new string[3];
                for (int n = 0; n < 3; n++)
                    excelColumnNamesTitle[n] = GetExcelColumnName(n);

                //build the title
                for (int i = 1; i <= 6; i++)
                {
                    rowTitle = new Row() { RowIndex = (UInt32Value)(uint)i };
                    for (int cellval = 0; cellval < 3; cellval++)
                    {
                        AppendTextCell(excelColumnNamesTitle[cellval] + i, string.Empty, rowTitle, 3);
                    }
                    sheetData1.Append(rowTitle);
                }

                MergeCells mergeCells = new MergeCells();

                Row currentRowTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)2);
                //add the business application name
                UpdateStringCellValue("A2", LanguageResource.UserReport, currentRowTitle, 5);

                //merge all cells in the title
                MergeCell mergeCell = new MergeCell();
                mergeCell.Reference = "A2:C4";
                mergeCells.Append(mergeCell);
                Drawing drawing = AddLogo(logoPath, worksheetPart);
                Columns columns = new Columns();
                columns.Append(CreateColumnData((UInt32Value)(uint)1, (UInt32Value)(uint)1, 26));
                columns.Append(CreateColumnData((UInt32Value)(uint)2, (UInt32Value)(uint)2, 34));
                columns.Append(CreateColumnData((UInt32Value)(uint)3, (UInt32Value)(uint)3, 86));
                worksheet.Append(columns);

                int rowIndex = 7;

                Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex.ToString(), LanguageResource.UserType, rowData, 2);
                AppendTextCell("B" + rowIndex.ToString(), LanguageResource.UserName, rowData, 2);
                AppendTextCell("C" + rowIndex.ToString(), LanguageResource.BusinessApplication, rowData, 2);
                sheetData1.Append(rowData);

                rowIndex = 8;
                foreach (var item in itemSource)
                {

                    rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };

                    AppendTextCell("A" + rowIndex.ToString(), item.UserType, rowData, 1);
                    AppendTextCell("B" + rowIndex.ToString(), item.Email, rowData, 1);
                    AppendTextCell("C" + rowIndex.ToString(), item.BusinessApplications, rowData, 1);

                    sheetData1.Append(rowData);
                    rowIndex++;
                }

                //add the information of the current sheet
                worksheet.Append(sheetData1);
                //add merged cells
                worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                worksheet.Append(drawing);
                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();

                //create the new sheet for this report 
                Sheet sheet = new Sheet() { Name = LanguageResource.Report, SheetId = (UInt32Value)1, Id = workbookPart.GetIdOfPart(worksheetPart) };
                sheets.Append(sheet);
                //add the new sheet to the report
                workbook.Append(sheets);
                //save all report
                workbook.Save();
                //close the stream.
                document.Close();
            }

            return ms;
        }
        #endregion

        #region GeneratePermissionReport

        /// <summary>
        /// Generate permissions report
        /// </summary>
        /// <param name="templatePath">Path of the template</param>
        /// <param name="itemSource">Data source</param>
        /// <param name="userName">Thhe name of selected user</param>
        /// <param name="userType">The type of user</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream GeneratePermissionReport(List<PermissionGridModel> itemSource, string userName, string userType, string logoPath)
        {
            MemoryStream ms = new MemoryStream();

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                //create the new workbook
                WorkbookPart workbookPart = document.AddWorkbookPart();
                Workbook workbook = new Workbook();
                workbookPart.Workbook = workbook;

                //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
                WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");

                //get and save the stylesheet
                Stylesheet stylesheet = VestalisStyleSheet();
                workbookStylesPart.Stylesheet = stylesheet;
                workbookStylesPart.Stylesheet.Save();

                //add the new workseet
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                Worksheet worksheet = new Worksheet();
                SheetData sheetData1 = new SheetData();

                Sheets sheets = new Sheets();

                //get the number of columns in the report
                Row rowTitle;

                //get the string name of the columns
                string[] excelColumnNamesTitle = new string[2];
                for (int n = 0; n < 2; n++)
                    excelColumnNamesTitle[n] = GetExcelColumnName(n);

                //build the title
                for (int i = 1; i <= 6; i++)
                {
                    rowTitle = new Row() { RowIndex = (UInt32Value)(uint)i };
                    for (int cellval = 0; cellval < 2; cellval++)
                    {
                        AppendTextCell(excelColumnNamesTitle[cellval] + i, string.Empty, rowTitle, 3);
                    }
                    sheetData1.Append(rowTitle);
                }

                MergeCells mergeCells = new MergeCells();

                Row currentRowTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)2);
                //add the business application name
                UpdateStringCellValue("B2", LanguageResource.UsersAndPermissions, currentRowTitle, 5);

                //merge all cells in the title
                MergeCell mergeCell = new MergeCell();
                mergeCell.Reference = "B2:B4";
                mergeCells.Append(mergeCell);
                Drawing drawing = AddLogo(logoPath, worksheetPart);
                Columns columns = new Columns();
                columns.Append(CreateColumnData((UInt32Value)(uint)1, (UInt32Value)(uint)1, 42));
                columns.Append(CreateColumnData((UInt32Value)(uint)2, (UInt32Value)(uint)2, 80));
                worksheet.Append(columns);

                int rowIndex = 8;

                Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex, LanguageResource.UserEmail, rowData, 2);
                AppendTextCell("B" + rowIndex, userName, rowData,1);
                sheetData1.Append(rowData);

                rowIndex = 9;
                rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex, LanguageResource.UserType, rowData, 2);
                AppendTextCell("B" + rowIndex, userType, rowData, 1);
                sheetData1.Append(rowData);

                rowIndex = 11;

                rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                AppendTextCell("A" + rowIndex.ToString(), LanguageResource.BusinessApplication, rowData, 2);
                AppendTextCell("B" + rowIndex.ToString(), LanguageResource.AssignedRoles, rowData, 2);
                sheetData1.Append(rowData);
                
                rowIndex = 12;
                foreach (var item in itemSource)
                {

                    rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };

                    AppendTextCell("A" + rowIndex.ToString(), item.BusinessApplication, rowData, 1);
                    AppendTextCell("B" + rowIndex.ToString(), item.Roles, rowData, 1);

                    sheetData1.Append(rowData);
                    rowIndex++;
                }

                //add the information of the current sheet
                worksheet.Append(sheetData1);
                //add merged cells
                worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                worksheet.Append(drawing);
                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();

                //create the new sheet for this report 
                Sheet sheet = new Sheet() { Name = LanguageResource.Report, SheetId = (UInt32Value)1, Id = workbookPart.GetIdOfPart(worksheetPart) };
                sheets.Append(sheet);
                //add the new sheet to the report
                workbook.Append(sheets);
                //save all report
                workbook.Save();
                //close the stream.
                document.Close();

            }

            return ms;
        }
        #endregion

        #region GenerateReportDinamically
        /// <summary>
        /// Generate an excel report dinamically
        /// </summary>
        /// <param name="model">Data source</param>
        public static MemoryStream GenerateReportDinamically(DynamicDataGrid model, string logoPath)
        {
            MemoryStream report = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(report, SpreadsheetDocumentType.Workbook))
            {
                //create the new workbook
                WorkbookPart workbookPart = document.AddWorkbookPart();
                Workbook workbook = new Workbook();
                workbookPart.Workbook = workbook;

                //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
                WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");

                //get and save the stylesheet
                Stylesheet stylesheet = VestalisStyleSheet();
                workbookStylesPart.Stylesheet = stylesheet;
                workbookStylesPart.Stylesheet.Save();

                //add the new workseet
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                Worksheet worksheet = new Worksheet();
                SheetData sheetData1 = new SheetData();

                Sheets sheets = new Sheets();

                //get the number of columns in the report
                Row rowTitle;
                int numberOfColumnsCaption = model.Captions.Count;
                
                //get the string name of the columns
                string[] excelColumnNamesTitle = new string[numberOfColumnsCaption];
                for (int n = 0; n < numberOfColumnsCaption; n++)
                    excelColumnNamesTitle[n] = GetExcelColumnName(n);

                //build the title
                for (int i = 1; i <= 6; i++)
                {
                    rowTitle = new Row() { RowIndex = (UInt32Value)(uint)i };
                    for (int cellval = 0; cellval < numberOfColumnsCaption; cellval++)
                    {
                        AppendTextCell(excelColumnNamesTitle[cellval] + i, string.Empty, rowTitle, 3);   
                    }
                    sheetData1.Append(rowTitle);
                }

                MergeCells mergeCells = new MergeCells();

                Row currentRowTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)2);
                //add the business application name
                UpdateStringCellValue("B2", model.BusinessApplicationName, currentRowTitle, 5);

                string lastColumnName = excelColumnNamesTitle.Last() + "2";
                //merge all cells in the title
                MergeCell mergeCell = new MergeCell();
                mergeCell.Reference = "B2:" + lastColumnName;
                mergeCells.Append(mergeCell);

                Drawing drawing = AddLogo(logoPath, worksheetPart);

                currentRowTitle = sheetData1.Elements<Row>().FirstOrDefault(row => row.RowIndex.Value == (uint)4);
                //add the form name
                UpdateStringCellValue("B4", model.FormName, currentRowTitle,5);

                lastColumnName = lastColumnName.Replace("2", "4");
                //merge all cell in the form name
                mergeCell = new MergeCell();
                mergeCell.Reference = "B4:" + lastColumnName;
                mergeCells.Append(mergeCell);
                

                int rowIndex = 7;

                //get the names of the columns
                string[] excelColumnNamesCaptions = new string[numberOfColumnsCaption];
                for (int n = 0; n < numberOfColumnsCaption; n++)
                    excelColumnNamesCaptions[n] = GetExcelColumnName(n);
                
                Row rowCaption = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                //build column names of the report
                Columns columns = new Columns();
                for (int i = 0; i < model.Captions.Count; i++)
                {
                    var caption = model.Captions[i];
                    AppendTextCell(excelColumnNamesCaptions[i] + rowIndex.ToString(), caption.Caption, rowCaption, 2);
                    columns.Append(CreateColumnData((UInt32Value)(uint)i + 1, (UInt32Value)(uint)i + 1, caption.ExcelColumnWidth));
                }
                sheetData1.Append(rowCaption);
                //add the new row with the name of the columns
                worksheet.Append(columns);
                rowIndex = 8;
                //write the data of the report
                foreach (var item in model.DataRows)
                {
                    int numberOfColumnsData = item.FieldValues.Count;
                    //get column names
                    string[] excelColumnNamesData = new string[numberOfColumnsData];
                    for (int n = 0; n < numberOfColumnsData; n++)
                        excelColumnNamesData[n] = GetExcelColumnName(n);

                    //build the data information
                    Row rowData = new Row() { RowIndex = (UInt32Value)(uint)rowIndex };
                    for (int colInx = 0; colInx < numberOfColumnsData; colInx++)
                    {
                        DynamicDataRowValue col = item.FieldValues[colInx];
                        switch (col.FieldType)
                        {
                            case (int)FieldType.Catalogue:
                            case (int)FieldType.RegularExpressionText:
                            case (int)FieldType.Time:
                            case (int)FieldType.SingleTextLine:
                            case (int)FieldType.MultipleTextLine:
                            case (int)FieldType.Datepicker:
                            case (int)FieldType.Boolean:
                            case (int)FieldType.AutoComplete:
                            case (int)FieldType.StatusField:
                                AppendTextCell(excelColumnNamesData[colInx] + rowIndex.ToString(), col.FieldValue, rowData, 1);
                                break;
                            case (int)FieldType.Integer:
                            case (int)FieldType.Decimal:
                                AppendNumberCell(excelColumnNamesData[colInx] + rowIndex.ToString(), col.FieldValue, rowData, 1);
                                break;
                            default:
                                break;
                        }
                    }
                    //add the new row to the report
                    sheetData1.Append(rowData);
                    rowIndex++;
                }

                //add the information of the current sheet
                worksheet.Append(sheetData1);
                //add merged cells
                worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                worksheet.Append(drawing);
                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();

                //create the new sheet for this report 
                Sheet sheet = new Sheet() { Name = model.FormName, SheetId = (UInt32Value)1, Id = workbookPart.GetIdOfPart(worksheetPart) };
                sheets.Append(sheet);
                //add the new sheet to the report
                workbook.Append(sheets);
                //save all report
                workbook.Save();
                //close the stream.
                document.Close();
            }
            return report;
        }
        #endregion

        #endregion

        #region private  methods

        #region GetExcelColumnName
        /// <summary>
        /// Get an excel column
        /// </summary>
        /// <param name="columnIndex">index</param>
        /// <returns></returns>
        private static string GetExcelColumnName(int columnIndex)
        {
            //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
            //
            //  eg  GetExcelColumnName(0) should return "A"
            //      GetExcelColumnName(1) should return "B"
            //      GetExcelColumnName(25) should return "Z"
            //      GetExcelColumnName(26) should return "AA"
            //      GetExcelColumnName(27) should return "AB"
            //      ..etc..
            //
            if (columnIndex < 26)
                return ((char)('A' + columnIndex)).ToString();

            char firstChar = (char)('A' + (columnIndex / 26) - 1);
            char secondChar = (char)('A' + (columnIndex % 26));

            return string.Format("{0}{1}", firstChar, secondChar);
        }
        #endregion

        #region AppendTextCell
        /// <summary>
        /// Append text in a cell
        /// </summary>
        /// <param name="cellReference">Reference</param>
        /// <param name="cellStringValue">Value</param>
        /// <param name="excelRow">Excel row</param>
        /// <param name="styleIndex">Style</param>
        private static void AppendTextCell(string cellReference, string cellStringValue, Row excelRow, UInt32Value styleIndex)
        {
            //  Add a new Excel Cell to our Row 
            Cell cell = new Cell() { CellReference = cellReference, DataType = CellValues.String };
            CellValue cellValue = new CellValue();
            cellValue.Text = cellStringValue;
            cell.StyleIndex = styleIndex;
            cell.Append(cellValue);
            excelRow.Append(cell);
        }
        #endregion

        #region AppendNumberCell
        /// <summary>
        /// Append text in a cell
        /// </summary>
        /// <param name="cellReference">Reference</param>
        /// <param name="cellStringValue">Value</param>
        /// <param name="excelRow">Excel row</param>
        /// <param name="styleIndex">Style</param>
        private static void AppendNumberCell(string cellReference, string cellStringValue, Row excelRow, UInt32Value styleIndex)
        {
            //  Add a new Excel Cell to our Row 
            Cell cell = new Cell() { CellReference = cellReference, DataType = CellValues.Number };
            CellValue cellValue = new CellValue();
            cellValue.Text = cellStringValue;
            cell.StyleIndex = styleIndex;
            cell.Append(cellValue);
            excelRow.Append(cell);
        }
        #endregion

        #region UpdateStringCellValue
        /// <summary>
        /// Update the value of a existing cell
        /// </summary>
        /// <param name="cellReference">Address of the cell</param>
        /// <param name="cellStringValue">Value to update</param>
        /// <param name="excelRow">Row of the cell</param>
        private static void UpdateStringCellValue(string cellReference, string cellStringValue, Row excelRow, UInt32Value styleIndex = null)
        {
            Cell currentCell = excelRow.Elements<Cell>().First(cell => cell.CellReference.Value == cellReference);
            currentCell.CellValue = new CellValue(cellStringValue);
            currentCell.DataType = new EnumValue<CellValues>(CellValues.String);
            if (styleIndex != null)
                currentCell.StyleIndex = styleIndex;
        }
        #endregion

        #region UpdateNumberCellValue
        /// <summary>
        /// Update the value of a existing cell
        /// </summary>
        /// <param name="cellReference">Address of the cell</param>
        /// <param name="cellStringValue">Value to update</param>
        /// <param name="excelRow">Row of the cell</param>
        private static void UpdateNumberCellValue(string cellReference, string cellStringValue, Row excelRow, UInt32Value styleIndex = null)
        {
            Cell currentCell = excelRow.Elements<Cell>().First(cell => cell.CellReference.Value == cellReference);
            currentCell.CellValue = new CellValue(cellStringValue);
            currentCell.DataType = new EnumValue<CellValues>(CellValues.Number);
            if (styleIndex != null)
                currentCell.StyleIndex = styleIndex;
        }
        #endregion

        #region CreateColumnData
        /// <summary>
        /// Create a column and set the height of the column
        /// </summary>
        /// <param name="StartColumnIndex">Initial index</param>
        /// <param name="EndColumnIndex">End index</param>
        /// <param name="ColumnWidth">Column width</param>
        /// <returns></returns>
        private static Column CreateColumnData(UInt32 StartColumnIndex, UInt32 EndColumnIndex, double ColumnWidth)
        {
            Column column;
            column = new Column();
            column.Min = StartColumnIndex;
            column.Max = EndColumnIndex;
            column.Width = ColumnWidth;
            column.CustomWidth = true;
            return column;
        }
        #endregion

        #region VestalisStyleSheet
        /// <summary>
        /// Create an stylesheet to use in excel files
        /// </summary>
        /// <returns></returns>
        private static Stylesheet VestalisStyleSheet()
        {
            Stylesheet styleSheet = new Stylesheet();

            Fonts fonts = new Fonts();
            //normal fonts
            DocumentFormat.OpenXml.Spreadsheet.Font myFornt = new DocumentFormat.OpenXml.Spreadsheet.Font()
            {
                FontSize = new FontSize() { Val = 11 },
                Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                FontName = new FontName() { Val = "Calibri" }
            };
            fonts.Append(myFornt);

            //font bold
            myFornt = new DocumentFormat.OpenXml.Spreadsheet.Font()
            {
                Bold = new Bold(),
                FontSize = new FontSize() { Val = 11 },
                Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                FontName = new FontName() { Val = "Calibri" }
            };
            fonts.Append(myFornt);

            //font title
            myFornt = new DocumentFormat.OpenXml.Spreadsheet.Font()
            {
                Bold = new Bold(),
                FontSize = new FontSize() { Val = 20 },
                Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "FFFFFF" } },
                FontName = new FontName() { Val = "Verdana" }
            };
            fonts.Append(myFornt);

            //font bold
            myFornt = new DocumentFormat.OpenXml.Spreadsheet.Font()
            {
                Bold = new Bold(),
                FontSize = new FontSize() { Val = 16 },
                Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                FontName = new FontName() { Val = "Calibri" }
            };
            fonts.Append(myFornt);

            Fills fills = new Fills();
            //default fill
            Fill fill = new Fill()
            {
               PatternFill =  new PatternFill() { PatternType = PatternValues.None }
            };
            fills.Append(fill);
            //default fill
            fill = new Fill()
            {
                PatternFill = new PatternFill() { PatternType = PatternValues.Gray125 }
            };
            fills.Append(fill);
            //title fill
            fill = new Fill()
            {
                PatternFill = new PatternFill()
                {
                    ForegroundColor = new ForegroundColor()
                    {
                        Rgb = new HexBinaryValue() { Value = "499EB1" }
                    },
                    PatternType = PatternValues.Solid
                }
            };
            fills.Append(fill);

            Borders borders = new Borders();
            //normal borders
            Border border = new Border()
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder(),
                BottomBorder = new BottomBorder(),
                DiagonalBorder = new DiagonalBorder()
            };
            borders.Append(border);

            //borders applied
            border = new Border()
            {
                LeftBorder = new LeftBorder() { Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }, Style = BorderStyleValues.Thin },
                RightBorder = new RightBorder() { Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }, Style = BorderStyleValues.Thin },
                TopBorder = new TopBorder() { Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }, Style = BorderStyleValues.Thin },
                BottomBorder = new BottomBorder() { Color = new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }, Style = BorderStyleValues.Thin },
                DiagonalBorder = new DiagonalBorder()
            };
            borders.Append(border);

            CellFormats cellFormats = new CellFormats();
            //0- normal
            CellFormat cellFormat = new CellFormat()
            {
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //1 - border
            cellFormat = new CellFormat()
            {
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                Alignment = new Alignment() { WrapText = true },
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //2 - border and bold
            cellFormat = new CellFormat()
            {
                FontId = 1,
                FillId = 0,
                BorderId = 1,
                Alignment = new Alignment() { WrapText = true },
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //3 -title
            cellFormat = new CellFormat()
            {
                FontId = 0,
                FillId = 2,
                BorderId = 0,
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //4 - title
            cellFormat = new CellFormat()
            {
                FontId = 2,
                FillId = 2,
                BorderId = 0,
                Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center },
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //5 - title
            cellFormat = new CellFormat()
            {
                FontId = 2,
                FillId = 2,
                BorderId = 0,
                Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.CenterContinuous, Vertical = VerticalAlignmentValues.Center },
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            //6 - section title
            cellFormat = new CellFormat()
            {
                FontId = 3,
                FillId = 0,
                BorderId = 1,                
                ApplyFill = true
            };
            cellFormats.Append(cellFormat);

            styleSheet.Append(fonts);
            styleSheet.Append(fills);
            styleSheet.Append(borders);
            styleSheet.Append(cellFormats);

            return styleSheet;
        }
        #endregion

        #region AddLogo
        /// <summary>
        /// Add the logo of the system
        /// </summary>
        /// <param name="logoPath">Path of the logo</param>
        /// <param name="worksheetPart">Worksheet Part</param>
        /// <returns>Drawing</returns>
        private static Drawing AddLogo(string logoPath, WorksheetPart worksheetPart)
        {
            string sImagePath = logoPath;
            DrawingsPart dp = worksheetPart.AddNewPart<DrawingsPart>();
            ImagePart imgp = dp.AddImagePart(ImagePartType.Png, worksheetPart.GetIdOfPart(dp));
            using (FileStream fs = new FileStream(sImagePath, FileMode.Open, FileAccess.Read))
            {
                imgp.FeedData(fs);
            }

            DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties nvdp = new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties();
            nvdp.Id = 1025;
            nvdp.Name = "Picture 1";
            nvdp.Description = "logo";
            DocumentFormat.OpenXml.Drawing.PictureLocks picLocks = new DocumentFormat.OpenXml.Drawing.PictureLocks();
            picLocks.NoChangeAspect = true;
            picLocks.NoChangeArrowheads = true;
            DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureDrawingProperties nvpdp = new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureDrawingProperties();
            nvpdp.PictureLocks = picLocks;
            DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureProperties nvpp = new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureProperties();
            nvpp.NonVisualDrawingProperties = nvdp;
            nvpp.NonVisualPictureDrawingProperties = nvpdp;

            DocumentFormat.OpenXml.Drawing.Stretch stretch = new DocumentFormat.OpenXml.Drawing.Stretch();
            stretch.FillRectangle = new DocumentFormat.OpenXml.Drawing.FillRectangle();

            DocumentFormat.OpenXml.Drawing.Spreadsheet.BlipFill blipFill = new DocumentFormat.OpenXml.Drawing.Spreadsheet.BlipFill();
            DocumentFormat.OpenXml.Drawing.Blip blip = new DocumentFormat.OpenXml.Drawing.Blip();
            blip.Embed = dp.GetIdOfPart(imgp);
            blip.CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print;
            blipFill.Blip = blip;
            blipFill.SourceRectangle = new DocumentFormat.OpenXml.Drawing.SourceRectangle();
            blipFill.Append(stretch);

            DocumentFormat.OpenXml.Drawing.Transform2D t2d = new DocumentFormat.OpenXml.Drawing.Transform2D();
            DocumentFormat.OpenXml.Drawing.Offset offset = new DocumentFormat.OpenXml.Drawing.Offset();
            offset.X = 0;
            offset.Y = 0;
            t2d.Offset = offset;
            Bitmap bm = new Bitmap(sImagePath);
            DocumentFormat.OpenXml.Drawing.Extents extents = new DocumentFormat.OpenXml.Drawing.Extents();
            extents.Cx = (long)bm.Width * (long)((float)914400 / bm.HorizontalResolution);
            extents.Cy = (long)bm.Height * (long)((float)914400 / bm.VerticalResolution);
            bm.Dispose();
            t2d.Extents = extents;
            DocumentFormat.OpenXml.Drawing.Spreadsheet.ShapeProperties sp = new DocumentFormat.OpenXml.Drawing.Spreadsheet.ShapeProperties();
            sp.BlackWhiteMode = DocumentFormat.OpenXml.Drawing.BlackWhiteModeValues.Auto;
            sp.Transform2D = t2d;
            DocumentFormat.OpenXml.Drawing.PresetGeometry prstGeom = new DocumentFormat.OpenXml.Drawing.PresetGeometry();
            prstGeom.Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle;
            prstGeom.AdjustValueList = new DocumentFormat.OpenXml.Drawing.AdjustValueList();
            sp.Append(prstGeom);
            sp.Append(new DocumentFormat.OpenXml.Drawing.NoFill());

            DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture picture = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture();
            picture.NonVisualPictureProperties = nvpp;
            picture.BlipFill = blipFill;
            picture.ShapeProperties = sp;

            DocumentFormat.OpenXml.Drawing.Spreadsheet.Position pos = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Position();
            pos.X = 18 * 914400 / 72;
            pos.Y = 35 * 914400 / 72;

            Extent ext = new Extent();
            ext.Cx = extents.Cx;
            ext.Cy = extents.Cy;
            AbsoluteAnchor anchor = new AbsoluteAnchor();
            anchor.Position = pos;
            anchor.Extent = ext;


            anchor.Append(picture);
            anchor.Append(new ClientData());
            WorksheetDrawing wsd = new WorksheetDrawing();
            wsd.Append(anchor);
            Drawing drawing = new Drawing();
            drawing.Id = dp.GetIdOfPart(imgp);

            wsd.Save(dp);
            return drawing;
        }
        #endregion

        #endregion

    }
}
