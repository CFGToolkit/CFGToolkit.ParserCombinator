using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator.Parsers.Graphs;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class RepeatParser<TToken, TResult> : IParser<TToken, List<TResult>> where TToken : IToken
    {
        private readonly IParser<TToken, TResult> _parser;
        private readonly int? _minimumCount;
        private readonly int? _maximumCount;
        private readonly bool _greedy;

        public RepeatParser(string name, IParser<TToken, TResult> parser, int? minimumCount, int? maximumCount, bool greedy = true)
        {
            Name = name;
            _parser = parser;
            _minimumCount = minimumCount;
            _maximumCount = maximumCount;
            _greedy = greedy;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> globalState, IParserState<TToken> parserState)
        {
            var nodes = new List<TreeNode<TToken>>();
            nodes.Add(
                new TreeNode<TToken>()
                {
                    Depth = 0,
                    Value = new UnionResultValue<TToken>(typeof(TResult))
                    {
                        ConsumedTokens = 0,
                        Reminder = input,
                        Position = input.Position,
                        Value = null,
                    },
                    IsLeaf = true
                });


            for (var iteration = 0; !_maximumCount.HasValue || iteration < _maximumCount; iteration++)
            {
                var nodesToProcess = nodes.Where(v => v.Depth == iteration);

                if (!nodesToProcess.Any())
                {
                    break;
                }

                foreach (var node in nodesToProcess.ToList())
                {
                    var next = _parser.Parse(node.Value.Reminder, globalState, parserState.Call(_parser, node.Value.Reminder));
                    if (next.WasSuccessful)
                    {
                        foreach (var item in next.Values)
                        {
                            if (!item.EmptyMatch)
                            {
                                var newResultValue = new UnionResultValue<TToken>(typeof(TResult))
                                {
                                    Value = item.Value,
                                    Reminder = item.Reminder,
                                    Position = item.Position,
                                    ConsumedTokens = item.ConsumedTokens,
                                };

                                node.IsLeaf = false;
                                nodes.Add(new TreeNode<TToken>()
                                {
                                    Depth = node.Depth + 1,
                                    Value = newResultValue,
                                    Parent = node,
                                    IsLeaf = true,
                                });
                            }
                        }
                    }
                }
            }
            var result = new List<IUnionResultValue<TToken>>();
            if (_greedy)
            {
                CollectResultsGreedy(nodes, result);
            }
            else
            {
                CollectResultsLazy(nodes, result);
            }

            if (result.Any())
            {
                return UnionResultFactory.Success(this, result);
            }
            else
            {
                return UnionResultFactory.Failure(this, "Failure", input);
            }
        }

        private void CollectResultsLazy(List<TreeNode<TToken>> nodes, List<IUnionResultValue<TToken>> result)
        {
            IEnumerable<TreeNode<TToken>> filtred;
            if (_minimumCount.HasValue)
            {
                filtred = nodes.Where(item => item.Depth >= _minimumCount.Value);
            }
            else
            {
                filtred = nodes;
            }

            foreach (var node in filtred)
            {
                var list = new List<TResult>();
                var @value = new UnionResultValue<TToken>(typeof(List<TResult>))
                {
                    Reminder = node.Value.Reminder,
                    Position = node.Value.Position,
                    ConsumedTokens = 0,
                    Value = list,
                };

                var tmp = node;
                while (tmp != null)
                {
                    var val = tmp.Value.GetValue<TResult>();
                    if (val != null && tmp.Parent != null)
                    {
                        list.Add(val);
                    }
                    @value.ConsumedTokens += tmp.Value.ConsumedTokens;
                    tmp = tmp.Parent;
                }

                list.Reverse();

                result.Add(value);
            }
        }

        private void CollectResultsGreedy(List<TreeNode<TToken>> nodes, List<IUnionResultValue<TToken>> result)
        {
            IEnumerable<TreeNode<TToken>> filtred;
            if (_minimumCount.HasValue)
            {
                filtred = nodes.Where(item => item.IsLeaf && item.Depth >= _minimumCount.Value);
            }
            else
            {
                filtred = nodes.Where(item => item.IsLeaf);
            }

            foreach (var leaf in filtred)
            {
                var list = new List<TResult>();
                var @value = new UnionResultValue<TToken>(typeof(List<TResult>))
                {
                    Reminder = leaf.Value.Reminder,
                    Position = leaf.Value.Position,
                    ConsumedTokens = 0,
                    Value = list,
                };

                var tmp = leaf;
                while (tmp != null)
                {
                    var val = tmp.Value.GetValue<TResult>();
                    if (val != null && tmp.Parent != null)
                    {
                        list.Add(val);
                    }
                    @value.ConsumedTokens += tmp.Value.ConsumedTokens;
                    tmp = tmp.Parent;
                }
                list.Reverse();
                result.Add(value);
            }
        }
    }
}
