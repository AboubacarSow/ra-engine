using System.Reflection;
using core.Nodes;

namespace transformer;

public class Transformer
{
    public static ExpressionNode Transform(SelectionNode node)
    {
        ConditionNode previous = node.Condition;
        ExpressionNode newsource=node.Source;
        while (newsource is not RelationNode or NaturalJoinNode)
        {
            if(newsource is SelectionNode s)
            {
                previous = new AndNode(s.Condition,previous);
                newsource = s.Source;
                continue;
            }
            if(newsource is ProjectionNode projection)
            {
                newsource = projection.Source;
                continue;
            }
            if(newsource is NaturalJoinNode)
            {
                return node;
            }

            if(newsource is CartesienProductNode cartesienProduct)
            {
                //var condition = previous as ComparisonNode;
                var newnode= new ThetaJoinNode(cartesienProduct.Left,cartesienProduct.Right,previous);
                return newnode;
            }
            
        }
        return new SelectionNode(previous,newsource);
        
    }
}