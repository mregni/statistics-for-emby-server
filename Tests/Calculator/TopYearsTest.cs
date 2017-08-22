using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using Moq;
using MediaBrowser.Controller.Entities.Movies;
using System.Linq;

namespace Tests.Calculator
{
    [TestClass]
    public class TopYearsTest
    {
        private List<BaseItem> baseItemList = new List<BaseItem>();
        private Mock<IUserManager> UserManagerMock;
        private Mock<IUserDataManager> UserDataManagerMock;
        private Mock<ILibraryManager> LibraryManagerMock;

        public TopYearsTest()
        {
            UserManagerMock = new Mock<IUserManager>();
            UserDataManagerMock = new Mock<IUserDataManager>();
        }

        [TestMethod]
        public void BaseitemsFound()
        {
            LibraryManagerMock = new Mock<ILibraryManager>();
            baseItemList = new List<BaseItem>()
            {
                new Movie() { ProductionYear = 2015  },
                new Movie() { ProductionYear = 2014  },
                new Movie() { ProductionYear = 2013  },
                new Movie() { ProductionYear = 2012  },
                new Movie() { ProductionYear = 2011  },
                new Movie() { ProductionYear = 2015  }

            };


            LibraryManagerMock.Setup(x => x.GetItemList(It.IsAny<InternalItemsQuery>())).Returns(baseItemList);

            var calc = new Statistics.Helpers.Calculator(null, UserManagerMock.Object, LibraryManagerMock.Object, UserDataManagerMock.Object);

            var result = calc.CalculateFavoriteYears();
            Assert.AreEqual("", result.ValueLineOne);
            Assert.AreEqual("", result.ValueLineTwo);
            Assert.AreEqual("Favorite Movie Years", result.Title);
            Assert.AreEqual("half", result.Size);
            Assert.IsNull(result.Id);
            Assert.IsNull(result.Raw);
            Assert.IsNull(result.ExtraInformation);
        }

        [TestMethod]
        public void NoBaseItemsWithoutUser()
        {
            var calc = new Statistics.Helpers.Calculator(null, UserManagerMock.Object, LibraryManagerMock.Object, UserDataManagerMock.Object);

            var result = calc.CalculateFavoriteYears();
            Assert.AreEqual("", result.ValueLineOne);
            Assert.AreEqual("", result.ValueLineTwo);
            Assert.AreEqual("Favorite Movie Years", result.Title);
            Assert.AreEqual("half", result.Size);
            Assert.IsNull(result.Id);
            Assert.IsNull(result.Raw);
            Assert.IsNull(result.ExtraInformation);
        }
    }
}
