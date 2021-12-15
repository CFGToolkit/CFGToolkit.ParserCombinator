using CFGToolkit.ParserCombinator.Input;
using CFGToolkit.ParserCombinator.Values;

namespace CFGToolkit.ParserCombinator.Parsers.Graphs
{
    public class TreeNode<TToken> where TToken : IToken
    {
        public IUnionResultValue<TToken> Value { get; set; }

        public TreeNode<TToken> Parent { get; set; }

        public int Depth { get; set; }

        public bool IsLeaf { get; set; }
    }
}
