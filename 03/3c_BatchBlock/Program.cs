using System;
using System.Threading.Tasks.Dataflow;

namespace _3c_BatchBlock
{
    class Program
    {
        static void Main(string[] args)
        {
            var batchBlock = new BatchBlock<int>(3);

            for (int i = 0; i < 10; i++)
            {
                batchBlock.Post(i);
            }

            batchBlock.Complete();
            batchBlock.Post(99); // This will be ignored

            for (int i = 0; i < 5; i++)
            {
                if (batchBlock.TryReceive(out int[] result))
                {

                    Console.Write($"Received batch {i}: ");
                    foreach (var item in result) { Console.Write($"{item} "); }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("The block completed.");
                    break;
                }
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
