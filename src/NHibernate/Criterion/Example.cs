using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	/// <summary>
	/// Support for <c>Query By Example</c>.
	/// </summary>
	/// <example>
	/// <code>
	/// List results = session.CreateCriteria(typeof(Parent))
	/// .Add( Example.Create(parent).IgnoreCase() )
	/// .CreateCriteria("child")
	/// .Add( Example.Create( parent.Child ) )
	/// .List();
	/// </code>
	/// </example>
	/// <remarks>
	/// "Examples" may be mixed and matched with "Expressions" in the same <see cref="ICriteria"/>
	/// </remarks>
	/// <seealso cref="ICriteria"/>
	[Serializable]
	public class Example : AbstractCriterion
	{
		private readonly object _entity;
		private readonly ISet<string> _excludedProperties = new HashedSet<string>();
		private IPropertySelector _selector;
		private bool _isLikeEnabled;
		private char? escapeCharacter;
		private bool _isIgnoreCaseEnabled;
		private MatchMode _matchMode;

		/// <summary>
		/// A strategy for choosing property values for inclusion in the query criteria
		/// </summary>
		public interface IPropertySelector
		{
			/// <summary>
			/// Determine if the Property should be included.
			/// </summary>
			/// <param name="propertyValue">The value of the property that is being checked for inclusion.</param>
			/// <param name="propertyName">The name of the property that is being checked for inclusion.</param>
			/// <param name="type">The <see cref="IType"/> of the property.</param>
			/// <returns>
			/// <see langword="true" /> if the Property should be included in the Query, 
			/// <see langword="false" /> otherwise.
			/// </returns>
			bool Include(object propertyValue, string propertyName, IType type);
		}

		//private static readonly IPropertySelector NotNull = new NotNullPropertySelector();
		protected static readonly IPropertySelector NotNullOrEmptyString = new NotNullOrEmptyStringPropertySelector();
		protected static readonly IPropertySelector All = new AllPropertySelector();
		protected static readonly IPropertySelector NotNullOrZero = new NotNullOrZeroPropertySelector();

		/// <summary>
		/// Implementation of <see cref="IPropertySelector"/> that includes all
		/// properties regardless of value.
		/// </summary>
		[Serializable]
		private class AllPropertySelector : IPropertySelector
		{
			public bool Include(object propertyValue, string propertyName, IType type)
			{
				return true;
			}
		}

		//private class NotNullPropertySelector : IPropertySelector
		//{
		//    public bool Include( object propertyValue, string propertyName, IType type )
		//    {
		//        return propertyValue != null;
		//    }
		//}

		[Serializable]
		private class NotNullOrZeroPropertySelector : IPropertySelector
		{
			private static bool IsZero(object value)
			{
				// Only try to check IConvertibles, to be able to handle various flavors
				// of nullable numbers, etc. Skip strings.
				if (value is IConvertible && !(value is string))
				{
					try
					{
						return Convert.ToInt64(value) == 0L;
					}
					catch (FormatException)
					{
						// Ignore
					}
					catch (InvalidCastException)
					{
						// Ignore
					}
				}

				return false;
			}

			public bool Include(object propertyValue, String propertyName, IType type)
			{
				return propertyValue != null && !IsZero(propertyValue);
			}
		}

		/// <summary>
		/// Implementation of <see cref="IPropertySelector"/> that includes the
		/// properties that are not <see langword="null" /> and do not have an <see cref="String.Empty"/>
		/// returned by <c>propertyValue.ToString()</c>.
		/// </summary>
		/// <remarks>
		/// This selector is not present in H2.1. It may be useful if nullable types
		/// are used for some properties.
		/// </remarks>
		[Serializable]
		private class NotNullOrEmptyStringPropertySelector : IPropertySelector
		{
			public bool Include(object propertyValue, String propertyName, IType type)
			{
				if (propertyValue == null) return false;
				return propertyValue.ToString().Length > 0;
			}
		}

		/// <summary> Set escape character for "like" clause</summary>
		public virtual Example SetEscapeCharacter(char? escapeCharacter)
		{
			this.escapeCharacter = escapeCharacter;
			return this;
		}

		/// <summary>
		/// Set the <see cref="IPropertySelector"/> for this <see cref="Example"/>.
		/// </summary>
		/// <param name="selector">The <see cref="IPropertySelector"/> to determine which properties to include.</param>
		/// <returns>This <see cref="Example"/> instance.</returns>
		/// <remarks>
		/// This should be used when a custom <see cref="IPropertySelector"/> has
		/// been implemented.  Otherwise use the methods <see cref="Example.ExcludeNulls"/> 
		/// or <see cref="Example.ExcludeNone"/> to set the <see cref="IPropertySelector"/>
		/// to the <see cref="IPropertySelector"/>s built into NHibernate.
		/// </remarks>
		public Example SetPropertySelector(IPropertySelector selector)
		{
			_selector = selector;
			return this;
		}

		/// <summary>
		/// Set the <see cref="IPropertySelector"/> for this <see cref="Example"/>
		/// to exclude zero-valued properties.
		/// </summary>
		public Example ExcludeZeroes()
		{
			return SetPropertySelector(NotNullOrZero);
		}

		/// <summary>
		/// Set the <see cref="IPropertySelector"/> for this <see cref="Example"/>
		/// to exclude no properties.
		/// </summary>
		public Example ExcludeNone()
		{
			SetPropertySelector(All);
			return this;
		}

		public Example ExcludeNulls()
		{
			SetPropertySelector(NotNullOrEmptyString);
			return this;
		}

		/// <summary>
		/// Use the "like" operator for all string-valued properties with
		/// the specified <see cref="MatchMode"/>.
		/// </summary>
		/// <param name="matchMode">
		/// The <see cref="MatchMode"/> to convert the string to the pattern
		/// for the <c>like</c> comparison.
		/// </param>
		public Example EnableLike(MatchMode matchMode)
		{
			_isLikeEnabled = true;
			_matchMode = matchMode;
			return this;
		}

		/// <summary>
		/// Use the "like" operator for all string-valued properties.
		/// </summary>
		/// <remarks>
		/// The default <see cref="MatchMode"/> is <see cref="MatchMode.Exact">MatchMode.Exact</see>.
		/// </remarks>
		public Example EnableLike()
		{
			return EnableLike(MatchMode.Exact);
		}

		public Example IgnoreCase()
		{
			_isIgnoreCaseEnabled = true;
			return this;
		}

		/// <summary>
		/// Exclude a particular named property
		/// </summary>
		/// <param name="name">The name of the property to exclude.</param>
		public Example ExcludeProperty(String name)
		{
			_excludedProperties.Add(name);
			return this;
		}

		/// <summary>
		/// Create a new instance, which includes all non-null properties 
		/// by default
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>A new instance of <see cref="Example" />.</returns>
		public static Example Create(object entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity", "null example");

			return new Example(entity, NotNullOrEmptyString);
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="Example" /> class for a particular
		/// entity.
		/// </summary>
		/// <param name="entity">The <see cref="Object"/> that the Example is being built from.</param>
		/// <param name="selector">The <see cref="IPropertySelector"/> the Example should use.</param>
		protected Example(object entity, IPropertySelector selector)
		{
			_entity = entity;
			_selector = selector;
		}

		public override String ToString()
		{
			return "example (" + _entity + ')';
		}

		/// <summary>
		/// Determines if the property should be included in the Query.
		/// </summary>
		/// <param name="value">The value of the property.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The <see cref="IType"/> of the property.</param>
		/// <returns>
		/// <see langword="true" /> if the Property should be included, <see langword="false" /> if
		/// the Property should not be a part of the Query.
		/// </returns>
		private bool IsPropertyIncluded(object value, String name, IType type)
		{
			return !_excludedProperties.Contains(name) &&
			       !type.IsAssociationType &&
			       _selector.Include(value, name, type);
		}

		private object[] GetPropertyValues(IEntityPersister persister, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			System.Type type = _entity.GetType();
			if(type == persister.GetMappedClass(GetEntityMode(criteria, criteriaQuery))) //not using anon object
			{
				return persister.GetPropertyValues(_entity, GetEntityMode(criteria, criteriaQuery));
			}
			ArrayList list = new ArrayList();
			for(int i = 0; i < persister.PropertyNames.Length; i++)
			{
				PropertyInfo pInfo = type.GetProperty(persister.PropertyNames[i]);
				if(pInfo != null)
				{
					list.Add(pInfo.GetValue(_entity, null));
				}
				else
				{
					list.Add(null); //to maintain same order as PropertyNames list
					_excludedProperties.Add(persister.PropertyNames[i]); //exclude the properties that aren't in the anon object (duplicates ok)
				}
			}
			return list.ToArray();
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			SqlStringBuilder builder = new SqlStringBuilder();
			builder.Add(StringHelper.OpenParen);

			IEntityPersister meta = criteriaQuery.Factory.GetEntityPersister(criteriaQuery.GetEntityName(criteria));
			String[] propertyNames = meta.PropertyNames;
			IType[] propertyTypes = meta.PropertyTypes;
			object[] propertyValues = GetPropertyValues(meta, criteria, criteriaQuery);
			for (int i = 0; i < propertyNames.Length; i++)
			{
				object propertyValue = propertyValues[i];
				String propertyName = propertyNames[i];

				bool isPropertyIncluded = i != meta.VersionProperty &&
				                          IsPropertyIncluded(propertyValue, propertyName, propertyTypes[i]);
				if (isPropertyIncluded)
				{
					if (propertyTypes[i].IsComponentType)
					{
						AppendComponentCondition(
							propertyName,
							propertyValue,
							(IAbstractComponentType) propertyTypes[i],
							criteria,
							criteriaQuery,
							enabledFilters,
							builder
							);
					}
					else
					{
						AppendPropertyCondition(
							propertyName,
							propertyValue,
							criteria,
							criteriaQuery,
							enabledFilters,
							builder
							);
					}
				}
			}
			if (builder.Count == 1)
			{
				builder.Add("1=1"); // yuck!
			}

			builder.Add(StringHelper.ClosedParen);
			return builder.ToSqlString();
		}


		//note: now that Criterion are adding typed values via ICriteriaQuery.AddUsedTypedValues this function is never called.
		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			IEntityPersister meta = criteriaQuery.Factory.GetEntityPersister(criteriaQuery.GetEntityName(criteria));
			string[] propertyNames = meta.PropertyNames;
			IType[] propertyTypes = meta.PropertyTypes;
			object[] values = GetPropertyValues(meta, criteria, criteriaQuery);

			ArrayList list = new ArrayList();
			for (int i = 0; i < propertyNames.Length; i++)
			{
				object value = values[i];
				IType type = propertyTypes[i];
				string name = propertyNames[i];

				bool isPropertyIncluded = (i != meta.VersionProperty && IsPropertyIncluded(value, name, type));

				if (isPropertyIncluded)
				{
					if (propertyTypes[i].IsComponentType)
					{
						AddComponentTypedValues(name, value, (IAbstractComponentType) type, list, criteria, criteriaQuery);
					}
					else
					{
						AddPropertyTypedValue(value, type, list);
					}
				}
			}

			return (TypedValue[]) list.ToArray(typeof(TypedValue));
		}

		public override IProjection[] GetProjections()
		{
			return null;
		}

		private EntityMode GetEntityMode(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			IEntityPersister meta = criteriaQuery.Factory.GetEntityPersister(criteriaQuery.GetEntityName(criteria));
			EntityMode? result = meta.GuessEntityMode(_entity);
			if (!result.HasValue)
			{
				return EntityMode.Poco; //this occurs for anon objects
				//throw new InvalidCastException(_entity.GetType().FullName);
			}
			return result.Value;
		}

		/// <summary>
		/// Adds a <see cref="TypedValue"/> based on the <c>value</c> 
		/// and <c>type</c> parameters to the <see cref="IList"/> in the
		/// <c>list</c> parameter.
		/// </summary>
		/// <param name="value">The value of the Property.</param>
		/// <param name="type">The <see cref="IType"/> of the Property.</param>
		/// <param name="list">The <see cref="IList"/> to add the <see cref="TypedValue"/> to.</param>
		/// <remarks>
		/// This method will add <see cref="TypedValue"/> objects to the <c>list</c> parameter.
		/// </remarks>
		protected void AddPropertyTypedValue(object value, IType type, IList list)
		{
			// TODO: I don't like this at all - why don't we have it return a TypedValue[]
			// or an ICollection that can be added to the list instead of modifying the
			// parameter passed in.
			if (value != null)
			{
				var stringValue = value as string;
				if (stringValue != null)
				{
					if (_isIgnoreCaseEnabled)
					{
						stringValue = stringValue.ToLower();
					}
					if (_isLikeEnabled)
					{
						stringValue = _matchMode.ToMatchString(stringValue);
					}
					value = stringValue;
				}
				list.Add(new TypedValue(type, value, EntityMode.Poco));
					// TODO NH Different behavior: In H3.2 EntityMode is nullable
			}
		}

		protected void AddComponentTypedValues(string path, object component, IAbstractComponentType type, IList list, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (component != null)
			{
				string[] propertyNames = type.PropertyNames;
				IType[] subtypes = type.Subtypes;
				object[] values = type.GetPropertyValues(component, GetEntityMode(criteria, criteriaQuery));

				for (int i = 0; i < propertyNames.Length; i++)
				{
					object value = values[i];
					IType subtype = subtypes[i];
					string subpath = StringHelper.Qualify(path, propertyNames[i]);
					if (IsPropertyIncluded(value, subpath, subtype))
					{
						if (subtype.IsComponentType)
						{
							AddComponentTypedValues(subpath, value, (IAbstractComponentType) subtype, list, criteria, criteriaQuery);
						}
						else
						{
							AddPropertyTypedValue(value, subtype, list);
						}
					}
				}
			}
		}

		protected void AppendPropertyCondition(
			String propertyName,
			object propertyValue,
			ICriteria criteria,
			ICriteriaQuery cq,
			IDictionary<string, IFilter> enabledFilters,
			SqlStringBuilder builder)
		{
			if (builder.Count > 1)
			{
				builder.Add(" and ");
			}

			ICriterion crit = propertyValue != null
			                  	? GetNotNullPropertyCriterion(propertyValue, propertyName)
			                  	: new NullExpression(propertyName);
			builder.Add(crit.ToSqlString(criteria, cq, enabledFilters));
		}

		protected virtual ICriterion GetNotNullPropertyCriterion(object propertyValue, string propertyName)
		{
			bool isString = propertyValue is string;
			return (_isLikeEnabled && isString)
			       	? (ICriterion)
			       	  new LikeExpression(propertyName, propertyValue.ToString(), _matchMode, escapeCharacter,
			       	                     _isIgnoreCaseEnabled)
			       	: new SimpleExpression(propertyName, propertyValue, " = ", _isIgnoreCaseEnabled && isString);
		}

		protected void AppendComponentCondition(
			String path,
			object component,
			IAbstractComponentType type,
			ICriteria criteria,
			ICriteriaQuery criteriaQuery,
			IDictionary<string, IFilter> enabledFilters,
			SqlStringBuilder builder)
		{
			if (component != null)
			{
				String[] propertyNames = type.PropertyNames;
				object[] values = type.GetPropertyValues(component, GetEntityMode(criteria, criteriaQuery));
				IType[] subtypes = type.Subtypes;
				for (int i = 0; i < propertyNames.Length; i++)
				{
					String subpath = StringHelper.Qualify(path, propertyNames[i]);
					object value = values[i];
					if (IsPropertyIncluded(value, subpath, subtypes[i]))
					{
						IType subtype = subtypes[i];
						if (subtype.IsComponentType)
						{
							AppendComponentCondition(
								subpath,
								value,
								(IAbstractComponentType) subtype,
								criteria,
								criteriaQuery,
								enabledFilters,
								builder);
						}
						else
						{
							AppendPropertyCondition(
								subpath,
								value,
								criteria,
								criteriaQuery,
								enabledFilters,
								builder
								);
						}
					}
				}
			}
		}
	}
}
