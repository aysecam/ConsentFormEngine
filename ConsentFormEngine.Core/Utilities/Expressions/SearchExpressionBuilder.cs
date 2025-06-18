using System.Linq.Expressions;

namespace ConsentFormEngine.Core.Utilities.Expressions
{
    public static class SearchExpressionBuilder
    {
        public static Expression<Func<TEntity, bool>> BuildSearchExpression<TEntity>(
            string searchText,
            params Expression<Func<TEntity, string>>[] stringProperties)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return x => true;

            var parameter = Expression.Parameter(typeof(TEntity), "x");

            Expression? orExpression = null;

            foreach (var prop in stringProperties)
            {
                var body = Expression.Call(
                    Expression.Invoke(prop, parameter),
                    typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                    Expression.Constant(searchText, typeof(string)));

                orExpression = orExpression == null ? body : Expression.OrElse(orExpression, body);
            }

            return Expression.Lambda<Func<TEntity, bool>>(orExpression!, parameter);
        }
    }

}
