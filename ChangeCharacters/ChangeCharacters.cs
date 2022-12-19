using Contract;
using System;
using System.Diagnostics;
using System.Text;

namespace ChangeCharacters
{
    public class ChangeCharacters : IRule
    {
        public string OldChar { get; set; }

        public char NewChar { get; set; }
        public string Name => "ChangeCharacters";

        public void EditRule(string data)
        {
            if (data != null)
            {
                OldChar = data.Substring(0, data.Length - 2);
                NewChar = data[data.Length - 1];
            }
        }
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public void reset()
        {
            OldChar = "";
            NewChar = '\n';

        }
        public IRule? Parse(string data)
        {
            const string Space = " ";
            var tokens = data.Split(
                new string[] { Space }, StringSplitOptions.None
            );
            var prefix = tokens[1];

            ChangeCharacters result = new ChangeCharacters();
            result.OldChar = "";
            result.NewChar = '\n';

            return result;
        }
        public string getData()
        {
            return OldChar + "?" + NewChar;
        }

        public string Rename(string origin, string type)
        {
            if (OldChar == "") return origin;
            if (NewChar == '\n') return origin;
            int index = origin.LastIndexOf('.');
            string namef = "", extension = "";
            if (index != -1 && type == "File")
            {
              
                namef = origin.Substring(0, index);
                extension = origin.Substring(index);
               
            }
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < namef.Length; i++)
            {
                if (OldChar.IndexOf(namef[i]) == -1)
                    stringBuilder.Append(namef[i]);
                else stringBuilder.Append(NewChar);

            }
            origin = stringBuilder.ToString() + extension;
            return origin;
        }
    }
}
