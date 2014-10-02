using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Mapping.ByCode;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.Type;
using NHibernate.Persister.Entity;

namespace NHibernate.Linq
{
	[Flags]
	public enum MatchMode
	{
		Exact = 0,
		IgnoreCase = 1,
		Start = 2,
		End = 4,
		Anywhere = (Start | End)
	}

	//NH-3714
	public interface IQueryableByExample<T> : IQueryable<T>
	{
		IQueryableByExample<T> Exclude(string propertyName);
		IQueryableByExample<T> Exclude<TProp>(Expression<Func<T, TProp>> prop);
		IQueryableByExample<T> SetMatchMode(MatchMode mode);
		IQueryableByExample<T> ExcludeZeroes();
		IQueryableByExample<T> ExcludeNulls();
		IQueryableByExample<T> ExcludeNone();
		IQueryableByExample<T> NullIsEmptyString();
	}

	//NH-3714
	class QueryableByExample<T> : IQueryableByExample<T>, IOrderedQueryable<T>
	{
		private readonly IQueryable<T> inner;
		private readonly T example;
		private readonly IClassMetadata classMetadata;
		private string[] propertyNames;
		private readonly MatchMode matchMode;
		private readonly bool excludeZeroes;
		private readonly bool excludeNulls;
		private readonly bool nullIsEmptyString;

		public QueryableByExample(IQueryable<T> inner, T example, IClassMetadata classMetadata) : this(inner, example, classMetadata, classMetadata.PropertyNames, MatchMode.Exact, false, false, false)
		{
		}

		public QueryableByExample(IQueryable<T> inner, T example, IClassMetadata classMetadata, string[] propertyNames, MatchMode matchMode, bool excludeZeroes, bool excludeNulls, bool nullIsEmptyString)
		{
			this.inner = inner;
			this.example = example;
			this.classMetadata = classMetadata;
			this.propertyNames = propertyNames;
			this.matchMode = matchMode;
			this.excludeZeroes = excludeZeroes;
			this.excludeNulls = excludeNulls;
			this.nullIsEmptyString = nullIsEmptyString;
		}

		#region IQueryableByExample<T> Members
		public IQueryableByExample<T> NullIsEmptyString()
		{
			if (this.nullIsEmptyString == true)
			{
				return this;
			}
			else
			{
				return new QueryableByExample<T>(this.inner, this.example, this.classMetadata, this.propertyNames, this.matchMode, this.excludeZeroes, this.excludeNulls, true);
			}
		}

		public IQueryableByExample<T> ExcludeNone()
		{
			var propertyNames = this.classMetadata.PropertyNames;

			return new QueryableByExample<T>(inner: this.inner, example: example, classMetadata: this.classMetadata, propertyNames: propertyNames, matchMode: MatchMode.Exact, excludeZeroes: false, excludeNulls: false, nullIsEmptyString: false);
		}

		public IQueryableByExample<T> ExcludeZeroes()
		{
			if (this.excludeZeroes == true)
			{
				return this;
			}
			else
			{
				return new QueryableByExample<T>(this.inner, this.example, this.classMetadata, this.propertyNames, this.matchMode, true, this.excludeNulls, this.nullIsEmptyString);
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
				return new QueryableByExample<T>(this.inner, this.example, this.classMetadata, this.propertyNames, this.matchMode, this.excludeZeroes, true, this.nullIsEmptyString);
			}
		}

		public IQueryableByExample<T> SetMatchMode(MatchMode mode)
		{
			if (matchMode.Equals(mode) == true)
			{
				return this;
			}
			else
			{
				return new QueryableByExample<T>(this.inner, this.example, this.classMetadata, this.propertyNames, mode, this.excludeZeroes, this.excludeNulls, this.nullIsEmptyString);
			}
		}

