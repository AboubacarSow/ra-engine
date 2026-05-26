
namespace core.Nodes;

public class AndNode : ConditionNode
{
    public ConditionNode Left { get; }
    public ConditionNode Right { get; }

    public AndNode(ConditionNode left, ConditionNode right)
    {
        Left = left;
        Right = right;
    }
}

public class CartesienProductNode : ExpressionNode
{
    public ExpressionNode Left{get;}
    public ExpressionNode Right{get;}

    public CartesienProductNode(ExpressionNode left,ExpressionNode right)
    {
        Left = left;
        Right = right;
    }
}
