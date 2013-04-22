using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using DataAnnotationsExtensions;

namespace WebSite.Models
{
    public class CreditCard
    {
        [Key]
        public int CreditCardId { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Cardholder's first name")]
        public string CardholderFirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Cardholder's last name")]
        public string CardholderLastName { get; set; }

        [Required]
        [Display(Name = "Credit card number")]
        public string Number { get; set; }

        [Required]
        [Display(Name = "Expiration Month")]
        public short ExpirationMonth { get; set; }

        [Required]
        [Display(Name = "Expiration Year")]
        public short ExpirationYear { get; set; }

        [MaxLength(4, ErrorMessage = "CVV must be at most 4 digits.")]
        [MinLength(3, ErrorMessage = "CVV must be at least 3 digits.")]
        public string CVV { get; set; }

        [ForeignKey("BillingAddress")]
        public int AddressId { get; set; }
        public virtual Address BillingAddress { get; set; }

        /// <summary>
        /// Encrypts the credit card's number.
        /// </summary>
        public void Encrypt()
        {
            using (RijndaelManaged rijndaelAlgorithm = new RijndaelManaged())
            {
                rijndaelAlgorithm.Key = this.GetKey(rijndaelAlgorithm.LegalKeySizes);
                rijndaelAlgorithm.IV = this.GetInitializationVector();

                ICryptoTransform encryptor = rijndaelAlgorithm.CreateEncryptor();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(this.Number);
                        }

                        this.Number = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts the current user's credit card information
        /// </summary>
        public void Decrypt()
        {
            using (RijndaelManaged rijndaelAlgorithm = new RijndaelManaged())
            {
                rijndaelAlgorithm.Key = this.GetKey(rijndaelAlgorithm.LegalKeySizes);
                rijndaelAlgorithm.IV = this.GetInitializationVector();

                ICryptoTransform decryptor = rijndaelAlgorithm.CreateDecryptor();

                using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(this.Number)))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            this.Number = reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public bool IsExpired()
        {
            DateTime now = DateTime.UtcNow;

            return this.ExpirationYear < now.Year || (this.ExpirationYear == now.Year && this.ExpirationMonth >= now.Month);
        }

        private byte[] GetKey(KeySizes[] validKeySizes)
        {
            // Ensure the supplied key is compatible with Rinjndael (256 bits)
            return Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["RijndaelKey"]).Take(256 / 8).ToArray();
        }

        private byte[] GetInitializationVector()
        {
            // Initialization vector must be a maximum of 128 bits
            return Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["RijndaelIV"]).Take(128 / 8).ToArray();
        }
    }
}