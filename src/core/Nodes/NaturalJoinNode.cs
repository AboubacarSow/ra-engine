namespace core.Nodes;

public class NaturalJoinNode : ExpressionNode
{
    public ExpressionNode Left { get; }
    public ExpressionNode Right { get; }

    public NaturalJoinNode(ExpressionNode left, ExpressionNode right)
    {
        Left = left;
        Right = right;
    }
}
