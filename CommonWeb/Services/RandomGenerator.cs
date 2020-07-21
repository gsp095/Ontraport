using System;
using System.Security.Cryptography;
using System.Text;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Generates random numbers in a thread-safe way.
    /// </summary>
    /// <remarks>This class should be registered as Singleton.</remarks>
    public class RandomGenerator : IRandomGenerator
    {
        // This implementation solves the problems with multi-threading explained here
        // http://web.archive.org/web/20160326010328/http://blogs.msdn.com/b/pfxteam/archive/2009/02/19/9434171.aspx

        /// <summary>
        /// The seeds will be obtained in a more secure way using crypto, ensuring multiple seeds remain unique even if created at the same time.
        /// </summary>
        private static readonly RNGCryptoServiceProvider _global = new RNGCryptoServiceProvider();
        [ThreadStatic]
        private Random? _local;

        /// <summary>
        /// Returns an instance of Random in a thread-safe way.
        /// </summary>
        private Random GetInstance() => _local ?? new Random(GetSecureInt32());

        /// <summary>
        /// Returns a non-negative random integer.
        /// </summary>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than MaxValue.</returns>
        public int GetInt() => GetInstance().Next();

        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to 0.</param>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0, and less than maxValue; that is, the range of return values ordinarily includes 0 but not maxValue. However, if maxValue equals 0, maxValue is returned.</returns>
        public int GetInt(int maxValue) => GetInstance().Next(maxValue);

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A 32-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        public int GetInt(int minValue, int maxValue) => GetInstance().Next(minValue, maxValue);

        /// <summary>
        /// Returns a non-negative random integer with specified amount of digits.
        /// </summary>
        /// <param name="digits">The amount of digits to generate.</param>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0 and has up to specified number of digits.</returns>
        public int GetDigits(int digits) => GetInstance().Next(10 ^ digits);

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
        /// </summary>
        /// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
        public double GetDouble() => GetInstance().NextDouble();

        /// <summary>
        /// Returns an array of bytes with a cryptographically strong sequence of random values.
        /// </summary>
        /// <param name="length">The length of the byte array to generate and return.</param>
        /// <returns>An array of cryptographically strong random bytes.</returns>
        public byte[] GetSecureBytes(int length)
        {
            var buffer = new byte[length];
            _global.GetBytes(buffer);
            return buffer;
        }

        /// <summary>
        /// Returns a cryptographically strong random Int32 value.
        /// </summary>
        public int GetSecureInt32() => BitConverter.ToInt32(GetSecureBytes(4));

        /// <summary>
        /// Returns a cryptographically strong random Int64 value.
        /// </summary>
        public long GetSecureInt64() => BitConverter.ToInt64(GetSecureBytes(8));

        /// <summary>
        /// Returns a cryptographically strong random Single value.
        /// </summary>
        public float GetSecureSingle() => BitConverter.ToSingle(GetSecureBytes(4));

        /// <summary>
        /// Returns a cryptographically strong random Double value.
        /// </summary>
        public double GetSecureDouble() => BitConverter.ToDouble(GetSecureBytes(8));

        /// <summary>
        /// Returns a cryptographically strong token of alphanumerical values of specified length.
        /// </summary>
        /// <param name="length">The length of the token to generate.</param>
        /// <returns>A cryptographically strong random token.</returns>
        public string GetSecureToken(int length)
        {
            byte[] data = GetSecureBytes(4 * length);
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;
                result.Append(chars[idx]);
            }
            return result.ToString();
        }
        private static readonly char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
    }
}
