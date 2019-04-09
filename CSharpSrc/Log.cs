using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureParser
{
    public static class Log
    {
        public static void WriteLine(string str)
        {
#if DEBUG
            // Console log
            Console.WriteLine(str);
#else
            // File log
            Console.WriteLine("Hello" + str);
#endif
        }
    }
}
