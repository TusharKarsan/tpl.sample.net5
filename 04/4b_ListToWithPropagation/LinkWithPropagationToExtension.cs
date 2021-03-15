using System;
using System.Threading.Tasks.Dataflow;

namespace _4b_ListToWithPropagation
{
    public static class LinkWithPropagationToExtension
    {
        public static IDisposable LinkWithPropagationTo<T>(this ISourceBlock<T> source, ITargetBlock<T> target)
        {
            return source.LinkTo(target, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });
        }
    }
}
