using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Cotecna.Vestalis.Core;
using Cotecna.Vestalis.Entities;
using Cotecna.Vestalis.Web.Common;
using System.Collections.Generic;

namespace Cotecna.Vestalis.Web.Controllers
{
    /// <summary>
    /// This class contains all action and validation methods user for managing the catalogue section
    /// </summary>
    public class CatalogueController : BaseController
    {

        #region Index
        /// <summary>
        /// Display the index page 
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index()
        {
            PaginatedList<CatalogueModel> model = new PaginatedList<CatalogueModel>();
            return View(model);
        }
        #endregion

        #region SearchCatalogueList
        /// <summary>
        /// Perform the search of catalogues
        /// </summary>
        /// <param name="page">Selected page</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <returns>PartialViewResult</returns>
        public PartialViewResult SearchCatalogueList(int? page, Guid? businessApplicationId)
        {
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;
            PaginatedList<CatalogueModel> model = new PaginatedList<CatalogueModel>();

            ParameterSearchCatalogues parameters = new ParameterSearchCatalogues()
            {
                BusinessApplicationId = businessApplicationId.GetValueOrDefault(),
                PageSize = pageSize,
                SortDirection = SortDirection.Ascending,
                SortedColumn = "BusinessApplicationName",
                SelectedPage = page.GetValueOrDefault(),
                UserName = UserName
            };

            model = CatalogueBusiness.SearchCategoryCatalogue(parameters);
            Session.Add("catalogueBusinessApplicationId", businessApplicationId);
            return PartialView("_CatalogueGrid", model);
        }
        #endregion

        #region SearchCatalogueListPaginated

        /// <summary>
        /// Perform the search of catalogues in the pagination buttons
        /// </summary>
        /// <param name="sortedColumn">Sorted column</param>
        /// <param name="page">Selected Page</param>
        /// <param name="sortDirection">Sort direction</param>
        /// <returns>PartialViewResult</returns>
        public PartialViewResult SearchCatalogueListPaginated(SortDirection? sortDirection, string sortedColumn, int? page)
        {
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;
            Guid? businessApplicationId = Session["catalogueBusinessApplicationId"] as Guid?;
            PaginatedList<CatalogueModel> model = new PaginatedList<CatalogueModel>();

            ParameterSearchCatalogues parameters = new ParameterSearchCatalogues()
            {
                BusinessApplicationId = businessApplicationId.GetValueOrDefault(),
                PageSize = pageSize,
                SortDirection = sortDirection.GetValueOrDefault(),
                SortedColumn = sortedColumn,
                SelectedPage = page.GetValueOrDefault(),
                UserName = UserName
            };

            model = CatalogueBusiness.SearchCategoryCatalogue(parameters);
            return PartialView("_CatalogueGrid", model);
        }
        #endregion

        #region SearchCatalogueValueList
        /// <summary>
        /// Display the list of catalogue values
        /// </summary>
        /// <param name="catalogueId">Id of selected catalogue</param>
        /// <returns>ActionResult</returns>
        public PartialViewResult SearchCatalogueValueList(Guid? catalogueId)
        {
            PaginatedList<CatalogueValue> model = new PaginatedList<CatalogueValue>();
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;
            model = CatalogueBusiness.SearchCatalogueValueList(catalogueId.GetValueOrDefault(), pageSize, SortDirection.Ascending, "CatalogueValueData", 1, false);

            Session.Add("catalogueId", catalogueId);
            return PartialView("_CatalogueValueGrid", model);
        }
        #endregion

        #region SearchCatalogueValueListPaginated

        /// <summary>
        /// Perform the search of catalogues values in the pagination buttons
        /// </summary>
        /// <param name="sortedColumn">Sorted column</param>
        /// <param name="page">Selected page</param>
        /// <param name="sortDirection">Sort direction</param>
        /// <returns>PartialViewResult</returns>
        public PartialViewResult SearchCatalogueValueListPaginated(SortDirection? sortDirection, string sortedColumn, int? page)
        {
            PaginatedList<CatalogueValue> model = new PaginatedList<CatalogueValue>();
            Guid? catalogueId = Session["catalogueId"] as Guid?;
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;
            model = CatalogueBusiness.SearchCatalogueValueList(catalogueId.GetValueOrDefault(), pageSize, sortDirection.GetValueOrDefault(), sortedColumn, page.GetValueOrDefault(), false);
            return PartialView("_CatalogueValueGrid", model);
        }
        #endregion

        #region NewCatalogue
        /// <summary>
        /// Call to the view for create a new catalogue
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NewCatalogue()
        {
            CatalogueModel model = new CatalogueModel();
            model.ScreenOpenMode = ScreenOpenMode.Add;
            return View("Catalogue", model);
        }
        #endregion

