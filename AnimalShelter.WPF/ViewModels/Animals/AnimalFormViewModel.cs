using System.Windows;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.WPF.ViewModels.Base;

namespace AnimalShelter.WPF.ViewModels.Animals
{
    public class AnimalFormViewModel : ViewModelBase
    {
        private readonly IAnimalService _animalService;
        private readonly string? _animalId;

        public bool IsEditMode => _animalId != null;
        public string Title => IsEditMode ? "Modifier un animal" : "Ajouter un animal";

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private SpeciesEnum _species;
        public SpeciesEnum Species
        {
            get => _species;
            set => SetProperty(ref _species, value);
        }

        private SexEnum _sex;
        public SexEnum Sex
        {
            get => _sex;
            set => SetProperty(ref _sex, value);
        }

        private string? _colors;
        public string? Colors
        {
            get => _colors;
            set => SetProperty(ref _colors, value);
        }

        private bool _isSterilised;
        public bool IsSterilised
        {
            get => _isSterilised;
            set => SetProperty(ref _isSterilised, value);
        }

        private DateTime? _sterilisationDate;
        public DateTime? SterilisationDate
        {
            get => _sterilisationDate;
            set => SetProperty(ref _sterilisationDate, value);
        }

        private DateTime? _birthDate;
        public DateTime? BirthDate
        {
            get => _birthDate;
            set => SetProperty(ref _birthDate, value);
        }

        private string? _description;
        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string? _particularities;
        public string? Particularities
        {
            get => _particularities;
            set => SetProperty(ref _particularities, value);
        }

        private AnimalStatusEnum _currentStatus = AnimalStatusEnum.Shelter;
        public AnimalStatusEnum CurrentStatus
        {
            get => _currentStatus;
            set => SetProperty(ref _currentStatus, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsSaveEnabled)); }
        }
        public bool IsSaveEnabled => !_isLoading;

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(ErrorVisibility)); }
        }
        public Visibility ErrorVisibility => string.IsNullOrEmpty(_errorMessage) ? Visibility.Collapsed : Visibility.Visible;

        public IEnumerable<SpeciesEnum> SpeciesOptions { get; } = Enum.GetValues<SpeciesEnum>();
        public IEnumerable<SexEnum> SexOptions { get; } = Enum.GetValues<SexEnum>();
        public IEnumerable<AnimalStatusEnum> StatusOptions { get; } = Enum.GetValues<AnimalStatusEnum>();

        public event Action? RequestClose;
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public AnimalFormViewModel(IAnimalService animalService)
        {
            _animalService = animalService;
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke());
        }

        public AnimalFormViewModel(IAnimalService animalService, string animalId) : this(animalService)
        {
            _animalId = animalId;
        }

        public async Task LoadAsync()
        {
            if (!IsEditMode) return;

            IsLoading = true;
            try
            {
                var animal = await _animalService.GetAnimalAsync(_animalId!);
                if (animal == null) return;

                _name = animal.Name;
                _species = animal.Species;
                _sex = animal.Sex;
                _colors = animal.Colors;
                _isSterilised = animal.IsSterilised;
                _sterilisationDate = animal.SterilisationDate;
                _birthDate = animal.BirthDate;
                _description = animal.Description;
                _particularities = animal.Particularities;
                _currentStatus = animal.CurrentStatus;

                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Species));
                OnPropertyChanged(nameof(Sex));
                OnPropertyChanged(nameof(Colors));
                OnPropertyChanged(nameof(IsSterilised));
                OnPropertyChanged(nameof(SterilisationDate));
                OnPropertyChanged(nameof(BirthDate));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(Particularities));
                OnPropertyChanged(nameof(CurrentStatus));
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveAsync()
        {
            ErrorMessage = null;
            IsLoading = true;
            try
            {
                var animal = new Animal
                {
                    Id = _animalId ?? string.Empty,
                    Name = Name,
                    Species = Species,
                    Sex = Sex,
                    Colors = Colors,
                    IsSterilised = IsSterilised,
                    SterilisationDate = SterilisationDate,
                    BirthDate = BirthDate,
                    Description = Description,
                    Particularities = Particularities,
                    CurrentStatus = CurrentStatus,
                };

                if (IsEditMode)
                    await _animalService.UpdateAnimalAsync(animal);
                else
                    await _animalService.RegisterAnimalAsync(animal);

                RequestClose?.Invoke();
            }
            catch (ShelterException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Une erreur inattendue s'est produite : {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

}
