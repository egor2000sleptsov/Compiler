namespace Compiler.Nodes {
    public class IdentifierNode : Node {
        private Lexem _lexem;

        public IdentifierNode(Lexem lexem) {
            _lexem = lexem;
        }

        public override string Print(int priority = 0) => GetValue();

        public override string GetValue() => _lexem.GetValue().ToString();
    }
}