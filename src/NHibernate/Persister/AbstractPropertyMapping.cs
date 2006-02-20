using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Base implementation of a PropertyMapping.
	/// </summary>
	public abstract class AbstractPropertyMapping : IPropertyMapping
	{
		private readonly Hashtable typesByPropertyPath = new Hashtable();
		private readonly Hashtable columnsByPropertyPath = new Hashtable();
		private readonly Hashtable formulaTemplatesByPropertyPath = new Hashtable();

		/// <summary>
		/// 
		/// </summary>
		public virtual string[] IdentifierColumnNames
		{
			get { throw new InvalidOperationException( "one-to-one is not supported here" ); }
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract string ClassName { get; }

		#region IPropertyMapping Members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public IType ToType(string propertyName)
		{
			IType type = typesByPropertyPath[ propertyName ] as IType;
			if ( type==null )
			{
				throw new QueryException( string.Format( "could not resolve property:{0} of :{1}", propertyName, ClassName ) );
			}
			return type;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public virtual string[] ToColumns( string alias, string propertyName )
		{
			string[] columns = columnsByPropertyPath[ propertyName ] as string[];
			if ( columns==null )
			{
				string template = formulaTemplatesByPropertyPath[ propertyName ] as string;
				if ( template==null )
				{
					throw new QueryException( string.Format( "could not resolve property:{0} of :{1}", propertyName, ClassName ) );
				}
				else
				{
					return new string[] { StringHelper.Replace( template, Template.Placeholder, alias ) } ;
				}
			}
			return StringHelper.Qualify( alias, columns );
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract IType Type { get; }
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="type"></param>
		/// <param name="columns"></param>
		protected void AddPropertyPath( string path, IType type, string[] columns )
		{
			typesByPropertyPath[ path ] = type;
			columnsByPropertyPath[ path ] = columns;
			HandlePath( path, type );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="type"></param>
		/// <param name="template"></param>
		protected void AddFormulaPropertyPath( string path, IType type, string template )
		{
			typesByPropertyPath[ path ] = type ;
			formulaTemplatesByPropertyPath[ path ] = template ;
			HandlePath( path, type );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="type"></param>
		/// <param name="columns"></param>
		/// <param name="formulaTemplate"></param>
		/// <param name="factory"></param>
		protected void InitPropertyPaths( string path, IType type, string[] columns, string formulaTemplate, ISessionFactoryImplementor factory )
		{
			if ( formulaTemplate != null )
			{
				AddFormulaPropertyPath( path, type, formulaTemplate );
			}
			else
			{
				InitPropertyPaths( path, type, columns, factory );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="type"></param>
		/// <param name="columns"></param>
		/// <param name="factory"></param>
		protected void InitPropertyPaths( string path, IType type, string[] columns, ISessionFactoryImplementor factory )
		{
			if(columns.Length != type.GetColumnSpan( factory ) )
			{
				throw new MappingException(
					string.Format( "broken column mapping for: {0} of: {1}, type {2} expects {3} columns, but {4} were mapped",
					path, ClassName, type.Name, type.GetColumnSpan( factory ), columns.Length ) );
			}

			if( type.IsAssociationType && ((IAssociationType) type).UseLHSPrimaryKey )
			{
				columns = IdentifierColumnNames;
			}

			if( path!=null )
			{
				AddPropertyPath( path, type, columns );
			}

			if( type.IsComponentType )
			{
				InitComponentPropertyPaths( path, (IAbstractComponentType) type, columns, factory );
			}
			else if( type.IsEntityType )
			{
				InitIdentifierPropertyPaths( path, (EntityType) type, columns, factory );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="etype"></param>
		/// <param name="columns"></param>
		/// <param name="factory"></param>
		protected void InitIdentifierPropertyPaths( string path, EntityType etype, string[] columns, ISessionFactoryImplementor factory )
		{
			IType idtype = etype.GetIdentifierOrUniqueKeyType( factory );

			if ( !etype.IsUniqueKeyReference )
			{
				string idpath1 = ExtendPath( path, PathExpressionParser.EntityID );
				AddPropertyPath( idpath1, idtype, columns );
				InitPropertyPaths( idpath1, idtype, columns, factory );
			}

			string idPropName = etype.GetIdentifierOrUniqueKeyPropertyName( factory );
			if ( idPropName!=null )
			{
				string idpath2 = ExtendPath( path, idPropName );
				AddPropertyPath( idpath2, idtype, columns );
				InitPropertyPaths( idpath2, idtype, columns, factory );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="type"></param>
		/// <param name="columns"></param>
		/// <param name="factory"></param>
		protected void InitComponentPropertyPaths( string path, IAbstractComponentType type, string[] columns, ISessionFactoryImplementor factory )
		{
			IType[] types = type.Subtypes;
			string[] properties = type.PropertyNames;
			int begin = 0;
			for ( int i=0; i<properties.Length; i++ )
			{
				string subpath = ExtendPath( path, properties[ i ] );
				try
				{
					int length = types[ i ].GetColumnSpan( factory );
					string[] columnSlice = ArrayHelper.Slice( columns, begin, length );
					InitPropertyPaths( subpath, types[ i ], columnSlice, factory );
					begin += length;
				}
				catch ( Exception e )
				{
					throw new MappingException( "bug in InitComponentPropertyPaths", e );
				}
			}
		}

		private static string ExtendPath( string path, string property )
		{
			if ( path==null )
			{
				return property;
			}
			else
			{
				return StringHelper.Qualify( path, property );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="type"></param>
		protected virtual void HandlePath( string path, IType type) {}
	}
}
