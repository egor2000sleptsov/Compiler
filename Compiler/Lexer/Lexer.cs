using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//todo: Ошибки, Директории обрабатывать, тесты, еще сделать зарезервированные слова 

namespace Compiler {
    public class Lexer {
        public enum States {
            Start = 1,
            Integer,
            Real,
            Identifier, //Переменная
            String, // TODO: Надо сделать: Операции, 
            Operation,
            OneLineComment,
            ManyLineComment, // фигурные скобки, обрабатывать как строку (логика).   КОММЕНТЫ ЭТО НЕ ЛЕКСЕМЫ НЕ ДОБАВЛЯЙ ИХ, ДУРАК
            Separator, // Разделитель 
            Assignment, // Присваивание 
            EOF,
        }

        private Dictionary<string, char> _reserved = new Dictionary<string, char> {
                                                         { "Space", ' ' },
                                                         { "Tab", '\t' },
                                                         { "NewLine", '\n' },
                                                         { "NewLine_r", '\r' },
                                                         { "Underline", '_' },
                                                     };

        private ArrayList _operations = new ArrayList() {
                                                            '+', '-', '*', '/', '=', '<', '>', "div", "mod", "not",
                                                            "and", "or", "ord", "chr", "sizeof", "pi",
                                                            "int", "trunc", "round", "frac", "odd", "**"
                                                        };

        public static readonly string testsPath = @"F:\Programming\projects\Compiler\Compiler\Lexer\tests\";
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
            while ( _buffer != "" || _symbol != '\uffff' ) {
                switch ( _state ) {
                    case States.Start: {
                        if ( _reserved.ContainsValue(_symbol) ) {
                            if ( _reserved["NewLine"] == _symbol ) NextLine();
                            GetNextSymbol();
                        }
                        else if ( Char.IsLetter(_symbol) ) KeepSymbol(States.Identifier, true);
                        else if ( Char.IsDigit(_symbol) ) KeepSymbol(States.Integer, true);
                        else if ( _symbol.Equals('\u0027') ) KeepSymbol(States.String, true);
                        else if ( _operations.Contains(_symbol) ) KeepSymbol(States.Operation, true);


                        break;
                    }

                    case States.Identifier: {
                        if ( Char.IsLetterOrDigit(_symbol) || _reserved["Underline"] == _symbol ) KeepSymbol();
                        else {
                            _lexem = new Lexem(_coordinates, _state, _buffer.ToLower(), _buffer); //todo: code and value
                            _state = States.Start;
                            return _lexem;
                        }

                        break;
                    }

                    case States.Integer: {
                        if ( char.IsDigit(_symbol) ) KeepSymbol();
                        else if ( _symbol == '.' ) KeepSymbol(States.Real);
                        else {
                            _lexem = new Lexem(_coordinates, _state, int.Parse(_buffer), _buffer);
                            _state = States.Start;
                            return _lexem;
                        }

                        break;
                    }

                    case States.Real: {
                        if ( char.IsDigit(_symbol) ) KeepSymbol();
                        else {
                            _lexem = new Lexem(_coordinates, _state, double.Parse(_buffer.Replace('.', ',')), _buffer);
                            _state = States.Start;
                            return _lexem;
                        }

                        break;
                    }

                    case States.String: {
                        KeepSymbol();
                        if ( _symbol.Equals('\u0027') ) {
                            KeepSymbol();
                            _lexem = new Lexem(_coordinates, _state, _buffer,
                                               _buffer); // todo : нужно ли убрать кавычку в начале и конце
                            _state = States.Start;
                            return _lexem;
                        }

                        break;
                    }

                    case States.Operation: {
                        if ( _buffer + _symbol == "//" ) {
                            _state = States.OneLineComment;
                        }
                        else {
                            if ( _operations.Contains(_symbol + _buffer) )
                                AddBuff();
                            _lexem = new Lexem(_coordinates, _state, double.Parse(_buffer.Replace('.', ',')), _buffer);
                            _state = States.Start;
                            return _lexem;
                        }

                        break;
                    }
                }
            }

            _lexem = new Lexem(_coordinates, States.EOF, _buffer, _buffer);
            return _lexem;
        }

        private void KeepSymbol(States state = States.EOF, bool saveCoordinates = false) {
            if ( state != States.EOF ) _state = state;
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
    }
}