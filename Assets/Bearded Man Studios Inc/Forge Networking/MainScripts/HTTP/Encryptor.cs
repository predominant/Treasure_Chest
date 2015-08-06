/*-----------------------------+------------------------------\
|                                                             |
|                        !!!NOTICE!!!                         |
|                                                             |
|  These libraries are under heavy development so they are    |
|  subject to make many changes as development continues.     |
|  For this reason, the libraries may not be well commented.  |
|  THANK YOU for supporting forge with all your feedback      |
|  suggestions, bug reports and comments!                     |
|                                                             |
|                               - The Forge Team              |
|                                 Bearded Man Studios, Inc.   |
|                                                             |
|  This source code, project files, and associated files are  |
|  copyrighted by Bearded Man Studios, Inc. (2012-2015) and   |
|  may not be redistributed without written permission.       |
|                                                             |
\------------------------------+-----------------------------*/



using System;
using System.IO;
#if NETFX_CORE
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif
using System.Text;
using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Encryptor
{
	public static Encoding Encoding { get { return Encoding.UTF8; } }
	private const int KEY_SIZE = 32;

#if NETFX_CORE
	public static string Encrypt(string key, string data, out string iv)
	{
		// Private key has to be exactly 16 characters
		if (key.Length > KEY_SIZE)
		{
			// Cut of the end if it exceeds 16 characters
			key = key.Substring(0, KEY_SIZE);
		}
		else
		{
			// Append zero to make it 16 characters if the provided key is less
			while (key.Length < KEY_SIZE)
			{
				key += "0";
			}
		}

		// We'll be using AES, CBC mode with PKCS#7 padding to encrypt
		SymmetricKeyAlgorithmProvider aesCbcPkcs7 = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);

		// Convert the private key to binary
		IBuffer keymaterial = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);

		// Create the private key
		CryptographicKey k = aesCbcPkcs7.CreateSymmetricKey(keymaterial);

		// Creata a 16 byte initialization vector
		IBuffer ivBin = keymaterial;

		// Encrypt the data
		byte[] plainText = Encoding.UTF8.GetBytes(data); // Data to encrypt

		IBuffer buff = CryptographicEngine.Encrypt(k, CryptographicBuffer.CreateFromByteArray(plainText), ivBin);

		iv = CryptographicBuffer.EncodeToBase64String(ivBin);
		return CryptographicBuffer.EncodeToBase64String(buff);
	}

	public static string DecryptMessage(string text, string key)
	{
		if (key.Length > KEY_SIZE)
		{
			key = key.Substring(0, KEY_SIZE);
		}
		else
		{
			// Fill key
			while (key.Length < KEY_SIZE)
			{
				key += "0";
			}
		}

		IBuffer val = CryptographicBuffer.DecodeFromBase64String(text);

		// Use AES, CBC mode with PKCS#7 padding (good default choice)
		SymmetricKeyAlgorithmProvider aesCbcPkcs7 =
			SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);

		IBuffer keymaterial = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);

		// Create an AES 128-bit (16 byte) key
		CryptographicKey k = aesCbcPkcs7.CreateSymmetricKey(keymaterial);

		// Creata a 16 byte initialization vector
		IBuffer iv = keymaterial;// CryptographicBuffer.GenerateRandom(aesCbcPkcs7.BlockLength);

		//IBuffer val = CryptographicBuffer.DecodeFromBase64String(value);

		IBuffer buff = CryptographicEngine.Decrypt(k, val, iv);

		return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buff);
	}
#else
	#region Encryption
	public static string Encrypt(string key, string data, out string iv)
	{
		Rijndael aes = Rijndael.Create();
		aes.KeySize = 256;
		aes.BlockSize = 256;
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		aes.Key = Encryptor.Encoding.GetBytes(key);

		ICryptoTransform crypto = aes.CreateEncryptor(aes.Key, aes.IV);
		byte[] txt = Encryptor.Encoding.GetBytes(data);
		byte[] cipherText = crypto.TransformFinalBlock(txt, 0, txt.Length);

		iv = Convert.ToBase64String(aes.IV);
		return Convert.ToBase64String(cipherText);
	}

	public static string DecryptMessage(string text, string key)
	{
		RijndaelManaged aes = new RijndaelManaged();
		aes.KeySize = 256;
		aes.BlockSize = 256;
		aes.Padding = PaddingMode.Zeros;
		aes.Mode = CipherMode.CBC;

		aes.Key = Encryptor.Encoding.GetBytes(key);

		text = Encryptor.Encoding.GetString(Convert.FromBase64String(text), 0, text.Length);

		string IV = text;
		IV = IV.Substring(IV.IndexOf("-[--IV-[-") + 9);
		text = text.Replace("-[--IV-[-" + IV, "");

		text = Convert.ToBase64String(Encryptor.Encoding.GetBytes(text));
		aes.IV = Encryptor.Encoding.GetBytes(IV);

		ICryptoTransform AESDecrypt = aes.CreateDecryptor(aes.Key, aes.IV);
		byte[] buffer = Convert.FromBase64String(text);

		return Encryptor.Encoding.GetString(AESDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
	}
	#endregion
#endif
}