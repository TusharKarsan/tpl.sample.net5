using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _3f_BroadcastBlock
{
    class Program
    {
        static void Main(string[] args)
        {
            var broadcastBlock = new BroadcastBlock<int>(a => a);

            var a1 = new ActionBlock<int>(n => {
                Console.WriteLine($"Message {n} received by Consumer 1");
                Task.Delay(300).Wait();
            }, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 2 // Will cause messages to be lost of not ready to receive
            });

            var a2 = new ActionBlock<int>(n => {
                Console.WriteLine($"Message {n} received by Consumer 2");
                Task.Delay(300).Wait();
            }, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 2 // Will cause messages to be lost of not ready to receive
            });

            broadcastBlock.LinkTo(a1); // Messages will be proposed in this order
            broadcastBlock.LinkTo(a2);

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
