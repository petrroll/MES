using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionSolverSocketSever
{
    class Program
    {
        static void Main(string[] args)
        {
            MESServer server = new MESServer();
            server.Work().Wait();
        }
    }
}
