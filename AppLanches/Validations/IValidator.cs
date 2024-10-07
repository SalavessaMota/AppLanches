using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Validations
{
    public interface IValidator
    {
        string NameError { get; set; }
        string EmailError { get; set; }
        string PhoneError { get; set; }
        string PasswordError { get; set; }

        Task<bool> ValidateAsync(string name, string email, string phone, string password);
    }
}
