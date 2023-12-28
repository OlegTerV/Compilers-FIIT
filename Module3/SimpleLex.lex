%using ScannerHelper;
%namespace SimpleScanner

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  {Digit}+
FLOATNUM {INTNUM}\.{INTNUM}
SYMBOLNUM  \'.\'
TEXTNUM \".*\"
DotChr [^\r\n]
//OneLineCmnt  \/\/{DotChr}*
NOT_ENDL [^\r\n]
ONELINE_COMMENT \/\/{NOT_ENDL}*
MULTILINE_COMMENT \/\*[^]*\*\/


ID {Alpha}{AlphaDigit}* 

// Здесь можно делать описания типов, переменных и методов - они попадают в класс Scanner
%{
  public int LexValueInt;
  public double LexValueFloat;
  public char LexValueSymbol;
  public String LexValueText;
%}

%x COMMENT

%%
{INTNUM} { 
  LexValueInt = int.Parse(yytext);
  return (int)Tok.INTNUM;
}

{FLOATNUM} { 
  LexValueFloat = float.Parse(yytext);
  return (int)Tok.FLOATNUM;
}
{SYMBOLNUM} { 
  LexValueSymbol = yytext[1];
  return (int)Tok.SYMBOLNUM;
}
{TEXTNUM} { 
	LexValueText = yytext.Substring(1, yytext.Length);
	return (int)Tok.TEXTNUM;
}
"{" { 
  return (int)Tok.BEGIN;
}

"}" { 
  return (int)Tok.END;
}

cycle { 
  return (int)Tok.CYCLE;
}

if { 
  return (int)Tok.IF;
}

else {
  return (int)Tok.ELSE;
}

while { 
  return (int)Tok.WHILE;
}

for { 
  return (int)Tok.FOR;
}

function { 
  return (int)Tok.FUNCTION;
}

":" { 
  return (int)Tok.COLON;
}

"," { 
  return (int)Tok.COMMA;
}

"=" { 
  return (int)Tok.ASSIGN;
}

";" { 
  return (int)Tok.SEMICOLON;
}

"+=" { 
  return (int)Tok.PLUSASSIGN;
}

"+" { 
  return (int)Tok.PLUS;
}

"-=" { 
  return (int)Tok.MINUSASSIGN;
}

"-" { 
  return (int)Tok.MINUS;
}

"*=" { 
  return (int)Tok.MULTASSIGN;
}

"*" { 
  return (int)Tok.MULT;
}

"/=" { 
  return (int)Tok.DIVASSIGN;
}

"/" { 
  return (int)Tok.DIVISION;
}

"==" { 
  return (int)Tok.EQ;
}

">=" { 
  return (int)Tok.GEQ;
}

">" { 
  return (int)Tok.GT;
}

"<=" { 
  return (int)Tok.LEQ;
}

"<" { 
  return (int)Tok.LT;
}

"!=" { 
  return (int)Tok.NEQ;
}

"!" { 
  return (int)Tok.NOT;
}

"." { 
  return (int)Tok.COLON;
}

"(" { 
  return (int)Tok.LEFT_BRACKET;
}

")" { 
  return (int)Tok.RIGHT_BRACKET;
}

"/%" { 
  return (int)Tok.DIV;
}

"%" { 
  return (int)Tok.MOD;
}

"&&" { 
  return (int)Tok.AND;
}

"||" { 
  return (int)Tok.OR;
}

"[" { 
  return (int)Tok.SQUARE_BRACKET_LEFT;
}

"]" { 
  return (int)Tok.SQUARE_BRACKET_RIGHT;
}

".." { 
  return (int)Tok.TO;
}

{ONELINE_COMMENT} {
}


"/*" {
  BEGIN(COMMENT);
}

<COMMENT> "*/" {   
  BEGIN(INITIAL);
}
<COMMENT>{ID} {
  return (int)Tok.ID_COMMENT;
}
		
"int" {
	return (int)Tok.INT;
}

"float" {
	return (int)Tok.FLOAT;
}

"symbol" {
	return (int)Tok.SYMBOL;
}

"text" {
	return (int)Tok.TEXT;
}


{ID}  { 
  return (int)Tok.ID;
}

[^ \r\n\t] {
	LexError();
	return 0; // конец разбора
}
%%

// Здесь можно делать описания переменных и методов - они тоже попадают в класс Scanner

public void LexError()
{
	Console.WriteLine("({0},{1}): Неизвестный символ {2}", yyline, yycol, yytext);
}

public string TokToString(Tok tok)
{
	switch (tok)
	{
		case Tok.ID:
			return tok + " " + yytext;
		case Tok.INTNUM:
			return tok + " " + LexValueInt;
		case Tok.FLOATNUM:
			return tok + " " + LexValueFloat;
		case Tok.SYMBOLNUM:
			return tok + " " + LexValueSymbol;
		case Tok.TEXTNUM:
			return tok + " " + LexValueText;
		default:
			return tok + "";
	}
}

