using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace bootcamp_arduino
{
    class Program
    {
        static void Main(string[] args)
        {
            SerialPort port = new SerialPort("COM3", 9600);
            port.Open();
            while (true)
            {
                string s = port.ReadLine();
                Console.WriteLine(s);
            }
        }
    }
}
