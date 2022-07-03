using Microsoft.VisualStudio.TestTools.UnitTesting;
using Y2KWMS.Controller;
using Y2KWMS.Model;
using System.Windows.Input;
using System.Windows.Markup;
using System;

namespace TestingY2KWorkManagementSystem
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            QueryController queryController = new QueryController();
            string[] email = { "mathula2504@gmail.com" };
            Assert.IsTrue(queryController.sendEmail(email, "subject", "body"),"Email not send");
        }


        [TestMethod]
        public void TestMethod2()
        {
            try
            {
                string password = "adminpassword";
                Assert.AreEqual(password, Y2KWMS.View.Pages.Project.User.Create.DefaultPassword);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            
        }

        [TestMethod]
        public void TestMethod3()
        {
            Project user = new Project();
            user.Title = "a";
            Assert.IsNull(user,"null user");

        }
    }
}
