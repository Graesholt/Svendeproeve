using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPI.Controllers;
using WebAPI.Models;

namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {
        //Unit tests of ValidateUserInfo()
        //Validation unittest names are selfexplanatory

        [TestMethod]
        public void ValidateUsernameAndPassword()
        {
            User user = new User();
            user.username = "FunRunTesting";
            user.password = "tQeWsEtRiTnYg5";

            Assert.AreEqual(UserController.ValidateUserInfo(user), true);
        }

        [TestMethod]
        public void ValidateUsernameMissing()
        {
            User user = new User();
            user.username = "";
            user.password = "tQeWsEtRiTnYg5";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);
        }

        [TestMethod]
        public void ValidateUsernameTooShort()
        {
            User user = new User();
            user.username = "FunRu";
            user.password = "tQeWsEtRiTnYg5";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);
        }

        [TestMethod]
        public void ValidateUsernameTooLong()
        {
            User user = new User();
            user.username = "FunRunTestingFunRunTestingFunRu";
            user.password = "tQeWsEtRiTnYg5";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);
        }

        [TestMethod]
        public void ValidateUsernameIllegalCharacters()
        {
            User user = new User();
            user.username = "Fun Run Testing";
            user.password = "tQeWsEtRiTnYg5";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);

            user.username = "FunRunTesting#";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);

            user.username = "FunRunTesting%";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);

            user.username = "FunRunTesting&";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);
        }

        [TestMethod]
        public void ValidateUsernameLegalCharacters()
        {
            User user = new User();
            user.username = "Fun3Run6Testing13.-_@!?";
            user.password = "tQeWsEtRiTnYg5";

            Assert.AreEqual(UserController.ValidateUserInfo(user), true);
        }

        [TestMethod]
        public void ValidatePasswordMissing()
        {
            User user = new User();
            user.username = "FunRunTesting";
            user.password = "";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);
        }

        [TestMethod]
        public void ValidatePasswordTooShort()
        {
            User user = new User();
            user.username = "FunRunTesting";
            user.password = "tQeWsE5";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);
        }

        [TestMethod]
        public void ValidatePasswordTooLong()
        {
            User user = new User();
            user.username = "FunRunTesting";
            user.password = "tQeWsEtRiTnYg5tQeWsEtRiTnYg5tQeWsEtRiTnYg5tQeWsEtRiTnYg5tQeWsEtRiTnYg5tQeWsEtRiTnYg5tQeWsEtRiTnYg5tQ";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);
        }

        [TestMethod]
        public void ValidatePasswordMissingUpperCase()
        {
            User user = new User();
            user.username = "FunRunTesting";
            user.password = "tqewsetritnyg5";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);
        }

        [TestMethod]
        public void ValidatePasswordMissingLowerCase()
        {
            User user = new User();
            user.username = "FunRunTesting";
            user.password = "TQEWSETRITNYG5";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);
        }

        [TestMethod]
        public void ValidatePasswordMissingNumberAndSpecialChar()
        {
            User user = new User();
            user.username = "FunRunTesting";
            user.password = "tQeWsEtRiTnYg";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);
        }

        [TestMethod]
        public void ValidatePasswordIllegalCharacters()
        {
            User user = new User();
            user.username = "FunRunTesting";
            user.password = "tQ eW sE tR iT nY g5";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);

            user.password = "tQeWsEtRiTnYg5#";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);

            user.password = "tQeWsEtRiTnYg5%";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);

            user.password = "tQeWsEtRiTnYg5&";

            Assert.AreEqual(UserController.ValidateUserInfo(user), false);
        }

        [TestMethod]
        public void ValidatePasswordLegalCharacters()
        {
            User user = new User();
            user.username = "FunRunTesting";
            user.password = "tQeWsEtRiTnYg.-_@!?";

            Assert.AreEqual(UserController.ValidateUserInfo(user), true);
        }
    }
}