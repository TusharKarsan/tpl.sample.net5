using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _4f_MultipleProducers
{
    class Program
    {
        static void Main(string[] args)
        {
            var producer1 = new TransformBlock<string, string>(n =>
            {
                Task.Delay(150).Wait();
                return n;
            });

            var producer2 = new TransformBlock<string, string>(n =>
            {
                Task.Delay(500).Wait();
                return n;
            });

            var printBlock = new ActionBlock<string>(n => Console.WriteLine(n));

            producer1.LinkTo(printBlock);
            producer2.LinkTo(printBlock);

            for (int i = 0; i < 10; i++)
            {
                producer1.Post($"Producer 1 message {i}");
                producer1.Post($"Producer 2 message {i}");
            }

            producer1.Complete();
            producer2.Complete();

            Task.WhenAll(
                producer1.Completion,
                producer1.Completion
            ).ContinueWith(a => printBlock.Complete());

            printBlock.Completion.Wait();

            Console.WriteLine("Finished!");
        }
    }
}
