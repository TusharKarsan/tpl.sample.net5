using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _3g_JoinBlock
{
    class Program
    {
        static void Main(string[] args)
        {
            var broadcastBlock = new BroadcastBlock<int>(a => a);

            var a1 = new TransformBlock<int, int>(n => {
                Console.WriteLine($"Message {n} received by Consumer 1");
                Task.Delay(n % 2 == 0 ? 300 : 100).Wait(); // Join block will still pair them correctly
                return n * -1;
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 });

            var a2 = new TransformBlock<int, int>(n => {
                Console.WriteLine($"Message {n} received by Consumer 2");
                Task.Delay(n % 2 == 0 ? 100 : 300).Wait(); // Join block will still pair them correctly
                return n;
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 });

            broadcastBlock.LinkTo(a1); // Messages will be proposed in this order
            broadcastBlock.LinkTo(a2);

            var joinBlock = new JoinBlock<int, int>();
            a1.LinkTo(joinBlock.Target1);
            a2.LinkTo(joinBlock.Target2);

            var printBlock = new ActionBlock<Tuple<int, int>>(a => Console.WriteLine($"Message {a.Item1},{a.Item2} was processed"));
            joinBlock.LinkTo(printBlock);

            for (int i = 0; i < 10; i++)
            {
                broadcastBlock.SendAsync(i) // Like Post, if receivers are not ready then they will be ignored, there is no retry
                    .ContinueWith(a =>
                    {
                        if (a.Result)
                            Console.WriteLine($"Accepted {i}");
                        else
                            Console.WriteLine($"Rejected {i}");
                    });
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
