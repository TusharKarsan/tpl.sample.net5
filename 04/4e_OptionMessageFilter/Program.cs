using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _4e_OptionMessageFilter
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
            });

            var a2 = new ActionBlock<int>(n => {
                Console.WriteLine($"Message {n} received by Consumer 2");
                Task.Delay(300).Wait();
            });

            bufferBlock.LinkTo(a1, a => a % 2 == 0);
            bufferBlock.LinkTo(a2, new DataflowLinkOptions { MaxMessages = 3 });

            // bufferBlock.LinkTo(DataflowBlock.NullTarget<int>()); // Discards everything it receives
            bufferBlock.LinkTo(new ActionBlock<int>(a => Console.WriteLine($"Discarded {a}")));

            for (int i = 0; i < 10; i++)
            {
                // Post(..) is not a blocking function, it will discard

                bufferBlock.SendAsync(i); // If no one accepts then it will hang
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
