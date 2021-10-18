using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Compiler {
    class Program {
        static void Main(string[] args) {
            string[] commands = args.Length > 1 ? args : Console.ReadLine().Trim().Split(' ');
            if ( commands[0] == "-l" ) {
                if ( commands[1] == "-f" ) {
                    var lexer = new Lexer(commands[2]);
                    try {
                        do {
                            if ( !lexer.GetNextLexem().isEOF() ) {
                                Console.WriteLine(lexer.GetCurrentLexem().ToString());
                            }
                        } while ( !lexer.GetCurrentLexem().isEOF() );
                    }
                    catch ( LexerException e ) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else if ( commands[0] == "-p" ) {
                if ( commands[1] == "-f" ) {
                    try {
                        var lexer = new Lexer(commands[2], true);
                        var node = new Parser(lexer).ParseExpr();
                        Console.WriteLine(node.Print());
                    }
                    catch ( Exception e ) {
                        Console.WriteLine(e);
                    }
                }
            }
        }
    }
}