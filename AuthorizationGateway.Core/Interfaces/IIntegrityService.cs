namespace AuthorizationGateway.Core.Interfaces
{
    /// <summary>
    /// Defines methods for validating data integrity.
    /// </summary>
    public interface IIntegrityService
    {
        /// <summary>
        /// Validates if no one has changed the data by comparing it with the provided signature.
        /// </summary>
        bool Validate(string data, string signature);
    }
}
