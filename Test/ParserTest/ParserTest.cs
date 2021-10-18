using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Compiler;
using NUnit.Framework;

namespace Test.ParserTest {
    public class ParserTest {
        private bool _isPassed;
        private byte _passedTests = 0;
        private byte _allTests = 0;
        private ArrayList _expectedList = new ArrayList();
        private ArrayList _mistakesList = new ArrayList();

        [Test]
        public void Test() {
            var inputFiles = Directory.EnumerateFiles(Lexer.parserTestsPath);
            foreach ( var inputFile in inputFiles ) {
                var fileName = inputFile.Substring(inputFile.LastIndexOf('\\') + 1);
                var sr = new StreamReader($"{Lexer.parserTestsPath}results\\{fileName}");
                try {
                    var lexer = new Lexer(fileName, true);
                    var node = new Parser(lexer).ParseExpr();

                    _isPassed = node.Print() == sr.ReadToEnd();
                }
                catch ( Exception e ) {
                    _isPassed = e.Message == sr.ReadToEnd();
                }
                finally {
                    Console.WriteLine($"{fileName} - {_isPassed}");
                    if ( _isPassed ) ++_passedTests;
                }

                ++_allTests;
            }


            Console.WriteLine($"\nTests: {_allTests}\nPassed: {_passedTests}");
            Assert.AreEqual(_allTests, _passedTests);
        }

        private void Test1() {
            IEnumerable<string> inputFiles = Directory.EnumerateFiles(Lexer.parserTestsPath);
            foreach ( var inputFile in inputFiles ) {
                var fileName = inputFile.Substring(inputFile.LastIndexOf('\\') + 1);
                var sw = new StreamWriter($"{Lexer.parserTestsPath}results\\{fileName}");
                var lexer = new Lexer(fileName, true);
                try {
                    var node = new Parser(lexer).ParseExpr();
                    sw.Write(node.Print());
                }
                catch ( Exception e ) {
                    sw.WriteLine(e.Message);
                }

                sw.Close();
            }
        }
    }
}