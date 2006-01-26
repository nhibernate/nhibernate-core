using System;
using Refly.CodeDom.Collections;

namespace Refly.CodeDom.Expressions
{
    public class ArrayIndexerExpression :Expression
    {
        private Expression targetExpression;
        private ExpressionCollection indices = new ExpressionCollection();

        public ArrayIndexerExpression(Expression targetExpression)
        {
            if (targetExpression == null)
                throw new ArgumentNullException("targetExpression");
            this.targetExpression = targetExpression;
        }

        public ArrayIndexerExpression(Expression targetExpression, params Expression[] indices)
            :this(targetExpression)
        {
            this.indices.AddRange(indices);
        }

        public Expression TargetExpression
        {
            get { return this.targetExpression; }
        }

        public ExpressionCollection Indices
        {
            get { return this.indices; }
        }

        public override System.CodeDom.CodeExpression ToCodeDom()
        {
            return new System.CodeDom.CodeArrayIndexerExpression(
                this.TargetExpression.ToCodeDom(),
                this.Indices.ToCodeDomArray()
                );
        }
    }
}
