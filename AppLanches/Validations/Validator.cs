using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppLanches.Validations
{
    public class Validator : IValidator
    {
        private const string EmptyNameErrorMsg = "Name cannot be empty";
        private const string InvalidNameErrorMsg = "Name must be valid";
        private const string EmptyEmailErrorMsg = "Email cannot be empty";
        private const string InvalidEmailErrorMsg = "Email must be valid";
        private const string EmptyPhoneErrorMsg = "Phone cannot be empty";
        private const string InvalidPhoneErrorMsg = "Phone must be valid";
        private const string EmptyPasswordErrorMsg = "Password cannot be empty";
        private const string InvalidPasswordErrorMsg = "Password must contain at least 8 characters, including letters and numbers";



        public string NameError { get; set; } = "";
        public string EmailError { get; set; } = "";
        public string PhoneError { get; set; } = "";
        public string PasswordError { get; set; } = "";






        public Task<bool> ValidateAsync(string name, string email, string phone, string password)
        {
            var isNameValid = ValidateName(name);
            var isEmailValid = ValidateEmail(email);
            var isPhoneValid = ValidatePhone(phone);
            var isPasswordValid = ValidatePassword(password);

            return Task.FromResult(isNameValid && isEmailValid && isPhoneValid && isPasswordValid);
        }

        private bool ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                NameError = EmptyNameErrorMsg;
                return false;
            }

            if (name.Length < 3)
            {
                NameError = InvalidNameErrorMsg;
                return false;
            }

            NameError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                EmailError = EmptyEmailErrorMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
            {
                EmailError = InvalidEmailErrorMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                PhoneError = EmptyPhoneErrorMsg;
                return false;
            }

            if (phone.Length < 12)
            {
                PhoneError = InvalidPhoneErrorMsg;
                return false;
            }

            PhoneError = "";
            return true;
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordError = EmptyPasswordErrorMsg;
                return false;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = InvalidPasswordErrorMsg;
                return false;
            }

            PasswordError = "";
            return true;
        }
    }
}
