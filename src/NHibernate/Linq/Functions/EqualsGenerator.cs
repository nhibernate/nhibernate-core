using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	public class EqualsGenerator : BaseHqlGeneratorForMethod
	{
		public EqualsGenerator()
		{
			SupportedMethods = new[]
				{
					ReflectHelper.GetMethodDefinition(() => string.Equals(default(string), default(string))),
					ReflectHelper.GetMethodDefinition<string>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<char>(x => x.Equals(x)),

					ReflectHelper.GetMethodDefinition<sbyte>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<byte>(x => x.Equals(x)),

					ReflectHelper.GetMethodDefinition<short>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<ushort>(x => x.Equals(x)),

					ReflectHelper.GetMethodDefinition<int>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<uint>(x => x.Equals(x)),

					ReflectHelper.GetMethodDefinition<long>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<ulong>(x => x.Equals(x)),

					ReflectHelper.GetMethodDefinition<float>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<double>(x => x.Equals(x)),
					
					ReflectHelper.GetMethodDefinition(() => decimal.Equals(default(decimal), default(decimal))),
					ReflectHelper.GetMethodDefinition<decimal>(x => x.Equals(x)),

					ReflectHelper.GetMethodDefinition<Guid>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<DateTime>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<DateTimeOffset>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<TimeSpan>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<bool>(x => x.Equals(default(bool))),

					ReflectHelper.GetMethodDefinition<IEquatable<string>>(x => x.Equals(default(string))),
					ReflectHelper.GetMethodDefinition<IEquatable<char>>(x => x.Equals(default(char))),
					ReflectHelper.GetMethodDefinition<IEquatable<sbyte>>(x => x.Equals(default(sbyte))),
					ReflectHelper.GetMethodDefinition<IEquatable<byte>>(x => x.Equals(default(byte))),
					ReflectHelper.GetMethodDefinition<IEquatable<short>>(x => x.Equals(default(short))),
					ReflectHelper.GetMethodDefinition<IEquatable<ushort>>(x => x.Equals(default(ushort))),
					ReflectHelper.GetMethodDefinition<IEquatable<int>>(x => x.Equals(default(int))),
					ReflectHelper.GetMethodDefinition<IEquatable<uint>>(x => x.Equals(default(uint))),
					ReflectHelper.GetMethodDefinition<IEquatable<long>>(x => x.Equals(default(long))),
					ReflectHelper.GetMethodDefinition<IEquatable<ulong>>(x => x.Equals(default(ulong))),
					ReflectHelper.GetMethodDefinition<IEquatable<float>>(x => x.Equals(default(float))),
					ReflectHelper.GetMethodDefinition<IEquatable<double>>(x => x.Equals(default(double))),
					ReflectHelper.GetMethodDefinition<IEquatable<decimal>>(x => x.Equals(default(decimal))),
					ReflectHelper.GetMethodDefinition<IEquatable<Guid>>(x => x.Equals(default(Guid))),
					ReflectHelper.GetMethodDefinition<IEquatable<DateTime>>(x => x.Equals(default(DateTime))),
					ReflectHelper.GetMethodDefinition<IEquatable<DateTimeOffset>>(x => x.Equals(default(DateTimeOffset))),
					ReflectHelper.GetMethodDefinition<IEquatable<TimeSpan>>(x => x.Equals(default(TimeSpan))),
					ReflectHelper.GetMethodDefinition<IEquatable<bool>>(x => x.Equals(default(bool)))
				};
		}

		public override bool AllowsNullableReturnType(MethodInfo method) => false;

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			Expression lhs = arguments.Count == 1 ? targetObject : arguments[0];
			Expression rhs = arguments.Count == 1 ? arguments[0] : arguments[1];

			return visitor.Visit(Expression.Equal(lhs, rhs));
		}
	}
}
