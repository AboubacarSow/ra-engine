namespace lexer.Models;
public enum TokenType
{
    // Relational Algebra Operators
    SELECT,      
    PROJECT,     
    RENAME,   

    //Inner Operator  
    JOIN,     
    DIFFERENCE, 
    CARTESIEN_PRODUCT,
    DIVISION,

    UNION,
    INTERSECT,

    // Logical Operators
    AND,         
    OR,          
    NOT,        

    // Structure
    LPAREN, RPAREN, COMMA,LSB, RSB,

    // Comparisons
    EQ, NEQ, LT, GT, LTE, GTE,

    // Data
    IDENTIFIER,
    NUMBER,
    STRING,

    EOF,
    THETA_JOIN,
    NATURAL_JOIN
}
