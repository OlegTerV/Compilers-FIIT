using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SimpleLexer;

namespace SimpleLangLexerTest
{
    class Program
    {
        public static void Main()
        {
            string fileContents = @"
int N = 100;

function max(int x, int y)[int m] {
   if (x < y) {
      m = y;
   } else {
      m = x;
   }
}


function f(int x, float Z)[int r1, int r2]<int N = NN, function m = max> {
    r1 = N / x - z;
    r2 = m(x + z, x * z);
}

function main() {
   for(i = 1..10) {
        int x = 0;
        while(x > 0) {
 			x = strtoi(read());
   			int r,r1 = f(x, 10)<120>;
   			print(r, r1);  
   		}
   }
}
";
            TextReader inputReader = new StringReader(fileContents);
            Lexer l = new Lexer(inputReader);
            try
            {
                do
                {
                    Console.WriteLine(l.TokToString(l.LexKind));
                    l.NextLexem();
                } while (l.LexKind != Tok.EOF);
            }
            catch (LexerException e)
            {
                Console.WriteLine("lexer error: " + e.Message);
            }
        }
    }
}