using System;
using System.Linq.Expressions;
using System.Reflection;
using TypeExtensions = NHibernate.Mapping.ByCode.TypeExtensions;

namespace NHibernate.Test.MappingByCode
{
	public static class For<T>
	{
		public static MemberInfo Property(Expression<Func<T, object>> propertyGetter)
		{
			if (propertyGetter == null)
			{
				return null;
			}
			return TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
		}
	}
}
