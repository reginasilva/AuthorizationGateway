namespace AuthorizationGateway.Core.Utils
{

    public static class TlvParser
    {
        /// <summary>
        /// Parses a hexadecimal string representing EMV data into a dictionary of tag-value pairs.
        /// </summary>
        /// <remarks>
        /// The method processes the input string by extracting tags and their corresponding
        /// values based on the EMV specification. 
        /// Each tag is expected to be followed by a length byte, which indicates the number of bytes in the value.
        /// </remarks>
        /// <param name="emvHex">A hexadecimal string containing EMV data. The string must be a valid sequence of EMV tags and values.</param>
        /// <returns>A dictionary where each key is an EMV tag and each value is the corresponding data in hexadecimal format.</returns>
        public static Dictionary<string, string> Parse(string emvHex)
        {
            var tags = new Dictionary<string, string>();

            for (int i = 0; i < emvHex.Length;)
            {
                // Tag: first two positions (5A, 9F, etc.)
                var tag = emvHex.Substring(i, 2);
                i += 2;

                // Tag value size (9F02 -> 02 means 2 bytes = 4 hex chars)
                var lengthHex = emvHex.Substring(i, 2);
                i += 2;

                // Calculate length and extract value
                var length = Convert.ToInt32(lengthHex, 16) * 2;
                var value = emvHex.Substring(i, length);

                // Move to the beginning of the next tag
                i += length;

                // Store tag and value, it's like this because if a tag appears twice, the last one needs to be taken
                tags[tag.ToUpper()] = value;
            }

            return tags;
        }
    }
}
