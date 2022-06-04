using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.Parsers.Graphs;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class SequenceParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly Func<(string valueParserName, object value)[], TResult> _factory;
        private readonly ThreadLocal<IParser<TToken>>[] _parserFactories;

        public SequenceParser(string name, Func<(string valueParserName, object value)[], TResult> select, params ThreadLocal<IParser<TToken>>[] parserFactories)
        {
            Name = name;
            _factory = select;
            _parserFactories = parserFactories;
        }

        public string Name { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            if (_parserFactories.Length == 1)
            {
                var parser = _parserFactories[0].Value;
                var result = parser.Parse(input, globalState, parserCallStack.Call(parser, input));

                if (result.WasSuccessful)
                {
                    var values = new List<IUnionResultValue<TToken>>(result.Values.Count);
                    foreach (var value in result.Values)
                    {
                        var newValue = new UnionResultValue<TToken>(typeof(TResult));
                        newValue.Reminder = value.Reminder;
                        newValue.Value = _factory(new[] { (parser.Name, (object)value.Value) });
                        newValue.ConsumedTokens = value.ConsumedTokens;
                        newValue.Position = value.Position;
                        values.Add(newValue);
                    }

                    return UnionResultFactory.Success(this, values);
                }
                else
                {
                    return UnionResultFactory.Failure(this, "Parser failed", input);
                }
            }

            var parsers = new IParser<TToken>[_parserFactories.Length];
            var nodes = new List<TreeNode<TToken>>[_parserFactories.Length];

            for (var i = 0; i < _parserFactories.Length; i++)
            {
                var parser = _parserFactories[i].Value;
                parsers[i] = parser;
                

                if (i == 0)
                {
                    var result = parser.Parse(input, globalState, parserCallStack.Call(parser, input));

                    if (!result.WasSuccessful)
                    {
                        return UnionResultFactory.Failure(this, $"Parser {parser.Name} was not successful", input);
                    }
                    else
                    {
                        nodes[0] = new List<TreeNode<TToken>>(result.Values.Count);
                        foreach (IUnionResultValue<TToken> item in result.Values)
                        {
                            nodes[0].Add(new TreeNode<TToken>() { Depth = 0, Parent = null, Value = item });
                        }
                    }
                }
                else
                {
                    nodes[i] = new List<TreeNode<TToken>>();

                    foreach (var node in nodes[i - 1])
                    {
                        var tmp = parser.Parse(node.Value.Reminder, globalState, parserCallStack.Call(parser, node.Value.Reminder));
                        if (tmp.Values != null)
                        {
                            foreach (IUnionResultValue<TToken> secondItem in tmp.Values)
                            {
                                nodes[i].Add(new TreeNode<TToken>() { Depth = i, Parent = node, Value = secondItem });
                            }
                        }
                    }

                    if (nodes[i] == null || !nodes[i].Any())
                    {
                        return UnionResultFactory.Failure(this, "Parser failed", input);
                    }
                }
            }

            var resultValues = new List<IUnionResultValue<TToken>>(nodes[parsers.Length - 1].Count);
            foreach (var leaf in nodes[parsers.Length - 1])
            {
                var paths = new TreeNode<TToken>[parsers.Length];
                paths[paths.Length - 1] = leaf;

                for (var k = paths.Length - 2; k >= 0; k--)
                {
                    paths[k] = paths[k + 1].Parent;
                }

                var value = new UnionResultValue<TToken>(typeof(TResult));
                value.Reminder = leaf.Value.Reminder;

                var args = new (string valueParserName, object value)[paths.Length];

                for (var i = 0; i < paths.Length; i++)
                {
                    args[i] = (parsers[i].Name, paths[i].Value.Value);
                    value.ConsumedTokens += paths[i].Value.ConsumedTokens;
                }
                value.Value = _factory(args);
                value.Position = paths[0].Value.Position;

                resultValues.Add(value);
            }

            return UnionResultFactory.Success(this, resultValues);
        }
    }
}
