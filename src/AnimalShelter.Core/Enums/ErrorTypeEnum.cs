namespace AnimalShelter.Core.Enums
{
    public enum ErrorTypeEnum
    {
        DatabaseError,    // Erreur SQL, connexion perdue, etc.
        ValidationError,  // Données invalides (ex: nom trop court)
        NotFound,         // Ressource inexistante
        Conflict,         // Doublon (ex: même ID animal)
        InternalError     // Erreur inattendue
    }
}
