namespace Compiler.Nodes {
    public abstract class Node {
        public abstract string Print(int priority = 0);

        public abstract string GetValue();
    }
}