namespace lexer.Models;

public class Lexer
{
    private readonly string _input;
    private int _position;

    public Lexer(string input)
    {
        _input = input;
        
        _position = 0;
    }
    private readonly char [] _doubleCharOperator = ['>','<', '⨝', '⋈'];
    public Token NextToken()
    {
        SkipWhitespace();

        if (_position >= _input.Length)
            return new Token(TokenType.EOF, "");
        var previous = _position > 0 ? _input[_position - 1] : '\0';
        
        char current = _input[_position];
        if (_doubleCharOperator.Contains(current) && _position + 1 < _input.Length
            && (_input[_position+1]=='=' || _input[_position+1]== 'θ'))
        {                
                var multiToken= TryReadMultiCharOperator();
                if(multiToken!= null)
                    return multiToken;
            
        }

        if(SingleCharTokens.TryGetValue(current, out var type))
        {
            _position++;
            return new Token(type,current.ToString());
        }

        if (char.IsLetter(current))
            return Classify(ScanWord());
        if(current.Equals('\''))
            return ScanString();

        if (char.IsDigit(current))
            return ScanNumber();


        throw new Exception($"Unexpected character: {current} after token {previous}");
    }
    private void SkipWhitespace()
    {
        while (_position < _input.Length &&
            char.IsWhiteSpace(_input[_position]))
            
        {
            _position++;
        }
    }
    private static Token Classify(string word) => new(TokenType.IDENTIFIER, word);


    private string ScanWord()
    {
        int start = _position;

        while (_position < _input.Length &&
            (char.IsLetterOrDigit(_input[_position]) 
            || _input[_position] == '_' 
            || _input[_position]=='.'))
        {
            _position++;
        }

        return _input[start.._position];
    }
    private Token ScanNumber()
    {
        int start = _position;

        while (_position < _input.Length &&(
            char.IsDigit(_input[_position]) || _input[_position]=='.'))
        {
            _position++;
        }

        string value = _input[start.._position];

        return new Token(TokenType.NUMBER, value);
    }

    private Token ScanString()
    {
        _position++; 
        int start = _position;

        while (_position < _input.Length &&
            _input[_position] != '\'')
        {
            _position++;
        }

        if (_position >= _input.Length)
            throw new Exception("Unterminated string literal");

        string value = _input[start.._position];

        _position++; // skip closing '

        return new Token(TokenType.STRING, value);
    }
    
    private Token? TryReadMultiCharOperator()
    {
        if (Match("<="))
            return new Token(TokenType.LTE, "<=");

        if (Match(">="))
            return new Token(TokenType.GTE, ">=");

        if (Match("⨝θ"))
            return new Token(TokenType.THETA_JOIN, "⨝θ");

        if (Match("⋈θ"))
            return new Token(TokenType.THETA_JOIN, "⋈θ");

        return null;
    }
    private bool Match(string expected)
    {
        if(_position + expected.Length > _input.Length)
            return false;
        var result = _input.Substring(_position,expected.Length) == expected;
        if(result)
        {
            _position += expected.Length;
        }
        return result;
    }
    private static readonly Dictionary<char, TokenType> SingleCharTokens = new()
    {
        ['('] = TokenType.LPAREN,
        [')'] = TokenType.RPAREN,
        [','] = TokenType.COMMA,
        ['['] = TokenType.LSB,
        [']'] = TokenType.RSB,

        ['π'] = TokenType.PROJECT,
        ['σ'] = TokenType.SELECT,
        ['⨝'] = TokenType.JOIN,
        ['⋈'] = TokenType.JOIN,
        ['ρ'] = TokenType.RENAME,
        ['∪'] = TokenType.UNION,
        ['∩'] = TokenType.INTERSECT,
        ['−'] = TokenType.DIFFERENCE,
        ['-'] = TokenType.DIFFERENCE,
        ['×'] = TokenType.CARTESIEN_PRODUCT,

        ['¬'] = TokenType.NOT,
        ['>'] = TokenType.GT,
        ['<'] = TokenType.LT,
        ['='] = TokenType.EQ,
        ['≠'] = TokenType.NEQ,
        ['∧']  = TokenType.AND,
        ['∨'] = TokenType.OR,
    };

   
}