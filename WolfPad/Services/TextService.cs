using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WolfPad.Services
{
    public static class TextService
    {
        public static void ReverseFileAdress(ref string adress)
        {
            StringBuilder builder = new();
            for (int i = adress.Length - 1; i >= 0; i--)
            {
                builder.Append(adress[i]);
            }
            adress = builder.ToString();
        }

        public static string GetFileName(string fadress)
        {
            if (string.IsNullOrWhiteSpace(fadress))
                return null;

            ReverseFileAdress(ref fadress);
            StringBuilder builder = new();
            bool read = false;
            foreach (char ch in fadress)
            {
                if (ch == '\\')
                    break;
                if (ch == '.')
                {
                    read = true;
                    continue;
                }
                if (read)
                    builder.Append(ch);
            }

            fadress = builder.ToString();
            ReverseFileAdress(ref fadress);
            return fadress;
        }

        public static string RemoveEscapeSequences(string sText, string sReplace = "")

        {
            sText = sText.Replace("\a", sReplace);
            sText = sText.Replace("\b", sReplace);
            sText = sText.Replace("\f", sReplace);
            sText = sText.Replace("\n", sReplace);
            sText = sText.Replace("\r", sReplace);
            sText = sText.Replace("\t", sReplace);
            sText = sText.Replace("\v", sReplace);
            sText = sText.Replace("\'", sReplace);
            sText = sText.Replace("\"", sReplace);
            sText = sText.Replace("\\", sReplace);

            return sText;
        }

    }
}
