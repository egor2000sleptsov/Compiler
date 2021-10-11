using System.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler {
    class Program {
        static void Main(string[] args) {
            // var command = Console.ReadLine().Trim().Split(' ');
            var command = args; //todo: Удали потом
            var typeAnalyze = "";
            var typeRead = "";
            var path = "";
            foreach ( var str in command ) {
                if ( str == "lexer" ) {
                    typeAnalyze = str;
                }
                else if ( str == "file" ) {
                    typeRead = str;
                }
                else if ( str == "dir" ) {
                    typeRead = str;
                }
                else if ( File.Exists(Lexer.testsPath + str) || Directory.Exists(Lexer.testsPath + str) ) {
                    path = str;
                }
            }

            if ( typeAnalyze == "lexer" ) {
                if ( typeRead == "file" ) {
                    if ( path != "" ) {
                        var lexer = new Lexer(path);
                        do {
                            lexer.GetNextLexem();
                            if ( !lexer.GetCurrentLexem().isEOF() ) {
                                Console.WriteLine(lexer.GetCurrentLexem().ToString());
                            }

                        } while ( !lexer.GetCurrentLexem().isEOF() );
                        lexer.Close();
                    }
                }
            }
        }
    }
}