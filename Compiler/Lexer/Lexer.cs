using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Text;

//todo: Ошибки, Директории обрабатывать, тесты, еще сделать зарезервированные слова 

namespace Compiler {
    public class Lexer {
        public enum States {
            Start = 1,
            Integer, //done
            Reserved, //done
            Real, //done
            RealE, //done
            RealEDegree, //done
            Identifier, //Переменная done
            String, // done
            StringASCII, // done
            Operation, // done
            OneLineComment, // done
            ManyLineComment, // фигурные скобки, обрабатывать как строку (логика).   КОММЕНТЫ ЭТО НЕ ЛЕКСЕМЫ НЕ ДОБАВЛЯЙ ИХ, ДУРАК done
            Separator, // Разделитель done
            Assignment, // Присваивание done
            Error,
            Null,
            EOF,
        }

        private Dictionary<string, char> _whitespaces = new Dictionary<string, char> {
                                                            { "Space", ' ' },
                                                            { "Tab", '\t' },
                                                            { "NewLine", '\n' },
                                                            { "NewLine_r", '\r' },
                                                        };

        private ArrayList _operations = new ArrayList() {
                                                            '+', '-', '*', '/', '=', '<', '>', "div", "mod", "not",
                                                            "and", "or", "ord", "chr", "sizeof", "pi",
                                                            "int", "trunc", "round", "frac", "odd", "**"
                                                        };

        private ArrayList _reserved = new ArrayList() {
                                                          "and", "array", "as", "auto", "begin", "case", "class",
                                                          "const", "constructor", "destructor", "div", "do", "downto",
                                                          "else", "end", "event", "except", "extensionmethod", "file",
                                                          "finalization", "finally", "for", "foreach", "function",
                                                          "goto", "if", "implementation", "in", "inherited",
                                                          "initialization", "interface", "is", "label", "lock", "loop",
                                                          "mod", "nil", "not", "of", "operator", "or", "procedure",
                                                          "program", "property", "raise", "record", "repeat", "sealed",
                                                          "set", "sequence", "shl", "shr", "sizeof", "template", "then",
                                                          "to", "try", "type", "typeof", "until", "uses", "using",
                                                          "var", "where", "while", "with", "xor", "abstract", "default",
                                                          "external", "forward", "internal", "on", "overload",
                                                          "override", "params", "private", "protected", "public",
                                                          "read", "readln", "reintroduce", "unit", "virtual", "write",
                                                          "writeln"
                                                      };

        private ArrayList _separators = new ArrayList() { '.', ',', ':', ';', '(', ')', '[', ']', ".." };
        private ArrayList _assignments = new ArrayList() { ":=", "+=", "-=", "*=", "/=" };

        public static readonly string testsPath = @"F:\Programming\projects\Compiler\Test\LexerTest\tests\";
        private string _buffer;
        private char _symbol;
        private StreamReader _file;
        private int _col;
        private int _line;
        private Tuple<int, int> _coordinates;
        private States _state;
        private Lexem _lexem;

        public Lexer(string path) {
            _file = new StreamReader(testsPath + path);
            _col = 0;
            _line = 1;
            _state = States.Start;
            GetNextSymbol();
        }

