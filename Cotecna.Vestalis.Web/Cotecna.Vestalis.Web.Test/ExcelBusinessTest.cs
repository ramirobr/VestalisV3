using Cotecna.Vestalis.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Web.Security;
using System.Linq;

namespace Cotecna.Vestalis.Web.Test
{
    
    
    /// <summary>
    ///This is a test class for ExcelBusinessTest and is intended
    ///to contain all ExcelBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExcelBusinessTest
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


        [TestMethod()]
        public void GenerateReportDinamicallyTest()
        {
            bool isClient = true;
            Guid businessApplicationId = new Guid("BFE0C1B1-B4B7-4342-BEE7-ED6CC097B6F5");
            string userName = "coordinator@cotecna.ch";

            ParameterSearchServicerOrder parameters = new ParameterSearchServicerOrder
            {
                FormCollection = new System.Web.Mvc.FormCollection(),
                BusinessApplicationId = businessApplicationId,
                RolesForUser = Roles.GetRolesForUser(userName).ToList(),
                Page = 0,
                PageSize = 0,
                IsExport = true,
                IsClient = isClient
            };

            DynamicDataGrid def = ServiceOrderBusiness.GetServiceOrderGridDefinition(businessApplicationId, isClient);

            var model = ServiceOrderBusiness.SearchOrderList(parameters);
            model.Captions = def.Captions;
            model.BusinessApplicationName = def.BusinessApplicationName;
            model.FormName = def.FormName;
            string path = @"D:\NewTfs\Vestalis3TPMain\Cotecna.Vestalis.Web\Cotecna.Vestalis.Web\Templates\demo.xlsx";

            if (File.Exists(path))
                File.Delete(path);

            MemoryStream result = ExcelBusiness.GenerateReportDinamically(model, "");
            result.Position = 0;
            File.WriteAllBytes(path, result.ToArray());
        }
    }
}
