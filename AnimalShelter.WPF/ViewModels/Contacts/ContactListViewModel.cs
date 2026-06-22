using System.Collections.ObjectModel;
using System.Windows;
using AnimalShelter.Core.Interfaces;
using AnimalShelter.WPF.Mappers;
using AnimalShelter.WPF.Models.Contacts;
using AnimalShelter.WPF.ViewModels.Base;

namespace AnimalShelter.WPF.ViewModels.Contacts
{
    public class ContactListViewModel : ViewModelBase
    {
        private readonly IContactService _contactService;
        private IEnumerable<ContactListingModel> _allContacts = [];
        private CancellationTokenSource? _searchCts;

        public ObservableCollection<ContactListingModel> Contacts { get; } = [];

        private string? _filterName;
        public string? FilterName
        {
            get => _filterName;
            set
            {
                _filterName = value;
                OnPropertyChanged();
                ApplyFiltersWithDebounce();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); OnPropertyChanged(nameof(LoadingVisibility)); }
        }
        public Visibility LoadingVisibility => IsLoading ? Visibility.Visible : Visibility.Collapsed;

        public event Action<Guid>? RequestNavigateToDetails;
        public event Action? RequestNavigateToAdd;

        public RelayCommand ShowDetailsCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand ResetFiltersCommand { get; }

        public ContactListViewModel(IContactService contactService)
        {
            _contactService = contactService;
            ShowDetailsCommand = new RelayCommand(param =>
            {
                if (param is ContactListingModel m)
                    RequestNavigateToDetails?.Invoke(m.Id);
            });
            AddCommand = new RelayCommand(_ => RequestNavigateToAdd?.Invoke());
            ResetFiltersCommand = new RelayCommand(_ => ResetFilters());
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var contacts = await _contactService.GetAllContactsAsync();
                _allContacts = contacts.Select(c => c.ToListingModel()).ToList();
                ApplyFilters();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilters()
        {
            var filtered = _allContacts.Where(c =>
                string.IsNullOrWhiteSpace(_filterName) ||
                c.FullName.Contains(_filterName, StringComparison.OrdinalIgnoreCase)
            );

            Contacts.Clear();
            foreach (var c in filtered)
                Contacts.Add(c);
        }

        private async void ApplyFiltersWithDebounce()
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;
            try
            {
                await Task.Delay(400, token);
                if (!token.IsCancellationRequested)
                    ApplyFilters();
            }
            catch (TaskCanceledException) { }
        }

        private void ResetFilters()
        {
            _filterName = null;
            OnPropertyChanged(nameof(FilterName));
            ApplyFilters();
        }
    }
}
