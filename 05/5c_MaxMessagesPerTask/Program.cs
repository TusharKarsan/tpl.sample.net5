using _4b_ListToWithPropagation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _5c_MaxMessagesPerTask
{
    class Program
    {
        private const int _count = 10;
        private static readonly Stopwatch _stopWatch = new Stopwatch();
        private static readonly ConcurrentDictionary<int, ConcurrentBag<Tuple<long, string>>> _timestampedList = new ConcurrentDictionary<int, ConcurrentBag<Tuple<long, string>>>();

        static void Main(string[] args)
        {
            var inputBlock = new BroadcastBlock<int>(a => a);

            var consumerBlocks = new List<ActionBlock<int>>();

            for (int i = 0; i < _count; i++)
            {
                var actionBlock = CreateConsumingBlock(i);
                inputBlock.LinkWithPropagationTo(actionBlock);
                consumerBlocks.Add(actionBlock);
            }

            _stopWatch.Start();

            for (int i = 0; i < 100; i++)
                inputBlock.Post(i);

            inputBlock.Complete();
            Task.WaitAll(consumerBlocks.Select(a => a.Completion).ToArray());

            _stopWatch.Stop();

            foreach(int key in _timestampedList.Keys)
            {
                Console.WriteLine($"{key} : {string.Join(",", _timestampedList[key].Select(a => a.Item2))}");
            }

            Console.WriteLine($"Elapsed {_stopWatch.ElapsedMilliseconds} ms");
        }

        private static ActionBlock<int> CreateConsumingBlock(int id)
        {
            return new ActionBlock<int>(a =>
            {
                Task.Delay(100).Wait();
                var blockLog = Tuple.Create(_stopWatch.ElapsedTicks, id.ToString());
                var bag = _timestampedList.GetOrAdd(
                        Thread.CurrentThread.ManagedThreadId,
                        new ConcurrentBag<Tuple<long, string>>()
                    );
                bag.Add(blockLog);
            }, new ExecutionDataflowBlockOptions { MaxMessagesPerTask = 5 });
        }
    }
}
