using System;

namespace Contract
{

    public interface IRule : ICloneable
    {
      
        string Rename(string origin,string type);
        IRule? Parse(string data);
        void EditRule(string data);
        string getData();
        void reset();
        string Name { get; }
    }

}
