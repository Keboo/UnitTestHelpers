using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestHelpers.Tests
{
    [TestClass]
    public class PropertyChangedTests
    {
        [TestMethod]
        public async Task IsLoadingIsSetWhileLoadingData()
        {
            //Arrange
            var dataService = new Mock<IDataService>();
            var viewModel = new ViewModel( dataService.Object );
            var isLoadingChanges = viewModel.WatchPropertyChanges<bool>( nameof( ViewModel.IsLoading ) );

            //Act
            await viewModel.LoadDataAsync();

            //Assert
            CollectionAssert.AreEqual( new[] { true, false }, isLoadingChanges.ToList() );
        }

        [TestMethod]
        public void CanWaitForAnyPropertyChanged()
        {
            //Arrange
            var dataService = new Mock<IDataService>();
            dataService.Setup( x => x.LoadData() )
                .Returns( GetSlowTask<IEnumerable<string>>() );
            var viewModel = new ViewModel( dataService.Object );
            var isLoadingChanges = viewModel.WatchPropertyChanges<bool>( nameof( ViewModel.IsLoading ) );
            WaitHandle waitHandle = isLoadingChanges.WaitForChange();

            //Act
            viewModel.Load();
            Assert.IsTrue( waitHandle.WaitOne( TimeSpan.FromSeconds( 1 ) ) );

            //Assert
            CollectionAssert.AreEqual( new[] { true }, isLoadingChanges.ToList() );
        }

        [TestMethod]
        public void CanWaitForSpecificChange()
        {
            //Arrange
            var dataService = new Mock<IDataService>();
            dataService.Setup( x => x.LoadData() )
                .Returns( GetSlowTask<IEnumerable<string>>() );
            var viewModel = new ViewModel( dataService.Object );
            var isLoadingChanges = viewModel.WatchPropertyChanges<bool>( nameof( ViewModel.IsLoading ) );
            WaitHandle waitHandle = isLoadingChanges.WaitFor( x => x == false );

            //Act
            viewModel.Load();
            Assert.IsTrue( waitHandle.WaitOne( TimeSpan.FromSeconds( 10 ) ) );

            //Assert
            CollectionAssert.AreEqual( new[] { true, false }, isLoadingChanges.ToList() );
        }

        private static async Task<T> GetSlowTask<T>()
        {
            await Task.Delay( TimeSpan.FromSeconds( 1 ) );
            return default( T );
        }
    }
}