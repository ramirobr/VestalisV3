using System.IO;
using Cotecna.Vestalis.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cotecna.Vestalis.Web.Test
{


    /// <summary>
    ///This is a test class for EmailBusinessTest and is intended
    ///to contain all EmailBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EmailBusinessTest
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
        ///A test for SendEmail
        ///</summary>
        [TestMethod()]
        public void SendEmailTest()
        {
            string toAddress = "ramiro.batallas@cotecna.com.ec";
            string body = File.ReadAllText(@"D:\NewTfs\Vestalis3TPMain\Cotecna.Vestalis.Web\Cotecna.Vestalis.Web\Templates\PasswordResetMessageRequest.htm");
            EmailBusiness.SendEmail(toAddress, body, "", "", "");
        }
    }
}
