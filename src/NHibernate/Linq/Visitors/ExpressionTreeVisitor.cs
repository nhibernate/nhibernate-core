using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.Expressions;

namespace NHibernate.Linq.Visitors
{
    /// <summary>
    /// This class is used to expose the members from the base class that get internalized when the other class is ilmerged.
    /// We do this instead of exposing the base class directly by name, since we don't want it to be part of our public API.
    /// </summary>
    public class ExpressionTreeVisitor : Remotion.Data.Linq.Parsing.ExpressionTreeVisitor
    {
        public override ReadOnlyCollection<T> VisitAndConvert<T>(ReadOnlyCollection<T> expressions, string callerName)
        {
            return base.VisitAndConvert<T>(expressions, callerName);
        }

        public override T VisitAndConvert<T>(T expression, string methodName)
        {
            return base.VisitAndConvert<T>(expression, methodName);
        }

        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            return base.VisitBinaryExpression(expression);
        }

        protected override Expression VisitConditionalExpression(ConditionalExpression expression)
        {
            return base.VisitConditionalExpression(expression);
        }

        protected override Expression VisitConstantExpression(ConstantExpression expression)
        {
            return base.VisitConstantExpression(expression);
        }

        protected override ElementInit VisitElementInit(ElementInit elementInit)
        {
            return base.VisitElementInit(elementInit);
        }

        protected override ReadOnlyCollection<ElementInit> VisitElementInitList(ReadOnlyCollection<ElementInit> expressions)
        {
            return base.VisitElementInitList(expressions);
        }

        public override Expression VisitExpression(Expression expression)
        {
            return base.VisitExpression(expression);
        }

        protected override Expression VisitExtensionExpression(ExtensionExpression expression)
        {
            return base.VisitExtensionExpression(expression);
        }

        protected override Expression VisitInvocationExpression(InvocationExpression expression)
        {
            return base.VisitInvocationExpression(expression);
        }

        protected override Expression VisitLambdaExpression(LambdaExpression expression)
        {
            return base.VisitLambdaExpression(expression);
        }

        protected override Expression VisitListInitExpression(ListInitExpression expression)
        {
            return base.VisitListInitExpression(expression);
        }

        protected override MemberBinding VisitMemberAssignment(MemberAssignment memberAssigment)
        {
            return base.VisitMemberAssignment(memberAssigment);
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding memberBinding)
        {
            return base.VisitMemberBinding(memberBinding);
        }

        protected override ReadOnlyCollection<MemberBinding> VisitMemberBindingList(ReadOnlyCollection<MemberBinding> expressions)
        {
            return base.VisitMemberBindingList(expressions);
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            return base.VisitMemberExpression(expression);
        }

        protected override Expression VisitMemberInitExpression(MemberInitExpression expression)
        {
            return base.VisitMemberInitExpression(expression);
        }

        protected override MemberBinding VisitMemberListBinding(MemberListBinding listBinding)
        {
            return base.VisitMemberListBinding(listBinding);
        }

        protected override MemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            return base.VisitMemberMemberBinding(binding);
        }

        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            return base.VisitMethodCallExpression(expression);
        }

        protected override Expression VisitNewArrayExpression(NewArrayExpression expression)
        {
            return base.VisitNewArrayExpression(expression);
        }

        protected override Expression VisitNewExpression(NewExpression expression)
        {
            return base.VisitNewExpression(expression);
        }

        protected override Expression VisitParameterExpression(ParameterExpression expression)
        {
            return base.VisitParameterExpression(expression);
        }

        protected override Expression VisitQuerySourceReferenceExpression(Remotion.Data.Linq.Clauses.Expressions.QuerySourceReferenceExpression expression)
        {
            return base.VisitQuerySourceReferenceExpression(expression);
        }

        protected override Expression VisitSubQueryExpression(Remotion.Data.Linq.Clauses.Expressions.SubQueryExpression expression)
        {
            return base.VisitSubQueryExpression(expression);
        }

        protected override Expression VisitTypeBinaryExpression(TypeBinaryExpression expression)
        {
            return base.VisitTypeBinaryExpression(expression);
        }

        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            return base.VisitUnaryExpression(expression);
        }

        [Obsolete]
        protected override Expression VisitUnknownExpression(Expression expression)
        {
            return base.VisitUnknownExpression(expression);
        }

        protected override Expression VisitUnknownNonExtensionExpression(Expression expression)
        {
            return base.VisitUnknownNonExtensionExpression(expression);
        }
    }
}
