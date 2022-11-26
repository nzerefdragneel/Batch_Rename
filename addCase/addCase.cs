using System;
using System.CodeDom;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Contract;
namespace addCase
{
    class AddCase : IRule
    {
        private string @case;
        public void reset()
        {
            @case = "";
        }
        public string GetCase()
        {
            return @case;
        }

        public void SetCase(string value)
        {
            @case = value;
        }

        public string Name => "AddCase";

        public object Clone()
        {
            throw new NotImplementedException();
        }
        public void EditRule(string data) {
            
            SetCase(data);
            Debug.WriteLine(GetCase());
            Debug.WriteLine("VVVVVVVVVVVVVVVVVVVVVVVVVVVV");
        }
        public IRule? Parse(string data)
        {
            const string Space = " ";
            var tokens = data.Split(
                new string[] { Space }, StringSplitOptions.None
            );
            var thiscase= tokens[1];

            AddCase result = new AddCase();

            return result;
        }

        public string Rename(string origin, string type)
        {
            int index = origin.LastIndexOf('.');
            string name = "", extension = "";
            if (index != -1 && type == "File")
            {
                name = origin.Substring(0, index);
                extension = origin.Substring(index);
            }
            else name = origin;
            Debug.WriteLine(GetCase());
            Debug.WriteLine("VVVVVVVVVVVVVVVVVVVVVVVVVVVV");
            if (GetCase() == "PascalCase")
            {
                Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
                Regex whiteSpace = new Regex(@"(?<=\s)");
                Regex startsWithLowerCaseChar = new Regex("^[a-z]");
                Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
                Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
                Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

               
                var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(name, "_"), string.Empty)
                    .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                    .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                    .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                    .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));
                origin = string.Concat(pascalCase) + extension;
            }
            else if (GetCase() == "UpCase")
            {
                origin = origin.ToUpper();
            }
            else if (GetCase() == "LowerCase")
            {
                origin = origin.ToLower();
            }
            else if (GetCase() == "RemoveAllSpace")
            {
               
                origin = Regex.Replace(origin, @"\s+", "");
            }
            return origin;
        }
    }
}