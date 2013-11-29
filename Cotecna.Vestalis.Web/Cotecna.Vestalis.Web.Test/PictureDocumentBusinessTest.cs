using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cotecna.Vestalis.Web.Test
{


    /// <summary>
    ///This is a test class for PictureDocumentBusinessTest and is intended
    ///to contain all PictureDocumentBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PictureDocumentBusinessTest
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
        ///A test for SearchPictures
        ///</summary>
        [TestMethod()]
        public void SearchPicturesTest()
        {
            //FormCollection formCollection = new FormCollection();

            //List<BusinessApplicationByUser> businessApplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser("coordinator");
            //Guid businessApplicationId = businessApplicationsByUser.FirstOrDefault().Id;

            //DynamicDataGrid serviceOrderList = ServiceOrderBusiness.SearchOrderList(formCollection, businessApplicationId, null, 0, 0, false);
            //DynamicDataRow dynamicDataRow = serviceOrderList.DataRows.FirstOrDefault();
            //Guid? serviceOrderId = dynamicDataRow.RowIdentifier;

            //int pageSize = 20;
            //int selectedPage = 1;
            //bool isClient = false;
            //PictureSearchModel expected = null; // TODO: Initialize to an appropriate value
            //PictureSearchModel actual;
            //actual = PictureDocumentBusiness.SearchPictures(serviceOrderId.GetValueOrDefault(), pageSize, selectedPage, isClient);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
