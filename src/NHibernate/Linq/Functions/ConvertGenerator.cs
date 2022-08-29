using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	public abstract class ConvertToGenerator<T> : BaseHqlGeneratorForMethod
	{
		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Cast(visitor.Visit(arguments[0]).AsExpression(), typeof(T));
		}
	}

	//NH-3720
	public class ConvertToDateTimeGenerator : ConvertToGenerator<DateTime>
	{
		public ConvertToDateTimeGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(DateTime.Parse, default(string)),
				ReflectHelper.FastGetMethod(Convert.ToDateTime, default(string))
			};
		}
	}

	//NH-3720
	public class ConvertToBooleanGenerator : ConvertToGenerator<Boolean>
	{
		public ConvertToBooleanGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(bool.Parse, default(string)),
				ReflectHelper.FastGetMethod(Convert.ToBoolean, default(string))
			};
		}
	}

	public class ConvertToInt32Generator : ConvertToGenerator<int>
	{
		public ConvertToInt32Generator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(int.Parse, default(string)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(bool)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(byte)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(char)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(decimal)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(double)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(float)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(int)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(long)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(object)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(sbyte)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(short)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(string)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(uint)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(ulong)),
				ReflectHelper.FastGetMethod(Convert.ToInt32, default(ushort))
			};
		}
	}

	public class ConvertToDecimalGenerator : ConvertToGenerator<decimal>
	{
		public ConvertToDecimalGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(decimal.Parse, default(string)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(bool)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(byte)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(decimal)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(double)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(float)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(int)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(long)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(object)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(sbyte)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(short)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(string)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(uint)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(ulong)),
				ReflectHelper.FastGetMethod(Convert.ToDecimal, default(ushort))
			};
		}
	}

	public class ConvertToDoubleGenerator : ConvertToGenerator<double>
	{
		public ConvertToDoubleGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(double.Parse, default(string)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(bool)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(byte)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(decimal)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(double)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(float)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(int)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(long)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(object)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(sbyte)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(short)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(string)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(uint)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(ulong)),
				ReflectHelper.FastGetMethod(Convert.ToDouble, default(ushort))
			};
		}
	}
}
