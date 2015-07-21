using System;
using System.Linq.Expressions;
using NHibernate.Util;

namespace NHibernate.Linq.Expressions
{
	public class NhAverageExpression : NhAggregatedExpression
	{
		public NhAverageExpression(Expression expression) : base(expression, CalculateAverageType(expression.Type), NhExpressionType.Average)
		{
		}

		private static System.Type CalculateAverageType(System.Type inputType)
		{
			var isNullable = false;

			if (inputType.IsNullable())
			{
				isNullable = true;
				inputType = inputType.NullableOf();
			}

			switch (System.Type.GetTypeCode(inputType))
			{
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return isNullable ? typeof(double?) : typeof (double);
				case TypeCode.Decimal:
					return isNullable ? typeof(decimal?) : typeof(decimal);
			}

			throw new NotSupportedException(inputType.FullName);
		}

		public override Expression CreateNew(Expression expression)
		{
			return new NhAverageExpression(expression);
		}
	}
}