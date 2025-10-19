namespace AuthorizationGateway.Core.Tests.TestUtils
{
    internal static class TlvHelper
    {
        /// <summary>
        /// Helper builds a TLV fragment according to the repository's TlvParser implementation:
        /// tag (hex chars) + length (one byte as hex) + value (raw hex string)
        /// </summary>
        internal static string BuildTlv(string tag, string valueHex)
        {
            var lengthBytes = valueHex.Length / 2;
            var lengthHex = lengthBytes.ToString("X2");

            return string.Concat(tag, lengthHex, valueHex);
        }
    }
}
