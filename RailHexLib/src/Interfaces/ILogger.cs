using System;
using System.Collections.Generic;
using System.Text;

namespace RailHexLib.Interfaces
{
    public interface ILogger
    {
        public void Log(string msg, string category = null);
        string Category { get; }
    }
}
