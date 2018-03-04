using System.Text.RegularExpressions;

namespace EchoDesertTrips.Desktop.Support
{
    public class PhoneNumberValidation
    {
        public static bool IsPhoneNumber(string number)
        {
            //return Regex.Match(number, @"^(\+[0-9]{9})$").Success;
            return true;
        }
    }
}
