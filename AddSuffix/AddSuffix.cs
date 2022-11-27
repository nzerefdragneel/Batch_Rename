using System;
using Contract;
namespace AddSuffix
{
    class AddSuffix : IRule
    {
        
        public string Suffix { get; set; }

     public string getData()
        {
            return Suffix;
        }
        public void reset()
        {
            Suffix= string.Empty;
        }
        public string Name => "AddSuffix";

        public object Clone()
        {
            throw new NotImplementedException();
        }
        public void EditRule(string data) {
            Suffix = data;
        }
        public IRule? Parse(string data)
        {
            const string Space = " ";
            var tokens = data.Split(
                new string[] { Space }, StringSplitOptions.None
            );
            var Suffix = tokens[1];

            AddSuffix result = new AddSuffix();
            result.Suffix = "";

            return result;
        }

        public string Rename(string origin,string type)
        {
            if (Suffix == "") return origin;
            int index = origin.LastIndexOf('.');
            string name = "", extension = "";
            if (index != -1 && type == "File")
            {
                name = origin.Substring(0, index);
                extension = origin.Substring(index);
            }
            else name = origin;
            origin = name + " " + Suffix +extension;
            return origin;
        }
    }
}