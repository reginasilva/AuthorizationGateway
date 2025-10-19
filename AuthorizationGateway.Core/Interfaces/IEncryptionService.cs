namespace AuthorizationGateway.Core.Interfaces
{
    /// <summary>
    /// Provides methods for encrypting and decrypting text.
    /// </summary>
    /// <remarks>
    /// This interface defines methods for symmetric encryption and decryption, where the same key is
    /// used for both operations. Implementations of this interface are expected to handle key management and ensure
    /// secure encryption practices.
    /// </remarks>
    public interface IEncryptionService
    {
        /// <summary>
        /// Encrypts the specified plain text using the configured encryption algorithm.
        /// </summary>
        /// <param name="plainText">The plain text to encrypt.</param>
        /// <returns>The encrypted text as a string.</returns>
        string Encrypt(string plainText);

        /// <summary>
        /// Decrypts the specified cipher text and returns the original plain text.
        /// </summary>
        /// <param name="cipherText">The encrypted text to decrypt.</param>
        /// <returns>The decrypted plain text.</returns>
        string Decrypt(string cipherText);
    }
}
