using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cotecna.Vestalis.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cotecna.Vestalis.Web.Test
{


    /// <summary>
    ///This is a test class for ServiceOrderBusinessTest and is intended
    ///to contain all ServiceOrderBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ServiceOrderBusinessTest
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
        ///A test for GetServiceOrderGridDefinition
        ///</summary>
        [TestMethod()]
        public void GetServiceOrderGridDefinitionTest()
        {
            List<BusinessApplicationByUser> businessApplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser("adminGlobal");
            Guid businessApplicationId = businessApplicationsByUser.FirstOrDefault().Id;
            DynamicDataGrid expected = null;
            DynamicDataGrid actual;
            actual = ServiceOrderBusiness.GetServiceOrderGridDefinition(businessApplicationId, true);
            Assert.AreNotEqual(expected, actual);

        }

        /// <summary>
        ///A test for GetServiceOrderForm
        ///</summary>
        [TestMethod()]
        public void GetServiceOrderFormTest()
        {
            List<BusinessApplicationByUser> businessApplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser("adminGlobal");
            Guid businessApplicationId = businessApplicationsByUser.FirstOrDefault().Id;

            Form expected = null;
            Form actual;
            actual = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, null);
            Assert.AreNotEqual(expected, actual);

        }

        /// <summary>
        ///A test for SearchOrderList
        ///</summary>
        [TestMethod()]
        public void SearchOrderListTest()
        {
            //FormCollection formCollection = new FormCollection();
            //formCollection.Add("Client", "BB4439C8-E0CA-4725-8108-56C788495ACB");
            //formCollection.Add("BookingNumber", "");
            //formCollection.Add("OrderNumber", "");
            //List<BusinessApplicationByUser> businessApplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser("adminGlobal");
            //Guid businessApplicationId = businessApplicationsByUser.FirstOrDefault().Id;
            //DynamicDataGrid expected = null;
            //DynamicDataGrid actual;
            //actual = ServiceOrderBusiness.SearchOrderList(formCollection, businessApplicationId, null, 0, 0, false);
            //Assert.AreNotEqual(expected, actual);

        }

        /// <summary>
        ///A test for AddServiceOrder
        ///</summary>
        [TestMethod()]
        public void AddServiceOrderTest()
        {
            string userName = "adminGlobal";
            FormCollection formCollection = new FormCollection();
            formCollection.Add("Client", "BB4439C8-E0CA-4725-8108-56C788495ACB");
            formCollection.Add("BookingNumber", "");
            formCollection.Add("OrderNumber", "");
            List<BusinessApplicationByUser> businessApplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser(userName);
            Guid businessApplicationId = businessApplicationsByUser.FirstOrDefault().Id;
            ServiceOrderBusiness.AddServiceOrder(formCollection, businessApplicationId, userName);
        }

        /// <summary>
        ///A test for EditServiceOrder
        ///</summary>
        [TestMethod()]
        public void EditServiceOrderTest()
        {
            string userName = "adminGlobal";
            FormCollection formCollection = new FormCollection();
            formCollection.Add("Client", "BB4439C8-E0CA-4725-8108-56C788495ACB");
            formCollection.Add("BookingNumber", "");
            formCollection.Add("OrderNumber", "");
            List<BusinessApplicationByUser> businessApplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser(userName);
            Guid businessApplicationId = businessApplicationsByUser.FirstOrDefault().Id;

            Guid serviceOrderId = new Guid("B2F4184A-B1A6-434F-948A-BCF17F1D4FEE");

            ServiceOrderBusiness.EditServiceOrder(formCollection, businessApplicationId, userName, serviceOrderId);
        }

        /// <summary>
        ///A test for SearchInspectionReportsByServiceOrder
        ///</summary>
        [TestMethod()]
        public void SearchInspectionReportsByServiceOrderTest()
        {
            //Guid serviceOrderId = new Guid("D68318EA-59E5-E111-87A7-0026551F18F4");
            //Guid businessApplicationId = new Guid("BFE0C1B1-B4B7-4342-BEE7-ED6CC097B6F5");
            //string userName = "adminGlobal";
            //Dictionary<int, DynamicDataGrid> expected = null;
            //Dictionary<int, DynamicDataGrid> actual;
            //actual = ServiceOrderBusiness.SearchInspectionReportsByServiceOrder(serviceOrderId, businessApplicationId, userName, new Lisstring { "Time Log" });
            //Assert.AreNotEqual(expected, actual);
        }
    }
}