        public Lexem GetNextLexem() {
            ClearBuff();
            while ( _buffer != "" || _state != States.EOF ) {
                switch ( _state ) {
                    case States.Start:
                        if ( _whitespaces.ContainsValue(_symbol) ) {
                            if ( '\n' == _symbol ) NextLine();
                            GetNextSymbol();
                        }
                        else if ( Char.IsLetter(_symbol) ) KeepSymbol(States.Identifier, true);
                        else if ( Char.IsDigit(_symbol) ) KeepSymbol(States.Integer, true);
                        else if ( _symbol.Equals('\u0027') ) KeepSymbol(States.String, true);
                        else if ( _operations.Contains(_symbol) ) KeepSymbol(States.Operation, true);
                        else if ( _symbol.Equals('{') ) KeepSymbol(States.ManyLineComment, true);
                        else if ( _separators.Contains(_symbol) ) KeepSymbol(States.Separator, true);
                        else if ( _symbol == '#' ) KeepSymbol(States.StringASCII, true);
                        else if ( _symbol.Equals('\uffff') ) _state = States.EOF;


                        break;

                    case States.Identifier:
                        if ( Char.IsLetterOrDigit(_symbol) || '_' == _symbol ) KeepSymbol();
                        else if ( _operations.Contains(_buffer.ToLower()) )
                            return SetAndReturnLexem(_buffer.ToLower(), States.Operation);
                        else if ( _reserved.Contains(_buffer.ToLower()) )
                            return SetAndReturnLexem(_buffer.ToLower(), States.Reserved);
                        else if ( _separators.Contains(_symbol) ||
                                  _whitespaces.ContainsValue(_symbol) ||
                                  _operations.Contains(_symbol) ||
                                  _symbol.Equals('\uffff') ) {
                            return SetAndReturnLexem();
                        }
                        else {
                            ThrowException();
                        }

                        break;

                    case States.Integer:
                        if ( char.ToLower(_symbol) == 'e' ) KeepSymbol(States.RealE);
                        else if ( char.IsDigit(_symbol) ) KeepSymbol();
                        else if ( _symbol == '.' ) KeepSymbol(States.Real);
                        else if ( _separators.Contains(_symbol) ||
                                  _whitespaces.ContainsValue(_symbol) ||
                                  _operations.Contains(_symbol) ||
                                  _symbol.Equals('\uffff')
                        ) return SetAndReturnLexem(int.Parse(_buffer));
                        else ThrowException();

                        break;

                    case States.Real:
                        if ( char.IsDigit(_symbol) ) KeepSymbol();
                        else if ( char.ToLower(_symbol) == 'e' ) KeepSymbol(States.RealE);
                        else if ( _separators.Contains(_symbol) ||
                                  _whitespaces.ContainsValue(_symbol) ||
                                  _operations.Contains(_symbol) )
                            return SetAndReturnLexem(double.Parse(_buffer.Replace('.', ',')));
                        else ThrowException();
                        break;

                    case States.RealE:
                        if ( char.IsDigit(_symbol) ) KeepSymbol(States.RealEDegree);
                        else if ( _symbol == '+' || _symbol == '-' ) {
                            KeepSymbol();
                            if ( char.IsDigit(_symbol) ) KeepSymbol(States.RealEDegree);
                            else ThrowException();
                        }
                        else ThrowException();


                        break;

                    case States.RealEDegree:
                        if ( char.IsDigit(_symbol) ) KeepSymbol();
                        else if ( _operations.Contains(_symbol) ||
                                  _separators.Contains(_symbol) ||
                                  _whitespaces.ContainsValue(_symbol) )
                            return SetAndReturnLexem(double.Parse(_buffer.Replace('.', ',')), States.Real);
                        else {
                            ThrowException();
                        }
                        
                        break;
                    case States.String:
                        if ( _symbol.Equals('\u0027') ) {
                            KeepSymbol();
                            if ( _symbol == '#' ) {
                                KeepSymbol(States.StringASCII);
                            }
                            else {
                                var buffer = _buffer;
                                if ( buffer.Remove(buffer.IndexOf('\u0027'), buffer.IndexOf('\u0027', 1) + 1) == "" ) {
                                    return SetAndReturnLexem(_buffer.Substring(1, _buffer.Length - 2));
                                }

                                return SetAndReturnLexem(ParseASCII(_buffer));
                            }
                        }
                        else if ( _symbol == '\n' || _symbol == '\uffff' ) {
                            ThrowException();
                        }
                        else {
                            KeepSymbol();
                        }

                        break;

                    case States.StringASCII:
                        if ( char.IsDigit(_symbol) ) KeepSymbol();
                        else if ( _symbol == '#' ) {
                            if ( _buffer[_buffer.Length - 1] == '#' ) ThrowException();
                            KeepSymbol();
                        }
                        else if ( _symbol == '\u0027' ) {
                            if ( _buffer[_buffer.Length - 1] == '#' ) ThrowException();
                            KeepSymbol(States.String);
                        }
                        else if ( _symbol == '\uffff' ) {
                            if ( _buffer[_buffer.Length - 1] == '#' ) ThrowException();
                            return SetAndReturnLexem(ParseASCII(_buffer), States.String);
                        }
                        else {
                            if ( _buffer[_buffer.Length - 1] == '#' ) ThrowException();
                            if ( _operations.Contains(_symbol) ||
                                 _separators.Contains(_symbol) ||
                                 _whitespaces.ContainsValue(_symbol)
                            ) {
                                return SetAndReturnLexem(ParseASCII(_buffer), States.String);
                            }

                            ThrowException();
                        }

                        break;

                    case States.Operation:
                        if ( _buffer + _symbol == "//" ) _state = States.OneLineComment;
                        else if ( _assignments.Contains(_buffer + _symbol) ) {
                            KeepSymbol(States.Assignment);
                        }
                        else {
                            if ( _operations.Contains(_symbol + _buffer) ) KeepSymbol();
                            return SetAndReturnLexem(_buffer.ToLower());
                        }

                        break;
                    case States.OneLineComment:
                        if ( _symbol.Equals('\n') || _symbol.Equals('\uffff') ) {
                            _state = States.Start;
                            ClearBuff();
                        }
                        else KeepSymbol();

                        break;

                    case States.ManyLineComment:
                        if ( _symbol.Equals('}') ) {
                            KeepSymbol(States.Start);
                            ClearBuff();
                            // return GetNextLexem();
                        }
                        else if ( _symbol == '\uffff' ) {
                            _state = States.EOF;
                            ClearBuff();
                        }
                        else KeepSymbol();

                        break;

                    case States.Separator:
                        if ( _buffer + _symbol == ":=" ) KeepSymbol(States.Assignment);
                        else if ( _separators.Contains(_buffer + _symbol) ) {
                            KeepSymbol();
                            return SetAndReturnLexem();
                        }
                        else return SetAndReturnLexem();

                        break;
                    case States.Assignment:
                        return SetAndReturnLexem();
                }
            }

            _lexem = new Lexem(_coordinates, States.EOF, _buffer, _buffer);
            return _lexem;
        }