        #region SaveCatalogue
        /// <summary>
        /// Perform the save operation
        /// </summary>
        /// <param name="model">Catalogue model</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult SaveCatalogue(CatalogueModel model)
        {
            if (model.BusinessApplicationId == null)
            {
                model.Errors.Add("Please select a business application");
            }
            if (string.IsNullOrEmpty(model.CatalogueCategoryName))
            {
                model.Errors.Add("Please write a name for the catalogue");
            }
            if (!model.HasErrors)
            {
                model = CatalogueBusiness.SaveCategoryCatalogue(model, UserName);
                return Json(model);
            }
            else
            {
                return Json(model);
            }
        }
        #endregion

        #region EditCatalogue
        /// <summary>
        /// Peroform the edit operation
        /// </summary>
        /// <param name="catalogueIdEdit">Id of catalogue for edit</param>
        /// <returns>ActionResult</returns>
        public ActionResult EditCatalogue(Guid? catalogueIdEdit)
        {
            Catalogue catalogue = CatalogueBusiness.GetCatalogueCategory(catalogueIdEdit.GetValueOrDefault());
            CatalogueModel model = new CatalogueModel
            {
                BusinessApplicationId = catalogue.BusinessApplicationId,
                CatalogueCategoryName = catalogue.CatalogueCategoryName,
                CatalogueId = catalogue.CatalogueId,
                ScreenOpenMode = ScreenOpenMode.Edit
            };
            return View("Catalogue", model);
        }
        #endregion

        #region DeleteCatalogue
        /// <summary>
        /// Perform the delete operation in each row
        /// </summary>
        /// <param name="catalogueId">Id of catalogue</param>
        /// <param name="businessApplicationId">Id of business application</param>
        public ActionResult DeleteCatalogue(Guid? catalogueId, Guid? businessApplicationId)
        {
           string message = CatalogueBusiness.DeleteCategoryCatalogue(catalogueId.GetValueOrDefault(), 
               businessApplicationId.GetValueOrDefault(), UserName);
           return Json(message);
        }
        #endregion

        #region DeleteSelectedCategories
        /// <summary>
        /// Perform the delete operation of each selected item
        /// </summary>
        /// <param name="selectedIds">Ids of selected catalogues</param>
        /// <returns>ActionResult</returns>
        public ActionResult DeleteSelectedCategories(string selectedIds)
        {
            string[] selectedCategories = selectedIds.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> selectedCatalogueIds = new List<Guid>();
            selectedCategories.ToList().ForEach(id => selectedCatalogueIds.Add(new Guid(id)));
            List<string> result = CatalogueBusiness.DeleteSelectedCategoryCatalogues(selectedCatalogueIds, UserName);
            return Json(result);
        }
        #endregion

        #region NewCatalogueValue
        /// <summary>
        /// Call the view for creating a new catalogue value
        /// </summary>
        /// <param name="catalogueId">Id of category catalogue</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NewCatalogueValue(Guid? catalogueId)
        {
            CatalogueValue model = new CatalogueValue();
            model.CatalogueId = catalogueId.GetValueOrDefault();
            return View("CatalogueValue", model);
        }
        #endregion

        #region SaveCatalogueValue
        /// <summary>
        /// Perform the save operation for the current catalogue value
        /// </summary>
        /// <param name="model">CatalogueValue</param>
        /// <returns>ActionResult</returns>
        public ActionResult SaveCatalogueValue(CatalogueValueModel model)
        {
            if (string.IsNullOrEmpty(model.CatalogValData))
            {
                model.Errors.Add("Please write a valid value");
            }
            if (!model.HasErrors)
            {
                model = CatalogueBusiness.SaveCatalogueValue(model, UserName);
                return Json(model);
            }
            else
            {
                return Json(model);
            }
        }
        #endregion

        #region EditCatalogueValue
        /// <summary>
        /// Get the select catalogue value to edit
        /// </summary>
        /// <param name="catalogueValueId">Id of the selected catalogue value</param>
        /// <returns>ActionResult</returns>
        public ActionResult EditCatalogueValue(Guid? catalogueValueId)
        {
            CatalogueValue model = CatalogueBusiness.GetCatalogueValue(catalogueValueId.GetValueOrDefault());
            return View("CatalogueValue", model);
        }
        #endregion

        #region DeleteCatalogueValue

        /// <summary>
        /// Perform the delete operation for any catalogue value
        /// </summary>
        /// <param name="catalogueValueId">Id of catalogue Value</param>
        /// <param name="businessApplicationId">Id of business application</param>
        public ActionResult DeleteCatalogueValue(Guid? catalogueValueId, Guid? businessApplicationId)
        {
            string message = CatalogueBusiness.DeleteCatalogueValue(catalogueValueId.GetValueOrDefault(), 
                businessApplicationId.GetValueOrDefault(), UserName);
            return Json(message);
        }
        #endregion

