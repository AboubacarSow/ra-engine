
namespace core.Nodes;

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
