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
        private string caseName{get;set; }

         public string getData()
        {
            return caseName;
        }
        public void reset()
        {
           caseName = "PascalCase";
        }
        public string GetCase()
        {
            return caseName;
        }

        public void SetCase(string value)
        {
            caseName = value;
        }

        public string Name => "AddCase";

        public object Clone()
        {
            throw new NotImplementedException();
        }
        public void EditRule(string data) {
            if (data != "")
            {
                caseName = data;
            }
           
        }
        public IRule? Parse(string data)
        {
            const string Space = " ";
            var tokens = data.Split(
                new string[] { Space }, StringSplitOptions.None
            );
            var thiscase= tokens[1];

            AddCase result = new AddCase();
            result.caseName = "PascalCase";
          
            return result;
        }

        public string Rename(string origin, string type)
        {
            if (caseName == "") return origin;
            int index = origin.LastIndexOf('.');
            string name = "", extension = "";
            if (index != -1 && type == "File")
            {
                name = origin.Substring(0, index);
                extension = origin.Substring(index);
            }
            else name = origin;
       
            if (caseName == "PascalCase")
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
            else if (caseName == "UpCase")
            {
                origin = origin.ToUpper();
            }
            else if (caseName == "LowerCase")
            {
                origin = origin.ToLower();
            }
            else if (caseName == "RemoveAllSpace")
            {
               
                origin = Regex.Replace(origin, @"\s+", "");
            }
            return origin;
        }
    }
}