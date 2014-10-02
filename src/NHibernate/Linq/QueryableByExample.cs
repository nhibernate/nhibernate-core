using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NHibernate.Metadata;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Linq
{
	[Flags]
	public enum ExampleMatchMode
	{
		Exact = 0,
		IgnoreCase = 1,
		Start = 2,
		End = 4,
		Anywhere = Start | End
	}

	public static class ExampleMatchModeExtensions
	{
		public static bool IsStart(this ExampleMatchMode mode)
		{
			return (mode & ExampleMatchMode.Start) == ExampleMatchMode.Start;
		}

		public static bool IsEnd(this ExampleMatchMode mode)
		{
			return (mode & ExampleMatchMode.End) == ExampleMatchMode.End;
		}

		public static bool IsAnywhere(this ExampleMatchMode mode)
		{
			return (mode & ExampleMatchMode.Anywhere) == ExampleMatchMode.Anywhere;
		}

		public static bool IsExact(this ExampleMatchMode mode)
		{
			return (mode & ExampleMatchMode.Exact) == ExampleMatchMode.Exact;
		}

		public static bool IsIgnoreCase(this ExampleMatchMode mode)
		{
			return (mode & ExampleMatchMode.IgnoreCase) == ExampleMatchMode.IgnoreCase;
		}
	}

	//NH-3714
	public interface IQueryableByExample<T> : IQueryable<T>
	{
		IQueryableByExample<T> MatchMode(ExampleMatchMode mode);
		IQueryableByExample<T> Exclude(string propertyName);
		IQueryableByExample<T> Exclude<TProp>(Expression<Func<T, TProp>> prop);
		IQueryableByExample<T> ExcludeZeroes();
		IQueryableByExample<T> ExcludeNulls();
		IQueryableByExample<T> ExcludeNone();
		IQueryableByExample<T> IncludeCollectionsCount();
		IQueryableByExample<T> IncludeDefaultValues();
	}

	//NH-3714
	class QueryableByExample<T> : IQueryableByExample<T>, IOrderedQueryable<T>
	{
		private readonly IQueryable<T> inner;
		private readonly T example;
		private readonly IClassMetadata classMetadata;
		private string[] propertyNames;
		private readonly ExampleMatchMode matchMode;
		private readonly bool excludeZeroes;
		private readonly bool excludeNulls;
		private readonly bool includeCollectionsCount;
		private readonly bool includeDefaultValues;

		public QueryableByExample(IQueryable<T> inner, T example, IClassMetadata classMetadata) : this(inner, example, classMetadata, classMetadata.PropertyNames, ExampleMatchMode.Exact, false, false, false, false)
		{
		}

		public QueryableByExample(IQueryable<T> inner, T example, IClassMetadata classMetadata, string[] propertyNames, ExampleMatchMode matchMode, bool excludeZeroes, bool excludeNulls, bool includeCollectionsCount, bool includeDefaultValues)
		{
			this.inner = inner;
			this.example = example;
			this.classMetadata = classMetadata;
			this.propertyNames = propertyNames;
			this.matchMode = matchMode;
			this.excludeZeroes = excludeZeroes;
			this.excludeNulls = excludeNulls;
			this.includeCollectionsCount = includeCollectionsCount;
			this.includeDefaultValues = includeDefaultValues;
		}

		#region IQueryableByExample<T> Members
		public IQueryableByExample<T> IncludeDefaultValues()
		{
			if (this.includeDefaultValues == true)
			{
				return this;
			}
			else
			{
				return new QueryableByExample<T>(this.inner, this.example, this.classMetadata, this.propertyNames, this.matchMode, this.excludeZeroes, this.excludeNulls, this.includeCollectionsCount, true);
			}
		}

		public IQueryableByExample<T> IncludeCollectionsCount()
		{
			if (this.includeCollectionsCount == true)
			{
				return this;
			}
			else
			{
				return new QueryableByExample<T>(this.inner, this.example, this.classMetadata, this.propertyNames, this.matchMode, this.excludeZeroes, this.excludeNulls, true, this.includeDefaultValues);
			}
		}

		public IQueryableByExample<T> ExcludeNone()
		{
			var propertyNames = this.classMetadata.PropertyNames;

			return new QueryableByExample<T>(inner: this.inner, example: example, classMetadata: this.classMetadata, propertyNames: propertyNames, matchMode: ExampleMatchMode.Exact, excludeZeroes: false, excludeNulls: false, includeCollectionsCount: false, includeDefaultValues: false);
		}

		public IQueryableByExample<T> ExcludeZeroes()
		{
			if (this.excludeZeroes == true)
			{
				return this;
			}
			else
			{
				return new QueryableByExample<T>(this.inner, this.example, this.classMetadata, this.propertyNames, this.matchMode, true, this.excludeNulls, this.includeCollectionsCount, this.includeDefaultValues);
			}
		}

		public IQueryableByExample<T> ExcludeNulls()
		{
			if (this.excludeNulls == true)
			{
				return this;
			}
			else
			{
				return new QueryableByExample<T>(this.inner, this.example, this.classMetadata, this.propertyNames, this.matchMode, this.excludeZeroes, true, this.includeCollectionsCount, this.includeDefaultValues);
			}
		}

		public IQueryableByExample<T> MatchMode(ExampleMatchMode mode)
		{
			if (matchMode.Equals(mode) == true)
			{
				return this;
			}
			else
			{
				return new QueryableByExample<T>(this.inner, this.example, this.classMetadata, this.propertyNames, mode, this.excludeZeroes, this.excludeNulls, this.includeCollectionsCount, this.includeDefaultValues);
			}
		}

		public IQueryableByExample<T> Exclude(string propertyName)
		{
			return new QueryableByExample<T>(this.inner, this.example, this.classMetadata, this.propertyNames.Where(x => x != propertyName).ToArray(), this.matchMode, this.excludeZeroes, this.excludeNulls, this.includeCollectionsCount, this.includeDefaultValues);
		}

		public IQueryableByExample<T> Exclude<TProp>(Expression<Func<T, TProp>> prop)
		{
			return this.Exclude((prop.Body as System.Linq.Expressions.MemberExpression).Member.Name);
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return this.Provider.CreateQuery<T>(this.Expression).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region IQueryable Members

		public System.Type ElementType
		{
			get { return this.inner.ElementType; }
		}

		private IQueryable<T> CreateExpression(IQueryable<T> newQuery, System.Type entityType, EntityMode entityMode, string [] propertyNames, object [] propertyValues, IType [] propertyTypes, string componentPath = null)
		{
			for (var i = 0; i < propertyNames.Length; ++i)
			{
				if (propertyTypes[i].IsCollectionType == true)
				{
					var collectionValue = this.classMetadata.GetPropertyValue(this.example, propertyNames[i], entityMode);

					if (collectionValue != null)
					{
						if (this.includeCollectionsCount == true)
						{
							var parameter = Expression.Parameter(typeof(T), "p");
							var member = entityType.GetPropertyOrFieldMatchingName(propertyNames[i]);
							var prop = Expression.MakeMemberAccess(parameter, member);
							var count = Expression.Call(null, ReflectionHelper.GetMethod<IEnumerable<object>>(x => x.Count()), prop);
							var equals = Expression.MakeBinary(ExpressionType.Equal, count, Expression.Constant((propertyValues[i] as System.Collections.IEnumerable).OfType<object>().Count()));

							var cond = Expression.Lambda<Func<T, bool>>(equals, parameter);

							newQuery = newQuery.Where(cond);
						}
					}

					continue;
				}

				if (propertyTypes[i].IsComponentType == true)
				{
					var componentValue = this.classMetadata.GetPropertyValue(this.example, propertyNames[i], entityMode);

					if (componentValue != null)
					{
						var componentType = (propertyTypes[i] as ComponentType);
						var componentPropertyNames = componentType.PropertyNames;
						var componentPropertyValues = componentType.GetPropertyValues(componentValue, entityMode);
						var componentPropertyTypes = componentPropertyValues.Select((value, index) => NHibernateUtil.GuessType(componentPropertyValues[index])).ToArray();

						newQuery = this.CreateExpression(newQuery, componentType.ReturnedClass, entityMode, componentPropertyNames, componentPropertyValues, componentPropertyTypes, propertyNames[i]);
					}

					continue;
				}

				if (propertyTypes[i].IsAssociationType == true)
				{
					var entityValue = this.classMetadata.GetPropertyValue(this.example, propertyNames[i], entityMode);

					if (entityValue != null)
					{

					}
				}

				if (this.IsDefaultValue(propertyValues[i], propertyTypes[i].ReturnedClass) == false)
				{
					newQuery = this.AppendRestrictionToQuery(newQuery, entityType, propertyNames[i], propertyValues[i], propertyTypes[i], componentPath);
				}
			}

			return newQuery;
		}

		public Expression Expression
		{
			get
			{
				var entityMode = EntityMode.Poco;
				var propertyIndexes = this.classMetadata.PropertyNames.Select((name, index) => new { original_name = name, original_index = index }).Where(n => this.propertyNames.Contains(n.original_name)).Select((n, index) => n.original_index).ToArray();
				var propertyValues = this.classMetadata.GetPropertyValues(this.example, entityMode).Where((value, index) => propertyIndexes.Contains(index)).ToArray();
				var propertyTypes = this.classMetadata.PropertyTypes.Where((value, index) => propertyIndexes.Contains(index)).ToArray();
				var entityType = this.ElementType;

				return this.CreateExpression(this.inner, entityType, entityMode, this.propertyNames, propertyValues, propertyTypes).Expression;
			}
		}

		private IQueryable<T> AppendRestrictionToQuery(IQueryable<T> query, System.Type entityType, string propertyName, object propertyValue, IType propertyType, string componentPath = null)
		{
			var newQuery = query;
			var numericTypes = new[] { typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) };

			var member = null as MemberInfo;
			var prop = null as MemberExpression;
			var parameter = Expression.Parameter(typeof(T), "p");

			if (componentPath == null)
			{
				member = entityType.GetPropertyOrFieldMatchingName(propertyName);
				prop = Expression.MakeMemberAccess(parameter, member);
			}
			else
			{
				member = typeof(T).GetPropertyOrFieldMatchingName(componentPath);
				prop = Expression.MakeMemberAccess(parameter, member);
				prop = Expression.MakeMemberAccess(prop, entityType.GetPropertyOrFieldMatchingName(propertyName));
			}

			var value = Expression.Constant(propertyValue);
			var equals = Expression.MakeBinary(ExpressionType.Equal, prop, value);

			if (propertyValue == null)
			{
				if (this.excludeNulls == true)
				{
					return newQuery;
				}
			}

			if (propertyType.ReturnedClass == typeof(string))
			{
				var stringValue = propertyValue as string;

				if (this.matchMode.IsExact() == false)
				{
					if (this.matchMode.IsAnywhere() == true)
					{
						value = Expression.Constant(String.Concat("%", stringValue, "%"));
					}
					else if (this.matchMode.IsStart() == true)
					{
						value = Expression.Constant(String.Concat(stringValue, "%"));
					}
					else if (this.matchMode.IsEnd() == true)
					{
						value = Expression.Constant(String.Concat("%", stringValue));
					}

					if (this.matchMode.IsIgnoreCase() == true)
					{
						value = Expression.Constant(stringValue.ToUpper());
						equals = Expression.MakeBinary(ExpressionType.Equal, Expression.Call(prop, ReflectionHelper.GetMethod<string>(x => x.ToUpper())), value);
					}
					else
					{
						equals = Expression.MakeBinary(ExpressionType.Equal, Expression.Call(null, ReflectionHelper.GetMethod<string>(x => x.Like(null)), prop, value), Expression.Constant(true));
					}
				}
			}
			else if (numericTypes.Contains(propertyType.ReturnedClass) == true)
			{
				if (this.excludeZeroes == true)
				{
					if (Convert.ToInt32(propertyValue) == 0)
					{
						return newQuery;
					}
				}
			}

			var cond = Expression.Lambda<Func<T, bool>>(equals, parameter);

			newQuery = newQuery.Where(cond);

			return newQuery;
		}

		public IQueryProvider Provider
		{
			get { return this.inner.Provider; }
		}

		#endregion

		private bool IsDefaultValue(object value, System.Type type)
		{
			if (this.includeDefaultValues == true)
			{
				return false;
			}

			if (value == null)
			{
				return true;
			}

			if (type.IsGenericCollection() == true)
			{
				return true;
			}

			if (type == typeof(string))
			{
				return (value as string) == string.Empty;
			}

			return (type.IsClass == false) ? value.Equals(Activator.CreateInstance(type)) : false;
		}
	}
}
