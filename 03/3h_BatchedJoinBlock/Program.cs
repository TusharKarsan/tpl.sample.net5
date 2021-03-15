using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _3h_BatchedJoinBlock
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

            var batchedJoinBlock = new BatchedJoinBlock<int, int>(3);
            a1.LinkTo(batchedJoinBlock.Target1);
            a2.LinkTo(batchedJoinBlock.Target2);

            var printBlock = new ActionBlock<Tuple<IList<int>, IList<int>>>(
                a => Console.WriteLine($"Message {string.Join(",", a.Item1)} : {string.Join(",", a.Item2)}")
            );

            batchedJoinBlock.LinkTo(printBlock);

            for (int i = 0; i < 10; i++)
            {
                broadcastBlock.SendAsync(i) // List Post, if receivers are not ready then they will be ignored, there is no retry
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
