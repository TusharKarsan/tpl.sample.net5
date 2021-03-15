using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using _4b_ListToWithPropagation;

namespace _5b_Scheduling
{
    class Program
    {
        private static Random _random = new Random(100);

        private static int _val = 0;

        static void Main(string[] args)
        {
            var inputBlock = new BroadcastBlock<int>(a => a);

            Action<int> actionBlockFunction = (int a) =>
            {
                int counterValue = GetSharedObjectValue();
                Console.WriteLine($"{a}, counterValue {counterValue}");
                Task.Delay(_random.Next(300)).Wait();
                SetSharedObjectValue(counterValue + 1);
            };

            var scheduler = new ConcurrentExclusiveSchedulerPair(); // Avoid locks in business logic.

            var incrementingBlocl1 = new ActionBlock<int>(actionBlockFunction, new ExecutionDataflowBlockOptions {
                TaskScheduler = scheduler.ExclusiveScheduler
            });

            var incrementingBlocl2 = new ActionBlock<int>(actionBlockFunction, new ExecutionDataflowBlockOptions {
                TaskScheduler = scheduler.ExclusiveScheduler
            });

            inputBlock.LinkWithPropagationTo(incrementingBlocl1);
            inputBlock.LinkWithPropagationTo(incrementingBlocl2);

            for (int i = 0; i < 10; i++)
                inputBlock.Post(i);
            inputBlock.Complete();

            incrementingBlocl1.Completion.Wait();
            incrementingBlocl2.Completion.Wait();

            Console.WriteLine($"Current counter value {GetSharedObjectValue()}");
        }

        private static int GetSharedObjectValue()
        {
            return _val;
        }

        private static void SetSharedObjectValue(int val)
        {
            _val = val;
        }
    }
}
