using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Y2KWMS.Model
{
    public class User : BindableBase
    {
        private int id;
        private string firstName;
        private string lastName;
        private string mobile; 
        private string address;
        private string email;
        private string password;
        private string branch;
        private string type;
        private string status;


        public const String strPermutation = "ouiveyxaqtd";
        public const Int32 bytePermutation1 = 0x19;
        public const Int32 bytePermutation2 = 0x59;
        public const Int32 bytePermutation3 = 0x17;
        public const Int32 bytePermutation4 = 0x41;

        public User(int id)
        {
            this.id = id;
        }

        public User(string password)
        {
            this.Password = password;
        }

        public User(string email, string password, string type)
        {
            this.Email = email;
            this.Password = password;
            this.Type = type;

        }

        public User()
        {

        }
        public User(string firstName, string lastName, string mobile, string address, string email, string password, string branch, string type, string status)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Mobile = mobile;
            this.Address = address;
            this.Email = email;
            this.Password = password;
            this.Branch = branch;
            this.Type = type;
            this.Status = status;
        }

        public User(string id, string firstName, string lastName, string mobile, string address, string email, string password, string branch, string type, string status)
        {
            this.Id = int.Parse(id);
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Mobile = mobile;
            this.Address = address;
            this.Email = email;
            this.Password = password;
            this.Branch = branch;
            this.Type = type;
            this.Status = status;
        }

        public User(string email, string password)
        {
            this.Email = email;
            this.Password = password;
        }

        public int Id { get => id; set => id = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Mobile { get => mobile; set => mobile = value; }
        public string Address { get => address; set => address = value; }
        public string Email { get => email; set => email = value; }
        public string Password { get => password; set => password = Encrypt(value); }
        public string Branch { get => branch; set => branch = value; }
        public string Type { get => type; set => type = value; }
        public string Status { get => status; set => status = value; }

        //http://eddiejackson.net/wp/?p=13434

        // encoding
        public static string Encrypt(string strData)
        {

            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(strData)));
            // reference https://msdn.microsoft.com/en-us/library/ds4kkd55(v=vs.110).aspx

        }


        // decoding
        public static string Decrypt(string strData)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(strData)));
            // reference https://msdn.microsoft.com/en-us/library/system.convert.frombase64string(v=vs.110).aspx

        }

        // encrypt
        public static byte[] Encrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(strPermutation,
            new byte[] { bytePermutation1,
                            bytePermutation2,
                         bytePermutation3,
                        bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

        // decrypt
        public static byte[] Decrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(strPermutation,
            new byte[] { bytePermutation1,
                         bytePermutation2,
                         bytePermutation3,
                         bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }
    }
}
