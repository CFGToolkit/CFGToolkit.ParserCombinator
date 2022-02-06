using System.Collections.Generic;
using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.State;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers
{
    public class CastParser<TToken, TBase, TDerive> : IParser<TToken, TBase> where TToken : IToken where TDerive : TBase
    {
        private readonly IParser<TToken, TDerive> _parser;

        public CastParser(string name, IParser<TToken, TDerive> parser)
        {
            Name = name;
            _parser = parser;
        }

        public string Name { get; set; }

        public IUnionResult<TToken> Parse(IInputStream<TToken> input, IGlobalState<TToken> globalState, IParserCallStack<TToken> parserCallStack)
        {
            var firstResult = _parser.Parse(input, globalState, parserCallStack.Call(_parser, input));

            if (firstResult.WasSuccessful)
            {
                var values = new List<IUnionResultValue<TToken>>();

                foreach (var item in firstResult.Values)
                {
                    values.Add(new UnionResultValue<TToken>(typeof(TBase))
                    {
                        Value = item.Value,
                        ConsumedTokens = item.ConsumedTokens,
                        Reminder = item.Reminder,
                        Position = item.Position,
                    });
                }
                return UnionResultFactory.Success<TToken, TBase>(this, values);
            }
            else
            {
                return UnionResultFactory.Failure(this, "Parser failed before casting", input);
            }
        }
    }
}
