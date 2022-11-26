using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batch_Rename
{
    internal class FileIOS : INotifyPropertyChanged
    {
        public string Filename { get; set; }
        public string NewFilename { get; set; }
        public string Pathname { get; set; }
        public string Error { get; set; }
        public string Result { get; set; }
        public string Type { get; set; }

        public int Stt { get; set; }
        public int Status { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public string getType()
        {
            return "File";
        }
        public int getStatus()
        {
            return this.Status;
        }
    }
    internal class FolderIOS : INotifyPropertyChanged
    {
        public string Filename { get; set; }

        public int Stt { get; set; }
        public string NewFilename { get; set; }
        public string Pathname { get; set; }
        public string Result { get; set; }
        public string Type { get; set; }
        public string Error { get; set; }
        public int Status { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;



        public string getType()
        {
            return "Folder";
        }
        public int getStatus()
        {
            return this.Status;
        }
    }
    public class Base
    {

        public string thing { get; set; }

        public T GetAttribute<T>(string _name)
        {
            return (T)GetType().GetField(_name).GetValue(this);
        }
    }
}
