using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.Parsers.Graphs;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class RepeatParser<TToken, TResult> : BaseParser<TToken, List<TResult>> where TToken : IToken
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

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var nodes = new List<TreeNode<TToken>>
            {
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
                }
            };


            for (var iteration = 0; !_maximumCount.HasValue || iteration < _maximumCount; iteration++)
            {
                var toAdd = new List<TreeNode<TToken>>();

                foreach (var node in Where(nodes, iteration))
                {
                    var next = _parser.Parse(node.Value.Reminder, globalState, parserCallStack.Call(_parser, node.Value.Reminder));
                    if (next.IsSuccessful)
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
                                toAdd.Add(new TreeNode<TToken>()
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
                if (toAdd.Count == 0)
                {
                    break;
                }
                nodes.AddRange(toAdd);
            }
            List<IUnionResultValue<TToken>> result;

            if (_greedy)
            {
                result = CollectResultsGreedy(nodes);
            }
            else
            {
                result = CollectResultsLazy(nodes);
            }

            if (result.Any())
            {
                return UnionResultFactory.Success(this, result);
            }
            else
            {
                return UnionResultFactory.Failure(this, "Failure", nodes.Count > 0 ? nodes.Max(n => n.Value.ConsumedTokens) : 0, input.Position);
            }
        }

        private IEnumerable<TreeNode<TToken>> Where(List<TreeNode<TToken>> list, int depth)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Depth == depth)
                {
                    yield return list[i];
                }

                if (list[i].Depth < depth)
                {
                    break;
                }
            }

            yield break;
        }

        private List<IUnionResultValue<TToken>> CollectResultsLazy(List<TreeNode<TToken>> nodes)
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

            var result = new List<IUnionResultValue<TToken>>(filtred.Count());

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

            return result;
        }

        private List<IUnionResultValue<TToken>>  CollectResultsGreedy(List<TreeNode<TToken>> nodes)
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

            var result = new List<IUnionResultValue<TToken>>(filtred.Count());

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

            return result;
        }
    }
}
