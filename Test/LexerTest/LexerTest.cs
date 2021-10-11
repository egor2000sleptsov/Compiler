using System;
using System.IO;
using Compiler;
using NUnit.Framework;

namespace Test {
    public class LexerTest {
        [Test]
        [TestCase(@"identifiers")]
        [TestCase(@"integers")]
        [TestCase(@"strings")]
        public void Test(string testDir) {
            var path = Lexer.testsPath + testDir + @"\";
            var lexer = new Lexer(testDir + @"\" + "input.txt");
            var flag = true;
            using ( var sw = new StreamWriter(path + "output.txt") ) {
                do {
                    lexer.GetNextLexem();
                    if ( !lexer.GetCurrentLexem().isEOF() ) {
                        sw.WriteLine(lexer.GetCurrentLexem().ToString());
                    }
                } while ( !lexer.GetCurrentLexem().isEOF() );
            }

            lexer.Close();
            using ( var correctAnswerFileReader = new StreamReader(path + "correctAnswer.txt") )
                using ( var outputFileReader = new StreamReader(path + "output.txt") ) {
                    while ( !correctAnswerFileReader.EndOfStream && !outputFileReader.EndOfStream && flag ) {
                        if ( !((char)correctAnswerFileReader.Read() == (char)outputFileReader.Read()) ) {
                            flag = false;
                        }

                        if ( correctAnswerFileReader.EndOfStream != outputFileReader.EndOfStream ) {
                            flag = false;
                        }
                    }
                }

            Assert.AreEqual(true, flag);
        }
    }
}