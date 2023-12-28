namespace ScannerHelper
{
    public enum Tok {
        EOF=0,//???
        ID,//???
        INUM,//???
        COLON,
        SEMICOLON,
        ASSIGN,
        BEGIN,
        END,
        CYCLE,
        COMMA,
        PLUS,
        MINUS,
        MULT,
        DIVISION,//???
        MOD,
        DIV,
        AND,
        OR,
        NOT,
        MULTASSIGN,
        DIVASSIGN,
        PLUSASSIGN,
        MINUSASSIGN,
        LT,  //lesser
        GT,  //greater
        LEQ, //less or equal
        GEQ, //greater or equal
        EQ,  //equal
        NEQ, //not equal
        WHILE,
        DO,
        FOR,
        IF,
        ELSE,
        LEFT_BRACKET,
        RIGHT_BRACKET,
        FUNCTION,
        SQUARE_BRACKET_LEFT,
        SQUARE_BRACKET_RIGHT,
        INT,
        FLOAT,
        SYMBOL,
        TEXT,

        INTNUM,
        FLOATNUM,
        SYMBOLNUM,
        TEXTNUM,
        
        ID_COMMENT,
        TO
    };
}