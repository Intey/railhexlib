using RailHexLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RailHexLib.DevTools
{
    public class Logger : ILogger
    {
        public void Log(string msg)
        {
            Debug.WriteLine(msg);
        }
    }
}
