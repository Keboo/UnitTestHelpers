using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace UnitTestHelpers
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly IDataService _dataService;
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel(IDataService dataService)
        {
            if (dataService == null) throw new ArgumentNullException(nameof(dataService));
            _dataService = dataService;
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public async void Load()
        {
            await LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            IsLoading = true;
            var data = await _dataService.LoadData();
            //TODO: use data
            IsLoading = false;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}