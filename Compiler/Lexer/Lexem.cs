using System;

namespace Compiler {
    public class Lexem {
        private Tuple<int, int> _coordinates;
        private Lexer.States _state;
        private dynamic _value; //используй object
        private string _code;

        public Lexem(Tuple<int, int> coordinates, Lexer.States state, dynamic value, string code) {
            _coordinates = coordinates;
            _state = state;
            _value = value;
            _code = code;
        }

        public override string ToString() => $"{_coordinates}\t\t{_state}\t\t{_value}\t\t{_code}";
        

        public Tuple<int, int> GetCoordinates() => _coordinates;

        public new Lexer.States GetType() => _state;

        public string GetValue() => _value;

        public string GetCode() => _code;

        public bool isEOF() {
            return _state == Lexer.States.EOF;
        }
    }
}