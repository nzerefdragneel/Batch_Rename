using System;
using Contract;
namespace AddPrefix
{
    class AddPrefix : IRule
    {
        public string Prefix { get; set; }

        public string getData()
        {
            return Prefix;
        }
        public void reset()
        {
            Prefix = "";
        }
        public string Name => "AddPrefix";

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public void EditRule(string data) { 
            Prefix= data;
        }
        public IRule? Parse(string data)
        {
            const string Space = " ";
            var tokens = data.Split(
                new string[] { Space }, StringSplitOptions.None
            );
            var prefix = tokens[1];

            AddPrefix result = new AddPrefix();
            result.Prefix = "";

            return result;
        }

        public string Rename(string origin,string type)
        {
            if (Prefix != "")
            {
                string newName = $"{Prefix} {origin}";
                return newName;
            }
            return origin;
        }
    }
}