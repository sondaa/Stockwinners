using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebSite.Helpers
{
    public static class ApacheEncryption
    {
        private const string ValidSaltCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private const string ValidApache64RepresentationCharacters = "./0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// 1-1 port from the original algorithm. Why a more standard encoding is not used is beyond me.
        /// </summary>
        static private string ToApache64Representation(long v, int size)
        {
            StringBuilder result = new StringBuilder();

            while (--size >= 0)
            {
                result.Append(ValidApache64RepresentationCharacters[((int)(v & 0x3f))]);
                v >>= 6;
            }

            return result.ToString();
        }

        static private void ClearBits(byte[] bits)
        {
            for (int i = 0; i < bits.Length; i++)
            {
                bits[i] = 0;
            }
        }

        /// <summary>
        /// Convert an encoded unsigned byte value into a int with the unsigned value.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static private int Bytes2Unsigned(byte input)
        {
            return (int)input & 0xff;
        }


        /// <summary>
        /// Generate a Linux compatible MD5-encoded password based on the raw input. The output will be in form
        /// $1$salt$hash
        /// </summary>
        static public string Encrypt(string password)
        {
            StringBuilder salt = new StringBuilder();
            Random rand = new Random();

            while (salt.Length < 8)
            {
                int index = (int)(rand.NextDouble() * ValidSaltCharacters.Length);
                salt.Append(ValidSaltCharacters.Substring(index, index + 1));
            }

            return ApacheEncryption.Encrypt(password, salt.ToString());
        }

        static public string Encrypt(string password, string salt)
        {
            return ApacheEncryption.Encrypt(password, salt, "$1$");
        }


        /// <summary>
        /// Create an Apache compatible MD5 encoded password based off of the raw input. The output will be in form
        /// $apr1$salt$hash
        /// </summary>
        static public string ApacheEncrypt(string password)
        {
            StringBuilder salt = new StringBuilder();
            Random randgen = new Random();

            while (salt.Length < 8)
            {
                int index = (int)(randgen.NextDouble() * ValidSaltCharacters.Length);
                salt.Append(ValidSaltCharacters.Substring(index, index + 1));
            }

            return ApacheEncryption.ApacheEncrypt(password, salt.ToString());
        }

        static public string ApacheEncrypt(string password, string salt)
        {
            return ApacheEncryption.Encrypt(password, salt, "$apr1$");
        }

        static public string Encrypt(string password, string salt, string magic)
        {
            byte[] finalInput;
            MD5 md5 = MD5.Create();

            // We might be getting a complete hashed value as the salt parameter here. If that's the case, then
            // extract the real salt from the complete hashed value
            if (salt.StartsWith(magic))
            {
                salt = salt.Substring(magic.Length);
            }

            // Look for the salt value until we see a $ or hit a maximum of 8 characters
            if (salt.IndexOf('$') != -1)
            {
                salt = salt.Substring(0, salt.IndexOf('$'));
            }

            if (salt.Length > 8)
            {
                salt = salt.Substring(0, 8);
            }

            List<byte> pieceOne = new List<byte>();

            pieceOne.AddRange(Encoding.ASCII.GetBytes(password));  // The password first, since that is what is most unknown
            pieceOne.AddRange(Encoding.ASCII.GetBytes(magic));     // Then our magic string
            pieceOne.AddRange(Encoding.ASCII.GetBytes(salt));      // Then the raw salt

            /* Then just as many characters of the MD5(pw,salt,pw) */
            List<byte> pieceTwo = new List<byte>();
            pieceTwo.AddRange(Encoding.ASCII.GetBytes(password));
            pieceTwo.AddRange(Encoding.ASCII.GetBytes(salt));
            pieceTwo.AddRange(Encoding.ASCII.GetBytes(password));
            finalInput = md5.ComputeHash(pieceTwo.ToArray());

            for (int pl = password.Length; pl > 0; pl -= 16)
            {
                for (int i = 0; i < (pl > 16 ? 16 : pl); i++)
                {
                    pieceOne.Add(finalInput[i]);
                }
            }

            // The array is being cleared as a safety measure in the original algorithm, but setting all the bits to 0 here,
            // affects the loop below, so we still do this even though it's not in theory required since we are in C#
            ClearBits(finalInput);

            // The "weird" step from the algorithm
            for (int i = password.Length; i != 0; i >>= 1)
            {
                if ((i & 1) != 0)
                {
                    // Should really just be 0 since finalInput is set to 0s, but keeping this to keep the port 1-1 from Apache code
                    pieceOne.Add(finalInput[0]);
                }
                else
                {
                    pieceOne.Add(Encoding.ASCII.GetBytes(password)[0]);
                }
            }

            finalInput = md5.ComputeHash(pieceOne.ToArray());

            // Original comment from C file:
            /*
             * And now, just to make sure things don't run too fast..
             * On a 60 Mhz Pentium this takes 34 msec, so you would
             * need 30 seconds to build a 1000 entry dictionary...
             */
            for (int i = 0; i < 1000; i++)
            {
                pieceTwo.Clear();

                if ((i & 1) != 0)
                {
                    pieceTwo.AddRange(Encoding.ASCII.GetBytes(password));
                }
                else
                {
                    pieceTwo.AddRange(finalInput.ToList().GetRange(0, 16));
                }

                if ((i % 3) != 0)
                {
                    pieceTwo.AddRange(Encoding.ASCII.GetBytes(salt));
                }

                if ((i % 7) != 0)
                {
                    pieceTwo.AddRange(Encoding.ASCII.GetBytes(password));
                }

                if ((i & 1) != 0)
                {
                    pieceTwo.AddRange(finalInput.ToList().GetRange(0, 16));
                }
                else
                {
                    pieceTwo.AddRange(Encoding.ASCII.GetBytes(password));
                }

                finalInput = md5.ComputeHash(pieceTwo.ToArray());
            }

            StringBuilder result = new StringBuilder();

            result.Append(magic);
            result.Append(salt);
            result.Append("$");

            long l;

            l = (Bytes2Unsigned(finalInput[0]) << 16) | (Bytes2Unsigned(finalInput[6]) << 8) | Bytes2Unsigned(finalInput[12]);
            result.Append(ToApache64Representation(l, 4));

            l = (Bytes2Unsigned(finalInput[1]) << 16) | (Bytes2Unsigned(finalInput[7]) << 8) | Bytes2Unsigned(finalInput[13]);
            result.Append(ToApache64Representation(l, 4));

            l = (Bytes2Unsigned(finalInput[2]) << 16) | (Bytes2Unsigned(finalInput[8]) << 8) | Bytes2Unsigned(finalInput[14]);
            result.Append(ToApache64Representation(l, 4));

            l = (Bytes2Unsigned(finalInput[3]) << 16) | (Bytes2Unsigned(finalInput[9]) << 8) | Bytes2Unsigned(finalInput[15]);
            result.Append(ToApache64Representation(l, 4));

            l = (Bytes2Unsigned(finalInput[4]) << 16) | (Bytes2Unsigned(finalInput[10]) << 8) | Bytes2Unsigned(finalInput[5]);
            result.Append(ToApache64Representation(l, 4));

            l = Bytes2Unsigned(finalInput[11]);
            result.Append(ToApache64Representation(l, 2));

            // Again, unnecessary in C#, but keeping it to keep the port close to 1-1
            ClearBits(finalInput);

            return result.ToString();
        }

        /// <summary>
        /// Verifies that <paramref name="plainText"/> would result in <paramref name="hashedValue"/> if hashed based on
        /// the algorithm.
        /// </summary>
        static public bool VerifyPassword(string plainText, string hashedValue)
        {
            if (hashedValue.StartsWith("$1$"))
            {
                return hashedValue.Equals(ApacheEncryption.Encrypt(plainText, hashedValue));
            }
            else if (hashedValue.StartsWith("$apr1$"))
            {
                return hashedValue.Equals(ApacheEncryption.ApacheEncrypt(plainText, hashedValue));
            }
            else
            {
                throw new InvalidOperationException("Bad plain text input, does not conform to Apache.");
            }
        }
    }
}