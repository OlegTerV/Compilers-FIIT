﻿using System;
using System.Collections.Generic;
using System.Text;
using SimpleLexer;
namespace SimpleLangParser
{
    public class ParserException : System.Exception
    {
        public ParserException(string msg)
            : base(msg)
        {
        }

    }

    public class Parser
    {
        private SimpleLexer.Lexer l;

        public Parser(SimpleLexer.Lexer lexer)
        {
            l = lexer;
        }

        private void CheckTokNext(Tok trueTok)
        {
            l.NextLexem();
            if (l.LexKind != trueTok)
            {
                SyntaxError("False tok " + trueTok.ToString());
            }
        }

        private void CheckTok(Tok trueTok)
        {
            if (l.LexKind != trueTok)
            {
                SyntaxError("False tok " + trueTok.ToString());
            }
        }

        public void Progr()
        {
            StFList();
        }

        private void StFList()
        {
            while (l.LexKind != Tok.EOF)
            {
                if (l.LexKind == Tok.FUNCTION)
                {
                    FuncDecl();
                } else
                {
                    VarDecl();
                    CheckTok(Tok.SEMICOLON);
                }
                l.NextLexem();
            }
        }

        private void FuncDecl()
        {
            CheckTokNext(Tok.ID);
            CheckTokNext(Tok.LEFT_BRACKET);
            ParamsList();
            CheckTok(Tok.RIGHT_BRACKET);
            l.NextLexem();
            RetParams();
            TemplateParams();
            Block();
        }

        bool IsType(Tok tok)
        {
            return tok == Tok.INT || tok == Tok.FLOAT || tok == Tok.SYMBOL || tok == Tok.TEXT;
        }

        private void ParamsList()
        {
            l.NextLexem();
            while (IsType(l.LexKind) || l.LexKind == Tok.FUNCTION)
            {
                CheckTokNext(Tok.ID);
                l.NextLexem();
                if (l.LexKind == Tok.COMMA)
                {
                    l.NextLexem();
                } else
                {
                    return;
                }
            }
        }

        private void RetParams()
        {
            if (l.LexKind == Tok.LEFT_SQUARE_BRACKET)
            {
                ParamsList();
                CheckTok(Tok.RIGHT_SQUARE_BRACKET);
                l.NextLexem();
            }
        }

        private void TemplateParams()
        {
            if (l.LexKind == Tok.LT)
            {
                TemplateParamsList();
                CheckTok(Tok.GT);
                l.NextLexem();
            }
        }

        private void TemplateParamsList()
        {
            l.NextLexem();
            while (IsType(l.LexKind) || l.LexKind == Tok.FUNCTION)
            {
                CheckTokNext(Tok.ID);
                CheckTokNext(Tok.ASSIGN);
                l.NextLexem();
                CompTerm();
                if (l.LexKind == Tok.COMMA)
                {
                    l.NextLexem();
                }
                else
                {
                    return;
                }
            }
        }

        private void Block()
        {
            CheckTok(Tok.BEGIN);
            StList();
            CheckTok(Tok.END);
        }

        private void StList()
        {
            l.NextLexem();
            while (l.LexKind != Tok.END)
            {
                Statement();
            }
        }

        private void Statement()
        {
           if (IsType(l.LexKind))
            {
                VarDecl();
                CheckTok(Tok.SEMICOLON);
                l.NextLexem();
            } else if (l.LexKind == Tok.ID)
            {
                l.NextLexem();
                if (l.LexKind == Tok.LEFT_BRACKET)
                {
                    FuncCall();
                } else if (l.LexKind == Tok.ASSIGN)
                {
                    AssignSt();
                } else
                {
                    SyntaxError("Function call or assign statement is expected");
                }
                CheckTok(Tok.SEMICOLON);
                l.NextLexem();
            } else if (l.LexKind == Tok.IF)
            {
                IfSt();
            } else if (l.LexKind == Tok.WHILE)
            {
                WhileSt();
            } else if (l.LexKind == Tok.FOR)
            {
                ForSt();
            } else
            {
                SyntaxError("Statement is expected");
            }
        }

        private void VarDecl()
        {
            DeclList();
        }

        private void DeclList()
        {
            while (true)
            {
                CheckTokNext(Tok.ID);
                l.NextLexem();
                if (l.LexKind == Tok.ASSIGN)
                {
                    AssignSt();
                    if (l.LexKind != Tok.COMMA)
                    {
                        return;
                    }
                } else if (l.LexKind != Tok.COMMA)
                {
                    return;
                }
            }
        }

        private void FuncCall()
        {
            CheckTok(Tok.LEFT_BRACKET);
            ExprList();
            CheckTok(Tok.RIGHT_BRACKET);
            TemplArgs();
        }

        private void TemplArgs()
        {
            l.NextLexem();
            if (l.LexKind == Tok.LT)
            {
                l.NextLexem();
                while(true)
                {
                    CompTerm();
                    if (l.LexKind != Tok.COMMA)
                    {
                        break;
                    }
                }
                CheckTok(Tok.GT);
                l.NextLexem();
            }
        }

