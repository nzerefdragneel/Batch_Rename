using System;
using System.Diagnostics;
using System.Text;
using Contract;
namespace AddCounter
{
    class AddCounter : IRule
    {
        public int NoDigits { get; set; }
        public string Kind { get; set; }
        public int Step { get; set; }
        public int CounterFile { get; set; }
        public int CounterFolder { get; set; }
        public string Name => "AddCounter";
        public void EditRule(string data) {
            Kind = data;
        }
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public IRule? Parse(string data)
        {
            const string Space = " ";
            var tokens = data.Split(
                new string[] { Space }, StringSplitOptions.None
            );
            var kind = tokens[1];
            int num = Int32.Parse(tokens[2]);
            AddCounter result = new AddCounter();
            result.NoDigits =0;
            result.Kind = "Pre";
            result.Step = 1;
            result.CounterFile = 0;
            result.CounterFolder = 0;
            return result;
        }

        public void reset()
        {
            this.CounterFile = 0;
            this.CounterFolder = 0;
            this.NoDigits= 0;
        }
        public string Rename(string origin, string type)
        {
            Debug.WriteLine("add counter!!!!!!!!");

            int index = origin.LastIndexOf('.');
            string name = "", extension = "";
            string thisCount;
            if (index != -1 && type == "File")
            {
                name = origin.Substring(0, index);
                extension = origin.Substring(index);
                CounterFile += Step;
                thisCount = CounterFile.ToString();

            }
            else { name = origin;
                CounterFolder += Step;
                thisCount = CounterFolder.ToString();
            }

            for (int i = thisCount.Length; i <= NoDigits; i++) thisCount = "0" + thisCount;
            StringBuilder stringBuilder = new StringBuilder();
            if (Kind == "Pre")
            {
                stringBuilder.Append(name);
                stringBuilder.Append(" ");
                stringBuilder.Append(thisCount);
                stringBuilder.Append(extension);
            }
            else
            {
                stringBuilder.Append(thisCount);
                stringBuilder.Append(" ");
                stringBuilder.Append(name);
                stringBuilder.Append(extension);

            }
            origin = stringBuilder.ToString();
            return origin;
        }
    }
}