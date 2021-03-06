﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using HabboEncryption.Security.Cryptography;
using HabboEncryption.Utils;

namespace HabboEncryption
{
    public class HabboEncryption
    {
        private RSACrypto _rsa;
        private DiffieHellman _dh;

        private static HabboEncryption _instance;

        public static HabboEncryption GetInstance()
        {
            if (_instance == null)
            {
                throw new NullReferenceException("HabboEncryption not initialized");
            }

            return _instance;
        }

        public static HabboEncryption GetInstance(RSACParameters parameters, int dhBitLength)
        {
            if (_instance == null)
            {
                _instance = new HabboEncryption(parameters, dhBitLength);
            }

            return GetInstance();
        }


        public HabboEncryption(RSACParameters parameters, int dhBitLength)
        {
            this._rsa = new RSACrypto(parameters);
            this._dh = DiffieHellman.CreateInstance(dhBitLength);
        }

        private string GetRSAEncryptedString(string data, bool usePrivate)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] encryptedBytes = this._rsa.Encrypt(bytes, usePrivate);

            return Converter.BytesToHexString(bytes);
        }

        private string GetRSADecryptedString(string data, bool usePrivate)
        {
            byte[] bytes = Converter.HexStringToBytes(data);
            byte[] decryptedBytes = this._rsa.Decrypt(bytes, usePrivate);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public string GetRSADiffieHellmanPKey()
        {
            string key = this._dh.P.ToString();
            return this.GetRSAEncryptedString(key, true);
        }

        public string GetRSADiffieHellmanGKey()
        {
            string key = this._dh.G.ToString();
            return this.GetRSAEncryptedString(key, true);
        }

        public string GetRSADiffieHellmanPublicKey()
        {
            string key = this._dh.PublicKey.ToString();
            return this.GetRSAEncryptedString(key, true);
        }

        public BigInteger CalculateDiffieHellmanSharedKey(string publicKey)
        {
            publicKey = this.GetRSADecryptedString(publicKey, false);

            return this._dh.CalculateSharedKey(BigInteger.Parse(publicKey));
        }

        public static ARC4 InitializeARC(BigInteger sharedKey)
        {
            byte[] sharedKeyBytes = sharedKey.ToByteArray(false);

            return new ARC4(sharedKeyBytes);
        }
    }
}