        private bool IsExprStart(Tok tok)
        {
            return tok != Tok.RIGHT_BRACKET && tok != Tok.GT;
        }

        private void ExprList()
        {
            l.NextLexem();
            while (IsExprStart(l.LexKind))
            {
                Expr();
                if (l.LexKind == Tok.COMMA)
                {
                    l.NextLexem();
                }
                else
                {
                    return;
                }
            }
        }

        private bool IsCompOp(Tok tok)
        {
            return tok == Tok.LT || tok == Tok.GT || tok == Tok.LEQ || tok == Tok.GEQ || tok == Tok.EQ || tok == Tok.NEQ;
        }

        private bool IsAddOp(Tok tok)
        {
            return tok == Tok.PLUS || tok == Tok.MINUS;
        }

        private bool IsLogOp(Tok tok)
        {
            return tok == Tok.AND || tok == Tok.OR;
        }

        private bool IsMultOp(Tok tok)
        {
            return tok == Tok.DIV || tok == Tok.MULT || tok == Tok.DIVISION || tok == Tok.MOD;
        }

        private void Expr()
        {
            LogTerm();
            LogOpTerm();
        }

        private void LogOpTerm()
        {
            if (IsLogOp(l.LexKind))
            {
                l.NextLexem();
                Expr();
            }
        }

        private void LogTerm()
        {
            CompTerm();
            CompOpTerm();
        }

        private void CompOpTerm()
        {
            if (IsCompOp(l.LexKind))
            {
                l.NextLexem();
                Expr();
            }
        }

        private void CompTerm()
        {
            Term();
            OpTerm();
        }

        private void OpTerm()
        {
            if (IsAddOp(l.LexKind))
            {
                l.NextLexem();
                Expr();
            }
        }

        private void Term()
        {
            Factor();
            OpFactor();
        }

        private void Factor()
        {
            if (l.LexKind == Tok.ID)
            {
                l.NextLexem();
                if (l.LexKind == Tok.LEFT_BRACKET)
                {
                    FuncCall();
                }
            } 
            else if (l.LexKind == Tok.INT_VAL)
            {
                l.NextLexem();
            } 
            else if (l.LexKind == Tok.FLOAT_VAL)
            {
                l.NextLexem();
            }
            else if (l.LexKind == Tok.SYMBOL_VAL)
            {
                l.NextLexem();
            }
            else if (l.LexKind == Tok.TEXT_VAL)
            {
                l.NextLexem();
            }
            else if (l.LexKind == Tok.LEFT_BRACKET)
            {
                l.NextLexem();
                Expr();
                CheckTok(Tok.RIGHT_BRACKET);
                l.NextLexem();
            } else if (l.LexKind == Tok.MINUS)
            {
                l.NextLexem();
                Factor();
            }
            else if (l.LexKind == Tok.NOT)
            {
                l.NextLexem();
                Factor();
            }
        }

        private void OpFactor()
        {
            if (IsMultOp(l.LexKind))
            {
                l.NextLexem();
                Term();
            }
        }

        private void AssignSt()
        {
            CheckTok(Tok.ASSIGN);
            l.NextLexem();
            Expr();
        }

        private void IfSt()
        {
            CheckTok(Tok.IF);
            CheckTokNext(Tok.LEFT_BRACKET);
            l.NextLexem();
            Expr();
            CheckTok(Tok.RIGHT_BRACKET);
            l.NextLexem();
            Block();
            l.NextLexem();
            if (l.LexKind == Tok.ELSE)
            {
                l.NextLexem();
                Block();
                l.NextLexem();
            }
        }

        private void WhileSt()
        {
            CheckTok(Tok.WHILE);
            CheckTokNext(Tok.LEFT_BRACKET);
            l.NextLexem();
            Expr();
            CheckTok(Tok.RIGHT_BRACKET);
            l.NextLexem();
            Block();
            l.NextLexem();
        }

        private void ForSt()
        {
            CheckTok(Tok.FOR);
            CheckTokNext(Tok.LEFT_BRACKET);
            CheckTokNext(Tok.ID);
            CheckTokNext(Tok.ASSIGN);
            l.NextLexem();
            CompTerm();
            CheckTok(Tok.RANGE);
            l.NextLexem();
            CompTerm();
            CheckTok(Tok.RIGHT_BRACKET);
            l.NextLexem();
            Block();
            l.NextLexem();
        }
        public void SyntaxError(string message) 
        {
            var errorMessage = "Syntax error in line " + l.LexRow.ToString() + ":\n";
            errorMessage += l.FinishCurrentLine() + "\n";
            errorMessage += new String(' ', l.LexCol - 1) + "^\n";
            if (message != "")
            {
                errorMessage += message;
            }
            throw new ParserException(errorMessage);
        }
   
    }
}
