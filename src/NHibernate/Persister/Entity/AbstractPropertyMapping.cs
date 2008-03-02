using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Hql.Classic;
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

		public virtual string[] IdentifierColumnNames
		{
			get { throw new InvalidOperationException("one-to-one is not supported here"); }
		}

		protected abstract string EntityName { get; }

		protected QueryException PropertyException(String propertyName)
		{
			return new QueryException(string.Format("could not resolve property: {0} of: {1}", propertyName, EntityName));
		}

		#region IPropertyMapping Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public IType ToType(string propertyName)
		{
			IType type = typesByPropertyPath[propertyName] as IType;
			if (type == null)
			{
				throw PropertyException(propertyName);
			}
			return type;
		}

		public virtual string[] ToColumns(string alias, string propertyName)
		{
			//TODO: *two* hashmap lookups here is one too many...
			string[] columns = (string[]) columnsByPropertyPath[propertyName];
			if (columns == null)
			{
				throw PropertyException(propertyName);
			}

			string[] templates = (string[]) formulaTemplatesByPropertyPath[propertyName];
			string[] result = new string[columns.Length];
			for (int i = 0; i < columns.Length; i++)
			{
				if (columns[i] == null)
					result[i] = StringHelper.Replace(templates[i], Template.Placeholder, alias);
				else
					result[i] = StringHelper.Qualify(alias, columns[i]);
			}
			return result;
		}

		public virtual string[] ToColumns(string propertyName)
		{
			string[] columns = (string[])columnsByPropertyPath[propertyName];
			if (columns == null)
				throw PropertyException(propertyName);

			string[] templates = (string[])formulaTemplatesByPropertyPath[propertyName];
			string[] result = new string[columns.Length];
			for (int i = 0; i < columns.Length; i++)
			{
				if (columns[i] == null)
					result[i] = StringHelper.Replace(templates[i], Template.Placeholder, string.Empty);
				else
					result[i] = columns[i];
			}
			return result;
		}

		public abstract IType Type { get; }

		#endregion

		protected void AddPropertyPath(string path, IType type, string[] columns, string[] formulaTemplates)
		{
			typesByPropertyPath[path] = type;
			columnsByPropertyPath[path] = columns;

			if (formulaTemplates != null)
			{
				formulaTemplatesByPropertyPath[path] = formulaTemplates;
			}
			HandlePath(path, type);
		}

		protected internal void InitPropertyPaths( string path, IType type, string[] columns, string[] formulaTemplates, IMapping factory )
		{
			if (columns.Length != type.GetColumnSpan(factory))
			{
				throw new MappingException(
					string.Format("broken column mapping for: {0} of: {1}, type {2} expects {3} columns, but {4} were mapped",
					              path, EntityName, type.Name, type.GetColumnSpan(factory), columns.Length));
			}

			if (type.IsAssociationType)
			{
				IAssociationType actType = (IAssociationType)type;
				if (actType.UseLHSPrimaryKey)
				{
					columns = IdentifierColumnNames;
				}
				else
				{
					string foreignKeyProperty = actType.LHSPropertyName;
					if (foreignKeyProperty != null)
					{
						columns = (string[])columnsByPropertyPath[foreignKeyProperty];
					}
				}
			}

			if (path != null)
			{
				AddPropertyPath(path, type, columns, formulaTemplates);
			}

			if (type.IsComponentType)
			{
				InitComponentPropertyPaths(path, (IAbstractComponentType) type, columns, formulaTemplates, factory);
			}
			else if (type.IsEntityType)
			{
				InitIdentifierPropertyPaths(path, (EntityType) type, columns, factory);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="etype"></param>
		/// <param name="columns"></param>
		/// <param name="factory"></param>
		protected void InitIdentifierPropertyPaths(string path, EntityType etype, string[] columns, IMapping factory)
		{
			IType idtype = etype.GetIdentifierOrUniqueKeyType(factory);

			if (!etype.IsUniqueKeyReference)
			{
				string idpath1 = ExtendPath(path, PathExpressionParser.EntityID);
				AddPropertyPath(idpath1, idtype, columns, null);
				InitPropertyPaths(idpath1, idtype, columns, null, factory);
			}

			string idPropName = etype.GetIdentifierOrUniqueKeyPropertyName(factory);
			if (idPropName != null)
			{
				string idpath2 = ExtendPath(path, idPropName);
				AddPropertyPath(idpath2, idtype, columns, null);
				InitPropertyPaths(idpath2, idtype, columns, null, factory);
			}
		}

		protected void InitComponentPropertyPaths(string path, IAbstractComponentType type, string[] columns,
		                                          string[] formulaTemplates, IMapping factory)
		{
			IType[] types = type.Subtypes;
			string[] properties = type.PropertyNames;
			int begin = 0;
			for (int i = 0; i < properties.Length; i++)
			{
				string subpath = ExtendPath(path, properties[i]);
				try
				{
					int length = types[i].GetColumnSpan(factory);
					string[] columnSlice = ArrayHelper.Slice(columns, begin, length);
					string[] formulaSlice = formulaTemplates == null ?
					                        null : ArrayHelper.Slice(formulaTemplates, begin, length);

					InitPropertyPaths(subpath, types[i], columnSlice, formulaSlice, factory);
					begin += length;
				}
				catch (Exception e)
				{
					throw new MappingException("bug in InitComponentPropertyPaths", e);
				}
			}
		}

		private static string ExtendPath(string path, string property)
		{
			if (path == null)
			{
				return property;
			}
			else
			{
				return StringHelper.Qualify(path, property);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="type"></param>
		protected virtual void HandlePath(string path, IType type)
		{
		}

		public string[] GetColumnNames(string propertyName)
		{
			string[] cols = (string[]) columnsByPropertyPath[propertyName];
			if (cols == null)
			{
				throw new MappingException("unknown property: " + propertyName);
			}
			return cols;
		}
	}
}
