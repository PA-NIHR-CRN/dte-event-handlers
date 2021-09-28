namespace Common.Interfaces
{
    public interface ICryptoService
    {
        byte[] Encrypt(string plainText);
        byte[] Encrypt(string plainText, byte[] key, byte[] iv);
        string Decrypt(byte[] cipherText);
        string Decrypt(byte[] cipherText, byte[] key, byte[] iv);
    }
}