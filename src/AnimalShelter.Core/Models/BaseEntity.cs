namespace AnimalShelter.Core.Models
{
    public abstract class BaseEntity<TId>
    {
        /*
        Utilisation d'un type générique TId car l'Animal utilise un string (yymmdd...)
        alors que les autres tables utilisent des Guid.
        */
        public TId Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; } // Pour le Soft Delete        
    }
}
