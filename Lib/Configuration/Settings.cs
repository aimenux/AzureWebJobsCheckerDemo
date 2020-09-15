using System;
using Lib.Models;

namespace Lib.Configuration
{
    public class Settings
    {
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Url)
                   && !string.IsNullOrWhiteSpace(UserName)
                   && !string.IsNullOrWhiteSpace(Password)
                   && !string.IsNullOrWhiteSpace(Name)
                   && Enum.TryParse(Type, out WebJobType _);
        }
    }
}
