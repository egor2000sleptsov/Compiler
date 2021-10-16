using System;

namespace Compiler {
    public class LexerException : Exception {
        public LexerException(string msg) : base(msg) { }
    }
}