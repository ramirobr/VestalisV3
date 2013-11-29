using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Cotecna.Vestalis.Entities;
using Cotecna.Vestalis.Core.Resources;
using System.Web.Security;
using Cotecna.Vestalis.Core.DynamicForm;

namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class contains all methods for manage all about catalogues
    /// </summary>
    public static class CatalogueBusiness
    {

        #region methods

        #region GetCatalogueList
        /// <summary>
        /// Get the list of catalogues
        /// </summary>
        /// <param name="catalogueName">Name of the catalogue</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <returns>List of CatalogueValue</returns>
        public static IList<CatalogueValue> GetCatalogueList(string catalogueName, Guid businessApplicationId)
        {
            using (VestalisEntities ctx = new VestalisEntities())
            {
                return (from catalogueValues in ctx.CatalogueValues
                        where
                            catalogueValues.Catalogue.CatalogueCategoryName == catalogueName &&
                            catalogueValues.IsDeleted == false && catalogueValues.Catalogue.BusinessApplicationId == businessApplicationId
                        select catalogueValues).ToList();
            }
        }
        #endregion

        #region DeleteCatalogueValue

        /// <summary>
        /// Delete a value from a catalog
        /// </summary>
        /// <param name="catalogValueId">id of catalog value</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <param name="userName">The name of the user</param>
        public static string DeleteCatalogueValue(Guid catalogValueId, Guid businessApplicationId, string userName)
        {
            using (VestalisEntities context = new VestalisEntities())
            {
                bool existValueServiceOrder = false;
                bool existValueInspectionReport = false;
                
                VerifyCatalogueValue(catalogValueId, businessApplicationId, context, ref existValueServiceOrder, ref existValueInspectionReport);

                CatalogueValue catalogueValueDeleted = context.CatalogueValues.FirstOrDefault(data => data.IsDeleted == false && data.CatalogueValueId == catalogValueId);
                if (!existValueServiceOrder && !existValueInspectionReport)
                {
                    
                    //if the current catalogue is not null, the system continues with the process
                    if (catalogueValueDeleted != null)
                    {
                        //set IsDeleted in true
                        catalogueValueDeleted.IsDeleted = true;
                        //set auditory fields
                        catalogueValueDeleted.ModificationBy = userName;
                        catalogueValueDeleted.ModificationDate = DateTime.UtcNow;
                    }
                    context.SaveChanges();
                    return string.Empty;
                }
                else
                {
                    return string.Format(LanguageResource.CatalogueValueNonDeleteMultipleMessage, catalogueValueDeleted.CatalogueValueData);
                }
            }
        }

        
        /// <summary>
        /// Verify if the system might delete the current catalogue value.
        /// </summary>
        /// <param name="catalogValueId">Id of catalogue value</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <param name="context">Database context</param>
        /// <param name="existValueServiceOrder">Exists in service order</param>
        /// <param name="existValueInspectionReport">Exists in inspection report</param>
        private static void VerifyCatalogueValue(Guid catalogValueId, Guid businessApplicationId, VestalisEntities context, ref bool existValueServiceOrder, ref bool existValueInspectionReport)
        {
            //verify if exists in any service orders
            existValueServiceOrder = (from catalogueValue in context.CatalogueValues
                                      join catalogue in context.Catalogues on catalogueValue.CatalogueId equals catalogue.CatalogueId
                                      join formValue in context.FormValues on catalogueValue.CatalogueValueId equals formValue.CatalogueValueId
                                      join serviceOrder in context.ServiceOrders on formValue.ServiceOrderId equals serviceOrder.ServiceOrderId
                                      where catalogueValue.IsDeleted == false && catalogueValue.CatalogueValueId == catalogValueId
                                      && catalogue.IsDeleted == false
                                      && formValue.IsDeleted == false && formValue.InspectionReportItemId == null
                                      && serviceOrder.IsDeleted == false && serviceOrder.BusinessApplicationId == businessApplicationId
                                      select formValue.CatalogueValueId).Any();

            //verify if exists in any inspection report
            existValueInspectionReport = (from catalogueValue in context.CatalogueValues
                                          join catalogue in context.Catalogues on catalogueValue.CatalogueId equals catalogue.CatalogueId
                                          join formValue in context.FormValues on catalogueValue.CatalogueValueId equals formValue.CatalogueValueId
                                          join inspectionReportItem in context.InspectionReportItems on formValue.InspectionReportItemId equals inspectionReportItem.InspectionReportItemId
                                          join inspectionReport in context.InspectionReports on inspectionReportItem.InspectionReportId equals inspectionReport.InspectionReportId
                                          join serviceOrder in context.ServiceOrders on inspectionReport.ServiceOrderId equals serviceOrder.ServiceOrderId
                                          where catalogueValue.IsDeleted == false && catalogueValue.CatalogueValueId == catalogValueId
                                          && formValue.IsDeleted == false && formValue.InspectionReportItemId != null
                                          && inspectionReportItem.IsDeleted == false
                                          && inspectionReport.IsDeleted == false
                                          && serviceOrder.IsDeleted == false
                                          && catalogue.IsDeleted == false && catalogue.BusinessApplicationId == businessApplicationId
                                          select formValue.CatalogueValueId).Any();
        }
        #endregion

        #region DeleteSelectedValues
        /// <summary>
        /// Delete selected values
        /// </summary>
        /// <param name="valueIds">Ids of selected values</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <param name="userName">The name of the user</param>
        /// <returns>List of string</returns>
        public static List<string> DeleteSelectedValues(List<Guid> valueIds, Guid businessApplicationId, string userName)
        {
            List<string> result = new List<string>();
            foreach (Guid id in valueIds)
            {
                string temp = DeleteCatalogueValue(id, businessApplicationId, userName);
                if (!string.IsNullOrEmpty(temp))
                    result.Add(temp);
            }
            return result;
        }
        #endregion

        #region DeleteCategoryCatalogue
        /// <summary>
        /// Delete a catalogue 
        /// </summary>
        /// <param name="catalogueId">id of catalog</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <param name="userName">Name of logged user</param>
        public static string DeleteCategoryCatalogue(Guid catalogueId,Guid businessApplicationId, string userName)
        {
            Catalogue catalogueToDelete = null;
            string message = string.Empty;
            using (VestalisEntities context = new VestalisEntities())
            {
                catalogueToDelete = context.Catalogues.FirstOrDefault(data => data.IsDeleted == false && data.CatalogueId == catalogueId);
                if (catalogueToDelete != null)
                {
                    //Get the fields definition for the business application
                    Fields fieldBusinessApplication = CacheHandler.Get(String.Format("Field{0}", businessApplicationId),
                                                    () => DynamicFormEngine.GetFields(businessApplicationId));

                    List<FieldsCatalogueField> catalogueFields = fieldBusinessApplication.Items
                        .Where(data => data is FieldsCatalogueField)
                        .Cast<FieldsCatalogueField>()
                        .ToList();


                    if (!catalogueFields.Any(data => data.CatalogueName == catalogueToDelete.CatalogueCategoryName))
                    {
                        catalogueToDelete.IsDeleted = true;
                        catalogueToDelete.ModificationBy = userName;
                        catalogueToDelete.ModificationDate = DateTime.UtcNow;
                        context.SaveChanges();
                    }
                    else
                    {
                        message = string.Format(Resources.LanguageResource.CatalogueCategoryNonDeleteMultipleMessage, 
                            catalogueToDelete.CatalogueCategoryName);
                    }
                }
            }
            return message;
        }
        #endregion

        #region DeleteSelectedCategoryCatalogues
        /// <summary>
        /// Delete selected categories
        /// </summary>
        /// <param name="selectedIds">Ids of selected categories</param>
        /// <param name="userName">The name of the current user</param>
        /// <returns>List of string</returns>
        public static List<string> DeleteSelectedCategoryCatalogues(List<Guid> selectedIds, string userName)
        {
            Guid? businessAppId = Guid.Empty;
            List<string> result = new List<string>();
            using (VestalisEntities context = new VestalisEntities())
            {
                foreach (Guid id in selectedIds)
                {
                    businessAppId = context.Catalogues.FirstOrDefault(category => category.CatalogueId == id).BusinessApplicationId;
                    string temp = DeleteCategoryCatalogue(id, businessAppId.GetValueOrDefault(), userName);
                    if (!string.IsNullOrEmpty(temp))
                        result.Add(temp);
                }
            }
            return result;
        }
        #endregion

        #region GetCatalogueCategory
        /// <summary>
        /// Get a catalogue category by id
        /// </summary>
        /// <param name="catalogueId">id of catalogue category</param>
        /// <returns>Catalogue</returns>
        public static Catalogue GetCatalogueCategory(Guid catalogueId)
        {
            Catalogue result = null;
            using (VestalisEntities context = new VestalisEntities())
            {
                result = context.Catalogues
                    .FirstOrDefault(data => data.IsDeleted == false && data.CatalogueId == catalogueId) ?? new Catalogue();
            }
            return result;
        }
        #endregion

        #region GetCatalogueDescription
        /// <summary>
        /// Get the catalogue description
        /// </summary>
        /// <param name="catalogValueId">id of catalogue value</param>
        /// <returns>CatalogueValue</returns>
        public static string GetCatalogueDescription(Guid catalogValueId)
        {
            CatalogueValue result = GetCatalogueValue(catalogValueId);

            return result.CatalogueValueDescription;
        }
        #endregion

        #region GetCatalogueValue
        /// <summary>
        /// Get a catalogue value by id
        /// </summary>
        /// <param name="catalogValueId">id of catalogue value</param>
        /// <returns>CatalogueValue</returns>
        public static CatalogueValue GetCatalogueValue(Guid catalogValueId)
        {
            CatalogueValue result = null;
            using (VestalisEntities context = new VestalisEntities())
            {
                result = context.CatalogueValues.FirstOrDefault(data => data.CatalogueValueId == catalogValueId && data.IsDeleted == false);
            }

            return result;
        }
        #endregion

        #region SearchCatalogueValueList

        /// <summary>
        /// Get the list of values of a specific catalogue
        /// </summary>
        /// <param name="catalogueId">id of catalogue</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="sortDirection">Sort direction</param>
        /// <param name="sortedColumn">Column for sorting</param>
        /// <param name="selectedPage">Selected page</param>
        /// <param name="isExport">Is or not export</param>
        /// <returns>list of CatalogueValue</returns>
        public static PaginatedList<CatalogueValue> SearchCatalogueValueList(Guid catalogueId, int pageSize, SortDirection sortDirection, string sortedColumn, int selectedPage, bool isExport)
        {
            List<CatalogueValue> tempResult = null;
            PaginatedList<CatalogueValue> finalResult = new PaginatedList<CatalogueValue>();
            int currentIndex = (selectedPage - 1) * pageSize;
            int countElements = 0;
            using (VestalisEntities context = new VestalisEntities())
            {
                //Retrieve the list of catalogue values.
                countElements = context.CatalogueValues.Where(data => data.CatalogueId == catalogueId && data.IsDeleted == false).Count();

                var queryTempResult = (from catalogueValue in context.CatalogueValues
                                       where catalogueValue.CatalogueId == catalogueId && catalogueValue.IsDeleted == false
                                       select catalogueValue);

                if (!isExport)
                {
                    //order the result
                    tempResult = (sortDirection == SortDirection.Ascending
                                           ? queryTempResult.OrderBy(ExtensionMethods.GetField<CatalogueValue>(sortedColumn))
                                           : queryTempResult.OrderByDescending(ExtensionMethods.GetField<CatalogueValue>(sortedColumn)))
                                           .Skip(currentIndex).Take(pageSize).ToList();
                }
                else
                {
                    //order the result
                    tempResult = (sortDirection == SortDirection.Ascending
                                           ? queryTempResult.OrderBy(ExtensionMethods.GetField<CatalogueValue>(sortedColumn))
                                           : queryTempResult.OrderByDescending(ExtensionMethods.GetField<CatalogueValue>(sortedColumn))).ToList();
                }

                //if exists data, the system makes the pagination
                if (tempResult != null)
                {
                    finalResult.SortDirection = sortDirection;
                    finalResult.SortedColumn = sortedColumn;
                    //set the paginated colletion
                    finalResult.Collection = tempResult;
                    //set the quantity of elements without pagination
                    finalResult.TotalCount = countElements;
                    //set the number of pages
                    finalResult.NumberOfPages = (int)Math.Ceiling((double)finalResult.TotalCount / (double)pageSize);
                    //set the current page
                    finalResult.Page = selectedPage;
                    //set the page size
                    finalResult.PageSize = pageSize;
                }
            }
            return finalResult;
        }
        #endregion

        #region SaveCatalogueValue
        /// <summary>
        /// Save a new catalogue in the database
        /// </summary>
        /// <param name="model">catalogueValue</param>
        /// <param name="userName">The name of the user</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static CatalogueValueModel SaveCatalogueValue(CatalogueValueModel model, string userName)
        {
            using (VestalisEntities context = new VestalisEntities())
            {

                bool existCatalogueValue = context.CatalogueValues.Any(data => data.IsDeleted == false
                        && data.CatalogueId == model.CatalogueId
                        && data.CatalogueValueData == model.CatalogValData);


                //if the CatalogueValueId is empty, the system will perform the insert operation
                //otherwise the system will perform the edit operation
                if (model.CatalogValId == null)
                {
                    //if the current catalogue is not exists, the system continues with the process
                    if (!existCatalogueValue)
                    {
                        CatalogueValue catalogueValue = new CatalogueValue();
                        catalogueValue.CatalogueValueData = model.CatalogValData;
                        catalogueValue.CatalogueValueDescription = model.CatalogDesc;
                        catalogueValue.CatalogueId = model.CatalogueId.GetValueOrDefault();
                        //set  auditory fields
                        catalogueValue.CreationBy = userName;
                        catalogueValue.CreationDate = DateTime.UtcNow;
                        //add the new object
                        context.CatalogueValues.AddObject(catalogueValue);
                        //save all changes
                        context.SaveChanges();
                    }
                    else
                    {
                        model.Errors.Add(LanguageResource.CatalogueValueExists);
                    }

                }
                else
                {
                    UpdateCatalogValue(model, userName, context, existCatalogueValue);
                }

                //Remove from the cache the catalogue
                Catalogue catalogue = GetCatalogueCategory(model.CatalogueId.GetValueOrDefault());
                CacheHandler.Remove(String.Format("{0}{1}", catalogue.CatalogueCategoryName, catalogue.BusinessApplicationId));


            }
            return model;
        }

        private static void UpdateCatalogValue(CatalogueValueModel model, string userName, VestalisEntities context, bool existCatalogueValue)
        {
            //get the current catalogue
            CatalogueValue catalogueValueModified = context.CatalogueValues.FirstOrDefault(data => data.IsDeleted == false && data.CatalogueValueId == model.CatalogValId);
            //if the current catalogue is not null, the system continues with the process
            if (catalogueValueModified != null && catalogueValueModified.CatalogueValueData == model.CatalogValData)
            {
                //set the fields to update the information
                catalogueValueModified.CatalogueValueData = model.CatalogValData;
                catalogueValueModified.CatalogueValueDescription = model.CatalogDesc;
                //set auditory fields
                catalogueValueModified.ModificationBy = userName;
                catalogueValueModified.ModificationDate = DateTime.UtcNow;
                //save all changes
                context.SaveChanges();
            }
            else if (catalogueValueModified != null && catalogueValueModified.CatalogueValueData != model.CatalogValData)
            {
                if (!existCatalogueValue)
                {
                    //set the fields to update the information
                    catalogueValueModified.CatalogueValueData = model.CatalogValData;
                    catalogueValueModified.CatalogueValueDescription = model.CatalogDesc;
                    //set auditory fields
                    catalogueValueModified.ModificationBy = userName;
                    catalogueValueModified.ModificationDate = DateTime.UtcNow;
                    //save all changes
                    context.SaveChanges();
                }
                else
                {
                    model.Errors.Add(LanguageResource.CatalogueValueExists);
                }
            }
        }
        #endregion

        #region SaveCategoryCatalogue
        /// <summary>
        /// Save a new catalogue in the database
        /// </summary>
        /// <param name="model">Catalogue model</param>
        /// <param name="userName">Name of logged user</param>
        public static CatalogueModel SaveCategoryCatalogue(CatalogueModel model, string userName)
        {
            bool categoryExist = false;
            using (VestalisEntities context = new VestalisEntities())
            {

                categoryExist = context.Catalogues
                    .Any(data => data.IsDeleted == false && data.BusinessApplicationId == model.BusinessApplicationId
                        && data.CatalogueCategoryName == model.CatalogueCategoryName);


                //if the entity don't have a catalogue id, the system will perform insert operation
                //otherwise, the system will perform edit operation
                if (model.CatalogueId == Guid.Empty)
                {
                    if (!categoryExist)
                    {
                        Catalogue catalogue = new Catalogue
                        {
                            BusinessApplicationId = model.BusinessApplicationId,
                            CatalogueCategoryName = model.CatalogueCategoryName,
                        };
                        //set auditory fields
                        catalogue.CreationBy = userName;
                        catalogue.CreationDate = DateTime.UtcNow;
                        context.Catalogues.AddObject(catalogue);

                        //save changes
                        context.SaveChanges();
                        model.CatalogueId = catalogue.CatalogueId;
                    }
                    else
                    {
                        model.Errors.Add(LanguageResource.CatalogueCategoryExists);
                    }
                }
                else
                {
                    UpdateCatalogueCategory(model, userName, categoryExist, context);
                }

            }
            return model;
        }

        private static void UpdateCatalogueCategory(CatalogueModel model, string userName, bool categoryExist, VestalisEntities context)
        {
            //get the current catalogue to edit
            Catalogue CatalogueModified = context.Catalogues
            .FirstOrDefault(data => data.IsDeleted == false && data.CatalogueId == model.CatalogueId);

            if (CatalogueModified != null && CatalogueModified.CatalogueCategoryName == model.CatalogueCategoryName)
            {
                //set fields to modify the information
                CatalogueModified.BusinessApplicationId = model.BusinessApplicationId;
                CatalogueModified.CatalogueCategoryName = model.CatalogueCategoryName;
                //set auditory fields
                CatalogueModified.ModificationBy = userName;
                CatalogueModified.ModificationDate = DateTime.UtcNow;
                //save changes
                context.SaveChanges();
            }
            else if (CatalogueModified != null && CatalogueModified.CatalogueCategoryName != model.CatalogueCategoryName)
            {
                if (!categoryExist)
                {
                    //set fields to modify the information
                    CatalogueModified.BusinessApplicationId = model.BusinessApplicationId;
                    CatalogueModified.CatalogueCategoryName = model.CatalogueCategoryName;
                    //set auditory fields
                    CatalogueModified.ModificationBy = userName;
                    CatalogueModified.ModificationDate = DateTime.UtcNow;
                    //save changes
                    context.SaveChanges();
                }
                else
                {
                    model.Errors.Add(LanguageResource.CatalogueCategoryExists);
                }
            }
        }
        #endregion

        #region SearchCategoryCatalogue
        /// <summary>
        /// Retrieve the list of catalogues by business application
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <returns>List of Catalogue</returns>
        public static PaginatedList<CatalogueModel> SearchCategoryCatalogue(ParameterSearchCatalogues parameters)
        {
            PaginatedList<CatalogueModel> finalResult = new PaginatedList<CatalogueModel>();
            List<CatalogueModel> temResult = new List<CatalogueModel>();
            int currentIndex = (parameters.SelectedPage - 1) * parameters.PageSize;
            int countElements = 0;
            using (VestalisEntities context = new VestalisEntities())
            {
                //if the user doesn't select any business application, the system will retrieve the completed list of catalogues
                //otherwise the system will retrieve the list of catalogues filtered by business application
                if (parameters.BusinessApplicationId != Guid.Empty)
                {
                    GetCataloguesByBusinessAppId(parameters, ref temResult, currentIndex, ref countElements, context);
                }
                else
                {
                    if (Roles.IsUserInRole(parameters.UserName, "GlobalAdministrator"))
                    {
                        GetAllCataloguesGlobalAdmin(parameters, ref temResult, currentIndex, ref countElements, context);
                    }
                    else
                    {
                        List<BusinessApplicationByUser> businessAplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser(parameters.UserName);
                        string[] roles = Roles.GetRolesForUser(parameters.UserName);
                        List<Guid?> businessAppsIds = new List<Guid?>();
                        foreach (var item in businessAplicationsByUser)
                        {
                            string localAdminRoles = string.Format("ApplicationAdministrator_{0}", item.Prefix);
                            if (roles.Contains(localAdminRoles))
                                businessAppsIds.Add(item.Id);
                        }

                        GetCataloguesByBusinessAppId(parameters, businessAppsIds, ref temResult, currentIndex, ref countElements, context);
                    }
                }

                //if exists results, the system makes the pagination
                if (temResult != null)
                {

                    finalResult.SortDirection = parameters.SortDirection;
                    finalResult.SortedColumn = parameters.SortedColumn;
                    //set the paginated colletion
                    finalResult.Collection = temResult;
                    //set the quantity of elements without pagination
                    finalResult.TotalCount = countElements;
                    //set the number of pages
                    finalResult.NumberOfPages = (int)Math.Ceiling((double)finalResult.TotalCount / (double)parameters.PageSize);
                    //set the current page
                    finalResult.Page = parameters.SelectedPage;
                    //set the page size
                    finalResult.PageSize = parameters.PageSize;
                }

            }
            return finalResult;
        }


        /// <summary>
        /// Retrieve the list of catalogues by business application ids
        /// </summary>
        /// <param name="parameters">Search parameters</param>
        /// <param name="businessApplicationIds">List of business application ids</param>
        /// <param name="temResult">Result</param>
        /// <param name="currentIndex">Current page index</param>
        /// <param name="countElements">Total elements</param>
        /// <param name="context">Database context</param>
        private static void GetCataloguesByBusinessAppId(ParameterSearchCatalogues parameters,List<Guid?> businessApplicationIds, ref List<CatalogueModel> temResult, int currentIndex, ref int countElements, VestalisEntities context)
        {
            countElements = (from catalogue in context.Catalogues
                             where catalogue.IsDeleted == false && businessApplicationIds.Contains(catalogue.BusinessApplicationId) 
                             select catalogue).Count();

            var filterTemResult = (from catalogue in context.Catalogues.Include("BusinessApplication")
                                   where catalogue.IsDeleted == false && businessApplicationIds.Contains(catalogue.BusinessApplicationId) 
                                   orderby catalogue.CatalogueCategoryName
                                   select new CatalogueModel
                                   {
                                       BusinessApplicationName = catalogue.BusinessApplication.BusinessApplicationName,
                                       CatalogueId = catalogue.CatalogueId,
                                       CatalogueCategoryName = catalogue.CatalogueCategoryName
                                   });

            if (!parameters.IsExport)
            {
                //order the result
                temResult = (parameters.SortDirection == SortDirection.Ascending
                                       ? filterTemResult.OrderBy(ExtensionMethods.GetField<CatalogueModel>(parameters.SortedColumn))
                                       : filterTemResult.OrderByDescending(ExtensionMethods.GetField<CatalogueModel>(parameters.SortedColumn)))
                                       .Skip(currentIndex).Take(parameters.PageSize).ToList();
            }
            else
            {
                temResult = filterTemResult.ToList();
            }
        }

        
        /// <summary>
        /// Retrieve the list of catalogues by business application id
        /// </summary>
        /// <param name="parameters">Search parameters</param>
        /// <param name="temResult">Result</param>
        /// <param name="currentIndex">Current page index</param>
        /// <param name="countElements">Total elements</param>
        /// <param name="context">Database context</param>
        private static void GetCataloguesByBusinessAppId(ParameterSearchCatalogues parameters, ref List<CatalogueModel> temResult, int currentIndex, ref int countElements, VestalisEntities context)
        {
            countElements = (from catalogue in context.Catalogues
                             where catalogue.IsDeleted == false && catalogue.BusinessApplicationId == parameters.BusinessApplicationId
                             select catalogue).Count();

            var filterTemResult = (from catalogue in context.Catalogues.Include("BusinessApplication")
                                   where catalogue.IsDeleted == false && catalogue.BusinessApplicationId == parameters.BusinessApplicationId
                                   orderby catalogue.CatalogueCategoryName
                                   select new CatalogueModel
                                   {
                                       BusinessApplicationName = catalogue.BusinessApplication.BusinessApplicationName,
                                       CatalogueId = catalogue.CatalogueId,
                                       CatalogueCategoryName = catalogue.CatalogueCategoryName
                                   });
            if (parameters.IsExport)
            {
                //order the result
                temResult = (parameters.SortDirection == SortDirection.Ascending
                                       ? filterTemResult.OrderBy(ExtensionMethods.GetField<CatalogueModel>(parameters.SortedColumn))
                                       : filterTemResult.OrderByDescending(ExtensionMethods.GetField<CatalogueModel>(parameters.SortedColumn))).ToList();
            }
            else
            {
                //order the result
                temResult = (parameters.SortDirection == SortDirection.Ascending
                                       ? filterTemResult.OrderBy(ExtensionMethods.GetField<CatalogueModel>(parameters.SortedColumn))
                                       : filterTemResult.OrderByDescending(ExtensionMethods.GetField<CatalogueModel>(parameters.SortedColumn)))
                                       .Skip(currentIndex).Take(parameters.PageSize).ToList();
            }

        }

        /// <summary>
        /// Retrieve the list of catalogues for all business applications
        /// </summary>
        /// <param name="parameters">Search parameters</param>
        /// <param name="temResult">Result</param>
        /// <param name="currentIndex">Current page index</param>
        /// <param name="countElements">Total elements</param>
        /// <param name="context">Database context</param>
        private static void GetAllCataloguesGlobalAdmin(ParameterSearchCatalogues parameters, ref List<CatalogueModel> temResult, int currentIndex, ref int countElements, VestalisEntities context)
        {
            var tempQueryResults = (from catalogue in context.Catalogues.Include("BusinessApplication")
                                    where catalogue.IsDeleted == false
                                    select new CatalogueModel
                                    {
                                        BusinessApplicationName = catalogue.BusinessApplication.BusinessApplicationName,
                                        CatalogueId = catalogue.CatalogueId,
                                        CatalogueCategoryName = catalogue.CatalogueCategoryName
                                    }).ToList();

            var groupResults = (from resultTemp in tempQueryResults
                                group resultTemp by resultTemp.BusinessApplicationName into resultGroup
                                select resultGroup);


            foreach (var item in groupResults.OrderBy(data => data.Key).AsEnumerable())
            {
                temResult.AddRange(item.OrderBy(data => data.CatalogueCategoryName).AsEnumerable());
            }

            countElements = temResult.Count;

            temResult = (parameters.SortDirection == SortDirection.Ascending
                                   ? temResult.OrderBy(ExtensionMethods.GetField<CatalogueModel>(parameters.SortedColumn))
                                   : temResult.OrderByDescending(ExtensionMethods.GetField<CatalogueModel>(parameters.SortedColumn))).ToList();

            if (!parameters.IsExport)
                temResult = temResult.Skip(currentIndex).Take(parameters.PageSize).ToList();
        }

        #endregion


        #endregion
    }
}
