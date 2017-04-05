using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using System.Collections.ObjectModel;
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
				ReflectHelper.GetMethodDefinition<string>(s => DateTime.Parse(s)),
				ReflectHelper.GetMethodDefinition<string>(o => Convert.ToDateTime(o))
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
				ReflectHelper.GetMethodDefinition<string>(s => Boolean.Parse(s)),
				ReflectHelper.GetMethodDefinition<string>(o => Convert.ToBoolean(o))
			};
		}
	}


	public class ConvertToInt32Generator : ConvertToGenerator<int>
	{
		public ConvertToInt32Generator()
		{
			SupportedMethods = new[]
								   {
									   ReflectHelper.GetMethodDefinition<string>(s => int.Parse(s)),
									   ReflectHelper.GetMethodDefinition<bool>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<byte>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<char>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<decimal>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<double>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<float>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<int>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<long>(o => Convert.ToInt32(o)), 
									   ReflectHelper.GetMethodDefinition<object>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<sbyte>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<short>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<string>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<uint>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<ulong>(o => Convert.ToInt32(o)),
									   ReflectHelper.GetMethodDefinition<ushort>(o => Convert.ToInt32(o))
								   };
		}
	}

	public class ConvertToDecimalGenerator : ConvertToGenerator<decimal>
	{
		public ConvertToDecimalGenerator()
		{
			SupportedMethods = new[]
								   {
									   ReflectHelper.GetMethodDefinition<string>(s => decimal.Parse(s)),
									   ReflectHelper.GetMethodDefinition<bool>(o => Convert.ToDecimal(o)),
									   ReflectHelper.GetMethodDefinition<byte>(o => Convert.ToDecimal(o)),
									   ReflectHelper.GetMethodDefinition<decimal>(o => Convert.ToDecimal(o)),
									   ReflectHelper.GetMethodDefinition<double>(o => Convert.ToDecimal(o)),
									   ReflectHelper.GetMethodDefinition<float>(o => Convert.ToDecimal(o)),
									   ReflectHelper.GetMethodDefinition<int>(o => Convert.ToDecimal(o)),
									   ReflectHelper.GetMethodDefinition<long>(o => Convert.ToDecimal(o)), 
									   ReflectHelper.GetMethodDefinition<object>(o => Convert.ToDecimal(o)),
									   ReflectHelper.GetMethodDefinition<sbyte>(o => Convert.ToDecimal(o)),
									   ReflectHelper.GetMethodDefinition<short>(o => Convert.ToDecimal(o)),
									   ReflectHelper.GetMethodDefinition<string>(o => Convert.ToDecimal(o)),
									   ReflectHelper.GetMethodDefinition<uint>(o => Convert.ToDecimal(o)),
									   ReflectHelper.GetMethodDefinition<ulong>(o => Convert.ToDecimal(o)),
									   ReflectHelper.GetMethodDefinition<ushort>(o => Convert.ToDecimal(o))
								   };
		}
	}

	public class ConvertToDoubleGenerator : ConvertToGenerator<double>
	{
		public ConvertToDoubleGenerator()
		{
			SupportedMethods = new[]
								   {
									   ReflectHelper.GetMethodDefinition<string>(s => double.Parse(s)),
									   ReflectHelper.GetMethodDefinition<bool>(o => Convert.ToDouble(o)),
									   ReflectHelper.GetMethodDefinition<byte>(o => Convert.ToDouble(o)),
									   ReflectHelper.GetMethodDefinition<decimal>(o => Convert.ToDouble(o)),
									   ReflectHelper.GetMethodDefinition<double>(o => Convert.ToDouble(o)),
									   ReflectHelper.GetMethodDefinition<float>(o => Convert.ToDouble(o)),
									   ReflectHelper.GetMethodDefinition<int>(o => Convert.ToDouble(o)),
									   ReflectHelper.GetMethodDefinition<long>(o => Convert.ToDouble(o)), 
									   ReflectHelper.GetMethodDefinition<object>(o => Convert.ToDouble(o)),
									   ReflectHelper.GetMethodDefinition<sbyte>(o => Convert.ToDouble(o)),
									   ReflectHelper.GetMethodDefinition<short>(o => Convert.ToDouble(o)),
									   ReflectHelper.GetMethodDefinition<string>(o => Convert.ToDouble(o)),
									   ReflectHelper.GetMethodDefinition<uint>(o => Convert.ToDouble(o)),
									   ReflectHelper.GetMethodDefinition<ulong>(o => Convert.ToDouble(o)),
									   ReflectHelper.GetMethodDefinition<ushort>(o => Convert.ToDouble(o))
								   };
		}
	}
}
