using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace _4g_ErrorHandling
{
    class Program
    {
        static void Main(string[] args)
        {
            var transformBlock = new TransformBlock<int, string>(n =>
            {
                if (n == 5)
                    throw new Exception("Something went wrong");
                // Removed all messages in input queue and goes into faulted state
                Console.WriteLine($"Received {n}");
                return n.ToString();
            });

            var printBlock = new ActionBlock<string>(n => Console.WriteLine(n));
            transformBlock.LinkTo(printBlock, new DataflowLinkOptions { PropagateCompletion = true });

            for (int i = 0; i < 10; i++)
            {
                transformBlock.Post(i);
            }

            transformBlock.Complete();

            try
            {
                printBlock.Completion.Wait();
            }
            catch (AggregateException ae)
            {
                throw ae.Flatten().InnerException;
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
