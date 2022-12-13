using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDemo
{
    internal class InputOutput
    {
        public static void Print(String messege)
        {
            Console.WriteLine(messege);
        }

        public static String GetPath()
        {
            Print("Please enter the path to your gkpg: ");
            return Console.ReadLine();
        }
    }
}
