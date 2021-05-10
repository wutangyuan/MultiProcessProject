using PESocket.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            
           ServerRoot.Instance.Init();
          
            Console.ReadKey();
        }
    }
}
