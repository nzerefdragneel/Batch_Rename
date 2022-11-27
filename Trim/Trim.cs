using System;
using Contract;
namespace Trim
{
    class Trim: IRule
    {

       
         public void reset()
        {
        }
        public string Name => "Trim";

        public object Clone()
        {
            throw new NotImplementedException();
        }
        public void EditRule(string data) { }

        public IRule? Parse(string data)
        {
          
           Trim result = new Trim();
           
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
            origin =name.Trim()+ extension;
            return origin;
        }

        string IRule.getData()
        {
            throw new NotImplementedException();
        }
    }
}