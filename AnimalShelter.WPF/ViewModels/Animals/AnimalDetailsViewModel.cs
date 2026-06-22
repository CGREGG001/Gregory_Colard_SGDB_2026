using System.Collections.ObjectModel;
using System.Windows;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.WPF.Mappers;
using AnimalShelter.WPF.Models.Animals;
using AnimalShelter.WPF.Models.Contacts;
using AnimalShelter.WPF.ViewModels.Base;

namespace AnimalShelter.WPF.ViewModels.Animals;

public class AnimalDetailsViewModel : ViewModelBase
{
    private readonly IAnimalService _animalService;
    private readonly IVaccinationService _vaccinationService;
    private readonly ICompatibilityService _compatibilityService;
    private readonly IFosterService _fosterService;
    private readonly IContactService _contactService;
    private readonly IAdoptionService _adoptionService;
    private readonly string _animalId;

    private AnimalDetailsModel? _animal;
    public AnimalDetailsModel? Animal
    {
        get => _animal;
        private set { _animal = value; OnPropertyChanged(); }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); OnPropertyChanged(nameof(LoadingVisibility)); }
    }
    public Visibility LoadingVisibility => IsLoading ? Visibility.Visible : Visibility.Collapsed;

    public ObservableCollection<Vaccination> Vaccinations { get; } = [];
    public ObservableCollection<Compatibility> Compatibilities { get; } = [];
    public ObservableCollection<FosterStay> FosterHistory { get; } = [];

    // --- Formulaire ajout / édition de vaccination ---

    private string _newVaccineName = string.Empty;
    public string NewVaccineName
    {
        get => _newVaccineName;
        set { _newVaccineName = value; OnPropertyChanged(); }
    }

    private DateTime _newVaccineDate = DateTime.Today;
    public DateTime NewVaccineDate
    {
        get => _newVaccineDate;
        set { _newVaccineDate = value; OnPropertyChanged(); }
    }

    private bool _newVaccineIsDone = true;
    public bool NewVaccineIsDone
    {
        get => _newVaccineIsDone;
        set { _newVaccineIsDone = value; OnPropertyChanged(); }
    }

    private Guid? _editingVaccinationId;
    public bool IsVaccinationEditMode => _editingVaccinationId.HasValue;
    public Visibility VaccinationCancelVisibility => IsVaccinationEditMode ? Visibility.Visible : Visibility.Collapsed;
    public string VaccinationFormTitle => IsVaccinationEditMode ? "Modifier le vaccin" : "Ajouter un vaccin";
    public string VaccinationSaveLabel => IsVaccinationEditMode ? "Modifier" : "+ Ajouter";

    private string? _vaccinationErrorMessage;
    public string? VaccinationErrorMessage
    {
        get => _vaccinationErrorMessage;
        set { _vaccinationErrorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(VaccinationErrorVisibility)); }
    }
    public Visibility VaccinationErrorVisibility => string.IsNullOrEmpty(_vaccinationErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

    private bool _isVaccinationSaving;
    public bool IsVaccinationSaving
    {
        get => _isVaccinationSaving;
        set { _isVaccinationSaving = value; OnPropertyChanged(); }
    }

    // --- Formulaire ajout / édition de compatibilité ---

    private CompatibilityTypeEnum _newCompatType;
    public CompatibilityTypeEnum NewCompatType
    {
        get => _newCompatType;
        set { _newCompatType = value; OnPropertyChanged(); }
    }

    private CompatibilityValueEnum _newCompatValue;
    public CompatibilityValueEnum NewCompatValue
    {
        get => _newCompatValue;
        set { _newCompatValue = value; OnPropertyChanged(); }
    }

    private string? _newCompatDescription;
    public string? NewCompatDescription
    {
        get => _newCompatDescription;
        set { _newCompatDescription = value; OnPropertyChanged(); }
    }

    private bool _isCompatEditMode;
    public bool IsCompatEditMode
    {
        get => _isCompatEditMode;
        set
        {
            _isCompatEditMode = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CompatFormTitle));
            OnPropertyChanged(nameof(CompatSaveLabel));
            OnPropertyChanged(nameof(CompatCancelVisibility));
            OnPropertyChanged(nameof(CompatTypeEnabled));
        }
    }

    public string CompatFormTitle => IsCompatEditMode ? "Modifier la compatibilité" : "Ajouter une compatibilité";
    public string CompatSaveLabel => IsCompatEditMode ? "Modifier" : "+ Ajouter";
    public Visibility CompatCancelVisibility => IsCompatEditMode ? Visibility.Visible : Visibility.Collapsed;
    public bool CompatTypeEnabled => !IsCompatEditMode;

    private string? _compatErrorMessage;
    public string? CompatErrorMessage
    {
        get => _compatErrorMessage;
        set { _compatErrorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(CompatErrorVisibility)); }
    }
    public Visibility CompatErrorVisibility => string.IsNullOrEmpty(_compatErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

    public IEnumerable<CompatibilityTypeEnum> CompatTypeOptions { get; } = Enum.GetValues<CompatibilityTypeEnum>();
    public IEnumerable<CompatibilityValueEnum> CompatValueOptions { get; } = Enum.GetValues<CompatibilityValueEnum>();

    // --- Formulaire démarrer / terminer un accueil ---

    public ObservableCollection<ContactListingModel> FosterContactOptions { get; } = [];

    private ContactListingModel? _selectedFosterContact;
    public ContactListingModel? SelectedFosterContact
    {
        get => _selectedFosterContact;
        set { _selectedFosterContact = value; OnPropertyChanged(); }
    }

    private DateTime _newFosterStartDate = DateTime.Today;
    public DateTime NewFosterStartDate
    {
        get => _newFosterStartDate;
        set { _newFosterStartDate = value; OnPropertyChanged(); }
    }

    private Guid? _endingStayId;
    public bool IsFosterEndMode => _endingStayId.HasValue;
    public Visibility FosterStartFormVisibility => IsFosterEndMode ? Visibility.Collapsed : Visibility.Visible;
    public Visibility FosterEndFormVisibility => IsFosterEndMode ? Visibility.Visible : Visibility.Collapsed;
    public Visibility FosterCancelVisibility => IsFosterEndMode ? Visibility.Visible : Visibility.Collapsed;
    public string FosterFormTitle => IsFosterEndMode ? "Terminer l'accueil" : "Démarrer un accueil";

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

    // --- Dossiers d'adoption ---

    public ObservableCollection<AdoptionFile> Adoptions { get; } = [];

    private ContactListingModel? _selectedAdoptionContact;
    public ContactListingModel? SelectedAdoptionContact
    {
        get => _selectedAdoptionContact;
        set { _selectedAdoptionContact = value; OnPropertyChanged(); }
    }

    private Guid? _processingAdoptionId;
    public bool IsAdoptionProcessMode => _processingAdoptionId.HasValue;
    public Visibility AdoptionRequestFormVisibility => IsAdoptionProcessMode ? Visibility.Collapsed : Visibility.Visible;
    public Visibility AdoptionProcessFormVisibility => IsAdoptionProcessMode ? Visibility.Visible : Visibility.Collapsed;
    public Visibility AdoptionCancelVisibility => IsAdoptionProcessMode ? Visibility.Visible : Visibility.Collapsed;
    public string AdoptionFormTitle => IsAdoptionProcessMode ? "Traiter le dossier" : "Nouvelle demande d'adoption";

    private AdoptionStatusEnum _selectedAdoptionStatus = AdoptionStatusEnum.Approved;
    public AdoptionStatusEnum SelectedAdoptionStatus
    {
        get => _selectedAdoptionStatus;
        set { _selectedAdoptionStatus = value; OnPropertyChanged(); }
    }

    public IEnumerable<AdoptionStatusEnum> AdoptionProcessStatusOptions { get; } =
    [
        AdoptionStatusEnum.Approved,
        AdoptionStatusEnum.EnvRejected,
        AdoptionStatusEnum.BehaviourRejected,
    ];

    private string? _adoptionErrorMessage;
    public string? AdoptionErrorMessage
    {
        get => _adoptionErrorMessage;
        set { _adoptionErrorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(AdoptionErrorVisibility)); }
    }
    public Visibility AdoptionErrorVisibility => string.IsNullOrEmpty(_adoptionErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

    // -----------------------------------------

    public event Action<string>? RequestNavigateToEdit;
    public event Action? RequestGoBack;

    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SaveVaccinationCommand { get; }
    public RelayCommand CancelVaccinationEditCommand { get; }
    public RelayCommand EditVaccinationCommand { get; }
    public RelayCommand DeleteVaccinationCommand { get; }
    public RelayCommand SaveCompatibilityCommand { get; }
    public RelayCommand EditCompatibilityCommand { get; }
    public RelayCommand DeleteCompatibilityCommand { get; }
    public RelayCommand CancelCompatibilityEditCommand { get; }
    public RelayCommand StartFosterCommand { get; }
    public RelayCommand EndFosterCommand { get; }
    public RelayCommand ConfirmFosterEndCommand { get; }
    public RelayCommand CancelFosterEndCommand { get; }
    public RelayCommand RequestAdoptionCommand { get; }
    public RelayCommand ProcessAdoptionCommand { get; }
    public RelayCommand ConfirmAdoptionProcessCommand { get; }
    public RelayCommand CancelAdoptionProcessCommand { get; }

    public AnimalDetailsViewModel(
        IAnimalService animalService,
        IVaccinationService vaccinationService,
        ICompatibilityService compatibilityService,
        IFosterService fosterService,
        IContactService contactService,
        IAdoptionService adoptionService,
        string animalId)
    {
        _animalService = animalService;
        _vaccinationService = vaccinationService;
        _compatibilityService = compatibilityService;
        _fosterService = fosterService;
        _contactService = contactService;
        _adoptionService = adoptionService;
        _animalId = animalId;

        EditCommand = new RelayCommand(_ => RequestNavigateToEdit?.Invoke(_animalId));
        DeleteCommand = new RelayCommand(async _ => await DeleteAsync());
        SaveVaccinationCommand = new RelayCommand(
            async _ => await SaveVaccinationAsync(),
            _ => !string.IsNullOrWhiteSpace(NewVaccineName) && !IsVaccinationSaving);
        CancelVaccinationEditCommand = new RelayCommand(_ => ResetVaccinationForm());
        EditVaccinationCommand = new RelayCommand(param =>
        {
            if (param is Vaccination v) LoadVaccinationIntoForm(v);
        });
        DeleteVaccinationCommand = new RelayCommand(async param =>
        {
            if (param is Vaccination v) await DeleteVaccinationAsync(v);
        });
        SaveCompatibilityCommand = new RelayCommand(async _ => await SaveCompatibilityAsync());
        EditCompatibilityCommand = new RelayCommand(param =>
        {
            if (param is Compatibility c) LoadCompatibilityIntoForm(c);
        });
        DeleteCompatibilityCommand = new RelayCommand(async param =>
        {
            if (param is Compatibility c) await DeleteCompatibilityAsync(c);
        });
        CancelCompatibilityEditCommand = new RelayCommand(_ => ResetCompatibilityForm());
        StartFosterCommand = new RelayCommand(
            async _ => await StartFosterAsync(),
            _ => SelectedFosterContact != null);
        RequestAdoptionCommand = new RelayCommand(
            async _ => await RequestAdoptionAsync(),
            _ => SelectedAdoptionContact != null);
        ProcessAdoptionCommand = new RelayCommand(param =>
        {
            if (param is AdoptionFile f) LoadAdoptionIntoProcessForm(f);
        });
        ConfirmAdoptionProcessCommand = new RelayCommand(async _ => await ConfirmAdoptionProcessAsync());
        CancelAdoptionProcessCommand = new RelayCommand(_ => ResetAdoptionForm());
        EndFosterCommand = new RelayCommand(param =>
        {
            if (param is FosterStay f) LoadEndFosterIntoForm(f);
        });
        ConfirmFosterEndCommand = new RelayCommand(async _ => await ConfirmEndFosterAsync());
        CancelFosterEndCommand = new RelayCommand(_ => ResetFosterForm());
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var animal = await _animalService.GetAnimalAsync(_animalId);
            if (animal == null) return;
            Animal = animal.ToDetailsModel();

            await RefreshVaccinationsAsync();
            await RefreshCompatibilitiesAsync();
            await RefreshFosterHistoryAsync();
            await RefreshAdoptionsAsync();

            var contacts = await _contactService.GetAllContactsAsync();
            FosterContactOptions.Clear();
            foreach (var c in contacts)
                FosterContactOptions.Add(c.ToListingModel());
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RefreshVaccinationsAsync()
    {
        var vaccinations = await _vaccinationService.GetAnimalVaccinationHistoryAsync(_animalId);
        Vaccinations.Clear();
        foreach (var v in vaccinations) Vaccinations.Add(v);
    }

    private void LoadVaccinationIntoForm(Vaccination v)
    {
        _editingVaccinationId = v.Id;
        NewVaccineName = v.VaccineName;
        NewVaccineDate = v.VaccineDate;
        NewVaccineIsDone = v.IsDone;
        VaccinationErrorMessage = null;
        OnPropertyChanged(nameof(IsVaccinationEditMode));
        OnPropertyChanged(nameof(VaccinationFormTitle));
        OnPropertyChanged(nameof(VaccinationSaveLabel));
        OnPropertyChanged(nameof(VaccinationCancelVisibility));
    }

    private void ResetVaccinationForm()
    {
        _editingVaccinationId = null;
        NewVaccineName = string.Empty;
        NewVaccineDate = DateTime.Today;
        NewVaccineIsDone = true;
        VaccinationErrorMessage = null;
        OnPropertyChanged(nameof(IsVaccinationEditMode));
        OnPropertyChanged(nameof(VaccinationFormTitle));
        OnPropertyChanged(nameof(VaccinationSaveLabel));
        OnPropertyChanged(nameof(VaccinationCancelVisibility));
    }

    private async Task SaveVaccinationAsync()
    {
        VaccinationErrorMessage = null;
        IsVaccinationSaving = true;
        try
        {
            if (IsVaccinationEditMode)
            {
                var vaccination = new Vaccination
                {
                    Id = _editingVaccinationId!.Value,
                    AnimalId = _animalId,
                    VaccineName = NewVaccineName,
                    VaccineDate = NewVaccineDate,
                    IsDone = NewVaccineIsDone,
                };
                await _vaccinationService.UpdateVaccinationAsync(vaccination);
            }
            else
            {
                var vaccination = new Vaccination
                {
                    AnimalId = _animalId,
                    VaccineName = NewVaccineName,
                    VaccineDate = NewVaccineDate,
                    IsDone = NewVaccineIsDone,
                };
                await _vaccinationService.RegisterVaccinationAsync(vaccination);
            }

            ResetVaccinationForm();
            await RefreshVaccinationsAsync();
        }
        catch (ShelterException ex)
        {
            VaccinationErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            VaccinationErrorMessage = $"Erreur inattendue : {ex.Message}";
        }
        finally
        {
            IsVaccinationSaving = false;
        }
    }

    private void LoadCompatibilityIntoForm(Compatibility c)
    {
        NewCompatType = c.TargetType;
        NewCompatValue = c.ValueEnum;
        NewCompatDescription = c.Description;
        CompatErrorMessage = null;
        IsCompatEditMode = true;
    }

    private void ResetCompatibilityForm()
    {
        NewCompatType = default;
        NewCompatValue = default;
        NewCompatDescription = null;
        CompatErrorMessage = null;
        IsCompatEditMode = false;
    }

    private async Task SaveCompatibilityAsync()
    {
        CompatErrorMessage = null;
        try
        {
            var compat = new Compatibility
            {
                AnimalId = _animalId,
                TargetType = NewCompatType,
                ValueEnum = NewCompatValue,
                Description = NewCompatDescription,
            };
            await _compatibilityService.SetCompatibilityAsync(compat);
            ResetCompatibilityForm();
            await RefreshCompatibilitiesAsync();
        }
        catch (ShelterException ex) { CompatErrorMessage = ex.Message; }
        catch (Exception ex) { CompatErrorMessage = $"Erreur inattendue : {ex.Message}"; }
    }

    private async Task DeleteCompatibilityAsync(Compatibility c)
    {
        var result = MessageBox.Show(
            $"Supprimer la compatibilité « {c.TargetType} » ?",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _compatibilityService.DeleteCompatibilityAsync(_animalId, c.TargetType);
            if (IsCompatEditMode && NewCompatType == c.TargetType)
                ResetCompatibilityForm();
            await RefreshCompatibilitiesAsync();
        }
        catch (ShelterException ex) { CompatErrorMessage = ex.Message; }
        catch (Exception ex) { CompatErrorMessage = $"Erreur inattendue : {ex.Message}"; }
    }

    private async Task RefreshCompatibilitiesAsync()
    {
        var compatibilities = await _compatibilityService.GetAnimalCompatibilitiesAsync(_animalId);
        Compatibilities.Clear();
        foreach (var c in compatibilities) Compatibilities.Add(c);
    }

    private async Task DeleteVaccinationAsync(Vaccination v)
    {
        var result = MessageBox.Show(
            $"Supprimer le vaccin « {v.VaccineName} » ?",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _vaccinationService.DeleteVaccinationAsync(v.Id);
            if (_editingVaccinationId == v.Id)
                ResetVaccinationForm();
            await RefreshVaccinationsAsync();
        }
        catch (ShelterException ex)
        {
            VaccinationErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            VaccinationErrorMessage = $"Erreur inattendue : {ex.Message}";
        }
    }

    private async Task RefreshFosterHistoryAsync()
    {
        var history = await _fosterService.GetAnimalHistoryAsync(_animalId);
        FosterHistory.Clear();
        foreach (var f in history) FosterHistory.Add(f);
    }

    private void LoadEndFosterIntoForm(FosterStay f)
    {
        _endingStayId = f.Id;
        FosterEndDate = DateTime.Today;
        FosterErrorMessage = null;
        NotifyFosterFormChanged();
    }

    private void ResetFosterForm()
    {
        _endingStayId = null;
        SelectedFosterContact = null;
        NewFosterStartDate = DateTime.Today;
        FosterEndDate = DateTime.Today;
        FosterErrorMessage = null;
        NotifyFosterFormChanged();
    }

    private void NotifyFosterFormChanged()
    {
        OnPropertyChanged(nameof(IsFosterEndMode));
        OnPropertyChanged(nameof(FosterFormTitle));
        OnPropertyChanged(nameof(FosterStartFormVisibility));
        OnPropertyChanged(nameof(FosterEndFormVisibility));
        OnPropertyChanged(nameof(FosterCancelVisibility));
    }

    private async Task StartFosterAsync()
    {
        FosterErrorMessage = null;
        try
        {
            var stay = new FosterStay
            {
                AnimalId = _animalId,
                ContactId = SelectedFosterContact!.Id,
                StartDate = NewFosterStartDate,
            };
            await _fosterService.StartFosterStayAsync(stay);
            ResetFosterForm();
            await RefreshFosterHistoryAsync();
            var animal = await _animalService.GetAnimalAsync(_animalId);
            if (animal != null) Animal = animal.ToDetailsModel();
        }
        catch (ShelterException ex) { FosterErrorMessage = ex.Message; }
        catch (Exception ex) { FosterErrorMessage = $"Erreur inattendue : {ex.Message}"; }
    }

    private async Task ConfirmEndFosterAsync()
    {
        FosterErrorMessage = null;
        try
        {
            await _fosterService.EndFosterStayAsync(_endingStayId!.Value, FosterEndDate);
            ResetFosterForm();
            await RefreshFosterHistoryAsync();
            var animal = await _animalService.GetAnimalAsync(_animalId);
            if (animal != null) Animal = animal.ToDetailsModel();
        }
        catch (ShelterException ex) { FosterErrorMessage = ex.Message; }
        catch (Exception ex) { FosterErrorMessage = $"Erreur inattendue : {ex.Message}"; }
    }

    private async Task RefreshAdoptionsAsync()
    {
        var adoptions = await _adoptionService.GetAnimalAdoptionsAsync(_animalId);
        Adoptions.Clear();
        foreach (var a in adoptions) Adoptions.Add(a);
    }

    private void LoadAdoptionIntoProcessForm(AdoptionFile f)
    {
        _processingAdoptionId = f.Id;
        SelectedAdoptionStatus = AdoptionStatusEnum.Approved;
        AdoptionErrorMessage = null;
        NotifyAdoptionFormChanged();
    }

    private void ResetAdoptionForm()
    {
        _processingAdoptionId = null;
        SelectedAdoptionContact = null;
        SelectedAdoptionStatus = AdoptionStatusEnum.Approved;
        AdoptionErrorMessage = null;
        NotifyAdoptionFormChanged();
    }

    private void NotifyAdoptionFormChanged()
    {
        OnPropertyChanged(nameof(IsAdoptionProcessMode));
        OnPropertyChanged(nameof(AdoptionFormTitle));
        OnPropertyChanged(nameof(AdoptionRequestFormVisibility));
        OnPropertyChanged(nameof(AdoptionProcessFormVisibility));
        OnPropertyChanged(nameof(AdoptionCancelVisibility));
    }

    private async Task RequestAdoptionAsync()
    {
        AdoptionErrorMessage = null;
        try
        {
            var file = new AdoptionFile
            {
                AnimalId = _animalId,
                ContactId = SelectedAdoptionContact!.Id,
            };
            await _adoptionService.RequestAdoptionAsync(file);
            ResetAdoptionForm();
            await RefreshAdoptionsAsync();
        }
        catch (ShelterException ex) { AdoptionErrorMessage = ex.Message; }
        catch (Exception ex) { AdoptionErrorMessage = $"Erreur inattendue : {ex.Message}"; }
    }

    private async Task ConfirmAdoptionProcessAsync()
    {
        AdoptionErrorMessage = null;
        try
        {
            await _adoptionService.ProcessAdoptionAsync(_processingAdoptionId!.Value, SelectedAdoptionStatus);
            ResetAdoptionForm();
            await RefreshAdoptionsAsync();
            var animal = await _animalService.GetAnimalAsync(_animalId);
            if (animal != null) Animal = animal.ToDetailsModel();
        }
        catch (ShelterException ex) { AdoptionErrorMessage = ex.Message; }
        catch (Exception ex) { AdoptionErrorMessage = $"Erreur inattendue : {ex.Message}"; }
    }

    private async Task DeleteAsync()
    {
        var result = MessageBox.Show(
            $"Supprimer définitivement {Animal?.Name} ?",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _animalService.SoftDeleteAnimalAsync(_animalId);
            RequestGoBack?.Invoke();
        }
    }
}
