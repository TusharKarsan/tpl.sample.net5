using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _4c_OptionsAppend
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

            bufferBlock.LinkTo(a1); // Changes the order of offer sequence
            bufferBlock.LinkTo(a2, new DataflowLinkOptions { Append = false });

            for (int i = 0; i < 10; i++)
            {
                // Post(..) is not a blocking function, it will discard

                bufferBlock.SendAsync(i);
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
