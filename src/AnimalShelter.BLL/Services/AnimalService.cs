namespace AnimalShelter.BLL.Services;

public class AnimalService
{
    private readonly IAnimalRepository _repository;

    public AnimalService(IAnimalRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> AddAnimalAsync(Animal animal)
    {
        // Validation métier corporate
        if (string.IsNullOrWhiteSpace(animal.Name))
            throw new ArgumentException("Animal name is required.");

        if (animal.BirthDate > DateTime.Now)
            throw new ArgumentException("Birth date cannot be in the future.");

        return await _repository.AddAsync(animal);
    }

    public async Task<IEnumerable<Animal>> GetActiveAnimalsAsync()
    {
        return await _repository.GetAllActiveAsync();
    }
}