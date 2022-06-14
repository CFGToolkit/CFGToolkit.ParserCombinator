using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator.Input;

namespace CFGToolkit.ParserCombinator.State
{
    public class GlobalState<TToken> : IGlobalState<TToken> where TToken : IToken
    {
        public GlobalState()
        {
        }

        public int LastConsumedPosition { get; set; } = -1;

        public int LastFailedPosition { get; set; } = -1;

        public Lazy<List<Frame<TToken>>> LastConsumedCallStack { get; set; }

        public ConcurrentBag<Lazy<List<Frame<TToken>>>> LastFailedCallStacks { get; set; } = new ConcurrentBag<Lazy<List<Frame<TToken>>>>();

        public IUnionResult<TToken>[,] Cache { get; set; }

        public Action<bool> UpdateHandler { get; set; }

        public List<List<Frame<TToken>>> GetUniqueFailedCallStacks()
        {
            var failed = LastFailedCallStacks.Select(f => f.Value).ToList().OrderByDescending(f => f.Count).ToList();
            var result = new List<List<Frame<TToken>>>();

            var prefixes = new HashSet<string>();

            for (var i = 0; i < failed.Count; i++)
            {
                var callStackString = string.Join("|", failed[i].Select(frame => frame.Parser.Name));

                if (!prefixes.Any(p => p.StartsWith(callStackString)))
                {
                    prefixes.Add(callStackString);
                    result.Add(failed[i]);
                }
            }

            return result;
        }
    }
}
