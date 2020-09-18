using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ContourPlateBridge
{
    class PrefixMaker
    {
        private readonly string bearingMark;
        private Regex regex = new Regex(@"(.*-)(\d{2})$");
        Match match; 


        public PrefixMaker(string bearingMark)
        {
            this.bearingMark = bearingMark;
            this.match = regex.Match(bearingMark);
        }

        public bool IsMatch()
        {
            return match.Success;
        }

        public string GetAssembly()
        {
            if (getPrefix().StartsWith("0"))
            {
                return match.Groups[1].Value + "0";
            }
            else
            {
                return match.Groups[1].Value;
            }            
        }

        private string getPrefix()
        {
            return match.Groups[2].Value;
        }

        public int GetPrefix()
        {
            return int.Parse(getPrefix());
        }
    }
}
