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
        public int StartValue { get; set; }
        public int CounterFile { get; set; }
        public int CounterFolder { get; set; }
        public string Name => "AddCounter";
        public void EditRule(string data) {
            var x = data.Split('?');
            if (x[0] == "Kind"){ Kind = x[1]; Debug.WriteLine("Kind",Kind); }
            if (x[0]=="Step") Step = int.Parse(x[1]);
            if (x[0]=="NoDigits") NoDigits= int.Parse(x[1]);
            if (x[0] == "StartFile") { CounterFile = int.Parse(x[1]); StartValue = int.Parse(x[1]); }
            if (x[0] == "StartFolder") { CounterFile = int.Parse(x[1]); StartValue = int.Parse(x[1]); }
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
            result.StartValue = 0;
            result.CounterFile = result.StartValue;
            result.CounterFolder = result.StartValue;
            return result;
        }

        public string getData()
        {
            return Kind;
        }
        public void reset()
        {
            this.CounterFile = this.StartValue;
            this.CounterFolder = this.StartValue;
        }
        public string Rename(string origin, string type)
        {
      
            int index = origin.LastIndexOf('.');
            string name = "", extension = "";
            string thisCount;
            if (  type == "File")
            {
                if (index != -1)
                {
                    name = origin.Substring(0, index);
                    extension = origin.Substring(index);
                }
               
                thisCount = CounterFile.ToString();

            }
            else { name = origin;
              
                thisCount = CounterFolder.ToString();
            }

            for (int i = thisCount.Length; i < NoDigits; i++) thisCount = "0" + thisCount;
            StringBuilder stringBuilder = new StringBuilder();
            if (Kind == "Pre")
            {
               
                stringBuilder.Append(thisCount);
                stringBuilder.Append(" ");
                stringBuilder.Append(name);
                stringBuilder.Append(extension);
            }
            else
            {
                stringBuilder.Append(name);
                stringBuilder.Append(" ");
                stringBuilder.Append(thisCount);
                stringBuilder.Append(extension);

            }
            origin = stringBuilder.ToString();
            if (type == "File") CounterFile += Step;
            else CounterFolder+= Step; 
            return origin;
        }
    }
}