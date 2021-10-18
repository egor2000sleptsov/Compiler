using System;

namespace Compiler {
    public class ParserException : Exception {
        public ParserException(string msg) : base(msg) { }
    }
}