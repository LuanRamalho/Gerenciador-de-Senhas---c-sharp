using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace Gerenciador_de_Senhas;
public static class CryptoHelper {
    private static readonly string KeyPath = "chave.key";

    public static byte[] ObterOuCriarChave() {
        if (!File.Exists(KeyPath)) {
            using (Aes aes = Aes.Create()) {
                aes.KeySize = 256;
                aes.GenerateKey();
                File.WriteAllBytes(KeyPath, aes.Key);
                return aes.Key;
            }
        }
        return File.ReadAllBytes(KeyPath);
    }

    public static byte[] Criptografar(string texto, byte[] chave) {
        using (Aes aes = Aes.Create()) {
            aes.Key = chave;
            aes.GenerateIV();
            using (var ms = new MemoryStream()) {
                ms.Write(aes.IV, 0, aes.IV.Length); // Salva IV no início do stream
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                    byte[] dados = Encoding.UTF8.GetBytes(texto);
                    cs.Write(dados, 0, dados.Length);
                }
                return ms.ToArray();
            }
        }
    }

    public static string Descriptografar(byte[] dadosCripto, byte[] chave) {
        using (Aes aes = Aes.Create()) {
            aes.Key = chave;
            using (var ms = new MemoryStream(dadosCripto)) {
                byte[] iv = new byte[aes.BlockSize / 8];
                ms.Read(iv, 0, iv.Length);
                aes.IV = iv;
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read)) {
                    using (var sr = new StreamReader(cs)) {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}