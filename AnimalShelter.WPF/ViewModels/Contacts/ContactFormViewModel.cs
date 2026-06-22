using AnimalShelter.Core.Enums;
using AnimalShelter.Core.Exceptions;
using AnimalShelter.Core.Interfaces;
using AnimalShelter.Core.Models;
using AnimalShelter.WPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AnimalShelter.WPF.ViewModels.Contacts
{
    public class ContactFormViewModel : ViewModelBase
    {
        private readonly IContactService _contactService;
        private readonly Guid? _contactId;

        public bool IsEditMode => _contactId.HasValue;
        public Visibility NationalRegisterVisibility => IsEditMode ? Visibility.Collapsed : Visibility.Visible;
        public string Title => IsEditMode ? "Modifier un contact" : "Ajouter un contact";

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string? _nationalRegister;
        public string? NationalRegister
        {
            get => _nationalRegister;
            set => SetProperty(ref _nationalRegister, value);
        }

        private string? _gsm;
        public string? Gsm
        {
            get => _gsm;
            set => SetProperty(ref _gsm, value);
        }

        private string? _phone;
        public string? Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        private string? _email;
        public string? Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private DateTime? _rgpdConsentDate;
        public DateTime? RgpdConsentDate
        {
            get => _rgpdConsentDate;
            set => SetProperty(ref _rgpdConsentDate, value);
        }

        // Role flags as individual booleans
        private bool _isVolunteer;
        public bool IsVolunteer
        {
            get => _isVolunteer;
            set => SetProperty(ref _isVolunteer, value);
        }

        private bool _isAdopter;
        public bool IsAdopter
        {
            get => _isAdopter;
            set => SetProperty(ref _isAdopter, value);
        }

        private bool _isCandidate;
        public bool IsCandidate
        {
            get => _isCandidate;
            set => SetProperty(ref _isCandidate, value);
        }

        private bool _isOther;
        public bool IsOther
        {
            get => _isOther;
            set => SetProperty(ref _isOther, value);
        }

        // Address
        private string? _street;
        public string? Street
        {
            get => _street;
            set => SetProperty(ref _street, value);
        }

        private string? _number;
        public string? Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        private string? _box;
        public string? Box
        {
            get => _box;
            set => SetProperty(ref _box, value);
        }

        private string? _postCode;
        public string? PostCode
        {
            get => _postCode;
            set => SetProperty(ref _postCode, value);
        }

        private string? _city;
        public string? City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        private string _country = "Belgium";
        public string Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(ErrorVisibility)); }
        }
        public Visibility ErrorVisibility => string.IsNullOrEmpty(_errorMessage) ? Visibility.Collapsed : Visibility.Visible;

        public event Action? RequestClose;
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public ContactFormViewModel(IContactService contactService)
        {
            _contactService = contactService;
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke());
        }

        public ContactFormViewModel(IContactService contactService, Guid contactId) : this(contactService)
        {
            _contactId = contactId;
        }

        public async Task LoadAsync()
        {
            if (!IsEditMode) return;

            IsLoading = true;
            try
            {
                var contact = await _contactService.GetContactAsync(_contactId!.Value);
                if (contact == null) return;

                _lastName = contact.LastName;
                _firstName = contact.FirstName;
                _gsm = contact.Gsm;
                _phone = contact.Phone;
                _email = contact.Email;
                _rgpdConsentDate = contact.RgpdConsentDate;
                _isVolunteer = contact.RoleFlags.HasFlag(ContactRolesEnum.Volunteer);
                _isAdopter = contact.RoleFlags.HasFlag(ContactRolesEnum.Adopter);
                _isCandidate = contact.RoleFlags.HasFlag(ContactRolesEnum.Candidate);
                _isOther = contact.RoleFlags.HasFlag(ContactRolesEnum.Other);

                if (contact.Address != null)
                {
                    _street = contact.Address.Street;
                    _number = contact.Address.Number;
                    _box = contact.Address.Box;
                    _postCode = contact.Address.PostCode;
                    _city = contact.Address.City;
                    _country = contact.Address.Country;
                }

                OnPropertyChanged(nameof(LastName));
                OnPropertyChanged(nameof(FirstName));
                OnPropertyChanged(nameof(Gsm));
                OnPropertyChanged(nameof(Phone));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(RgpdConsentDate));
                OnPropertyChanged(nameof(IsVolunteer));
                OnPropertyChanged(nameof(IsAdopter));
                OnPropertyChanged(nameof(IsCandidate));
                OnPropertyChanged(nameof(IsOther));
                OnPropertyChanged(nameof(Street));
                OnPropertyChanged(nameof(Number));
                OnPropertyChanged(nameof(Box));
                OnPropertyChanged(nameof(PostCode));
                OnPropertyChanged(nameof(City));
                OnPropertyChanged(nameof(Country));
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
                var roleFlags = ContactRolesEnum.None;
                if (IsVolunteer) roleFlags |= ContactRolesEnum.Volunteer;
                if (IsAdopter) roleFlags |= ContactRolesEnum.Adopter;
                if (IsCandidate) roleFlags |= ContactRolesEnum.Candidate;
                if (IsOther) roleFlags |= ContactRolesEnum.Other;

                Address? address = null;
                if (!string.IsNullOrWhiteSpace(Street))
                {
                    address = new Address
                    {
                        Street = Street!,
                        Number = Number ?? string.Empty,
                        Box = Box,
                        PostCode = PostCode ?? string.Empty,
                        City = City ?? string.Empty,
                        Country = Country,
                    };
                }

                var contact = new Contact
                {
                    Id = _contactId ?? Guid.Empty,
                    LastName = LastName,
                    FirstName = FirstName,
                    Gsm = Gsm,
                    Phone = Phone,
                    Email = Email,
                    RoleFlags = roleFlags,
                    RgpdConsentDate = RgpdConsentDate,
                    Address = address,
                };

                if (IsEditMode)
                    await _contactService.UpdateContactAsync(contact);
                else
                    await _contactService.RegisterContactAsync(contact, NationalRegister);

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
