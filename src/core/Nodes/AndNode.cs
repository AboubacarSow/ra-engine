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
