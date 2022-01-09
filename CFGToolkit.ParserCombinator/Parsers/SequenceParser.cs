using System;
using System.Collections.Generic;
using System.Linq;
using CFGToolkit.ParserCombinator;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.Parsers.Graphs;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class SequenceParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly Func<(string valueParserName, object value)[], TResult> _factory;
        private readonly Lazy<IParser<TToken>>[] _parserFactories;

        public SequenceParser(string name, Func<(string valueParserName, object value)[], TResult> select, params Lazy<IParser<TToken>>[] parserFactories)
        {
            Name = name;
            _factory = select;
            _parserFactories = parserFactories;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var parsers = new IParser<TToken>[_parserFactories.Length];
            var nodes = new List<TreeNode<TToken>>[_parserFactories.Length];

            for (var i = 0; i < _parserFactories.Length; i++)
            {
                var parser = _parserFactories[i].Value;
                parsers[i] = parser;
                nodes[i] = new List<TreeNode<TToken>>();

                if (i == 0)
                {
                    var result = parser.Parse(input, globalState, parserCallStack.Call(parser, input));

                    if (!result.WasSuccessful)
                    {
                        return UnionResultFactory.Failure(this, $"Parser {parser} was not successful", input);
                    }
                    else
                    {
                        foreach (IUnionResultValue<TToken> item in result.Values)
                        {
                            nodes[0].Add(new TreeNode<TToken>() { Depth = 0, Parent = null, Value = item });
                        }
                    }
                }
                else
                {
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

                    if (!nodes[i].Any())
                    {
                        return UnionResultFactory.Failure(this, $"{this.Name} failed", input);
                    }
                }
            }

            var resultValues = new List<IUnionResultValue<TToken>>();
            foreach (var leaf in nodes[parsers.Length - 1])
            {
                var paths = new TreeNode<TToken>[parsers.Length];
                paths[paths.Length - 1] = leaf;

                bool success = true;
                for (var k = paths.Length - 2; k >= 0; k--)
                {
                    paths[k] = paths[k + 1].Parent;
                }

                if (success)
                {
                    var value = new UnionResultValue<TToken>(typeof(TResult));
                    value.Reminder = leaf.Value.Reminder;

                    int i = 0;
                    var args = paths.Select(pathObject => (parsers[i++].Name, pathObject.Value.Value)).ToArray();
                    value.Value = _factory(args);
                    value.ConsumedTokens = paths.Sum(path => (path.Value).ConsumedTokens);
                    value.Position = paths.First().Value.Position;
                    resultValues.Add(value);
                }
            }

            return UnionResultFactory.Success(this, resultValues);
        }
    }
}
