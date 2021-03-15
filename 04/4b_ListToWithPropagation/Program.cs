using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _4b_ListToWithPropagation
{
    class Program
    {
        static void Main(string[] args)
        {
            var broadcastBlock = new BroadcastBlock<int>(a => a);
            broadcastBlock.Completion.ContinueWith(a => Console.WriteLine("broadcastBlock completed"));

            var a1 = new TransformBlock<int, int>(n => {
                Console.WriteLine($"Message {n} received by Consumer 1");
                Task.Delay(n % 2 == 0 ? 300 : 100).Wait(); // Join block will still pair them correctly
                return n * -1;
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 2
            });

            var a2 = new TransformBlock<int, int>(n => {
                Console.WriteLine($"Message {n} received by Consumer 2");
                Task.Delay(n % 2 == 0 ? 100 : 300).Wait(); // Join block will still pair them correctly
                return n;
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 2
            });

            broadcastBlock.LinkWithPropagationTo(a1);
            broadcastBlock.LinkWithPropagationTo(a2);

            var joinBlock = new JoinBlock<int, int>();
            a1.LinkWithPropagationTo(joinBlock.Target1);
            a2.LinkWithPropagationTo(joinBlock.Target2);

            var finalBlock = new ActionBlock<Tuple<int, int>>(a => Console.WriteLine($"Message {a.Item1},{a.Item2} was processed"));
            joinBlock.LinkWithPropagationTo(finalBlock);

            for (int i = 0; i < 10; i++)
            {
                broadcastBlock.SendAsync(i); // Like Post, if receivers are not ready then they will be ignored, there is no retry
            }

            broadcastBlock.Complete();
            finalBlock.Completion.Wait();

            Console.WriteLine("Finished!");
        }
    }
}
