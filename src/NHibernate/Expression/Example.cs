using System;
using System.Collections;
using System.Text;
using Iesi.Collections;
using NHibernate;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Expression
{
	/// <summary>
	/// Summary description for Example.
	/// </summary>
	public class Example : Expression
	{
		private object entity;
		private ISet excludedProperties = new HashedSet();
		private PropertySelector selector;
		private bool isLikeEnabled;
		private MatchMode matchMode;
		private Conjunction expressions = new Conjunction();
		
		/// <summary>
		/// A strategy for choosing property values for inclusion in the query criteria
		/// </summary>
		public interface PropertySelector 
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="propertyValue"></param>
			/// <param name="propertyName"></param>
			/// <param name="type"></param>
			/// <returns></returns>
			bool Include(object propertyValue, String propertyName, IType type);
		}
	
		private static readonly PropertySelector NOT_NULL_OR_EMPTY_STRING = new NotNullOrEmptyStringPropertySelector();
		private static readonly PropertySelector ALL = new AllPropertySelector();
		private static readonly PropertySelector NOT_NULL_OR_ZERO = new NotNullOrZeroPropertySelector();
		
		private class AllPropertySelector : PropertySelector
		{
			public bool Include(object propertyValue, String propertyName, IType type)
			{
				return true;
			}
		}
		
		internal class NotNullOrEmptyStringPropertySelector : PropertySelector
		{
			
			public bool Include(object propertyValue, String propertyName, IType type)
			{
				if(propertyValue != null)
				{
					if(propertyValue.ToString() != "")
						return true;
					else
						return false;
				}
				else
					return false;

			}

		}

	
		/// <summary>
		/// This Can't work right now.  Have to figure out some way to use Magic values
		/// to avoid the non-nullable types in .NET.
		/// </summary>
		internal class NotNullOrZeroPropertySelector : PropertySelector 
		{
			public bool Include(object propertyValue, String propertyName, IType type) 
			{ 
				return propertyValue!=null ;/*&& (!char.IsNumber(propertyValue as char) || ( (int) propertyValue !=0));*/
			}
		}
	
		/// <summary>
		/// Set the property selector
		/// </summary>
		public Example SetPropertySelector(PropertySelector selector) 
		{
			this.selector = selector;
			return this;
		}
	
		
		/// <summary>
		/// Exclude zero-valued properties
		/// </summary>
		public Example ExcludeNulls() 
		{
			SetPropertySelector(NOT_NULL_OR_EMPTY_STRING);
			return this;
		}
	
		/// <summary>
		/// Don't exclude null or zero-valued properties
		/// </summary>
		public Example ExcludeNone() 
		{
			SetPropertySelector(ALL);
			return this;
		}
	
		/// <summary>
		/// Use the "like" operator for all string-valued properties
		/// </summary>
		public Example EnableLike(MatchMode matchMode) 
		{
			isLikeEnabled = true;
			this.matchMode = matchMode;
			return this;
		}
		
		/// <summary>
		/// Use the "like" operator for all string-valued properties
		/// </summary>
		public Example EnableLike() 
		{
			return EnableLike(MatchMode.EXACT);
		}

		/// <summary>
		/// Exclude a particular named property
		/// </summary>
		public Example ExcludeProperty(String name) 
		{
			excludedProperties.Add(name);
			return this;
		}

		/// <summary>
		/// Create a new instance, which includes all non-null properties 
		/// by default
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>a new instance of <code>Example</code></returns>
	
		public static Example create(object entity) 
		{
			if (entity==null) throw new ArgumentException("null example");
			return new Example(entity, NOT_NULL_OR_EMPTY_STRING);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="selector"></param>
		protected Example(object entity, PropertySelector selector) 
		{
			this.entity = entity;
			this.selector = selector;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override String ToString() 
		{
			return entity.ToString();
		}
	
		private bool IsPropertyIncluded(object value, String name, IType type) 
		{
			return !excludedProperties.Contains(name) &&
				!type.IsAssociationType &&
				selector.Include(value, name, type);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass)
		{
			
			return expressions.GetTypedValues(sessionFactory, persistentClass);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public override SqlString ToSqlString(ISessionFactoryImplementor factory, System.Type persistentClass, string alias)
		{
			IClassMetadata meta = factory.GetClassMetadata(persistentClass);
			String[] propertyNames = meta.PropertyNames;
			IType[] propertyTypes = meta.PropertyTypes;
			object[] propertyValues = meta.GetPropertyValues(entity);
			for (int i=0; i<propertyNames.Length; i++) 
			{
				object propertyValue = propertyValues[i];
				String propertyName = propertyNames[i];
			
				// Have to figure out how to get the name or index of the version property
				bool isPropertyIncluded = i!=meta.VersionProperty &&
					IsPropertyIncluded( propertyValue, propertyName, propertyTypes[i] );
				if (isPropertyIncluded) 
				{
					if ( propertyTypes[i].IsComponentType) 
					{
						AppendComponentCondition(
							propertyName, 
							propertyValue, 
							(IAbstractComponentType) propertyTypes[i], 
							persistentClass,
							alias,
							factory
							);
					}
					else 
					{
						AppendPropertyCondition(
							propertyName, 
							propertyValue, 
							persistentClass,
							alias,
							factory
							);
					}
				}
			}
			return expressions.ToSqlString(factory, persistentClass, alias);
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propertyValue"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <param name="sessionFactory"></param>
		protected void AppendPropertyCondition(
			String propertyName, 
			object propertyValue, 
			System.Type persistentClass,
			String alias,
			ISessionFactoryImplementor sessionFactory) 
		{
			Expression crit;
			if ( propertyValue!=null ) 
			{
				bool isString = propertyValue is String;
				crit = ( isLikeEnabled && isString ) ?
					(Expression) new LikeExpression( propertyName, propertyValue) :
					(Expression) new EqExpression( propertyName, propertyValue );
					
			}
			else 
			{
				crit = new NullExpression(propertyName);
			}
			expressions.Add(crit);
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="component"></param>
		/// <param name="type"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <param name="sessionFactory"></param>
		protected void AppendComponentCondition(
			String path, 
			object component, 
			IAbstractComponentType type, 
			System.Type persistentClass,
			String alias,
			ISessionFactoryImplementor sessionFactory) 
		{
		
			if (component!=null) 
			{
				String[] propertyNames = type.PropertyNames;
				object[] values = type.GetPropertyValues(component, null);
				IType[] subtypes = type.Subtypes;
				for (int i=0; i<propertyNames.Length; i++) 
				{
					String subpath = StringHelper.Qualify( path, propertyNames[i] );
					object value = values[i];
					if ( IsPropertyIncluded( value, subpath, subtypes[i] ) ) 
					{
						IType subtype = subtypes[i];
						if ( subtype.IsComponentType ) 
						{
							AppendComponentCondition(
								subpath,
								value,
								(IAbstractComponentType) subtype,
								persistentClass,
								alias,
								sessionFactory
								);
						} 
						else 
						{
							AppendPropertyCondition( 
								subpath,
								value, 
								persistentClass,
								alias,
								sessionFactory
								);
						}
					}
				}
			}
		}
	}
}
