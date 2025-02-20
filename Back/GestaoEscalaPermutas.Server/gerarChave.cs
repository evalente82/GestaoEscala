using System;
using System.Security.Cryptography;
using System.Text;

public class gerarChave
{
    public void teste()
    {
        using var rng = new RNGCryptoServiceProvider();
        byte[] secretKey = new byte[32]; // 🔹 256 bits (32 bytes)
        rng.GetBytes(secretKey);

        string chaveBase64 = Convert.ToBase64String(secretKey);
        Console.WriteLine($"🔑 SUA NOVA CHAVE SECRETA (Base64): {chaveBase64}");
    }
}
