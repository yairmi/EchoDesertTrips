using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Utils
{
    public class HashCodeUtil
    {
        public static string GetHashCodeBernstein(string stringToHashCode)
        {
            System.Data.HashFunction.BernsteinHash hash = new System.Data.HashFunction.BernsteinHash();
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(stringToHashCode);
            var hashCode = hash.ComputeHash(bytes);

            var convertedString = BitConverter.ToString(hashCode);
            return convertedString;
        }
    }
}
