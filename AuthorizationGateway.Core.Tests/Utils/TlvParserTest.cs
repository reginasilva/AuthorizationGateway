using AuthorizationGateway.Core.Tests.TestUtils;
using AuthorizationGateway.Core.Utils;

namespace AuthorizationGateway.Core.Tests.Utils
{
    public class TlvParserTest
    {
        [Fact]
        public void TlvParser_Parse_NoTag()
        {
            var tags = TlvParser.Parse("");

            Assert.NotNull(tags);
            Assert.Empty(tags);
        }

        [Fact]
        public void TlvParser_Parse_ParsesSimpleTag()
        {
            // value is ASCII-hex for "12345678" (each char two hex digits)
            var panAsciiHex = "3132333435363738";
            var tlv = TlvHelper.BuildTlv("5A", panAsciiHex);

            var tags = TlvParser.Parse(tlv);

            Assert.NotNull(tags);
            Assert.Single(tags);

            Assert.True(tags.ContainsKey("5A"));
            Assert.Equal(panAsciiHex, tags["5A"]);
        }

        [Fact]
        public void TlvParser_Parse_MultiSingleTags()
        {
            var panAsciiHex = "3132333435363738";
            var tlv = TlvHelper.BuildTlv("5A", panAsciiHex);

            var amountAsciiHex2 = "3132333435363739";
            tlv += TlvHelper.BuildTlv("9f02", amountAsciiHex2);

            var tags = TlvParser.Parse(tlv);

            Assert.NotNull(tags);
            Assert.Equal(2, tags.Count);

            Assert.True(tags.ContainsKey("5A"));
            Assert.Equal(panAsciiHex, tags["5A"]);

            Assert.True(tags.ContainsKey("9F02"));
            Assert.Equal(amountAsciiHex2, tags["9F02"]);
        }

        [Fact]
        public void TlvParser_Parse_MultiEqualTags()
        {
            var panAsciiHex = "3132333435363738";
            var tlv = TlvHelper.BuildTlv("5A", panAsciiHex);

            var amountAsciiHex2 = "3132333435363739";
            tlv += TlvHelper.BuildTlv("9f02", amountAsciiHex2);

            var amountAsciiHex3 = "3132333435363730";
            tlv += TlvHelper.BuildTlv("9f02", amountAsciiHex3);

            var tags = TlvParser.Parse(tlv);

            Assert.NotNull(tags);
            Assert.Equal(2, tags.Count);

            Assert.True(tags.ContainsKey("5A"));
            Assert.Equal(panAsciiHex, tags["5A"]);

            Assert.True(tags.ContainsKey("9F02"));
            Assert.Equal(amountAsciiHex3, tags["9F02"]);
        }

        [Fact]
        public void TlvParser_Parse_ParsesComplexTag()
        {
            var tlv = "9f02060000000001809f150212349f01031234565a051234567890570a0102030405061d2412235f340212349c01005f200212349f0b021234560212349f20021234990212349f4b02123493021234";

            var tags = TlvParser.Parse(tlv);

            Assert.NotNull(tags);

            Assert.Equal(14, tags.Count);

            // Tag  - Length - Value
            // 9f02 -   06   - 000000000180
            // 9f15 -   02   - 1234
            // 9f01 -   03   - 123456
            // 5a   -   05   - 1234567890
            // 57   -   0a   - 0102030405061d241223
            // 5f34 -   02   - 1234
            // 9c   -   01   - 00
            // 5f20 -   02   - 1234
            // 9f0b -   02   - 1234
            // 56   -   02   - 1234
            // 9f20 -   02   - 1234
            // 99   -   02   - 1234
            // 9f4b -   02   - 1234
            // 93   -   02   - 1234
            Assert.True(tags.ContainsKey("9F02"));
            Assert.Equal("000000000180", tags["9F02"]);

            Assert.True(tags.ContainsKey("9F15"));
            Assert.Equal("1234", tags["9F15"]);

            Assert.True(tags.ContainsKey("9F01"));
            Assert.Equal("123456", tags["9F01"]);

            Assert.True(tags.ContainsKey("5A"));
            Assert.Equal("1234567890", tags["5A"]);

            Assert.True(tags.ContainsKey("57"));
            Assert.Equal("0102030405061d241223", tags["57"]);

            Assert.True(tags.ContainsKey("5F34"));
            Assert.Equal("1234", tags["5F34"]);

            Assert.True(tags.ContainsKey("9C"));
            Assert.Equal("00", tags["9C"]);

            Assert.True(tags.ContainsKey("5F20"));
            Assert.Equal("1234", tags["5F20"]);

            Assert.True(tags.ContainsKey("9F0B"));
            Assert.Equal("1234", tags["9F0B"]);

            Assert.True(tags.ContainsKey("56"));
            Assert.Equal("1234", tags["56"]);

            Assert.True(tags.ContainsKey("9F20"));
            Assert.Equal("1234", tags["9F20"]);

            Assert.True(tags.ContainsKey("99"));
            Assert.Equal("1234", tags["99"]);

            Assert.True(tags.ContainsKey("9F4B"));
            Assert.Equal("1234", tags["9F4B"]);

            Assert.True(tags.ContainsKey("93"));
            Assert.Equal("1234", tags["93"]);
        }
    }
}
