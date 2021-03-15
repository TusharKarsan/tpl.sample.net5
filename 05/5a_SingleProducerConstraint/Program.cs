using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _5a_SingleProducerConstraint
{
    class Program
    {
        static void Main(string[] args)
        {
            var transformBlock = new TransformBlock<int, string>(n => {
                Task.Delay(500).Wait();
                return n.ToString();
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 2,
                SingleProducerConstrained = true // Assumes only a single producer
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
