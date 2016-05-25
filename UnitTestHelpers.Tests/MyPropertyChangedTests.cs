using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestHelpers.Tests
{
    [TestClass]
    public class MyPropertyChangedTests
    {
        [TestMethod]
        public async Task WatchIsLoading()
        {
            //Arrange
            var dataService = new Mock<IDataService>();
            var viewModel = new MyPropertyChanged(dataService.Object);
            var isLoadingChanges = viewModel.WatchPropertyChanges<bool>(nameof(MyPropertyChanged.IsLoading));
            
            //Act
            await viewModel.LoadDataAsync();

            //Assert
            CollectionAssert.AreEqual(new[] {true, false}, isLoadingChanges.ToList());
        }
    }
}