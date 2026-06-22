using System.Collections.ObjectModel;
using System.Windows;
using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Interfaces;
using AnimalShelter.WPF.Mappers;
using AnimalShelter.WPF.Models.Animals;
using AnimalShelter.WPF.ViewModels.Base;

namespace AnimalShelter.WPF.ViewModels.Animals
{
    public class AnimalListViewModel : ViewModelBase
    {
        private readonly IAnimalService _animalService;
        private IEnumerable<AnimalListingModel> _allAnimals = [];
        private CancellationTokenSource? _searchCts;

        public ObservableCollection<AnimalListingModel> Animals { get; } = [];

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

        private SpeciesEnum? _filterSpecies;
        public SpeciesEnum? FilterSpecies
        {
            get => _filterSpecies;
            set { _filterSpecies = value; OnPropertyChanged(); ApplyFilters(); }
        }

        private AnimalStatusEnum? _filterStatus;
        public AnimalStatusEnum? FilterStatus
        {
            get => _filterStatus;
            set { _filterStatus = value; OnPropertyChanged(); ApplyFilters(); }
        }

        private SexEnum? _filterSex;
        public SexEnum? FilterSex
        {
            get => _filterSex;
            set { _filterSex = value; OnPropertyChanged(); ApplyFilters(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); OnPropertyChanged(nameof(LoadingVisibility)); }
        }
        public Visibility LoadingVisibility => IsLoading ? Visibility.Visible : Visibility.Collapsed;

        public IEnumerable<SpeciesEnum?> SpeciesOptions { get; } =
            new SpeciesEnum?[] { null }.Concat(Enum.GetValues<SpeciesEnum>().Cast<SpeciesEnum?>());

        public IEnumerable<AnimalStatusEnum?> StatusOptions { get; } =
            new AnimalStatusEnum?[] { null }.Concat(Enum.GetValues<AnimalStatusEnum>().Cast<AnimalStatusEnum?>());

        public IEnumerable<SexEnum?> SexOptions { get; } =
            new SexEnum?[] { null }.Concat(Enum.GetValues<SexEnum>().Cast<SexEnum?>());

        public event Action<string>? RequestNavigateToDetails;
        public event Action? RequestNavigateToAdd;

        public RelayCommand ShowDetailsCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand ResetFiltersCommand { get; }

        public AnimalListViewModel(IAnimalService animalService)
        {
            _animalService = animalService;
            ShowDetailsCommand = new RelayCommand(param =>
            {
                if (param is AnimalListingModel m)
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
                var animals = await _animalService.GetAvailableAnimalsAsync();
                _allAnimals = animals.Select(a => a.ToListingModel()).ToList();
                ApplyFilters();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilters()
        {
            var filtered = _allAnimals.Where(a =>
                (string.IsNullOrWhiteSpace(_filterName) || a.Name.Contains(_filterName, StringComparison.OrdinalIgnoreCase)) &&
                (_filterSpecies == null || a.Species == _filterSpecies) &&
                (_filterStatus == null || a.CurrentStatus == _filterStatus) &&
                (_filterSex == null || a.Sex == _filterSex)
            );

            Animals.Clear();
            foreach (var a in filtered)
                Animals.Add(a);
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
            _filterSpecies = null;
            _filterStatus = null;
            _filterSex = null;
            OnPropertyChanged(nameof(FilterName));
            OnPropertyChanged(nameof(FilterSpecies));
            OnPropertyChanged(nameof(FilterStatus));
            OnPropertyChanged(nameof(FilterSex));
            ApplyFilters();
        }
    }
}
