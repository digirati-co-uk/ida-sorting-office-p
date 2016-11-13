using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdaSortingOffice.Models
{
    public static class Util
    {
        public static string GetFileSafeName(this string s)
        {
            s = s.Replace("http://", "").Replace("https://", "");
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                s = s.Replace(c, '-');
            }
            return s;
        }

        public static string GetMd5Hash(this string s)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.Unicode.GetBytes("TextToHash"));
                var sb = new StringBuilder();
                foreach (var t in hash) sb.Append(t.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
