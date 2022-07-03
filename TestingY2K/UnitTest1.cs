using Microsoft.VisualStudio.TestTools.UnitTesting;
using Y2KWMS.Controller;
using Y2KWMS.Model;

namespace TestingY2K
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string email = "mathula2@gmail.com";
            string password = "Y2Kcompany@2022";

            User user = new User();
            user.Email = email;
            user.Password = password;

            QueryController queryController = new QueryController();

            Assert.IsTrue(queryController.login(user));
        }
    }
}
