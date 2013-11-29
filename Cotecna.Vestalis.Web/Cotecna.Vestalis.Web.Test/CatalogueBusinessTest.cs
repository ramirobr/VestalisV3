using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cotecna.Vestalis.Web.Test
{


    /// <summary>
    ///This is a test class for CatalogueBusinessTest and is intended
    ///to contain all CatalogueBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CatalogueBusinessTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetCatalogueValue
        ///</summary>
        [TestMethod()]
        public void GetCatalogueValueTest()
        {

            //    List<BusinessApplicationByUser> businessApplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser("adminGlobal");
            //    Guid businessApplicationId = businessApplicationsByUser.FirstOrDefault().Id;
            //    List<Catalogue> catalogues = CatalogueBusiness.SearchCategoryCatalogue(businessApplicationId);
            //    Guid catalogueId = catalogues.FirstOrDefault().CatalogueId;
            //    List<CatalogueValue> catalogueValues = CatalogueBusiness.GetCatalogueValueList(catalogueId);

            //    Guid catalogValueId = catalogueValues.FirstOrDefault().CatalogueValueId;
            //    CatalogueValue expected = null;
            //    CatalogueValue actual;
            //    actual = CatalogueBusiness.GetCatalogueValue(catalogValueId);
            //    Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetCatalogueList
        ///</summary>
        [TestMethod()]
        public void GetCatalogueListTest()
        {
            //List<BusinessApplicationByUser> businessApplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser("adminGlobal");
            //Guid businessApplicationId = businessApplicationsByUser.FirstOrDefault().Id;
            //List<Catalogue> catalogues = CatalogueBusiness.SearchCategoryCatalogue(businessApplicationId);
            //string catalogueName = catalogues.FirstOrDefault().CatalogueCategoryName;

            //IList<CatalogueValue> expected = null;
            //IList<CatalogueValue> actual;
            //actual = CatalogueBusiness.GetCatalogueList(catalogueName, businessApplicationId);
            //Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetCatalogueDescription
        ///</summary>
        [TestMethod()]
        public void GetCatalogueDescriptionTest()
        {
            //List<BusinessApplicationByUser> businessApplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser("adminGlobal");

            //Guid businessApplicationId = businessApplicationsByUser.FirstOrDefault().Id;
            //List<Catalogue> catalogues = CatalogueBusiness.SearchCategoryCatalogue(businessApplicationId);
            //Guid catalogueId = catalogues.FirstOrDefault().CatalogueId;
            //List<CatalogueValue> catalogueValues = CatalogueBusiness.GetCatalogueValueList(catalogueId);

            //Guid catalogValueId = catalogueValues.FirstOrDefault().CatalogueValueId;
            //string expected = string.Empty;
            //string actual = CatalogueBusiness.GetCatalogueDescription(catalogValueId);
            //Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetCatalogueValueList
        ///</summary>
        [TestMethod()]
        public void GetCatalogueValueListTest()
        {
            //List<BusinessApplicationByUser> businessApplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser("adminGlobal");

            //Guid businessApplicationId = businessApplicationsByUser.FirstOrDefault().Id;
            //List<Catalogue> catalogues = CatalogueBusiness.SearchCategoryCatalogue(businessApplicationId);

            //Guid catalogueId = catalogues.FirstOrDefault().CatalogueId;
            //List<CatalogueValue> expected = null;
            //List<CatalogueValue> actual;
            //actual = CatalogueBusiness.GetCatalogueValueList(catalogueId);
            //Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///A test for SearchCategoryCatalogue
        ///</summary>
        [TestMethod()]
        public void SearchCategoryCatalogueTest()
        {

            //List<BusinessApplicationByUser> businessApplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser("adminGlobal");
            //Guid businessApplicationId = businessApplicationsByUser.FirstOrDefault().Id;
            //List<Catalogue> expected = null;
            //List<Catalogue> actual;
            //actual = CatalogueBusiness.SearchCategoryCatalogue(businessApplicationId);
            //Assert.AreNotEqual(expected, actual);
        }
    }
}
