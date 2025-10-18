namespace AuthorizationGateway.Core.Utils
{
    /// <summary>
    /// Provides functionality to mask sensitive data such as personal account numbers (PAN).
    /// </summary>
    /// <remarks>
    /// This class is designed to mask sensitive information by replacing the middle portion of a
    /// string with asterisks, while preserving the first and last four characters. It is useful for displaying
    /// sensitive data in a secure manner.
    /// </remarks>
    public static class SensitiveDataMasker
    {
        public static string? Mask(string? pan)
        {
            if (string.IsNullOrWhiteSpace(pan) ||
                pan.Length < 8)
            {
                return pan;
            }

            return $"{pan[..4]}********{pan[^4..]}";
        }
    }
}
