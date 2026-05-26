using System.Reflection;
using core.Nodes;

namespace transformer;

public class Transformer
{
    public static SelectionNode Transform(SelectionNode node)
    {
        ConditionNode previous = node.Condition;
        ExpressionNode newsource=node.Source;
        while (newsource is not RelationNode)
        {
            if(newsource is SelectionNode s)
            {
                previous = new AndNode(s.Condition,previous);
                newsource = s.Source;
            }
            else {
                if(newsource is ProjectionNode projection)
                {
                    newsource = projection.Source;
                    continue;
                }
            }
        }
        return new SelectionNode(previous,newsource);
        
    }
}