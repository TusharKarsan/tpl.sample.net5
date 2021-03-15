using System;
using System.Threading.Tasks.Dataflow;

namespace _3i_WriteOnceBlock
{
    class Program
    {
        static void Main(string[] args)
        {
            var writeOnceBlock = new WriteOnceBlock<int>(a => a);
            var actionBlock = new ActionBlock<int>(a => Console.WriteLine($"ActionBlock received {a}"));
            writeOnceBlock.LinkTo(actionBlock);

            for (int i = 0; i < 5; i++)
            {
                if (writeOnceBlock.Post(i))
                    Console.WriteLine($"Accepted {i}");
                else
                    Console.WriteLine($"Rejected {i}");
            }

            for (int i = 0; i < 10; i++) // Receive more than Post
            {
                if(writeOnceBlock.TryReceive(out int result))
                    Console.WriteLine($"Received {result}");
                else
                    Console.WriteLine($"Warning {result}");
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
