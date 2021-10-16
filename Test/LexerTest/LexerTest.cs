﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Channels;
using Compiler;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Test {
    public class LexerTest {
        private bool _isPassed;
        private byte _passedTests = 0;
        private byte _allTests = 0;
        private ArrayList _expectedList = new ArrayList();
        private ArrayList _mistakesList = new ArrayList();

        [Test]
        private void MakeResult() {
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
                    sw.WriteLine(e.Message);
                }
                sw.Close();
            }
        }
        
        [Test]
        public void Test() {
            IEnumerable<string> inputFiles = Directory.EnumerateFiles(Lexer.testsPath);
            foreach ( string inputFile in inputFiles ) {
                var fileName = inputFile.Substring(inputFile.LastIndexOf('\\') + 1);
                var sr = new StreamReader($"{Lexer.testsPath}result\\{fileName}");
                var lexer = new Lexer(fileName);
                try {
                    do {
                        if ( !lexer.GetNextLexem().isEOF() ) {
                            _isPassed = CheckResult(lexer.GetCurrentLexem().ToString(), sr.ReadLine());
                        }
                    } while ( !lexer.GetCurrentLexem().isEOF() );
                }
                catch ( Exception e ) {
                    _isPassed = CheckResult(e.Message, sr.ReadLine());
                }
                finally {
                    Console.WriteLine($"{fileName} - {_isPassed}");
                    if ( _isPassed ) ++_passedTests;
                    else {
                        for ( int i = 0; i < _expectedList.Count; i++ ) {
                            Console.WriteLine($"\nExpected: {_expectedList[i]}\nReceived: {_mistakesList[i]}\n");
                        }
                    }
                    

                }
                sr.Close();
                _allTests++;
            }

            Console.WriteLine($"\nTests: {_allTests}\nPassed{_passedTests}");
            Assert.AreEqual(0 , _mistakesList.Count);
        }

        private bool CheckResult(string mistake, string expected) {
            if ( mistake != expected) {
                _expectedList.Add(expected);
                _mistakesList.Add(mistake);
            }
            
            return mistake == expected;
        }
    }
}