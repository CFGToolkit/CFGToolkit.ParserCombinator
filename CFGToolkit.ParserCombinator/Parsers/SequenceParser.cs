using CFGToolkit.ParserCombinator.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class SequenceParser<TToken, TResult> : IParser<TToken, TResult> where TToken : IToken
    {
        private readonly Func<(string valueParserName, object value)[], TResult> select;
        private readonly Func<IParser<TToken>>[] parserFactories;

        public SequenceParser(string name, Func<(string valueParserName, object value)[], TResult> select, params Func<IParser<TToken>>[] parserFactories)
        {
            Name = name;
            this.select = select;
            this.parserFactories = parserFactories;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInput<TToken> input, IGlobalState<TToken> globalState, IParserState<TToken> parserState)
        {
            var parsers = new List<IParser<TToken>>();
            var nodes = new List<TreeNode<TToken>>();

            for (var i = 0; i < parserFactories.Length; i++)
            {
                var parser = parserFactories[i]();
                parsers.Add(parser);

                if (i == 0)
                {
                    var result = parser.Parse(input, globalState, parserState);

                    if (!result.WasSuccessful)
                    {
                        return UnionResultFactory.Failure(this, $"Parser {parser} was not successful", input);
                    }
                    else
                    {
                        foreach (IUnionResultValue<TToken> item in result.Values)
                        {
                            nodes.Add(new TreeNode<TToken>() { Depth = 0, Parent = null, Value = item });
                        }
                    }
                }
                else
                {
                    var previousNodes = nodes.Where(node => (node.Depth == i - 1)).ToList();
                    foreach (var node in previousNodes)
                    {
                        var tmp = parser.Parse(node.Value.Reminder, globalState, parserState.Call(parser, node.Value.Reminder));
                        foreach (IUnionResultValue<TToken> secondItem in tmp.Values)
                        {
                            nodes.Add(new TreeNode<TToken>() { Depth = i, Parent = node, Value = secondItem });
                        }
                    }

                    if (!nodes.Any(v => v.Depth == i))
                    {
                        return UnionResultFactory.Failure(this, $"{this.Name} failed", input);
                    }
                }
            }

            var resultValues = new List<IUnionResultValue<TToken>>();
            foreach (var leaf in nodes.Where(node => node.Depth == parsers.Count - 1))
            {
                var paths = new TreeNode<TToken>[parsers.Count];
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
                    value.Value = select(args);
                    value.ConsumedTokens = paths.Sum(path => (path.Value).ConsumedTokens);
                    value.Position = paths.First().Value.Position;
                    resultValues.Add(value);
                }
            }

            return UnionResultFactory.Success(this, resultValues);
        }
    }
}
