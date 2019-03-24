using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fly.Helper
{
    public static class StringHelper
    {
        public static string DisplayFormatedValue(this int val)
        {
            return val.ToString("D2");
        }

        public static bool ValidateFullName(this string name)
        {

            bool valid = true;

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (name.Length > 35)
                valid = false;

            if ((name.Split(' ')).Length < 4)
                valid = false;

            return valid;
        }
        
        public static string GetDisplayName(this Enum value)
        {
            if (value == null)
                return "-";
            var type = value.GetType();
            if (!type.IsEnum) throw new ArgumentException(String.Format("Type '{0}' is not Enum", type));

            var members = type.GetMember(value.ToString());
            if (members.Length == 0) throw new ArgumentException(String.Format("Member '{0}' not found in type '{1}'", value, type.Name));

            var member = members[0];
            var attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes.Length == 0) throw new ArgumentException(String.Format("'{0}.{1}' doesn't have DisplayAttribute", type.Name, value));

            var attribute = (DisplayAttribute)attributes[0];
            return attribute.GetName();
        }
        public static AccountNumberItems SplitAccountNumber(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
                return new AccountNumberItems();
            if (!accountNumber.Contains("-"))
                return new AccountNumberItems();

            var items = accountNumber.Split('-');

            if (items.Length < 3)
                return new AccountNumberItems();

            return new AccountNumberItems()
            {
                BranchNo = items[0],
                CustomerNo = items[1],
                Suffix = items[2]
            };
        }


        public static AccountNumberItems SplitAccountNumber13(string accountNumber)
        {
            if (accountNumber == null)
                return new AccountNumberItems();
            if (accountNumber.Length != 13)
                return new AccountNumberItems();


            var acc = new AccountNumberItems();




            acc.BranchNo = accountNumber.Substring(0, 4);
            acc.CustomerNo = accountNumber.Substring(4, 6);
            acc.Suffix = accountNumber.Substring(10, 3);
            return acc;
        }

        
        public static string CreditCardFormat(string creditCardNo)
        {
            if (string.IsNullOrEmpty(creditCardNo))
                throw new ArgumentNullException("creditCardNo");
            var first4 = creditCardNo.Substring(0, 4);
            //string middlePart = creditCardNo.Substring(4, creditCardNo.Length - 8);
            var last4 = creditCardNo.Substring(creditCardNo.Length - 8, 4);
            return first4 + "-" + "******" + "-" + last4;
        }

        public static string DisplayAccountNumber(string str)
        {
            //if (str.Length > 5)
            //{
            //    var appeared = str.Substring(str.Length - 5);
            //    return $"XXXX-XXXXX{appeared}";
            //}
            return str;
        }

        public static string SplitCapitalizedWords(this string source, bool perceiveAppreviations = true)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            var newText = new StringBuilder(source.Length * 2);
            newText.Append(source[0]);
            for (var i = 1; i < source.Length; i++)
            {
                if (char.IsUpper(source[i]))
                {
                    if (perceiveAppreviations)
                    {
                        if (i == source.Length - 1)
                        {
                            newText.Append(source[i]);
                            continue;
                        }

                        if (char.IsUpper(source[i - 1]) && char.IsUpper(source[i + 1]))
                        {
                            newText.Append(source[i]);
                            continue;
                        }

                        newText.Append(' ');
                    }
                    else
                    {
                        newText.Append(' ');
                    }
                }
                newText.Append(source[i]);
            }
            return newText.ToString();
        }
        public static string ToTitleCase(this string word)
        {
            var txtInfo = new CultureInfo("en-US", false).TextInfo;
            if (txtInfo == null)
                throw new ArgumentNullException("txtInfo");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("word");
            return txtInfo.ToTitleCase(word.ToLower());
        }

        public static string ToReadableText(this string text, bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
            if (text.Contains("IScoreNote"))
            {
                return "iScore Note";
            }
            if (text.Contains("IScore"))
            {
                return "iScore";
            }

            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            var newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (var i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }

            return newText.ToString().Replace("_slash_", "/").Replace("_and_", " & ").Replace("_", " ");

        }
        public static bool IsEmail(this string text)
        {
            //  var pattern = /^ ([a - z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
            var r = Regex.IsMatch(text,
                @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                RegexOptions.IgnoreCase);
            return r;
            // return  Regex.IsMatch(text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

        }
        public static bool IsMobileNo(this string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (text.Length == 10)
            {
                text = "0" + text;
            }
            var reg = new Regex("^(01)[0-2-5]{1}[0-9]{8,10}");
            return !string.IsNullOrEmpty(text) && reg.IsMatch(text) && text.Length <= 14;
        }

        public static bool IsHomePhoneNo(this string text)
        {
            var reg = new Regex(@"^[0-9]{5,9}");
            return !string.IsNullOrEmpty(text) && reg.IsMatch(text) && text.Length <= 9;
        }
        public static bool IsValidCustomerBasicAccountNo(this string text)
        {
            if (text.Length != 6)
                return false;
            var r = Regex.IsMatch(text,
                @"^[0-9]",
                RegexOptions.IgnoreCase);
            return r;

        }
        public static bool IsValidCompanyCode(this string text)
        {
            if (text.Length != 5)
                return false;
            var r = Regex.IsMatch(text,
                @"^[0-9]",
                RegexOptions.IgnoreCase);
            return r;

        }

        public static bool IsEnglishName(this string text)
        {
            var number = IsNumber(text);
            var english = IsEnglish(text);
            var special = IsSpecial(text);
            var forbidden = IsForbidden(text);
            var space = IsSpace(text);
            var arabic = IsArabic(text);


            var r = (!(special || forbidden || arabic) && ((number || space || english)));
            return r;

        }

        public static bool IsIban(this string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (text.Length < 4) return false;
            if (text.Length > 35) return false;

            var number = IsNumber(text);
            var english = IsEnglish(text);
            var special = IsSpecial(text, true);
            var forbidden = IsForbidden(text);
            var space = IsSpace(text);
            var arabic = IsArabic(text);
            var dash = IsDash(text);


            var r = (((!(special || forbidden || arabic))) && ((number || dash || space || english)));
            return r;

        }


        public static bool IsValidUserName(this string text)
        {
            /*
            var number = isNumber(text);
            var english = isEnglish(text);
            var special = isSpecial(text);
            var forbidden = isForbidden(text);
            var space = isSpace(text);
            var arabic = isArabic(text);


            var r = (!(space || special || forbidden || arabic) && ((number   || english)));
            return r;*/

            var r = Regex.IsMatch(text, @"
    # Validate username with 5 constraints.
    ^                          # Anchor to start of string.
    # 1- only contains alphanumeric characters , underscore and dot.
    # 2- underscore and dot can't be at the end or start of username,
    # 3- underscore and dot can't come next to each other.
    # 4- each time just one occurrence of underscore or dot is valid.
    (?=[A-Za-z0-9]+(?:[_.][A-Za-z0-9]+)*$)
    # 5- number of characters must be between 8 to 15.
    [A-Za-z0-9_.]{8,15}        # Apply constraint 5.
    $                          # Anchor to end of string.
    ", RegexOptions.IgnorePatternWhitespace);

            return r;

        }
        public static bool IsValidPassword(this string text)
        {
            var r = (Regex.IsMatch(text, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&\.'])[^ ]{8,15}$"));
            if (r)
            {
                if (string.IsNullOrEmpty(text))
                    throw new ArgumentNullException("text");
                char[] characters = text.ToCharArray();


                for (int i = 0; i < characters.Length - 2; i++)
                {
                    if (i <= characters.Length - 2)
                        if (characters[i] == characters[i + 1] && characters[i] == characters[i + 2])
                        {
                            r = false;
                            break;
                        }
                }

            }

            return r;
        }

        public static bool IsSpace(this string text)
        {
            return (!string.IsNullOrEmpty(text) &&
                    (Regex.IsMatch(text, @"[\x20]+")));
        }
        public static bool IsDash(this string text)
        {
            return (!string.IsNullOrEmpty(text) &&
                    (Regex.IsMatch(text, @"[\x2D]+")));
        }
        public static bool IsArabic(this string text)
        {
            var regex = new Regex("[\u0600-\u06ff]|[\u0750-\u077f]|[\ufb50-\ufc3f]|[\ufe70-\ufefc]");

            return (!string.IsNullOrEmpty(text) &&
                    (regex.IsMatch(text)));
        }
        public static bool IsDigit(this string text)
        {
            var noSpecialChar = new StringBuilder();
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            foreach (var c in text)
            {
                if (c >= '0' && c <= '9')
                {
                    noSpecialChar.Append(c);
                }
            }
            if (noSpecialChar.Length == text.Length)
            {
                return true;
            }
            return false;
            // return (!string.IsNullOrEmpty(text) &&
            // (Regex.IsMatch(text, @"[\x30-\x39]+")));
        }

        public static bool IsNumber(this string text)
        {
            return (!string.IsNullOrEmpty(text) &&
           (Regex.IsMatch(text, @"[\x30-\x39]+")));
        }
        public static bool IsEnglish(this string text)
        {
            return (!string.IsNullOrEmpty(text) &&
                    (Regex.IsMatch(text, @"[\x41-\x5A]+") || Regex.IsMatch(text, @"[\x61-\x7A]+")));
        }
        public static bool IscontainCaptailandSmail(this string text)
        {
            return (!string.IsNullOrEmpty(text) &&
                    (Regex.IsMatch(text, @"[\x41-\x5A]+") && Regex.IsMatch(text, @"[\x61-\x7A]+")));
        }
        public static bool IsSpecial(this string text, bool allowdash = false)
        {
            return (!string.IsNullOrEmpty(text) &&
                (Regex.IsMatch(text, @"[\x21-\x2c]+") || Regex.IsMatch(text, @"[\x2e-\x2f]+") ||
                 Regex.IsMatch(text, @"[\x3a-\x40]+") || Regex.IsMatch(text, @"[\x5b-\x60]+")) || (allowdash ? false : Regex.IsMatch(text, @"[\x2D]+")));

        }
        public static bool IsForbidden(this string text)
        {
            return (!string.IsNullOrEmpty(text) &&
             (Regex.IsMatch(text, @"[\x00-\x1f]+") || Regex.IsMatch(text, @"[\x7b-\x7f]+")));
        }
        public static bool IsSwiftcodes(this string text)
        {
            return (!string.IsNullOrEmpty(text) &&
             (Regex.IsMatch(text, @"^[A-Z]{6}[A-Z0-9]{2}([A-Z0-9]{3})?$")));
        }

        public static string ParseAccountNumber(string accountNumber)
        {
            var noSpecialChar = new StringBuilder();
            if (string.IsNullOrEmpty(accountNumber))
                throw new ArgumentNullException("accountNumber");
            foreach (var c in accountNumber)
            {
                if (c >= '0' && c <= '9')
                {
                    noSpecialChar.Append(c);
                }
            }
            if (noSpecialChar.Length < 10)
            {
                return noSpecialChar.ToString();
            }
            var accNo = noSpecialChar.ToString();
            var dif = 13 - accNo.Length;
            if (dif > 0)
            {
                var pad = string.Empty;
                for (int i = 0; i < dif; i++)
                {
                    pad += "0";
                }
                accNo = accNo.Insert(0, pad);
            }
            accNo = accNo.Insert(4, "-");
            accNo = accNo.Insert(11, "-");
            return accNo;
        }

        public static bool isValidPassword(this string text)
        {
            var r = (Regex.IsMatch(text, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&\.'])[^ ]{8,15}$"));
            if (r)
            {
                if (string.IsNullOrEmpty(text))
                    throw new ArgumentNullException("text");

                char[] characters = text.ToCharArray();


                for (int i = 0; i < characters.Length - 2; i++)
                {
                    if (i <= characters.Length - 2)
                        if (characters[i] == characters[i + 1] && characters[i] == characters[i + 2])
                        {
                            r = false;
                            break;
                        }
                }

            }


            return r;

        }

        public static bool isEmail(this string text)
        {
            //  var pattern = /^ ([a - z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
            var r = Regex.IsMatch(text,
                @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                RegexOptions.IgnoreCase);
            return r;
            // return  Regex.IsMatch(text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

        }


        public static bool isValidUserName(this string text)
        {
            /*
            var number = isNumber(text);
            var english = isEnglish(text);
            var special = isSpecial(text);
            var forbidden = isForbidden(text);
            var space = isSpace(text);
            var arabic = isArabic(text);


            var r = (!(space || special || forbidden || arabic) && ((number   || english)));
            return r;*/

            var r = Regex.IsMatch(text, @"
    # Validate username with 5 constraints.
    ^                          # Anchor to start of string.
    # 1- only contains alphanumeric characters , underscore and dot.
    # 2- underscore and dot can't be at the end or start of username,
    # 3- underscore and dot can't come next to each other.
    # 4- each time just one occurrence of underscore or dot is valid.
    (?=[A-Za-z0-9]+(?:[_.][A-Za-z0-9]+)*$)
    # 5- number of characters must be between 8 to 15.
    [A-Za-z0-9_.]{8,32}        # Apply constraint 5.
    $                          # Anchor to end of string.
    ", RegexOptions.IgnorePatternWhitespace);

            return r;

        }


    }
    public class AccountNumberItems
    {
        public string CustomerNo { get; set; }
        public string BranchNo { get; set; }

        public string Suffix { get; set; }
    }
}
