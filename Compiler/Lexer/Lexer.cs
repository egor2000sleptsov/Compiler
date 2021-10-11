﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//todo: Ошибки, Директории обрабатывать, тесты, еще сделать зарезервированные слова 

namespace Compiler {
    public class Lexer {
        public enum States {
            Start = 1,
            Integer, //done
            Real, //done
            RealE, //done
            RealEDegree, //done
            Identifier, //Переменная done
            String, // done
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
                                                         { "Underline", '_' },
                                                     };

        private ArrayList _operations = new ArrayList() {
                                                            '+', '-', '*', '/', '=', '<', '>', "div", "mod", "not",
                                                            "and", "or", "ord", "chr", "sizeof", "pi",
                                                            "int", "trunc", "round", "frac", "odd", "**"
                                                        };

        private ArrayList _separators = new ArrayList() { '.', ',', ':', ';', '(', ')', '[', ']',".." };
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
            while ( _buffer != "" || _state != States.EOF) {
                switch ( _state ) {
                    case States.Start:
                        if ( _whitespaces.ContainsValue(_symbol) ) {
                            if ( _whitespaces["NewLine"] == _symbol ) NextLine();
                            GetNextSymbol();
                        }
                        else if ( Char.IsLetter(_symbol) ) KeepSymbol(States.Identifier, true);
                        else if ( Char.IsDigit(_symbol) ) KeepSymbol(States.Integer, true);
                        else if ( _symbol.Equals('\u0027') ) KeepSymbol(States.String, true);
                        else if ( _operations.Contains(_symbol) ) KeepSymbol(States.Operation, true);
                        else if ( _symbol.Equals('{') ) KeepSymbol(States.ManyLineComment, true);
                        else if ( _separators.Contains(_symbol) ) KeepSymbol(States.Separator, true);
                        else if ( _symbol.Equals('\uffff') ) _state = States.EOF;


                        break;

                    case States.Identifier:
                        if ( Char.IsLetterOrDigit(_symbol) || _whitespaces["Underline"] == _symbol ) KeepSymbol();
                        else if ( _operations.Contains(_buffer.ToLower()) ) 
                            return SetAndReturnLexem(_buffer.ToLower(), States.Operation);
                        else return SetAndReturnLexem(_buffer.ToLower());
                        

                        break;

                    case States.Integer:
                        if ( char.IsDigit(_symbol) ) KeepSymbol();
                        else if ( _symbol == '.' ) KeepSymbol(States.Real);
                        else return SetAndReturnLexem(int.Parse(_buffer));

                        break;

                    case States.Real:
                        if ( char.IsDigit(_symbol) ) KeepSymbol();
                        else if ( char.ToLower(_symbol) == 'e' ) KeepSymbol(States.RealE);
                        else return SetAndReturnLexem(double.Parse(_buffer.Replace('.', ',')));

                        break;

                    case States.RealE:
                        if ( char.IsDigit(_symbol) ) KeepSymbol(States.RealEDegree);
                        else if ( _symbol == '+' || _symbol == '-' ) {
                            KeepSymbol();
                            if ( char.IsDigit(_symbol) ) KeepSymbol(States.RealEDegree);
                            else throw new Exception(); //TODO: Создай класс ошибок
                        }
                        else throw new Exception();


                        break;

                    case States.RealEDegree:
                        if ( char.IsDigit(_symbol) ) KeepSymbol();
                        else return SetAndReturnLexem(double.Parse(_buffer.Replace('.', ',')));

                        break;
                    case States.String:
                        KeepSymbol();
                        if ( _symbol.Equals('\u0027') ) {
                            KeepSymbol();
                            return SetAndReturnLexem();// todo : нужно ли убрать кавычку в начале и конце
                        }

                        break;

                    case States.Operation:
                        if ( _buffer + _symbol == "//" ) _state = States.OneLineComment;
                        else if ( _assignments.Contains(_buffer + _symbol) ) {
                            KeepSymbol(States.Assignment);
                        }
                        else {
                            if ( _operations.Contains(_symbol + _buffer) ) AddBuff();
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
                            KeepSymbol();
                            _state = States.Start;
                            ClearBuff();
                            // return GetNextLexem();
                        }
                        else KeepSymbol();

                        break;

                    case States.Separator:
                        if ( _buffer + _symbol == ":=" ) KeepSymbol(States.Assignment);
                        else if ( _separators.Contains(_buffer+_symbol) ) {
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

        public void Close() {
            _file.Close();
        }
    }
}