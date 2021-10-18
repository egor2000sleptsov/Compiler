using System.Linq;

namespace Compiler.Nodes {
    public class BinaryOperationNode : Node {
        private Lexem _operation;
        private Node _left;
        private Node _right;

        public BinaryOperationNode(Lexem operation, Node left, Node right) {
            _operation = operation;
            _left = left;
            _right = right;
        }

        public override string Print(int priority = 0) {
            var tmp = " |";
            var leftOperand = _left.Print(priority + 1);
            var rightOperand = _right.Print(priority + 1);

            return
                $"{GetValue()}\n{string.Concat(Enumerable.Repeat(tmp, priority+1))}{leftOperand}\n{string.Concat(Enumerable.Repeat(tmp, priority + 1))}{rightOperand}";
        }

        public override string GetValue() => _operation.GetValue().ToString();
    }
}