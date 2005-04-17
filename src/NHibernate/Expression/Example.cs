using System;
using System.Collections;

using Iesi.Collections;

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
	public class Example : AbstractCriterion
	{
		private object _entity;
		private ISet _excludedProperties = new HashedSet();
		private IPropertySelector _selector;
		private bool _isLikeEnabled;
		private MatchMode _matchMode;
		
		/// <summary>
		/// A strategy for choosing property values for inclusion in the query criteria
		/// </summary>
		public interface IPropertySelector 
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
	
		private static readonly IPropertySelector NotNullOrEmptyString = new NotNullOrEmptyStringPropertySelector();
		private static readonly IPropertySelector All = new AllPropertySelector();
		private static readonly IPropertySelector NotNullOrZero = new NotNullOrZeroPropertySelector();
		
		private class AllPropertySelector : IPropertySelector
		{
			public bool Include(object propertyValue, String propertyName, IType type)
			{
				return true;
			}
		}
		
		internal class NotNullOrEmptyStringPropertySelector : IPropertySelector
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
		internal class NotNullOrZeroPropertySelector : IPropertySelector 
		{
			public bool Include(object propertyValue, String propertyName, IType type) 
			{ 
				return propertyValue!=null ;/*&& (!char.IsNumber(propertyValue as char) || ( (int) propertyValue !=0));*/
			}
		}
	
		/// <summary>
		/// Set the property selector
		/// </summary>
		public Example SetPropertySelector(IPropertySelector selector) 
		{
			this._selector = selector;
			return this;
		}
	
		
		/// <summary>
		/// Exclude zero-valued properties
		/// </summary>
		public Example ExcludeNulls() 
		{
			SetPropertySelector(NotNullOrEmptyString);
			return this;
		}
	
		/// <summary>
		/// Don't exclude null or zero-valued properties
		/// </summary>
		public Example ExcludeNone() 
		{
			SetPropertySelector(All);
			return this;
		}
	
		/// <summary>
		/// Use the "like" operator for all string-valued properties
		/// </summary>
		public Example EnableLike(MatchMode matchMode) 
		{
			_isLikeEnabled = true;
			this._matchMode = matchMode;
			return this;
		}
		
		/// <summary>
		/// Use the "like" operator for all string-valued properties
		/// </summary>
		public Example EnableLike() 
		{
			return EnableLike(MatchMode.Exact);
		}

		/// <summary>
		/// Exclude a particular named property
		/// </summary>
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
		/// <returns>a new instance of <code>Example</code></returns>
		public static Example Create(object entity) 
		{
			if (entity==null) throw new ArgumentException("null example");
			return new Example(entity, NotNullOrEmptyString);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="selector"></param>
		protected Example(object entity, IPropertySelector selector) 
		{
			_entity = entity;
			_selector = selector;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override String ToString() 
		{
			return _entity.ToString();
		}
	
		private bool IsPropertyIncluded(object value, String name, IType type) 
		{
			return !_excludedProperties.Contains(name) &&
				!type.IsAssociationType &&
				_selector.Include(value, name, type);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses)
		{
			IClassMetadata meta = sessionFactory.GetClassMetadata( persistentClass );
			string[] propertyNames = meta.PropertyNames;
			IType[] propertyTypes = meta.PropertyTypes;
			object[] values = meta.GetPropertyValues( _entity );

			ArrayList list = new ArrayList();
			for( int i=0; i<propertyNames.Length; i++)
			{
				object value = values[ i ];
				IType type = propertyTypes[ i ];
				string name = propertyNames[ i ];

				bool isPropertyIncluded = ( i!=meta.VersionProperty && IsPropertyIncluded( value, name, type ) );

				if( isPropertyIncluded )
				{
					if( propertyTypes[ i ].IsComponentType )
					{
						AddComponentTypedValues( name, value, (IAbstractComponentType)type, list ); 
					}
					else
					{
						AddPropertyTypedValue( value, type, list );
					}
				}
			}

			return ( TypedValue[ ] ) list.ToArray( typeof( TypedValue ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public override SqlString ToSqlString(ISessionFactoryImplementor factory, System.Type persistentClass, string alias, IDictionary aliasClasses)
		{
			SqlStringBuilder builder = new SqlStringBuilder();
			builder.Add( StringHelper.OpenParen );

			IClassMetadata meta = factory.GetClassMetadata(persistentClass);
			String[] propertyNames = meta.PropertyNames;
			IType[] propertyTypes = meta.PropertyTypes;
			object[] propertyValues = meta.GetPropertyValues(_entity);
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
							aliasClasses,
							factory,
							builder
							);
					}
					else 
					{
						AppendPropertyCondition(
							propertyName, 
							propertyValue, 
							persistentClass,
							alias,
							aliasClasses,
							factory,
							builder
							);
					}
				}
			}
			if( builder.Count==1 )
			{
				builder.Add( "1=1" ); // yuck!
			}

			builder.Add( StringHelper.ClosedParen );
			return builder.ToSqlString();
		}
	
		/// <summary>
		/// Adds a <see cref="TypedValue"/> based on the <c>value</c> 
		/// and <c>type</c> parameters to the <see cref="IList"/> in the
		/// <c>list</c> parameter.
		/// </summary>
		/// <param name="value">The value of the Property.</param>
		/// <param name="type">The <see cref="IType"/> of the Property.</param>
		/// <param name="list">The <see cref="IList"/> to add the <see cref="TypedValue"/> to.</param>
		protected void AddPropertyTypedValue(object value, IType type, IList list)
		{
			// TODO: I don't like this at all - why don't we have it return a TypedValue[]
			// or an ICollection that can be added to the list instead of modifying the
			// parameter passed in.
			if( value!=null )
			{
				// TODO: h2.1 SYNCH: some code in here to check for 
				// IsIgnoreCaseEnabled and IsLikeEnabled
//				string stringValue = value as string;
//				if( stringValue!=null )
//				{
//					// 
//					// 
//				}

				list.Add( new TypedValue( type, value ) );
			}
		}

		protected void AddComponentTypedValues(string path, object component, IAbstractComponentType type, IList list)
		{
			//TODO: h2.1 SYNCH: resume here once IAbstractComponentType is fixed up.
			if( component!=null )
			{
				string[] propertyNames = type.PropertyNames;
				IType[] subtypes = type.Subtypes;
				object[] values = type.GetPropertyValues( component );
				for( int i=0; i<propertyNames.Length; i++ )
				{
					object value = values[i];
					IType subtype = subtypes[i];
					string subpath = StringHelper.Qualify( path, propertyNames[i] );
					if( IsPropertyIncluded( value, subpath, subtype ) )
					{
						if( subtype.IsComponentType )
						{
							AddComponentTypedValues( subpath, value, (IAbstractComponentType)subtype, list );
						}
						else
						{
							AddPropertyTypedValue( value, subtype, list );
						}
					}

				}
			}
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
			IDictionary aliasClasses,
			ISessionFactoryImplementor sessionFactory,
			SqlStringBuilder builder) 
		{
			if( builder.Count>1 )
			{
				builder.Add( " and " );
			}

			ICriterion crit;
			if ( propertyValue!=null ) 
			{
				bool isString = propertyValue is String;
				crit = ( _isLikeEnabled && isString ) ?
					(ICriterion) new LikeExpression( propertyName, propertyValue) :
					(ICriterion) new EqExpression( propertyName, propertyValue );
					
			}
			else 
			{
				crit = new NullExpression(propertyName);
			}
			builder.Add( crit.ToSqlString( sessionFactory, persistentClass, alias, aliasClasses ) );
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
			IDictionary aliasClasses,
			ISessionFactoryImplementor sessionFactory,
			SqlStringBuilder builder) 
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
								aliasClasses,
								sessionFactory,
								builder );
						} 
						else 
						{
							AppendPropertyCondition( 
								subpath,
								value, 
								persistentClass,
								alias,
                                aliasClasses,
								sessionFactory,
								builder
								);
						}
					}
				}
			}
		}
	}
}