		public IQueryableByExample<T> Exclude(string propertyName)
		{
			return new QueryableByExample<T>(this.inner, this.example, this.classMetadata, this.propertyNames.Where(x => x != propertyName).ToArray(), this.matchMode, this.excludeZeroes, this.excludeNulls, this.nullIsEmptyString);
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

		public Expression Expression
		{
			get
			{
				var entityMode = EntityMode.Poco;
				var propertyIndexes = this.classMetadata.PropertyNames.Select((name, index) => new { original_name = name, original_index = index }).Where(n => this.propertyNames.Contains(n.original_name)).Select((n, index) => n.original_index).ToArray();
				var propertyValues = this.classMetadata.GetPropertyValues(this.example, entityMode).Where((value, index) => propertyIndexes.Contains(index)).ToArray();
				var propertyTypes = this.classMetadata.PropertyTypes.Where((value, index) => propertyIndexes.Contains(index)).ToArray();
				var entityType = this.ElementType;
				var newQuery = this.inner;

				for (var i = 0; i < this.propertyNames.Length; ++i)
				{
					if (propertyTypes[i].IsCollectionType == true)
					{
						continue;
					}

					if (propertyTypes[i].IsComponentType == true)
					{
						var componentValue = this.classMetadata.GetPropertyValue(this.example, propertyNames[i], entityMode);

						if (componentValue != null)
						{
							var componentMetadata = (this.classMetadata as AbstractEntityPersister).Factory.GetClassMetadata(propertyTypes[i].ReturnedClass.FullName);

							var componentType = (propertyTypes[i] as ComponentType);
							var componentPropertyNames = componentType.PropertyNames;
							var componentPropertyValues = componentType.GetPropertyValues(componentValue, entityMode);
							var componentPropertyTypes = componentPropertyValues.Select((value, index) => NHibernateUtil.GuessType(componentPropertyValues[index])).ToArray();

							newQuery = this.AppendRestrictionToQuery(newQuery, componentType.ReturnedClass, componentPropertyNames, componentPropertyValues, componentPropertyTypes, propertyNames[i]);
						}

						continue;
					}

					if (this.IsDefaultValue(propertyValues[i], propertyTypes[i].ReturnedClass) == false)
					{
						newQuery = this.AppendRestrictionToQuery(newQuery, entityType, propertyNames, propertyValues, propertyTypes);
					}
				}

				return newQuery.Expression;
			}
		}

		private IQueryable<T> AppendRestrictionToQuery(IQueryable<T> query, System.Type entityType, string[] propertyNames, object[] propertyValues, IType[] propertyTypes, string componentPath = null)
		{
			var newQuery = query;
			var numericTypes = new[] { typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) };

			for (var i = 0; i < propertyNames.Length; ++i)
			{
				if (this.IsDefaultValue(propertyValues[i], propertyTypes[i].ReturnedClass) == false)
				{
					var member = entityType.GetPropertyOrFieldMatchingName(propertyNames[i]);
					var parameter = Expression.Parameter(entityType, "p");
					var prop = Expression.MakeMemberAccess(parameter, member);
					var value = Expression.Constant(propertyValues[i]);
					var equals = Expression.MakeBinary(ExpressionType.Equal, prop, value) as Expression;

					if (propertyValues[i] == null)
					{
						if (this.excludeNulls == true)
						{
							continue;
						}
					}

					if (propertyTypes[i].ReturnedClass == typeof(string))
					{
						var stringValue = propertyValues[i] as string;

						if (this.matchMode != MatchMode.Exact)
						{
							if ((this.matchMode & MatchMode.Anywhere) == MatchMode.Anywhere)
							{
								value = Expression.Constant(String.Concat("%", stringValue, "%"));
							}
							else if ((this.matchMode & MatchMode.Start) == MatchMode.Start)
							{
								value = Expression.Constant(String.Concat(stringValue, "%"));
							}
							else if ((this.matchMode & MatchMode.End) == MatchMode.End)
							{
								value = Expression.Constant(String.Concat("%", stringValue));
							}

							if ((this.matchMode & MatchMode.IgnoreCase) == MatchMode.IgnoreCase)
							{
								value = Expression.Constant(stringValue.ToUpper());
								equals = Expression.MakeBinary(ExpressionType.Equal, Expression.Call(prop, ReflectionHelper.GetMethod<string>(x => x.ToUpper())), value);
							}
							else
							{
								equals = Expression.MakeBinary(ExpressionType.Equal, Expression.Call(null, ReflectionHelper.GetMethod<string>(x => x.Like(null)), prop, value), Expression.Constant(true));
							}
						}
						else
						{
							if (this.nullIsEmptyString == true)
							{
								equals = Expression.Condition(Expression.MakeBinary(ExpressionType.Equal, prop, Expression.Constant(null)), Expression.Constant(string.Empty), value);
							}
						}

					}
					else if (numericTypes.Contains(propertyTypes[i].ReturnedClass) == true)
					{
						if (this.excludeZeroes == true)
						{
							if (Convert.ToInt32(propertyValues[i]) == 0)
							{
								continue;
							}
						}
					}

					var cond = null as Expression<Func<T, bool>>;

					if (entityType == typeof(T))
					{
						cond = Expression.Lambda<Func<T, bool>>(equals, parameter);
					}
					else
					{
						cond = Expression.Lambda<Func<T, bool>>(Expression.MakeMemberAccess(parameter, typeof(T).GetPropertyOrFieldMatchingName(componentPath)), parameter);
					}

					newQuery = newQuery.Where(cond);
				}
			}

			return newQuery;
		}

		public IQueryProvider Provider
		{
			get { return this.inner.Provider; }
		}

		#endregion

		private bool IsDefaultValue(object value, System.Type type)
		{
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
				return value == string.Empty;
			}

			return (type.IsClass == false) ? value == Activator.CreateInstance(type) : false;
		}
	}
}
