using core.Nodes;
using lexer.Models;


namespace parser.Models;

public class Parser
{
    private readonly Lexer _lexer;
    private Token _current;
    public Parser(Lexer lexer)
    {
        _lexer = lexer;
        _current = _lexer.NextToken();
    }

    private void Eat(TokenType type)
    {
        if (_current.Type == type)
            _current = _lexer.NextToken();
        else
            throw new Exception($"Expected {type}, got {_current.Type}");
    }
    public ExpressionNode Parse()
    {
        var expr = ParseBinaryExpression();
        return expr;
    }
    private bool IsBinaryExpression(){
        return _current.Type switch
        {
            TokenType.JOIN => true,
            TokenType.DIFFERENCE =>true,
            TokenType.UNION => true,
            TokenType.INTERSECT => true,
            TokenType.THETA_JOIN => true,
            TokenType.CARTESIEN_PRODUCT => true,
            _ => false
        };
    }

    private ExpressionNode ParseBinaryExpression()
    {
        var left = ParseExpression();

        while(IsBinaryExpression())
        {
            TokenType op = _current.Type;
            Eat(op);
            var condition = null as ConditionNode;
            if(_current.Type == TokenType.LSB)
            {
                Eat(TokenType.LSB);
                condition = ParseCondition();
                Eat(TokenType.RSB);
            }
            var right = ParseExpression();

            left = BuildBinaryNode(op,left,right,condition!);
        }

        return left;
    } 
    private ExpressionNode BuildBinaryNode(TokenType type,
    ExpressionNode left, ExpressionNode right, ConditionNode condition = null!)
    {
        return type switch
        {
            TokenType.JOIN => new JoinNode(left,right),
            TokenType.CARTESIEN_PRODUCT => new CartesienProductNode(left, right),
            TokenType.NATURAL_JOIN => new NaturalJoinNode(((RelationNode)left).Name,((RelationNode)right).Name),
            TokenType.THETA_JOIN => new ThetaJoinNode(left,right,condition),
            TokenType.DIFFERENCE => new DifferenceNode(left,right),
            TokenType.UNION => new UnionNode(left,right),
            TokenType.INTERSECT => new IntersectionNode(left,right),
            _ => throw new Exception($"Unexpected token type: {type.ToString()}")
        };
    }
    private ExpressionNode ParseExpression()
    {
        return _current.Type switch
        {
            TokenType.PROJECT => ParseProjection(),
            TokenType.SELECT => ParseSelection(),
            TokenType.RENAME => ParseRename(),
            TokenType.LPAREN => ParseParenthesized(),
            TokenType.IDENTIFIER => ParseRelation(),
            _ => throw new Exception($"Unexpected token: {_current.Type}")
        };
    }

    private ExpressionNode ParseRename()
    {
        Eat(TokenType.RENAME);

        var alias = _current.Value;

        Eat(TokenType.IDENTIFIER);

        Eat(TokenType.LPAREN);
        var source = ParseBinaryExpression();
        Eat(TokenType.RPAREN);

        return new RenameNode(alias,source);
    }

    private ExpressionNode ParseProjection()
    {
        Eat(TokenType.PROJECT);

        Eat(TokenType.LSB);
        var attributes = ParseAttributeList();
        Eat(TokenType.RSB);

        Eat(TokenType.LPAREN);
        var source = ParseBinaryExpression();
        Eat(TokenType.RPAREN);

        return new ProjectionNode(attributes, source);
    }

    private List<string> ParseAttributeList()
    {
        var list = new List<string>
        {
            _current.Value
        };
        Eat(TokenType.IDENTIFIER);

        while (_current.Type == TokenType.COMMA)
        {
            Eat(TokenType.COMMA);
            list.Add(_current.Value);
            Eat(TokenType.IDENTIFIER);
        }

        return list;
    }

    private ExpressionNode ParseRelation()
    {
        string name = _current.Value;
        Eat(TokenType.IDENTIFIER);

        return new RelationNode(name);
    }

    private ExpressionNode ParseParenthesized()
    {
        Eat(TokenType.LPAREN);
        var expr = ParseExpression();
        Eat(TokenType.RPAREN);

        return expr;
    }

    private ExpressionNode ParseSelection()
    {
        Eat(TokenType.SELECT);
        Eat(TokenType.LSB);

        var condition = ParseCondition();
        Eat(TokenType.RSB);

        Eat(TokenType.LPAREN);
        var source = ParseBinaryExpression();
        Eat(TokenType.RPAREN);

        return new SelectionNode(condition, source);
    }

    private ConditionNode ParsePrimaryCondition()
    {
        if (_current.Type == TokenType.LPAREN)
        {
            Eat(TokenType.LPAREN);
            var condition = ParseCondition();
            Eat(TokenType.RPAREN);

            return condition;
        }
        return ParseComparison();
    }

    private ConditionNode ParseComparison()
    {
        string left = _current.Value;
        Eat(TokenType.IDENTIFIER);

        TokenType opType = _current.Type;

        if (opType != TokenType.EQ &&
            opType != TokenType.NEQ &&
            opType != TokenType.LT &&
            opType != TokenType.GT &&
            opType != TokenType.LTE &&
            opType != TokenType.GTE)
        {
            throw new Exception($"Invalid comparator: {_current.Type}");
        }

        string op = _current.Value;
        Eat(opType);

        string right = _current.Value;
     
        if (_current.Type == TokenType.IDENTIFIER)
            Eat(TokenType.IDENTIFIER);
        else if(_current.Type == TokenType.STRING)
            Eat(TokenType.STRING);
        
        else if (_current.Type == TokenType.NUMBER)
            Eat(TokenType.NUMBER);
        else
            throw new Exception("Expected literal");

        return new ComparisonNode(left, op, right);
    }

    private ConditionNode ParseCondition()
    {
        return ParseDisjunction();
    }
    private ConditionNode ParseDisjunction()
    {
        var left = ParseConjunction();

        while (_current.Type == TokenType.OR || _current.Type == TokenType.AND)
        {
            if(_current.Type == TokenType.OR)
            {
                Eat(TokenType.OR);
                left = new OrNode(left,ParseConjunction());
            }
            else
            {
                Eat(TokenType.AND);
                left = new OrNode(left, ParseConjunction());
            }
        }

        return left;
    }

    private ConditionNode ParseConjunction()
    {
        var left = ParseNegation();

        while (_current.Type == TokenType.AND)
        {
            Eat(TokenType.AND);
            var right = ParseNegation();

            left = new AndNode(left, right);
        }

        return left;
    }
    private ConditionNode ParseNegation()
    {
        if (_current.Type == TokenType.NOT)
        {
            Eat(TokenType.NOT);
            var inner = ParseCondition();

            return new NotNode(inner);
        }

        return ParsePrimaryCondition();
    }


}