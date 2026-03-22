using CFGToolkit.ParserCombinator.Parsers;
using System.Linq;
using Xunit;

namespace CFGToolkit.ParserCombinator.Tests
{
    public class XOrMultipleParserTests
    {
        [Fact]
        public void XOrMultiple_MatchesFirstParser()
        {
            var parser = Parser.XOr<string>(
                "test",
                first: false,
                XOrParallelMode.None,
                Parser.String("abc"),
                Parser.String("def"));

            var result = parser.TryParse("abc");

            Assert.True(result.IsSuccessful);
            Assert.Equal("abc", result.Values[0].Value);
        }

        [Fact]
        public void XOrMultiple_MatchesSecondParser()
        {
            var parser = Parser.XOr<string>(
                "test",
                first: false,
                XOrParallelMode.None,
                Parser.String("abc"),
                Parser.String("def"));

            var result = parser.TryParse("def");

            Assert.True(result.IsSuccessful);
            Assert.Equal("def", result.Values[0].Value);
        }

        [Fact]
        public void XOrMultiple_FailsWhenNoParsersMatch()
        {
            var parser = Parser.XOr<string>(
                "test",
                first: false,
                XOrParallelMode.None,
                Parser.String("abc"),
                Parser.String("def"));

            var result = parser.TryParse("xyz");

            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public void XOrMultiple_FirstMode_FallsThroughToSequentialWhenFirstSetDoesNotMatch()
        {
            // Parser with first mode enabled, first set only covers parser 0 ("abc")
            // Parser 1 ("def") is not in the first set
            var parser = Parser.XOr<string>(
                "test",
                first: true,
                XOrParallelMode.None,
                Parser.String("abc"),
                Parser.String("def"))
                .Tag("First:0", "abc");

            // "def" doesn't match the first set ("abc"), but should still succeed
            // because the fix allows fallthrough to sequential parsing
            var result = parser.TryParse("def");

            Assert.True(result.IsSuccessful);
            Assert.Equal("def", result.Values[0].Value);
        }

        [Fact]
        public void XOrMultiple_FirstMode_UsesFirstSetWhenMatched()
        {
            var parser = Parser.XOr<string>(
                "test",
                first: true,
                XOrParallelMode.None,
                Parser.String("abc"),
                Parser.String("def"))
                .Tag("First:0", "abc")
                .Tag("First:1", "def");

            var result = parser.TryParse("abc");

            Assert.True(result.IsSuccessful);
            Assert.Equal("abc", result.Values[0].Value);
        }

        [Fact]
        public void XOrMultiple_FirstMode_FailsWhenNoParsersMatch()
        {
            var parser = Parser.XOr<string>(
                "test",
                first: true,
                XOrParallelMode.None,
                Parser.String("abc"),
                Parser.String("def"))
                .Tag("First:0", "abc")
                .Tag("First:1", "def");

            var result = parser.TryParse("xyz");

            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public void XOrMultiple_GenericToken_MatchesFirstParser()
        {
            var parser = Parser.String("hello")
                .XOr(Parser.String("world"));

            var result = parser.TryParse("hello");

            Assert.True(result.IsSuccessful);
            Assert.Equal("hello", result.Values[0].Value);
        }

        [Fact]
        public void XOrMultiple_GenericToken_MatchesSecondParser()
        {
            var parser = Parser.String("hello")
                .XOr(Parser.String("world"));

            var result = parser.TryParse("world");

            Assert.True(result.IsSuccessful);
            Assert.Equal("world", result.Values[0].Value);
        }

        [Fact]
        public void XOrMultiple_GenericToken_FailsWhenNoneMatch()
        {
            var parser = Parser.String("hello")
                .XOr(Parser.String("world"));

            var result = parser.TryParse("other");

            Assert.False(result.IsSuccessful);
        }
    }
}
