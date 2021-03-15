using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _3e_BufferBlock_BoundedCap
{
    class Program
    {
        static void Main(string[] args)
        {
            var bufferBlock = new BufferBlock<int>(new DataflowBlockOptions
            {
                BoundedCapacity = 2
            });

            var a1 = new ActionBlock<int>(n => {
                Console.WriteLine($"Message {n} received by Consumer 1");
                Task.Delay(300).Wait();
            }, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 2
            });

            var a2 = new ActionBlock<int>(n => {
                Console.WriteLine($"Message {n} received by Consumer 2");
                Task.Delay(300).Wait();
            }, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 2
            });

            bufferBlock.LinkTo(a1); // Messages will be proposed in this order
            bufferBlock.LinkTo(a2);

            for (int i = 0; i < 10; i++)
            {
                // Post(..) is not a blocking function, it will discard

                bufferBlock.SendAsync(i)
                    .ContinueWith(a =>
                    {
                        if(a.Result)
                            Console.WriteLine("Accepted");
                        else
                            Console.WriteLine("Rejected");
                    });
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
