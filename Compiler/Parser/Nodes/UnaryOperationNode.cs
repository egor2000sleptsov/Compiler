namespace Compiler.Nodes {
    public class UnaryOperationNode : Node {
        private Lexem _operation;
        private Node _operand;

        public UnaryOperationNode(Node operand, Lexem operation) {
            _operation = operation;
            _operand = operand;
        }

        public override string Print(int priority = 0) => GetValue() + _operand.GetValue();

        public override string GetValue() => _operation.GetValue().ToString();
    }
}