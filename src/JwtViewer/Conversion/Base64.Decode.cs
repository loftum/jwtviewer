using System.Text;

namespace JwtViewer.Conversion;

public static partial class Base64
{
    public static byte[] Decode(string base64)
    {
        if (base64 == null)
        {
            throw new ArgumentNullException(nameof(base64));
        }
        return Convert.FromBase64String(base64);
    }

    public static byte[] UrlDecode(string base64UrlFriendly)
    {
        if (base64UrlFriendly == null)
        {
            throw new ArgumentNullException(nameof(base64UrlFriendly));
        }

        var bytes = Encoding.UTF8.GetBytes(base64UrlFriendly);
        using var stream = new MemoryStream(bytes);
        return UrlDecode(stream);
    }
    
    public static byte[] UrlDecode(MemoryStream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        var byteCount = stream.Length;
        var remainder = byteCount % 4;
        var length = byteCount / 4 * 3 + remainder;
        var array = new byte[length];

        var buffer = new byte[4];
        var index = 0;
        int read;

        while ((read = stream.Read(buffer)) > 0)
        {
            switch (read)
            {
                case 1:
                {
                    // 1 remainder byte. Odd.
                    // Invalid base64url string.
                    //
                    // ¯\_(ツ)_/¯
                    //
                    // Example:
                    // 
                    // Q (00_010000)
                    // (2 "prefix" bits are stripped away)
                    //
                    // converts to
                    //
                    // 01000000 (@)
                    buffer[0] = Utf8ToBase64Url[buffer[0]];
                    
                    var first = (byte) (buffer[0] << 2 & 0b11111100);
                    array[index++] = first;
                    
                    break;
                }

                case 2:
                {
                    // 2 remainder bytes.
                    //
                    // Example:
                    // 
                    // Q (00_010000)
                    // U (00_010100)
                    // (2 "prefix" bits are stripped away)
                    //
                    // converts to
                    //
                    // 01000001 (A)
                    // 01000000 (@)
                    
                    buffer[0] = ToBase64Url(buffer[0]);
                    buffer[1] = ToBase64Url(buffer[1]);
                    
                    var first = (byte) ((buffer[0] << 2 & 0b11111100) | buffer[1] >> 4 & 0b00000011);
                    var second = (byte) (buffer[1] << 4 & 0b11110000);

                    array[index++] = first;
                    array[index++] = second;
                    break;
                }
                    
                case 3:
                {
                    // 3 remainder bytes.
                    //
                    // Example:
                    // 
                    // Q (00_010000)
                    // U (00_010100)
                    // J (00_001001)
                    // (2 "prefix" bits are stripped away)
                    //
                    // converts to
                    //
                    // 01000001 (A)
                    // 01000010 (B)
                    // 01000000 (@)
                    
                    buffer[0] = ToBase64Url(buffer[0]);
                    buffer[1] = ToBase64Url(buffer[1]);
                    buffer[2] = ToBase64Url(buffer[2]);
                    
                    var first = (byte) ((buffer[0] << 2 & 0b11111100) | buffer[1] >> 4 & 0b00000011);
                    var second = (byte) ((buffer[1] << 4 & 0b11110000) | buffer[2] >> 2 & 0b00001111);
                    var third = (byte) (buffer[2] << 6 & 0b11000000);
                    
                    array[index++] = first;
                    array[index++] = second;
                    array[index++] = third;
                    
                    break;
                }
                case 4:
                {
                    // Normal case. 4 6-bit "bytes" are perfectly transformed to 3 bytes. Lovely.
                    //
                    // Example:
                    // 
                    // Q (00_010000)
                    // U (00_010100)
                    // J (00_001001)
                    // D (00_000011)
                    // (2 "prefix" bits are stripped away)
                    //
                    // converts to
                    //
                    // 01000001 (A)
                    // 01000010 (B)
                    // 01000011 (C)

                    buffer[0] = ToBase64Url(buffer[0]);
                    buffer[1] = ToBase64Url(buffer[1]);
                    buffer[2] = ToBase64Url(buffer[2]);
                    buffer[3] = ToBase64Url(buffer[3]);
                    
                    var first = (byte) ((buffer[0] << 2 & 0b11111100) | buffer[1] >> 4 & 0b00000011);
                    var second = (byte) ((buffer[1] << 4 & 0b11110000) | buffer[2] >> 2 & 0b00001111);
                    var third = (byte) ((buffer[2] << 6 & 0b11000000) | buffer[3] & 0b00111111);

                    array[index++] = first;
                    array[index++] = second;
                    array[index++] = third;
                    
                    break;
                }
            }
        }

        return array;
    }
}