        #region DeleteSelectedValues
        /// <summary>
        /// Delete selected values
        /// </summary>
        /// <param name="selectedValues">Ids of selected values</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <returns></returns>
        public ActionResult DeleteSelectedValues(string selectedValues, Guid? businessApplicationId)
        {
            string[] ids = selectedValues.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> valueIds = new List<Guid>();
            ids.ToList().ForEach(id => valueIds.Add(new Guid(id)));
            List<string> result = CatalogueBusiness.DeleteSelectedValues(valueIds, businessApplicationId.GetValueOrDefault(), UserName);
            return Json(result);
        }
        #endregion

        #region ExportExcelCategories

        /// <summary>
        /// Retrieve the list of catalogues for exporting to excel
        /// </summary>
        /// <param name="businessAppId">Id of business application</param>
        public void SearchCatalogueCategoriesExcel(Guid? businessAppId)
        {
            PaginatedList<CatalogueModel> model = new PaginatedList<CatalogueModel>();

            ParameterSearchCatalogues parameters = new ParameterSearchCatalogues()
            {
                BusinessApplicationId = businessAppId.GetValueOrDefault(),
                PageSize = 0,
                SortDirection = SortDirection.Ascending,
                SortedColumn = "BusinessApplicationName",
                SelectedPage = 0,
                IsExport = true,
                UserName = UserName
            };


            model = CatalogueBusiness.SearchCategoryCatalogue(parameters);

            Session.Add("SearchCatalogueCategoriesExcel", model);
        }

        /// <summary>
        /// Export the complete list of catalogues to Microsoft Excel
        /// </summary>
        /// <returns>FileStreamResult</returns>
        [HttpPost]
        public FileStreamResult ExportExcelCategories(Guid? businessAppId)
        {
            PaginatedList<CatalogueModel> model = Session["SearchCatalogueCategoriesExcel"] as PaginatedList<CatalogueModel>;
            Session.Remove("SearchCatalogueCategoriesExcel");
            string logoPath = Server.MapPath("~/Images/logo.png");
            MemoryStream report = ExcelBusiness.GenerateCatalogueReport(model.Collection, logoPath);
            report.Position = 0;
            string currentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            MicrosoftExcelStreamResult result = new MicrosoftExcelStreamResult(report, "CatalogueReport" + currentDateTime + ".xlsx");

            return result;
        }
        #endregion

        #region ExportExcelValues

        /// <summary>
        /// Search the catalogues for exporting to excel
        /// </summary>
        /// <param name="businessApplicatioId">Id of business application</param>
        /// <param name="catalogueSelectedName">Name of the catalogue</param>
        /// <param name="catalogueId">Id of the catalogue</param>
        public void SearchCatalogueValuesExcel(Guid? businessApplicatioId, string catalogueSelectedName, Guid? catalogueId)
        {
            CatalogueValueSearchModel model = new CatalogueValueSearchModel();
            PaginatedList<CatalogueValue> searchResult = new PaginatedList<CatalogueValue>();
            searchResult = CatalogueBusiness.SearchCatalogueValueList(catalogueId.GetValueOrDefault(), 0, SortDirection.Ascending, "CatalogueValueData", 0, true);
            string businessApplicatioName = string.Empty;

            businessApplicatioName = AuthorizationBusiness.GetAllBusinessAplications()
                .First(data => data.BusinessApplicationId == businessApplicatioId.GetValueOrDefault()).BusinessApplicationName;

            model.CatalogueSelectedId = catalogueId.GetValueOrDefault();
            model.SearchResult = searchResult;
            model.CatalogueSelectedName = CatalogueBusiness.GetCatalogueCategory(catalogueId.Value).CatalogueCategoryName;
            model.BusinessApplicatioName = businessApplicatioName;

            Session.Add("SearchCatalogueValuesExcel", model);
        }


        /// <summary>
        /// Export to excel all catalogue values
        /// </summary>
        /// <returns>FileStreamResult</returns>
        [HttpPost]
        public FileStreamResult ExportExcelValues()
        {
            CatalogueValueSearchModel model = Session["SearchCatalogueValuesExcel"] as CatalogueValueSearchModel;
            Session.Remove("SearchCatalogueValuesExcel");
            string logoPath = Server.MapPath("~/Images/logo.png");
            MemoryStream report = ExcelBusiness.GenerateCatalogueValueReport(model, logoPath);
            report.Position = 0;
            string currentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            MicrosoftExcelStreamResult result = new MicrosoftExcelStreamResult(report, "CatalogueValue" + currentDateTime + ".xlsx");
            return result;
        }
        #endregion
    }
}
