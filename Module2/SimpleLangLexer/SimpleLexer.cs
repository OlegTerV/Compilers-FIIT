﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SimpleLexer
{

    public class LexerException : System.Exception
    {
        public LexerException(string msg)
            : base(msg)
        {
        }

    }

    public enum Tok
    {
        EOF,
        ID,
        COLON,
        SEMICOLON,
        ASSIGN,
        COMMA,
        RANGE,

        PLUS,
        MINUS,
        MULT,
        DIVISION,
        MOD,
        DIV,

        MULTASSIGN,
        DIVISIONASSIGN,
        PLUSASSIGN,
        MINUSASSIGN,
        DIVASSIGN,
        MODASSIGN,

        AND,
        OR,
        NOT,

        LT,  //lesser
        GT,  //greater
        LEQ, //less or equal
        GEQ, //greater or equal
        EQ,  //equal
        NEQ, //not equal

        WHILE,
        FOR,
        IF,
        ELSE,
        //CYCLE,
        //DO,
        //TO,
        //THEN,
        BEGIN,
        END,
        FUNCTION,

        LEFT_BRACKET,
        RIGHT_BRACKET,
        LEFT_SQUARE_BRACKET,
        RIGHT_SQUARE_BRACKET,

        INT,
        FLOAT,
        SYMBOL,
        TEXT,

        INT_VAL,
        FLOAT_VAL,
        SYMBOL_VAL,
        TEXT_VAL
    }

     public class Lexer
    {
        private int position;
        private char currentCh;                      // Текущий символ
        public int LexRow, LexCol;                  // Строка-столбец начала лексемы. Конец лексемы = LexCol+LexText.Length
        private int row, col;                        // текущие строка и столбец в файле
        private TextReader inputReader;
        private Dictionary<string, Tok> keywordsMap; // Словарь, сопоставляющий ключевым словам константы типа TLex. Инициализируется процедурой InitKeywords 
        public Tok LexKind;                         // Тип лексемы
        public string LexText;                      // Текст лексемы
        public int lexIntValue;
        public double lexFloatValue;
        public char lexSymbolValue;
        public string lexTextValue;

        private string CurrentLineText;  // Накапливает символы текущей строки для сообщений об ошибках
        

        public Lexer(TextReader input)
        {
            CurrentLineText = "";
            inputReader = input;
            keywordsMap = new Dictionary<string, Tok>();
            InitKeywords();
            row = 1; col = 0;
            NextCh();       // Считать первый символ в ch
            NextLexem();    // Считать первую лексему, заполнив LexText, LexKind и, возможно, LexValue
        }

        public void Init() {

        }

        private void PassSpaces()
        {
            while (char.IsWhiteSpace(currentCh))
            {
                NextCh();
            }
        }

        private void InitKeywords()
        {
            keywordsMap["function"] = Tok.FUNCTION;

            keywordsMap["int"] = Tok.INT;
            keywordsMap["float"] = Tok.FLOAT;
            keywordsMap["symbol"] = Tok.SYMBOL;
            keywordsMap["text"] = Tok.TEXT;

            keywordsMap["and"] = Tok.AND;
            keywordsMap["or"] = Tok.OR;
            keywordsMap["not"] = Tok.NOT;

            keywordsMap["if"] = Tok.IF;
            keywordsMap["else"] = Tok.ELSE;
            keywordsMap["while"] = Tok.WHILE;
            keywordsMap["for"] = Tok.FOR;
        }

        public string FinishCurrentLine()
        {
            return CurrentLineText + inputReader.ReadLine();
        }

        private void LexError(string message)
        {
            System.Text.StringBuilder errorDescription = new System.Text.StringBuilder();
            errorDescription.AppendFormat("Lexical error in line {0}:", row);
            errorDescription.Append("\n");
            errorDescription.Append(FinishCurrentLine());
            errorDescription.Append("\n");
            errorDescription.Append(new String(' ', col - 1) + '^');
            errorDescription.Append('\n');
            if (message != "")
            {
                errorDescription.Append(message);
            }
            throw new LexerException(errorDescription.ToString());
        }

        private void NextCh()
        {
            // В LexText накапливается предыдущий символ и считывается следующий символ
            LexText += currentCh;
            var nextChar = inputReader.Read();
            if (nextChar != -1)
            {
                currentCh = (char)nextChar;
                if (currentCh != '\n')
                {
                    col += 1;
                    CurrentLineText += currentCh;
                }
                else
                {
                    row += 1;
                    col = 0;
                    CurrentLineText = "";
                }
            }
            else
            {
                if (currentCh == 0)
                {
                    LexError("Unexpected end of file");
                }
                currentCh = (char)0; // если достигнут конец файла, то возвращается #0
            }
        }

        public void NextLexem()
        {
            PassSpaces();
            // R К этому моменту первый символ лексемы считан в ch
            LexText = "";
            LexRow = row;
            LexCol = col;
            // Тип лексемы определяется по ее первому символу
            // Для каждой лексемы строится синтаксическая диаграмма
            if (currentCh == '/')
            {
                NextCh();
                if (currentCh == '=')
                {
                    NextCh();
                    LexKind = Tok.DIVISIONASSIGN;
                    return;
                }
                else if (currentCh == '~')
                {
                    NextCh();
                    if (currentCh == '=')
                    {
                        NextCh();
                        LexKind = Tok.DIVASSIGN;
                        return;
                    }
                    else
                    {
                        LexKind = Tok.DIV;
                        return;
                    }
                }
                else if (currentCh == '/')
                {
                    FinishCurrentLine();
                    NextCh();
                }
                else if (currentCh == '*')
                {
                    NextCh();
                    while (true)
                    {
                        while (currentCh != '*')
                        {
                            NextCh();
                        }
                        NextCh();
                        if (currentCh == '/')
                        {
                            break;
                        }
                    }
                    NextCh();
                }
                else
                {
                    LexKind = Tok.DIVISION;
                    return;
                }
            }

            PassSpaces();
            LexText = "";
            LexRow = row;
            LexCol = col;

            if (currentCh == ';')
            {
                NextCh();
                LexKind = Tok.SEMICOLON;
            }
            else if (currentCh == ':')
            {
                NextCh();
                LexKind = Tok.COLON;
            }
            else if (currentCh == '=')
            {
                NextCh();
                if (currentCh == '=')
                {
                    NextCh();
                    LexKind = Tok.EQ;
                }
                else
                {
                    LexKind = Tok.ASSIGN;
                }

            }
            else if (currentCh == ',')
            {
                NextCh();
                LexKind = Tok.COMMA;
            }
            else if (currentCh == '.')
            {
                NextCh();
                if (currentCh != '.')
                {
                    LexError(". was expected");
                }
                NextCh();
                LexKind = Tok.RANGE;
            }
            else if (currentCh == '+')
            {
                NextCh();
                if (currentCh == '=')
                {
                    NextCh();
                    LexKind = Tok.PLUSASSIGN;
                }
                else
                {
                    LexKind = Tok.PLUS;
                }
            }
            else if (currentCh == '-')
            {
                NextCh();
                if (currentCh == '=')
                {
                    NextCh();
                    LexKind = Tok.MINUSASSIGN;
                }
                else
                {
                    LexKind = Tok.MINUS;
                }
            }
            else if (currentCh == '*')
            {
                NextCh();
                if (currentCh == '=')
                {
                    NextCh();
                    LexKind = Tok.MULTASSIGN;
                }
                else
                {
                    LexKind = Tok.MULT;
                }
            }
            else if (currentCh == '%')
            {
                NextCh();
                if (currentCh == '=')
                {
                    NextCh();
                    LexKind = Tok.MODASSIGN;
                }
                else
                {
                    LexKind = Tok.MOD;
                }
            }
            else if (currentCh == '<')
            {
                NextCh();
                if (currentCh == '=')
                {
                    NextCh();
                    LexKind = Tok.LEQ;
                }
                else
                {
                    LexKind = Tok.LT;
                }
            }
            else if (currentCh == '>')
            {
                NextCh();
                if (currentCh == '=')
                {
                    NextCh();
                    LexKind = Tok.GEQ;
                }
                else
                {
                    LexKind = Tok.GT;
                }
            }
            else if (currentCh == '!')
            {
                NextCh();
                if (currentCh != '=')
                {
                    LexError("= was expected");
                }
                NextCh();
                LexKind = Tok.NEQ;
            }
            else if (currentCh == '(')
            {
                NextCh();
                LexKind = Tok.LEFT_BRACKET;
            }
            else if (currentCh == ')')
            {
                NextCh();
                LexKind = Tok.RIGHT_BRACKET;
            }
            else if (currentCh == '[')
            {
                NextCh();
                LexKind = Tok.LEFT_SQUARE_BRACKET;
            }
            else if (currentCh == ']')
            {
                NextCh();
                LexKind = Tok.RIGHT_SQUARE_BRACKET;
            }
            else if (currentCh == '{')
            {
                NextCh();
                LexKind = Tok.BEGIN;
            }
            else if (currentCh == '}')
            {
                NextCh();
                LexKind = Tok.END;
            }
            else if (char.IsLetter(currentCh))
            {
                while (char.IsLetterOrDigit(currentCh))
                {
                    NextCh();
                }
                if (keywordsMap.ContainsKey(LexText))
                {
                    LexKind = keywordsMap[LexText];
                }
                else
                {
                    LexKind = Tok.ID;
                }
            }
            else if (char.IsDigit(currentCh))
            {
                while (char.IsDigit(currentCh))
                {
                    NextCh();
                }
                if (currentCh == '.')
                {
                    var nextChar = inputReader.Peek();
                    if (char.IsDigit((char)nextChar))
                    {
                        NextCh();
                        while (char.IsDigit(currentCh))
                        {
                            NextCh();
                        }
                        lexFloatValue = Double.Parse(LexText, CultureInfo.InvariantCulture);
                        LexKind = Tok.FLOAT_VAL;
                    } else
                    {
                        lexIntValue = Int32.Parse(LexText);
                        LexKind = Tok.INT_VAL;
                    }
                } else
                {
                    lexIntValue = Int32.Parse(LexText);
                    LexKind = Tok.INT_VAL;
                }
            }
            else if (currentCh == '\'') 
            {
                NextCh();
                NextCh();
                if (currentCh != '\'')
                {
                    LexError("' was expeced");
                }
                NextCh();
                lexSymbolValue = LexText[1];
                LexKind = Tok.SYMBOL_VAL;
            }
            else if (currentCh == '"')
            {
                NextCh();
                while(currentCh != '"')
                {
                    NextCh();
                }
                NextCh();
                lexTextValue = LexText.Substring(1, LexText.Length - 2);
                LexKind = Tok.TEXT_VAL;
            }
            else if ((int)currentCh == 0)
            {
                LexKind = Tok.EOF;
            }
            else
            {
                LexError("Incorrect symbol " + currentCh);
            }
        }

        public virtual void ParseToConsole()
        {
            do
            {
                Console.WriteLine(TokToString(LexKind));
                NextLexem();
            } while (LexKind != Tok.EOF);
        }

        public string TokToString(Tok t)
        {
            var result = t.ToString();
            switch (t)
            {
                case Tok.INT_VAL: result += ' ' + lexIntValue.ToString();
                    break;
                case Tok.FLOAT_VAL: result += ' ' + lexFloatValue.ToString();
                    break;
                case Tok.SYMBOL_VAL: result += ' ' + lexSymbolValue.ToString();
                    break;
                case Tok.TEXT_VAL: result += ' ' + lexTextValue;
                    break;
                default: result += ' ' + LexText;
                    break;
            }
            return result;
        }
    }
}