        private void KeepSymbol(States state = States.Null, bool saveCoordinates = false) {
            if ( state != States.Null ) _state = state;
            if ( saveCoordinates ) SaveCoordinates();
            AddBuff();
            GetNextSymbol();
        }

        public Lexem GetCurrentLexem() {
            return _lexem;
        }

        private void GetNextSymbol() {
            _symbol = (char)_file.Read();
            _col++;
        }

        private void NextLine() {
            _col = 0;
            _line++;
        }

        private void SaveCoordinates() {
            _coordinates = new Tuple<int, int>(_line, _col);
        }

        private void AddBuff() {
            _buffer += _symbol;
        }

        private void ClearBuff() {
            _buffer = "";
        }

        private Lexem SetAndReturnLexem(dynamic code = null, States state = States.Null) {
            code = code == null ? _buffer : code;
            state = state == States.Null ? _state : state;
            _state = States.Start;
            return _lexem = new Lexem(_coordinates, state, code, _buffer);
        }

        private string ParseASCII(string value) {
            byte[] tmp = new byte[1];
            char[] sharpAndQuote = { '#', '\u0027' };
            var result = "";
            var n = value.Count(x => x == '#');
            if ( value.StartsWith('\u0027') ) {
                result += value.Substring(1, value.IndexOf('#') - 2);
                value = value.Substring(value.IndexOf('#'));
            }

            for ( var i = 0; i < n; i++ ) {
                var j = value.Substring(1).IndexOfAny(sharpAndQuote) == -1
                            ? value.Length - 1
                            : value.IndexOfAny(sharpAndQuote, 1) - 1;
                tmp[0] = byte.Parse(value.Substring(1, j));
                result += Encoding.ASCII.GetString(tmp)[0];
                if ( i + 1 != n ) value = value.Substring(value.IndexOfAny(sharpAndQuote, 1));
                else if ( value.Contains('\u0027') ) {
                    value = value.Substring(j + 1);
                    result += value.Substring(1, value.Length - 2);
                }

                if ( value.StartsWith('\u0027') && i + 1 != n ) {
                    result += value.Substring(1, value.IndexOf('#') - 2);
                    value = value.Substring(value.IndexOf('#'));
                }
            }

            return result;
        }

        private void ThrowException() {
            throw new LexerException(
                $"Unexpected symbol '{_symbol}' at ({_coordinates.Item1},{_coordinates.Item2 + _buffer.Length})");
        }

        public void Close() {
            _file.Close();
        }
    }
}