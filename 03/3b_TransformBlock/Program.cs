using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _3b_TransformBlock
{
    class Program
    {
        static void Main(string[] args)
        {
            var transformBlock = new TransformBlock<int, string>(n => {
                Task.Delay(500).Wait();
                return n.ToString();
            }, new ExecutionDataflowBlockOptions {
                MaxDegreeOfParallelism = 2
            });

            for (int i = 0; i < 10; i++)
            {
                transformBlock.Post(i);
                Console.WriteLine($"Number of messages in the input queue {transformBlock.InputCount}");
            }

            for (int i = 0; i < 10; i++)
            {
                Console.Write($"Input {transformBlock.OutputCount}, Output {transformBlock.OutputCount}, received ");
                var result = transformBlock.Receive();
                Console.WriteLine(result);
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
