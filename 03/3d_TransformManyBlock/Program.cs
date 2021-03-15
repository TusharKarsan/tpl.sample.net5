using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace _3d_TransformManyBlock
{
    class Program
    {
        static void Main(string[] args)
        {
            var transformManyBlock = new TransformManyBlock<int, string>(a => FindEvenNumbers(a), new ExecutionDataflowBlockOptions { 
                MaxDegreeOfParallelism = 5
            });

            var printBlock = new ActionBlock<string>(a => Console.WriteLine($"Received message {a}"));
            transformManyBlock.LinkTo(printBlock);

            for (int i = 0; i < 10; i++)
            {
                transformManyBlock.Post(i);
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }

        private static IEnumerable<string> FindEvenNumbers(int number)
        {
            for (int i = 0; i < number; i++)
            {
                if (i % 2 == 0)
                {
                    yield return $"{number}:{i}";
                }
            }
        }
    }
}
