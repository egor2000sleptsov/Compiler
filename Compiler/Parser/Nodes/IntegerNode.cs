namespace Compiler.Nodes {
    public class IntegerNode : Node {
        private Lexem _lexem;

        public IntegerNode(Lexem lexem) {
            _lexem = lexem;
        }

        public override string Print(int priority = 0) => GetValue();

        public override string GetValue() => _lexem.GetValue().ToString();
    }
}