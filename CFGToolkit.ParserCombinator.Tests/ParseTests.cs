using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace CFGToolkit.ParserCombinator.Tests
{
    public class ParseTests
    {
        private static IParser<CharToken, string> Comment =>
               from _1 in Parse.String("//").Text()
               from _2 in Parse.AnyChar().Except(Parse.LineEnd).Many().Text()
               from lines in Parse.LineEnd.Token().Many()
               select _1 + _2 + string.Join("", lines);

        private static IParser<CharToken, string> String =>
              from str in Parse.Regex("\"((?:\\\\.|[^\\\"])*)\"").Token()
              select str;

        private static IParser<CharToken, string> ProductionAttribute =
                from start in Parse.Char('[')
                from name in Parse.AnyChar().Except(Parse.Char(']')).Many().Text()
                from end in Parse.Char(']')
                select name;

        private static IParser<CharToken, string> Identifier = Parse.Regex(@"[$A-Z_a-z][\-0-9A-Z_a-z]*");

        private static IParser<CharToken, string> Production =
            from name in Identifier
            from attributes in ProductionAttribute.Many()
            from spaces1 in Parse.WhiteSpace.Many()
            from equal in Parse.String("::=")
            from spaces2 in Parse.WhiteSpace.Many()
            from body in Parse.Regex("[^ ]((?!(\r?\n){2}).)+", RegexOptions.Singleline)
            from lines in Parse.LineEnd.Many()
            select name;

        public static IParser<CharToken, string> Statement =
           from s in Production
           select s;

        private static IParser<CharToken, IEnumerable<string>> ProductionNames =
            from _x1 in Statement.Token().Many()
            select _x1.AsEnumerable();


        private static IParser<CharToken, IEnumerable<char>> ALetters = Parse.DelimitedBy(Parse.Char('a').Token(), Parse.String("|"), 1, null);

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

            Assert.Equal(1, result[0].ToList().Count);
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
                (from _0 in Parse.String("module", true).Text()
                 select _0)
            .Or((from _0 in Parse.String("macromodule", true).Text()
                 select _0)
            .Or((from _0 in Parse.String("connectmodule", true).Text()
                 select _0)));

            var result = Parser.TryParse(parser, "    module    ");

            Assert.True(result.WasSuccessful);
        }

        [Fact]
        public void OptionalTest()
        {
            var parser =
                from _0 in Parse.String("module", true).Text()
                from _1 in Parse.String("aaa").Optional()
                select _0;
            var result = Parser.TryParse(parser, "    module    ");

            Assert.True(result.WasSuccessful);
        }

        [Fact]
        public void TokenTest()
        {
            var parser =
                from _0 in Parse.Regex("R")
                from _1 in Parse.Char('*', true)
                from _2 in Parse.String("bbb")
                select _0;

            var result = Parser.TryParse(parser.End(), "R * bbb");

            Assert.True(result.WasSuccessful);
        }

        [Fact]
        public void Optional2Test()
        {
            var parser =
                from _0 in Parse.String("aaa").Optional()
                from _1 in Parse.String("aaa")
                select _1;
            var result = Parser.TryParse(parser, "aaa");

            Assert.True(result.WasSuccessful);
        }

        [Fact]
        public void ProblematicGrammar()
        {
            var parser =
                (from x in Parse.String("a")
                 select x)
                .Or(
                    from x in Parse.String("ab")
                    select x
                ).Many();

           
            var result = Parser.Parse(parser, "abab");
            Assert.Equal(3, result.Count);
            var firstResult = result[0].ToList();
            var secondResult = result[1].ToList();
            var thirdResult = result[2].ToList();

            var parser2 = from x in parser
                          from y in Parse.String("x")
                          select "test";

            var result2 = Parser.Parse(parser2, "ababx");
            Assert.Single(result2);
        }
    }
}
