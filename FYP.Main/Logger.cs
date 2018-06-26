using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Main
{
    public class ConsoleLogger : ILogger
    {
        public void Write(string s)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {s}");
        }

        public void Write()
        {
            Console.WriteLine();
        }
    }

    public interface ILogger
    {
        void Write(string s);
        void Write();
    }
}
