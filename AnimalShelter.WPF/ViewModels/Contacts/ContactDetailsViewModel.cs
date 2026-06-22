using System.Collections.ObjectModel;
using System.Windows;
using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.WPF.Mappers;
using AnimalShelter.WPF.Models.Contacts;
using AnimalShelter.WPF.ViewModels.Base;

namespace AnimalShelter.WPF.ViewModels.Contacts
{
    public class ContactDetailsViewModel : ViewModelBase
    {
        private readonly IContactService _contactService;
        private readonly IFosterService _fosterService;
        private readonly IAdoptionService _adoptionService;
        private readonly Guid _contactId;

        private ContactDetailsModel? _contact;
        public ContactDetailsModel? Contact
        {
            get => _contact;
            private set { _contact = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); OnPropertyChanged(nameof(LoadingVisibility)); }
        }
        public Visibility LoadingVisibility => IsLoading ? Visibility.Visible : Visibility.Collapsed;

        public ObservableCollection<FosterStay> CurrentFosterAnimals { get; } = [];
        public ObservableCollection<FosterStay> FosterHistory { get; } = [];
        public ObservableCollection<AdoptionFile> AdoptionHistory { get; } = [];

        // --- Terminer un accueil depuis la fiche contact ---

        private Guid? _endingStayId;
        public bool IsFosterEndMode => _endingStayId.HasValue;
        public Visibility FosterEndFormVisibility => IsFosterEndMode ? Visibility.Visible : Visibility.Collapsed;

        private DateTime _fosterEndDate = DateTime.Today;
        public DateTime FosterEndDate
        {
            get => _fosterEndDate;
            set { _fosterEndDate = value; OnPropertyChanged(); }
        }

        private string? _fosterErrorMessage;
        public string? FosterErrorMessage
        {
            get => _fosterErrorMessage;
            set { _fosterErrorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(FosterErrorVisibility)); }
        }
        public Visibility FosterErrorVisibility => string.IsNullOrEmpty(_fosterErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

        // -----------------------------------------

        public event Action<Guid>? RequestNavigateToEdit;
        public event Action? RequestGoBack;

        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand EndFosterCommand { get; }
        public RelayCommand ConfirmFosterEndCommand { get; }
        public RelayCommand CancelFosterEndCommand { get; }

        public ContactDetailsViewModel(IContactService contactService, IFosterService fosterService, IAdoptionService adoptionService, Guid contactId)
        {
            _contactService = contactService;
            _fosterService = fosterService;
            _adoptionService = adoptionService;
            _contactId = contactId;

            EditCommand = new RelayCommand(_ => RequestNavigateToEdit?.Invoke(_contactId));
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync());
            EndFosterCommand = new RelayCommand(param =>
            {
                if (param is FosterStay f)
                {
                    _endingStayId = f.Id;
                    FosterEndDate = DateTime.Today;
                    FosterErrorMessage = null;
                    OnPropertyChanged(nameof(IsFosterEndMode));
                    OnPropertyChanged(nameof(FosterEndFormVisibility));
                }
            });
            ConfirmFosterEndCommand = new RelayCommand(async _ => await ConfirmEndFosterAsync());
            CancelFosterEndCommand = new RelayCommand(_ => ResetFosterEndForm());
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var contact = await _contactService.GetContactAsync(_contactId);
                if (contact == null) return;
                Contact = contact.ToDetailsModel();

                await RefreshFosterDataAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RefreshFosterDataAsync()
        {
            var current = await _fosterService.GetFamilyCurrentAnimalsAsync(_contactId);
            CurrentFosterAnimals.Clear();
            foreach (var a in current) CurrentFosterAnimals.Add(a);

            var history = await _fosterService.GetFamilyFosterHistoryAsync(_contactId);
            FosterHistory.Clear();
            foreach (var f in history) FosterHistory.Add(f);

            var adoptions = await _adoptionService.GetContactAdoptionsAsync(_contactId);
            AdoptionHistory.Clear();
            foreach (var a in adoptions) AdoptionHistory.Add(a);
        }

        private async Task ConfirmEndFosterAsync()
        {
            FosterErrorMessage = null;
            try
            {
                await _fosterService.EndFosterStayAsync(_endingStayId!.Value, FosterEndDate);
                ResetFosterEndForm();
                await RefreshFosterDataAsync();
            }
            catch (Exception ex) { FosterErrorMessage = ex.Message; }
        }

        private void ResetFosterEndForm()
        {
            _endingStayId = null;
            FosterEndDate = DateTime.Today;
            FosterErrorMessage = null;
            OnPropertyChanged(nameof(IsFosterEndMode));
            OnPropertyChanged(nameof(FosterEndFormVisibility));
        }

        private async Task DeleteAsync()
        {
            var result = MessageBox.Show(
                $"Supprimer le contact {Contact?.FirstName} {Contact?.LastName} ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await _contactService.DeleteContactAsync(_contactId);
                RequestGoBack?.Invoke();
            }
        }
    }
}
