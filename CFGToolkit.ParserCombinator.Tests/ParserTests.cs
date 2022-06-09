using CFGToolkit.ParserCombinator.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace CFGToolkit.ParserCombinator.Tests
{
    public class ParserTests
    {
        private static IParser<CharToken, string> Comment =>
               from _1 in Parser.String("//").Text()
               from _2 in Parser.AnyChar().Except(Parser.LineEnd).Many().Text()
               from lines in Parser.LineEnd.Token().Many()
               select _1 + _2 + string.Join("", lines);

        private static IParser<CharToken, string> String =>
              from str in Parser.Regex("\"((?:\\\\.|[^\\\"])*)\"").Token()
              select str;

        private static IParser<CharToken, string> ProductionAttribute =
                from start in Parser.Char('[')
                from name in Parser.AnyChar().Except(Parser.Char(']')).Many().Text()
                from end in Parser.Char(']')
                select name;

        private static IParser<CharToken, string> Identifier = Parser.Regex(@"[$A-Z_a-z][\-0-9A-Z_a-z]*");

        private static IParser<CharToken, string> Production =
            from name in Identifier
            from attributes in ProductionAttribute.Many()
            from spaces1 in Parser.WhiteSpace.Many()
            from equal in Parser.String("::=")
            from spaces2 in Parser.WhiteSpace.Many()
            from body in Parser.Regex("[^ ]((?!(\r?\n){2}).)+", options: RegexOptions.Singleline)
            from lines in Parser.LineEnd.Many()
            select name;

        public static IParser<CharToken, string> Statement =
           from s in Production
           select s;

        private static IParser<CharToken, IEnumerable<string>> ProductionNames =
            from _x1 in Statement.Token().Many()
            select _x1.AsEnumerable();


        private static IParser<CharToken, IEnumerable<char>> ALetters = Parser.DelimitedBy(Parser.Char('a').Token(), Parser.String("|"), 1, null);

        [Fact]
        public void ProductionTest()
        {
            var text = @"parameter_identifier[no-token] ::= identifier
    | regex 
	| identifier";

            var result = Parser.Parse(Production, text);

            Assert.Single(result);
        }
        [Fact]
        public void ProductionNamesTest()
        {
            var text = @"parameter_identifier[no-token] ::= identifier
    | regex 
	| identifier

parameter_identifier2[no-token] ::= identifier
    | regex 
	| identifier

#END
";

            var result = Parser.Parse(ProductionNames, text).ToList();
            Assert.Equal(2, result[0].ToList().Count);

        }

        [Fact]
        public void DelimitedByTest()
        {
            var result = Parser.Parse(ALetters, "a | a | a| a");
            Assert.Equal(4, result[0].ToList().Count);
        }

        [Fact]
        public void DelimitedBy2Test()
        {
            var result = Parser.Parse(ALetters, "a");

            Assert.Single(result[0].ToList());
        }

        [Fact]
        public void CommentTest()
        {
            var text = "// text text text text2 ";

            var result = Parser.TryParse(Comment.End(), text);

            Assert.Equal("// text text text text2 ", result.Values.ToList()[0].Value);
        }

        [Fact]
        public void IdentifierTest()
        {
            var text = "_ABC";

            var result = Parser.TryParse(Identifier, text);

            Assert.Equal("_ABC", result.Values.ToList()[0].Value);
        }

        [Fact]
        public void StringTest()
        {
            var text = "   \" abc\\\"dd \"   ";

            var result = Parser.TryParse(String, text);

            Assert.Equal("\" abc\\\"dd \"", result.Values.ToList()[0].Value);
        }

        [Fact]
        public void ProductionAttributeTest()
        {
            var text = "[aaa]";

            var result = Parser.TryParse(ProductionAttribute, text);

            Assert.Equal("aaa", result.Values.ToList()[0].Value);
        }

        [Fact]
        public void ModuleKeywords()
        {
            var parser =
                (from _0 in Parser.String("module", true).Text()
                 select _0)
            .Or((from _0 in Parser.String("macromodule", true).Text()
                 select _0)
            .Or((from _0 in Parser.String("connectmodule", true).Text()
                 select _0)));

            var result = Parser.TryParse(parser, "    module    ");

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void OptionalTest()
        {
            var parser =
                from _0 in Parser.String("module", true).Text()
                from _1 in Parser.String("aaa").Optional()
                select _0;
            var result = Parser.TryParse(parser, "    module    ");

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void TokenTest()
        {
            var parser =
                from _0 in Parser.Regex("R")
                from _1 in Parser.Char('*', true)
                from _2 in Parser.String("bbb")
                select _0;

            var result = Parser.TryParse(parser.End(), "R * bbb");

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void Optional2Test()
        {
            var parser =
                from _0 in Parser.String("aaa").Optional()
                from _1 in Parser.String("aaa")
                select _1;
            var result = Parser.TryParse(parser, "aaa");

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void TagTest()
        {
            var parser = Parser.String("test").Tag("t1", "value");

            Assert.Equal("value", parser.Tags["t1"]);
        }

        [Fact]
        public void Empty()
        {
            var parser = Parser.String(string.Empty).Tag("t1", "value");

            var result = Parser.TryParse(parser, "");

            Assert.True(result.IsSuccessful);
            Assert.True(result.Values[0].EmptyMatch);
            Assert.Equal(0, result.Values[0].ConsumedTokens);
        }

        [Fact]
        public void Empty2()
        {
            var parser =
            (from _0 in Parser.String("module", true).Text()
             select _0)
        .XOr((from _0 in Parser.String("macromodule", true).Text()
              select _0)
        .XOr((from _0 in Parser.Return(string.Empty)
              select _0)));

            var parser2 =
                from _0 in parser
                from _1 in Parser.String("b")
                select _1;

            var result = Parser.TryParse(parser2, "b");

            Assert.True(result.IsSuccessful);
            Assert.False(result.Values[0].EmptyMatch);
            Assert.Equal(1, result.Values[0].ConsumedTokens);
        }

        [Fact]
        public void Tag2Test()
        {
            var parser = Parser.String("test").Tag("t1", "value").Tag("t2", "value2");

            Assert.Equal("value", parser.Tags["t1"]);
            Assert.Equal("value2", parser.Tags["t2"]);
        }

        [Fact]
        public void ProblematicGrammar()
        {
            var parser =
                (from x in Parser.String("a")
                 select x)
                .Or(
                    from x in Parser.String("ab")
                    select x
                ).Many();

           
            var result = Parser.Parse(parser, "abab");
            Assert.Equal(3, result.Count);
            var firstResult = result[0].ToList();
            var secondResult = result[1].ToList();
            var thirdResult = result[2].ToList();

            var parser2 = from x in parser
                          from y in Parser.String("x")
                          select "test";

            var result2 = Parser.Parse(parser2, "ababx");
            Assert.Single(result2);
        }
    }
}
