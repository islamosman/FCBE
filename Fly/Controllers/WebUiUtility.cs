using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Fly.BLL;
using Fly.DomainModel;

namespace Fly.WebUI.Helpers
{
    public static class WebUiUtility
    {
        private static readonly byte[] initVectorBytes = Encoding.ASCII.GetBytes("MAKV2SPBNI36FLY");
        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;
        static string passPhrase = "&$#$%$";

        #region Encryption actions
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI36FLY";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI36FLY";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        #endregion

        #region session keys
        public static string Culture
        {
            get
            {
                if (HttpContext.Current.Session["CurrentCulture"] != null)
                {
                    return HttpContext.Current.Session["CurrentCulture"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["CurrentCulture"] = "ar-EG";
                    return HttpContext.Current.Session["CurrentCulture"].ToString();
                }
            }
            set
            {
                HttpContext.Current.Session["CurrentCulture"] = value;
            }
        }


        public static SecurityUser CurrentUser
        {
            //get { return new SecurityUser() { Id = 3 }; }
            get
            {
                if (HttpContext.Current == null)
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)) //session expired but user name saved in cockie
                    {
                        try
                        {
                            SecurityUser cockieUser = new SecurityUserRepository().GetUser(HttpContext.Current.User.Identity.Name);
                            HttpContext.Current.Session["CurrentUser"] = cockieUser;
                        }
                        catch (Exception)
                        {
                            return new SecurityUser();
                        }
                    }
                    else
                    {
                        System.Web.Security.FormsAuthentication.SignOut();
                        return new SecurityUser();
                    }
                }
                if (HttpContext.Current.Session == null)
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)) //session expired but user name saved in cockie
                    {
                        try
                        {
                            SecurityUser cockieUser = new SecurityUserRepository().GetUser(HttpContext.Current.User.Identity.Name);
                            HttpContext.Current.Session["CurrentUser"] = cockieUser;
                        }
                        catch (Exception)
                        {
                            return new SecurityUser();
                        }
                    }
                    else
                    {
                        System.Web.Security.FormsAuthentication.SignOut();
                        return new SecurityUser();
                    }
                }
                if (HttpContext.Current.Session["CurrentUser"] == null)
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)) //session expired but user name saved in cockie
                    {
                        try
                        {
                            SecurityUser cockieUser = new SecurityUserRepository().GetUser(HttpContext.Current.User.Identity.Name);
                            HttpContext.Current.Session["CurrentUser"] = cockieUser;
                        }
                        catch (Exception)
                        {
                            return new SecurityUser();
                        }
                    }
                    else
                    {
                        System.Web.Security.FormsAuthentication.SignOut();
                        return new SecurityUser();
                    }
                }

                return (SecurityUser)HttpContext.Current.Session["CurrentUser"];
            }
            set
            {
                HttpContext.Current.Session["CurrentUser"] = value;
            }
        }

        //public static SecurityRole CurrentUserRole
        //{
        //    get
        //    {
        //        SecurityUser checkCurrentUser = CurrentUser;
        //        if (string.IsNullOrEmpty(checkCurrentUser.UserName))
        //        {
        //            return new SecurityRole();
        //        }
        //        else
        //        {
        //            using (SecurityUserRoleProxy secUserRoleProxy = new SecurityUserRoleProxy())
        //            {
        //                return secUserRoleProxy.GetByUserId(checkCurrentUser.Id).SecurityRole;
        //            }
        //        }
        //    }
        //}
        #endregion

        #region Attachments
        // Convert Stream to byte
        public static byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
        #endregion

        #region formatDate

        public static DateTime FormateTime(string timeVar, int year, int month, int day)
        {
            if (string.IsNullOrEmpty(timeVar))
            {
                return DateTime.Now;
            }
            else
            {
                var splitTimeString = timeVar.Split(new[] { ' ' });
                var timeOnly = splitTimeString[0].Split(new[] { ':' });

                return new DateTime(year, month, day, int.Parse(timeOnly[0]), int.Parse(timeOnly[1]), 0);

            }
        }
        #endregion






    }

    public class AttachmentUtility
    {
        public static byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

    }
}