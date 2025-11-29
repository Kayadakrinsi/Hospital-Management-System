using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace HMSBAL.Common
{
    /// <summary>
    /// Global utility class for business logic layer
    /// </summary>
    public static class BLGlobalClass
    {
        #region Private Members

        /// <summary>
        /// Instacne of IConfigurationRoot to read appsettings.json
        /// </summary>
        static IConfigurationRoot _configuration;

        /// <summary>
        /// 64-character key (base64-safe string)
        /// </summary>
        static readonly string SecretKey = "Y8f2L0x9bT4Qm7Z2R1pS8vN5aE3uW6dK0cP9tX1yF5rG2hJ7nB4qM3lT9zD8sU2";

        #endregion

        #region Public Methods

        /// <summary>
        /// Get value from appsettings.json by key (supports nested keys using ':')
        /// </summary>
        public static string GetSetting(string key)
        {
            // Build configuration from appsettings.json
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())   // Set current directory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return _configuration[key];
        }

        /// <summary>
        /// Encrypts plain text using AES-256 and returns Base64 cipher text
        /// </summary>
        /// <param name="plainText">Plain text to be ecrypted</param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(SecretKey.Substring(0, 32)); // 256-bit key
                aes.GenerateIV(); // random IV for each encryption
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length); // prepend IV
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// Decrypts AES-256 Base64 cipher text back to plain text
        /// </summary>
        /// <param name="cipherText">Encrypted text to decrypt</param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(SecretKey.Substring(0, 32));
                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(fullCipher, iv, iv.Length);
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                int cipherStart = iv.Length;
                int cipherLength = fullCipher.Length - cipherStart;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(fullCipher, cipherStart, cipherLength))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Hashes a password using SHA-256 and returns Base64 string
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);

                // Convert to hexadecimal string instead of Base64
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        /// <summary>
        /// Copy properties from dto to poco model
        /// </summary>
        /// <typeparam name="TSource">DTO Model</typeparam>
        /// <typeparam name="TDestination">Poco Model</typeparam>
        /// <param name="source">Instance of DTO Model</param>
        /// <param name="destination">Instance of POCO Model</param>
        public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination)
        {
            if (source == null || destination == null)
                throw new ArgumentNullException("Source or Destination is null");

            var sourceProps = typeof(TSource)
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(p => p.CanRead);

            var destProps = typeof(TDestination)
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToDictionary(p => p.Name.ToLower());

            foreach (var sProp in sourceProps)
            {
                if (destProps.TryGetValue(sProp.Name.ToLower(), out var dProp)
                    && dProp.PropertyType == sProp.PropertyType)
                {
                    dProp.SetValue(destination, sProp.GetValue(source, null));
                }
            }
        }

        #endregion

    }

}