using System.Linq.Expressions;
using Rubicon.Data.Linq.Clauses;
using Rubicon.Data.Linq.DataObjectModel;
using Rubicon.Utilities;

namespace Rubicon.Data.Linq.Parsing.FieldResolving
{
  public class QueryModelFieldResolver
  {
    private readonly QueryModel _queryModel;

    public QueryModelFieldResolver (QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull ("queryExpression", queryModel);
      _queryModel = queryModel;
    }

    public FieldDescriptor ResolveField (ClauseFieldResolver resolver, Expression fieldAccessExpression)
    {
      ArgumentUtility.CheckNotNull ("resolver", resolver);
      ArgumentUtility.CheckNotNull ("fieldAccessExpression", fieldAccessExpression);

      QueryModelFieldResolverVisitor visitor = new QueryModelFieldResolverVisitor (_queryModel);
      QueryModelFieldResolverVisitor.Result visitorResult = visitor.ParseAndReduce (fieldAccessExpression);

      if (visitorResult != null)
        return visitorResult.ResolveableClause.ResolveField (resolver, visitorResult.ReducedExpression, fieldAccessExpression);
      else if (_queryModel.ParentQuery != null)
        return _queryModel.ParentQuery.ResolveField (resolver, fieldAccessExpression);
      else
      {
        string message = string.Format ("The field access expression '{0}' does not contain a from clause identifier.", fieldAccessExpression);
        throw new FieldAccessResolveException (message);
      }

    }
  }
}