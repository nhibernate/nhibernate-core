﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using System.Collections.ObjectModel;

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
									   ReflectionHelper.GetMethodDefinition<string>(s => DateTime.Parse(s)),
									   ReflectionHelper.GetMethodDefinition<string>(o => Convert.ToDateTime(o))
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
									   ReflectionHelper.GetMethodDefinition<string>(s => Boolean.Parse(s)),
									   ReflectionHelper.GetMethodDefinition<string>(o => Convert.ToBoolean(o))
								   };
		}
	}


	public class ConvertToInt32Generator : ConvertToGenerator<int>
	{
		public ConvertToInt32Generator()
		{
			SupportedMethods = new[]
								   {
									   ReflectionHelper.GetMethodDefinition<string>(s => int.Parse(s)),
									   ReflectionHelper.GetMethodDefinition<bool>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<byte>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<char>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<decimal>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<double>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<float>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<int>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<long>(o => Convert.ToInt32(o)), 
									   ReflectionHelper.GetMethodDefinition<object>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<sbyte>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<short>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<string>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<uint>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<ulong>(o => Convert.ToInt32(o)),
									   ReflectionHelper.GetMethodDefinition<ushort>(o => Convert.ToInt32(o))
								   };
		}
	}

	public class ConvertToDecimalGenerator : ConvertToGenerator<decimal>
	{
		public ConvertToDecimalGenerator()
		{
			SupportedMethods = new[]
								   {
									   ReflectionHelper.GetMethodDefinition<string>(s => decimal.Parse(s)),
									   ReflectionHelper.GetMethodDefinition<bool>(o => Convert.ToDecimal(o)),
									   ReflectionHelper.GetMethodDefinition<byte>(o => Convert.ToDecimal(o)),
									   ReflectionHelper.GetMethodDefinition<decimal>(o => Convert.ToDecimal(o)),
									   ReflectionHelper.GetMethodDefinition<double>(o => Convert.ToDecimal(o)),
									   ReflectionHelper.GetMethodDefinition<float>(o => Convert.ToDecimal(o)),
									   ReflectionHelper.GetMethodDefinition<int>(o => Convert.ToDecimal(o)),
									   ReflectionHelper.GetMethodDefinition<long>(o => Convert.ToDecimal(o)), 
									   ReflectionHelper.GetMethodDefinition<object>(o => Convert.ToDecimal(o)),
									   ReflectionHelper.GetMethodDefinition<sbyte>(o => Convert.ToDecimal(o)),
									   ReflectionHelper.GetMethodDefinition<short>(o => Convert.ToDecimal(o)),
									   ReflectionHelper.GetMethodDefinition<string>(o => Convert.ToDecimal(o)),
									   ReflectionHelper.GetMethodDefinition<uint>(o => Convert.ToDecimal(o)),
									   ReflectionHelper.GetMethodDefinition<ulong>(o => Convert.ToDecimal(o)),
									   ReflectionHelper.GetMethodDefinition<ushort>(o => Convert.ToDecimal(o))
								   };
		}
	}

	public class ConvertToDoubleGenerator : ConvertToGenerator<double>
	{
		public ConvertToDoubleGenerator()
		{
			SupportedMethods = new[]
								   {
									   ReflectionHelper.GetMethodDefinition<string>(s => double.Parse(s)),
									   ReflectionHelper.GetMethodDefinition<bool>(o => Convert.ToDouble(o)),
									   ReflectionHelper.GetMethodDefinition<byte>(o => Convert.ToDouble(o)),
									   ReflectionHelper.GetMethodDefinition<decimal>(o => Convert.ToDouble(o)),
									   ReflectionHelper.GetMethodDefinition<double>(o => Convert.ToDouble(o)),
									   ReflectionHelper.GetMethodDefinition<float>(o => Convert.ToDouble(o)),
									   ReflectionHelper.GetMethodDefinition<int>(o => Convert.ToDouble(o)),
									   ReflectionHelper.GetMethodDefinition<long>(o => Convert.ToDouble(o)), 
									   ReflectionHelper.GetMethodDefinition<object>(o => Convert.ToDouble(o)),
									   ReflectionHelper.GetMethodDefinition<sbyte>(o => Convert.ToDouble(o)),
									   ReflectionHelper.GetMethodDefinition<short>(o => Convert.ToDouble(o)),
									   ReflectionHelper.GetMethodDefinition<string>(o => Convert.ToDouble(o)),
									   ReflectionHelper.GetMethodDefinition<uint>(o => Convert.ToDouble(o)),
									   ReflectionHelper.GetMethodDefinition<ulong>(o => Convert.ToDouble(o)),
									   ReflectionHelper.GetMethodDefinition<ushort>(o => Convert.ToDouble(o))
								   };
		}
	}
}
