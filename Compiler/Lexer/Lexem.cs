using System;

namespace Compiler {
    public class Lexem {
        private Tuple<int, int> _coordinates;
        private Lexer.States _type;
        private dynamic _value;
        private string _code;

        public Lexem(Tuple<int, int> coordinates, Lexer.States type, dynamic value, string code) {
            _coordinates = coordinates;
            _type = type;
            _value = value;
            _code = code;
        }

        public override string ToString() {
            return $"{_coordinates.ToString()}\t\t{_type}\t\t{_value}\t\t{_code}";
        }

        public Tuple<int, int> GetCoordinates() {
            return _coordinates;
        }

        public new Lexer.States GetType() {
            return _type;
        }

        public string GetValue() {
            return _value;
        }

        public string GetCode() {
            return _code;
        }

        public bool isEOF() {
            return _type == Lexer.States.EOF;
        }
    }
}