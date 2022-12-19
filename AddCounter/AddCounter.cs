using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
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
        public int NumOffile { get; set; }
        public void EditRule(string dataChange)
        {
            var isChange = dataChange.Split(":");
            foreach (var data in isChange)
            {
              
                if (!string.IsNullOrEmpty(data))
                {

                    var x = data.Split('?');
                    int dataParse;

                    if (x[0] == "Kind") { Kind = x[1]; }
                    if (x[0] == "Step")
                    {
                        try
                        {
                            dataParse = int.Parse(x[1]);
                            Step = dataParse;
                        }
                        catch
                        {
                            MessageBox.Show(x[0], "Data err!");
                            return;
                        }

                    }
                    if (x[0] == "NumOfFile")
                    {
                        try
                        {
                            dataParse = int.Parse(x[1]);
                            NumOffile = dataParse;
                        }
                        catch
                        {
                            MessageBox.Show(x[0], "Data err!");
                            return;
                        }

                    }
                    if (x[0] == "StartFile")
                    {
                        try
                        {
                            dataParse = int.Parse(x[1]);
                            CounterFile = dataParse;
                            StartValue = dataParse;
                        }
                        catch
                        {
                            MessageBox.Show(x[0], "Data err!");
                            return;
                        }

                    }
                    if (x[0] == "StartFolder")
                    {
                        try
                        {
                            dataParse = int.Parse(x[1]);
                            CounterFolder = dataParse;
                            StartValue = dataParse;
                        }
                        catch
                        {
                            MessageBox.Show(x[0], "Data err!");
                            return;
                        }
                    }
                    if (x[0] == "NoDigits")
                    {
                        var num = int.Parse(x[2]);
                        NoDigits = int.Parse(x[1]);
                        NumOffile = num;
                    }
                }
            }
            var suggestNum = (int)Math.Round(Math.Log10(StartValue + (NumOffile - 1) * Step) + 0.5);
        
            if (NoDigits < suggestNum)
            {
               
                string messageslabel = NoDigits.ToString() + " Is to small, we suggest use " + suggestNum.ToString() + " or you can change it?";
                MessageBox.Show(messageslabel);
                NoDigits = suggestNum;
            }
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