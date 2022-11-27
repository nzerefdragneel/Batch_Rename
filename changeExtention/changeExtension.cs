using System;
using System.Diagnostics;
using Contract;
namespace changeExtension
{
    internal class ChangeExtension : IRule
    {
        public string Extension { get; set; }

     
        public string Name => "ChangeExtension";
            public string getData()
        {
            return Extension;
        }
        public void EditRule(string extension)
        {
            Extension = extension;
        }
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public void reset()
        {
            Extension = "";
        }
        public IRule? Parse(string data)
        {
            const string Space = " ";
            var tokens = data.Split(
                new string[] { Space }, StringSplitOptions.None
            );
            var prefix = tokens[1];

            ChangeExtension result = new ChangeExtension();
            result.Extension = "";

            return result;
        }

        public string Rename(string origin, string type)
        {
            if (Extension=="") return origin;
            if (type == "Folder") return origin;
            int index = origin.LastIndexOf('.');
            string name = "", extension = "";
            if (index != -1 && type == "File")
            {
                name = origin.Substring(0, index);
                extension = origin.Substring(index);
                origin = name + "." + Extension;
                return origin;
            }
            return origin;
        }
    }
}