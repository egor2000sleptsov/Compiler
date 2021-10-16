using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Channels;
using Compiler;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Test {
    public class LexerTest {

        [Test]
        public void Test() {
            IEnumerable<string> inputFiles = Directory.EnumerateFiles(Lexer.testsPath);
            foreach ( string inputFile in inputFiles ) {
                var fileName = inputFile.Substring(inputFile.LastIndexOf('\\') + 1);
                var sw = new StreamWriter($"{Lexer.testsPath}result\\{fileName}");
                var lexer = new Lexer(fileName);
                try {
                    do {
                        if ( !lexer.GetNextLexem().isEOF() ) {
                            sw.WriteLine(lexer.GetCurrentLexem().ToString());
                        }
                    } while ( !lexer.GetCurrentLexem().isEOF() );
                }
                catch ( Exception e ) {
                    sw.WriteLine(e);
                }
                sw.Close();
            }
        }
    }
}