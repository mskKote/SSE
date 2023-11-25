using System.Security.Cryptography;
using System.Text;

namespace task_4_3_2_HttpLib;

public static class Encryption
{
    public static string Encrypt(string input, string key)
    {
        byte[] results;
        var UTF8 = new UTF8Encoding();
        var HashProvider = new MD5CryptoServiceProvider();
        var TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(key));
        var TDESAlgorithm = new TripleDESCryptoServiceProvider();
        TDESAlgorithm.Key = TDESKey;
        TDESAlgorithm.Mode = CipherMode.ECB;
        TDESAlgorithm.Padding = PaddingMode.PKCS7;
        var dataToEncrypt = UTF8.GetBytes(input);
        try
        {
            var encryptor = TDESAlgorithm.CreateEncryptor();
            results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
        }
        finally
        {
            TDESAlgorithm.Clear();
            HashProvider.Clear();
        }

        return Convert.ToBase64String(results);
    }

    public static string Decrypt(string input, string key)
    {
        byte[] results;
        var UTF8 = new UTF8Encoding();
        var HashProvider = new MD5CryptoServiceProvider();
        var TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(key));
        var TDESAlgorithm = new TripleDESCryptoServiceProvider();
        TDESAlgorithm.Key = TDESKey;
        TDESAlgorithm.Mode = CipherMode.ECB;
        TDESAlgorithm.Padding = PaddingMode.PKCS7;
        var dataToDecrypt = Convert.FromBase64String(input);
        try
        {
            var decryptor = TDESAlgorithm.CreateDecryptor();
            results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
        }
        finally
        {
            TDESAlgorithm.Clear();
            HashProvider.Clear();
        }

        return UTF8.GetString(results);
    }
}