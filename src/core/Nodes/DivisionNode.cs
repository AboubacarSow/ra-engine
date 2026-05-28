
namespace core.Nodes;

public class DivisionNode : ExpressionNode
{
    public ProjectionNode Left{get;}
    public ProjectionNode Right{get;}

    public DivisionNode (ProjectionNode left, ProjectionNode right)
    {
        Left = left;
        Right = right;
    }
}
