using System;

namespace Compiler {
    public class Lexem {
        private Tuple<int, int> _coordinates;
        private Lexer.States _state;
        private object _value;
        private string _sourceCode;

        public Lexem(Tuple<int, int> coordinates, Lexer.States state, object value, string sourceCode) {
            _coordinates = coordinates;
            _state = state;
            _value = value;
            _sourceCode = sourceCode;
        }

        public override string ToString() => $"{_coordinates.Item1} {_coordinates.Item2}\t\t{_state}\t\t{_value}\t\t{_sourceCode}";
        
        public object GetValue() => _value;

        public Lexer.States GetType() => _state;

        public bool isEOF() => _state == Lexer.States.EOF;
    }
}