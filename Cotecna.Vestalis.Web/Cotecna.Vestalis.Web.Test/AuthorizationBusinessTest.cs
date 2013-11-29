using System;
using System.Collections.Generic;
using Cotecna.Vestalis.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cotecna.Vestalis.Web.Test
{


    /// <summary>
    ///This is a test class for AuthorizationBusinessTest and is intended
    ///to contain all AuthorizationBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AuthorizationBusinessTest
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
        ///A test for GetBusinessApplicationsByUser
        ///</summary>
        [TestMethod()]
        public void GetBusinessApplicationsByUserTest()
        {
            string userName = "adminGlobal";
            List<BusinessApplicationByUser> expected = null;
            List<BusinessApplicationByUser> actual;
            actual = AuthorizationBusiness.GetBusinessApplicationsByUser(userName);
            Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///A test for LogOn
        ///</summary>
        [TestMethod()]
        public void LogOnTest()
        {
            AuthorizationBusiness target = new AuthorizationBusiness();
            string userName = "adminGlobal";
            string password = "111111";
            bool expected = false;
            bool actual;
            actual = target.LogOn(userName, password);
            Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetUserParameter
        ///</summary>
        [TestMethod()]
        public void GetUserParameterTest()
        {
            string userName = "adminGlobal";
            string parameterName = "ApplicationDefault";
            string expected = string.Empty;
            string actual;
            actual = AuthorizationBusiness.GetUserParameter(userName, parameterName);
            Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///A test for SearchUsers
        ///</summary>
        [TestMethod()]
        public void SearchUsersTest()
        {
            Guid businessAplicationId = new Guid("BFE0C1B1-B4B7-4342-BEE7-ED6CC097B6F5");
            string roleName = "coordinator";
            List<UserInformation> expected = null;
            List<UserInformation> actual;
            actual = AuthorizationBusiness.Instance.SearchUsers(businessAplicationId, roleName);
            Assert.AreNotEqual(expected, actual);
        }


        /// <summary>
        ///A test for GetUserList
        ///</summary>
        [TestMethod()]
        public void GetUserListTest()
        {
            //int pageSize = 20;
            //int selectedPage = 1;
            //AuthorizationBusiness business = new AuthorizationBusiness();
            //PaginatedList<UserGridModel> expected = null; // TODO: Initialize to an appropriate value
            //PaginatedList<UserGridModel> actual;
            //Guid businessApplicationId = Guid.Empty;
            //actual = business.GetUserList(pageSize, selectedPage, businessApplicationId);
            //Assert.AreNotEqual(expected, actual);
        }

       
    }
}
