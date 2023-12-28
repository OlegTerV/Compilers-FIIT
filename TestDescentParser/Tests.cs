using System;
using NUnit.Framework;
using SimpleLexer;
using SimpleLangParser;
using System.IO;

namespace TestDescentParser
{
    [TestFixture]
    public class DescentParserTests
    {
        private bool Parse(string text)
        {
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            Parser p = new Parser(l);
            p.Progr();
            if (l.LexKind == Tok.EOF)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        [Test]
        //[Ignore("This test is disabled")]
        public void TestWhile()
        {
            Assert.IsTrue(Parse(@"function main() { while (a) { a = 2; } }"));
            
            Assert.IsTrue(Parse(@"function main() { 
                                     while(c > b) 
                                     { 
                                        a = 2;
                                        b = -3 * c + 12;
                                     }
                                  }"));
                                  
            Assert.IsTrue(Parse(@"function main() { 
                                     while (5)
                                     { 
                                       while (6)
                                       {
                                         a=2; 
                                       }
                                       while (7)
                                       {
                                         a=3;
                                         c=4;
                                       }
                                     }
                                  }"));
            
        }
        
        [Test]
        public void TestFor()
        {
            Assert.IsTrue(Parse(@"function main() {  
                                     for (a = 1..5)
                                     {
                                       b=1;
                                     }
                                  }"));
                                  
           Assert.IsTrue(Parse(@"function main() { 
                                     for (a = 1..5)
                                     { 
                                       for (i = 1..6)
                                       {
                                          c=1;
                                       }
                                       b=1;
                                     }
                                  }"));
            
        }
        
        [Test]
        //[Ignore("This test is disabled")]
        public void TestIf()
        {
            Assert.IsTrue(Parse(@"function main() {  
                                     if (2) {  
                                        a=2;
                                     } else {
                                        b=2;
                                     }
                                     if (3) {  
                                        if (c) {  
                                            c=4;
                                        } else {
                                            m=1;
                                        }
                                     } else {
                                        v=8;   
                                     }
                                     if (4) { 
                                       if (4) { 
                                         if (6) { 
                                            m=0;
                                         }
                                       }
                                     }
                                  }"));
            
        }
        
        [Test]
        //[Ignore("This test is disabled")]
        public void TestExpr()
        {
            Assert.IsTrue(Parse(@"function main() { 
                                     if (2+2*(c-d/3)) {
                                        a=2;
                                        while (2-3+f) { c=c*2; }
                                     }
                                     else {
                                        b=2-3*(c-d/f*3);
                                     }
                                     for (i = 2-3*(s-d)..(c-3)) {
                                         a=(a-(3-3));
                                     }
                                     if (3) {
                                        if (c-3) {
                                            c=4+2;
                                        } else {
                                            m=1;
                                        }
                                     } else {
                                        v=(8+2);
                                     }
                                  }"));
            
        }
    }
}