using Microsoft.VisualStudio.TestTools.UnitTesting;
using Y2KWMS.Model;
using Y2KWMS.Controller;

namespace TestingY2KWMS
{
    [TestClass]
    public class UnitTest1
    {       
        [TestMethod]
        public void TestMethodLogin()
        {
            string email = "mathula2@gmail.com";
            string password = "Y2Kcompany@2022";

            User user = new User();
            user.Email = email;
            user.Password = password;

            QueryController queryController = new QueryController();
            //bool returnMessage = queryController.login(user);
            
            Assert.IsTrue(queryController.login(user));
        }
    }
}
