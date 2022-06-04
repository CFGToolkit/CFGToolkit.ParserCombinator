using CFGToolkit.ParserCombinator.Input;
using System.Collections.Concurrent;
using System.Linq;

namespace CFGToolkit.ParserCombinator.State
{
    public class Scope<TToken> where TToken : IToken
    {
        public Scope(Scope<TToken> parent)
        {
            Parent = parent;
        }

        public int? StartPosition { get; set; }

        public int? EndPosition { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public ConcurrentDictionary<string, SymbolEntry> Symbols { get; set; } = new ConcurrentDictionary<string, SymbolEntry>();

        public Scope<TToken> Parent { get; }

        public void RegisterSymbol(string name, string type, int position)
        {
            var symbol = new SymbolEntry
            {
                Type = type,
                Name = name,
                Position = position,
            };

            Symbols[name] = symbol;
        }

        public bool HasSymbol(string type, string name)
        {
            return Symbols.ContainsKey(name) && Symbols[name].Type == type;
        }

        public bool HasSymbol(string[] types, string name)
        {
            return Symbols.ContainsKey(name) && types.Contains(Symbols[name].Type);
        }

        public void RegisterDefinition(string name, string type, int position)
        {
            RegisterSymbol(name, type, position);
        }

        public bool HasDefinition(string[] types, string name)
        {
            if (HasSymbol(types, name))
            {
                return true;
            }

            return Parent?.HasDefinition(types, name) ?? false;
        }

        public bool HasDefinition(string type, string name)
        {
            if (HasSymbol(type, name))
            {
                return true;
            }

            return Parent?.HasDefinition(type, name) ?? false;
        }

        public void OpenChildScope(string type, int position, IParserCallStack<TToken> callStack)
        {
            var @new = new Scope<TToken>(this) { Type = type, StartPosition = position };
            callStack.CurrentScope = @new;
        }

        public void Close(int position)
        {
            EndPosition = position;
        }
    }
}

