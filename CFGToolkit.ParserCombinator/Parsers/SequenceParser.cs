using System;
using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.Parsers.Graphs;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class SequenceParser<TToken, TResult> : BaseParser<TToken, TResult> where TToken : IToken
    {
        private readonly Func<IParser<TToken>, (IParser<TToken>, IUnionResultValue<TToken>)[], TResult> _factory;
        private readonly Lazy<IParser<TToken>>[] _parsers;

        public SequenceParser(string name, Func<IParser<TToken>, (IParser<TToken>, IUnionResultValue<TToken>)[], TResult> valueFactory, params Lazy<IParser<TToken>>[] parsers)
        {
            Name = name;

            _factory = valueFactory;
            _parsers = parsers;
        }

        protected override IUnionResult<TToken> ParseInternal(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            if (_parsers.Length == 1)
            {
                var parser = _parsers[0].Value;
                var result = parser.Parse(input, globalState, parserCallStack.Call(parser, input));

                if (result.IsSuccessful)
                {
                    var values = new List<IUnionResultValue<TToken>>(result.Values.Count);
                    foreach (var value in result.Values)
                    {
                        var newValue = new UnionResultValue<TToken>(typeof(TResult));
                        newValue.Reminder = value.Reminder;
                        newValue.Value = _factory(this, new[] { (parser, value) });
                        newValue.ConsumedTokens = value.ConsumedTokens;
                        newValue.Position = value.Position;
                        values.Add(newValue);
                    }

                    return UnionResultFactory.Success(this, values);
                }
                else
                {
                    return UnionResultFactory.Failure(this, "Parser failed", result.MaxConsumed, input.Position);
                }
            }

            var parsers = new IParser<TToken>[_parsers.Length];

            // Only keep the current level of tree nodes; previous levels are
            // still reachable via Parent pointers for path reconstruction.
            List<TreeNode<TToken>> currentNodes = null;

            for (var i = 0; i < _parsers.Length; i++)
            {
                var parser = _parsers[i].Value;
                parsers[i] = parser;

                if (i == 0)
                {
                    var result = parser.Parse(input, globalState, parserCallStack.Call(parser, input));

                    if (!result.IsSuccessful)
                    {
                        return UnionResultFactory.Failure(this, $"Parser {parser.Name} was not successful", result.MaxConsumed, input.Position);
                    }
                    else
                    {
                        currentNodes = new List<TreeNode<TToken>>(result.Values.Count);
                        foreach (IUnionResultValue<TToken> item in result.Values)
                        {
                            currentNodes.Add(new TreeNode<TToken>() { Depth = 0, Parent = null, Value = item, IsSuccess = true });
                        }
                    }
                }
                else
                {
                    var nextNodes = new List<TreeNode<TToken>>();

                    int max = 0;
                    foreach (var node in currentNodes)
                    {
                        if (node.IsSuccess)
                        {
                            var tmp = parser.Parse(node.Value.Reminder, globalState, parserCallStack.Call(parser, node.Value.Reminder));

                            if (tmp.IsSuccessful && tmp.Values != null)
                            {
                                foreach (IUnionResultValue<TToken> secondItem in tmp.Values)
                                {
                                    nextNodes.Add(new TreeNode<TToken>() { Depth = i, Parent = node, Value = secondItem, IsSuccess = true });
                                }
                            }
                        }
                        else
                        {
                            max = Options.FullErrorReporting ? Math.Max(max, node.Value.ConsumedTokens) : 0;
                        }
                    }

                    if (nextNodes.Count == 0)
                    {
                        return UnionResultFactory.Failure(this, "Parser failed", max, input.Position);
                    }

                    currentNodes = nextNodes;
                }
            }

            var resultValues = new List<IUnionResultValue<TToken>>(currentNodes.Count);
            foreach (var leaf in currentNodes)
            {
                var paths = new TreeNode<TToken>[parsers.Length];
                paths[paths.Length - 1] = leaf;

                for (var k = paths.Length - 2; k >= 0; k--)
                {
                    paths[k] = paths[k + 1].Parent;
                }

                var value = new UnionResultValue<TToken>(typeof(TResult));
                value.Reminder = leaf.Value.Reminder;

                var args = new (IParser<TToken>, IUnionResultValue<TToken>)[paths.Length];

                for (var i = 0; i < paths.Length; i++)
                {
                    args[i] = (parsers[i], paths[i].Value);
                    value.ConsumedTokens += paths[i].Value.ConsumedTokens;
                }
                value.Value = _factory(this, args);
                value.Position = paths[0].Value.Position;

                resultValues.Add(value);
            }

            return UnionResultFactory.Success(this, resultValues);
        }
    }
}
