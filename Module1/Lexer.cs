using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexer
{

    public class LexerException : System.Exception
    {
        public LexerException(string msg)
            : base(msg)
        {
        }

    }

    public class Lexer
    {

        protected int position;
        protected char currentCh; // очередной считанный символ
        protected int currentCharValue; // целое значение очередного считанного символа
        protected System.IO.StringReader inputReader;
        protected string inputString;

        public Lexer(string input)
        {
            inputReader = new System.IO.StringReader(input);
            inputString = input;
        }

        public void Error()
        {
            System.Text.StringBuilder o = new System.Text.StringBuilder();
            o.Append(inputString + '\n');
            o.Append(new System.String(' ', position - 1) + "^\n");
            o.AppendFormat("Error in symbol {0}", currentCh);
            throw new LexerException(o.ToString());
        }

        protected void NextCh()
        {
            this.currentCharValue = this.inputReader.Read();
            this.currentCh = (char) currentCharValue;
            this.position += 1;
        }

        public virtual bool Parse()
        {
            return true;
        }
    }

    public class IntLexer : Lexer
    {

        protected System.Text.StringBuilder intString;
        public int parseResult = 0;

        public IntLexer(string input)
            : base(input)
        {
            intString = new System.Text.StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '+' || currentCh == '-')
            {
                intString.Append(currentCh);
                NextCh();
            }
        
            if (char.IsDigit(currentCh))
            {
                intString.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (char.IsDigit(currentCh))
            {
                intString.Append(currentCh);
                NextCh();
            }


            if (currentCharValue != -1)
            {
                Error();
            }

            parseResult = Int32.Parse(intString.ToString());

            return true;

        }
    }
    
    public class IdentLexer : Lexer
    {
        private string parseResult;
        protected StringBuilder builder;
    
        public string ParseResult
        {
            get { return parseResult; }
        }
    
        public IdentLexer(string input) : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {

            NextCh();
            if (char.IsLetter(currentCh) || char.IsDigit(currentCh) )
            {
                builder.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (char.IsLetter(currentCh) || char.IsDigit(currentCh) )
            {
                builder.Append(currentCh);
                NextCh();
            }

            if (currentCh == '_')
            {
                builder.Append(currentCh);
                NextCh();
            }

            while (char.IsLetter(currentCh) || char.IsDigit(currentCh))
            {
                builder.Append(currentCh);
                NextCh();
            }

            if (currentCharValue != -1)//признак конца строки?
            {
                Error();
            }

            parseResult =builder.ToString();
            return true;

        }
       
    }

    public class IntNoZeroLexer : IntLexer
    {
        private string parseResult;
        protected StringBuilder builder;
        public IntNoZeroLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()//почему с 0 не работает?
        {
            NextCh();
            if (currentCh == '+' || currentCh == '-')
            {
                builder.Append(currentCh);
                NextCh();
            }

            if (char.IsDigit(currentCh) && (currentCh != '0'))
            {
                builder.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (char.IsDigit(currentCh))
            {
                builder.Append(currentCh);
                NextCh();
            }

            if (currentCharValue != -1)
            {
                Error();
            }

            parseResult = builder.ToString();
            return true;
        }
    }

    public class LetterDigitLexer : Lexer
    {
        protected StringBuilder builder;
        protected string parseResult;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public LetterDigitLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            int flag = 0;
            NextCh();
            while (flag == 0)
            {
                if (char.IsLetter(currentCh))
                {
                    builder.Append(currentCh);
                    NextCh();
                }
                else
                {
                    flag = 1;
                }
                if (char.IsDigit(currentCh))
                {
                    builder.Append(currentCh);
                    NextCh();
                }
                else
                {
                    flag = 1;
                }
            }           

            if (currentCharValue != -1)
            {
                Error();
            }

            parseResult = builder.ToString();
            return true;
        }
       
    }

    public class LetterListLexer : Lexer
    {
        protected List<char> parseResult;

        public List<char> ParseResult
        {
            get { return parseResult; }
        }

        public LetterListLexer(string input)
            : base(input)
        {
            parseResult = new List<char>();
        }

        public override bool Parse()
        {
            int flag = 0;
            char a = ' ';
            NextCh();
            while (flag == 0)
            {
                if (char.IsLetter(currentCh))
                {
                    ParseResult.Add(currentCh);
                    a = currentCh;
                    NextCh();
                }
                else
                {
                    flag = 1;
                }

                if (currentCh == ',' || currentCh == ';')
                {
                    a=currentCh;                   
                    NextCh();                  
                }
            }

            if (a== ',' || a == ';')
            {
                Error();
            }

            if (currentCharValue != -1)
                 {
                     Error();
                 }
            return true;
        }
    }

    public class DigitListLexer : Lexer
    {
        protected List<int> parseResult;

        public List<int> ParseResult
        {
            get { return parseResult; }
        }

        public DigitListLexer(string input)
            : base(input)
        {
            parseResult = new List<int>();
        }

        public override bool Parse()
        {
            
            NextCh();
            if (char.IsDigit(currentCh))
            {
                ParseResult.Add(Int32.Parse(currentCh.ToString()));
                NextCh();
            }
            else
            {
                Error();
            }

            int flag = 0;
            if (currentCharValue == -1)
            {
                flag = 1;
            }

            while (flag==0)
            {
                if (currentCh ==' ')
                {
                    while (currentCh == ' ')
                    {
                        NextCh();
                    }

                }
                else
                {
                    Error();
                    break;
                }
                if (char.IsDigit(currentCh))
                {
                    ParseResult.Add(Int32.Parse(currentCh.ToString()));
                    NextCh();
                    if (currentCharValue == -1)
                    {
                        break;
                    }
                }
            }          

            if (currentCharValue != -1)
            {
                Error();
            }

            return true;
        }
    }

    public class LetterDigitGroupLexer : Lexer
    {
        protected StringBuilder builder;
        protected string parseResult;

        public string ParseResult
        {
            get { return parseResult; }
        }
        
        public LetterDigitGroupLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            int count = 0;
            NextCh();
            while (true)
            {
                if (char.IsLetter(currentCh))
                {
                    while (char.IsLetter(currentCh))
                    {
                        count++;
                        builder.Append(currentCh);
                        NextCh();
                    }
                    if (currentCharValue == -1)
                    {
                        break;
                    }
                }
                else
                {
                    Error();
                    break;
                }
                if (count > 2)
                {
                    Error();
                    break;
                }
                count = 0;

                if (char.IsDigit(currentCh))
                {
                    while (char.IsDigit(currentCh))
                    {
                        count++;
                        builder.Append(currentCh);
                        NextCh();
                    }
                    if (currentCharValue == -1)
                    {
                        break;
                    }
                }
                else
                {
                    Error();
                    break;
                }
                if (count > 2)
                {
                    Error();
                    break;
                }
                count = 0;

            }


            if (currentCharValue != -1)
            {
                Error();
            }

            parseResult = builder.ToString();
            return true;
        }
       
    }

    public class DoubleLexer : Lexer
    {
        private StringBuilder builder;
        private double parseResult;

        public double ParseResult
        {
            get { return parseResult; }

        }

        public DoubleLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();


            if (char.IsDigit(currentCh))
            {
                while (char.IsDigit(currentCh)) {
                    builder.Append(currentCh);
                    NextCh();
                }
            }
            else
            {
                Error();
            }

            if (currentCh =='.')
            {
                builder.Append(currentCh);
                NextCh();
                if (currentCharValue == -1)
                {
                    Error();
                }
            }


            if (char.IsDigit(currentCh))
            {
                while (char.IsDigit(currentCh))
                {
                    builder.Append(currentCh);
                    NextCh();
                }
            }

            if (currentCharValue != -1)
            {
                Error();
            }

           // parseResult = Convert.ToDouble(builder);
           parseResult = Convert.ToDouble(builder.ToString());
            return true;
        }
       
    }

    public class StringLexer : Lexer
    {
        private StringBuilder builder;
        private string parseResult;

        public string ParseResult
        {
            get { return parseResult; }

        }

        public StringLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '\'')
            {
                builder.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            if (currentCh != '\'')
            {
                while ((currentCh != '\'')&&(currentCharValue != -1))
                {
                    builder.Append(currentCh);
                    NextCh();
                }
            }

            if (currentCh == '\'')
            {
                builder.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }


            if (currentCharValue != -1)
            {
                Error();
            }

            parseResult = builder.ToString();
            return true;
        }
    }

    public class CommentLexer : Lexer
    {
        private StringBuilder builder;
        private string parseResult;

        public string ParseResult
        {
            get { return parseResult; }

        }

        public CommentLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            
            NextCh();
            if (currentCh == '/' )
            {
                builder.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            if (currentCh == '*')
            {
                builder.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }
            while (true)
            {
                if (currentCh == '*')
                {
                    builder.Append(currentCh);
                    NextCh();
                    if (currentCh == '/')
                    {
                        builder.Append(currentCh);
                        NextCh();
                        break;
                    }
                    else
                    {
                        builder.Append(currentCh);
                        NextCh();
                        if (currentCharValue == -1)
                        {
                            Error();
                            break;
                        }
                    }
                }
                else
                {
                    builder.Append(currentCh);
                    NextCh();
                    if (currentCharValue == -1)
                    {
                        Error();
                        break;
                    }
                }
            }

            if (currentCharValue != -1)
            {
                Error();
            }

            parseResult = builder.ToString();
            return true;
        }
    }

    public class IdentChainLexer : Lexer
    {
        private StringBuilder builder;
        private List<string> parseResult;

        public List<string> ParseResult
        {
            get { return parseResult; }

        }

        public IdentChainLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
            parseResult = new List<string>();
        }

        public override bool Parse()
        {
            NextCh();
            while (currentCharValue != -1) {
                if (char.IsLetter(currentCh))
                {
                    while ((char.IsLetter(currentCh)) && (currentCharValue != -1))
                    {
                        ParseResult.Add(currentCh.ToString());
                        NextCh();
                    }
                }
                else
                {
                    Error();
                }

                if (currentCh != '.') {
                    if (currentCh == '_')
                    {
                        ParseResult.Add(currentCh.ToString());
                        NextCh();
                    }
                    if (char.IsDigit(currentCh))
                    {
                        while ((char.IsDigit(currentCh)) && (currentCharValue != -1))
                        {
                            ParseResult.Add(currentCh.ToString());
                            NextCh();
                        }
                    }
                    else
                    {
                        Error();
                    }
                }

                if ((currentCh == '.'))
                {
                    ParseResult.Add(currentCh.ToString());
                    NextCh();
                    if (currentCharValue == -1)
                    {
                        Error();
                    }
                    if (currentCh == '.')
                    {
                        Error();
                    }
                }
            }




            if (currentCharValue != -1)
            {
                Error();
            }


            
            return true;
        }
    }

    public class Program
    {
        public static void Main()
        {
            string input = "154216";
            Lexer L = new IntLexer(input);
            try
            {
                L.Parse();
            }
            catch (LexerException e)
            {
                System.Console.WriteLine(e.Message);
            }

        }
    }
}