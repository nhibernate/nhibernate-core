using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using NHibernate.Metadata;
using NHibernate.Type;
using System.Reflection;
using System.Collections.ObjectModel;

namespace NHibernate.Linq.Visitors
{
	public class VisitorUtil
	{
		public static bool IsDynamicComponentDictionaryGetter(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, ISessionFactory sessionFactory,out string memberName)
		{
			//an IDictionary item getter?
			if (method.Name == "get_Item" && typeof(IDictionary).IsAssignableFrom(targetObject.Type))
			{//a  string constant expression as the argument?	
				ConstantExpression key = arguments.First().As<ConstantExpression>();
				if (key != null && key.Type == typeof(string))
				{
					//The potential member name
					memberName = (string)key.Value;
					//need the owning member
					MemberExpression member = targetObject.As<MemberExpression>();
					if (member != null)
					{
						IClassMetadata metaData = sessionFactory.GetClassMetadata(member.Expression.Type);
						if (metaData != null)
						{
							// is it mapped as a component?
							IType propertyType = metaData.GetPropertyType(member.Member.Name);
							return (propertyType != null && propertyType.IsComponentType);
						}
					}
				}
			}
			memberName=null;
			return false;
		}

		public static bool IsDynamicComponentDictionaryGetter(MethodCallExpression expression, ISessionFactory sessionFactory, out string memberName)
		{
			return IsDynamicComponentDictionaryGetter(expression.Method,expression.Object,expression.Arguments,sessionFactory,out memberName);
		}

		public static bool IsDynamicComponentDictionaryGetter(MethodCallExpression expression, ISessionFactory sessionFactory)
		{
			string memberName;
			return IsDynamicComponentDictionaryGetter(expression, sessionFactory, out memberName);
		}
	}
}
