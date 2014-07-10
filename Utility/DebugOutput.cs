using System;
using System.IO;

namespace OuterSpace
{
    public class DebugOutput
    {
        private StreamWriter oWrite;

        public DebugOutput(string path)
        {
            oWrite = File.CreateText(path);
        }

        public void Output(string label)
        {
            oWrite.WriteLine(label);
        }        
    }
}