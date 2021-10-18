using System;
using Compiler.Nodes;
using static Compiler.Lexer;

namespace Compiler {
    public class Parser {
        private Lexer _lexer;
        private Lexem _currentLexem;
        
        public Parser(Lexer lexer) {
            _lexer = lexer;
            _currentLexem = lexer.GetNextLexem();
        }

        public Node ParseExpr() {
            if ( _currentLexem.isEOF() ) throw new Exception();

            var left = ParseTerm();
            var operation = _currentLexem;
            while ( operation.GetValue().ToString() == "-" || operation.GetValue().ToString() == "+") {
                _currentLexem = _lexer.GetNextLexem();
                var right = ParseExpr();
                return new BinaryOperationNode(operation, left, right);
            }
            
            return left;
        }

        private Node ParseTerm() {
            var left = ParseFactor();
            var operation = _currentLexem;
            while ( operation.GetValue().ToString() == "*" || operation.GetValue().ToString() == "/"  || operation.GetValue().ToString() == "mod" || operation.GetValue().ToString() == "div") {
                _currentLexem = _lexer.GetNextLexem();
                var right = ParseTerm();
                return new BinaryOperationNode(operation, left, right);
            }
            return left;
        }

        private Node ParseFactor() {
            var lexem = _currentLexem;
            _currentLexem = _lexer.GetNextLexem();
            if ( lexem.GetType() == States.Identifier ) return new IdentifierNode(lexem);
            if ( lexem.GetType() == States.Integer ) return new IntegerNode(lexem);
            if ( lexem.GetType() == States.Real ) return new RealNode(lexem);

            if ( lexem.GetValue().ToString() == "-" || lexem.GetValue().ToString() == "+") {
                return new UnaryOperationNode(ParseFactor(), lexem);
            }
            if ( lexem.GetValue().ToString() == "(" ) {
                var left = ParseExpr();
                lexem = _currentLexem;

                if ( lexem.GetValue().ToString() != ")" ) throw new Exception();

                _currentLexem = _lexer.GetNextLexem();
                return left;
            }
            throw new Exception();
        }
    }
}