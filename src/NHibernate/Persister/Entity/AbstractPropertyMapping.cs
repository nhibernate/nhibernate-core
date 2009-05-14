using System;
using System.Collections.Generic;
using NHibernate.Engine;
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
		private readonly Dictionary<string, IType> typesByPropertyPath = new Dictionary<string, IType>();
		private readonly Dictionary<string, string[]> columnsByPropertyPath = new Dictionary<string, string[]>();
		private readonly Dictionary<string, string[]> formulaTemplatesByPropertyPath = new Dictionary<string, string[]>();

		public virtual string[] IdentifierColumnNames
		{
			get { throw new InvalidOperationException("one-to-one is not supported here"); }
		}

		protected abstract string EntityName { get; }

		protected QueryException PropertyException(string propertyName)
		{
			return new QueryException(string.Format("could not resolve property: {0} of: {1}", propertyName, EntityName));
		}

		#region IPropertyMapping Members

		public IType ToType(string propertyName)
		{
			try
			{
				IType type = typesByPropertyPath[propertyName];
				return type;
			}
			catch(KeyNotFoundException)
			{
				throw PropertyException(propertyName);
			}
		}

		public bool TryToType(string propertyName, out IType type)
		{
			return typesByPropertyPath.TryGetValue(propertyName, out type);
		}

		public virtual string[] ToColumns(string alias, string propertyName)
		{
			//TODO: *two* hashmap lookups here is one too many...
			string[] columns = GetColumns(propertyName);

			string[] templates;
			formulaTemplatesByPropertyPath.TryGetValue(propertyName, out templates);
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

		private string[] GetColumns(string propertyName)
		{
			string[] columns;
			try
			{
				columns = columnsByPropertyPath[propertyName];
			}
			catch (KeyNotFoundException)
			{
				throw PropertyException(propertyName);
			}
			return columns;
		}

		public virtual string[] ToColumns(string propertyName)
		{
			string[] columns = GetColumns(propertyName);

			string[] templates;
			formulaTemplatesByPropertyPath.TryGetValue(propertyName, out templates);
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
				formulaTemplatesByPropertyPath[path] = formulaTemplates;
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
						//TODO: this requires that the collection is defined after the
						//      referenced property in the mapping file (ok?)
						if (!columnsByPropertyPath.TryGetValue(foreignKeyProperty, out columns))
							return; //get em on the second pass!
					}
				}
			}

			if (path != null)
			{
				AddPropertyPath(path, type, columns, formulaTemplates);
			}

			if (type.IsComponentType)
			{
				IAbstractComponentType actype = (IAbstractComponentType)type;
				InitComponentPropertyPaths(path, actype, columns, formulaTemplates, factory);
				if (actype.IsEmbedded)
				{
					InitComponentPropertyPaths(path == null ? null : StringHelper.Qualifier(path), actype, columns, formulaTemplates, factory);
				}
			}
			else if (type.IsEntityType)
			{
				InitIdentifierPropertyPaths(path, (EntityType) type, columns, factory);
			}
		}

		protected void InitIdentifierPropertyPaths(string path, EntityType etype, string[] columns, IMapping factory)
		{
			IType idtype = etype.GetIdentifierOrUniqueKeyType(factory);
			string idPropName = etype.GetIdentifierOrUniqueKeyPropertyName(factory);
			bool hasNonIdentifierPropertyNamedId = HasNonIdentifierPropertyNamedId(etype, factory);

			if (etype.IsReferenceToPrimaryKey)
			{
				if (!hasNonIdentifierPropertyNamedId)
				{
					string idpath1 = ExtendPath(path, EntityPersister.EntityID);
					AddPropertyPath(idpath1, idtype, columns, null);
					InitPropertyPaths(idpath1, idtype, columns, null, factory);
				}
			}

			if (idPropName != null)
			{
				string idpath2 = ExtendPath(path, idPropName);
				AddPropertyPath(idpath2, idtype, columns, null);
				InitPropertyPaths(idpath2, idtype, columns, null, factory);
			}
		}

		private bool HasNonIdentifierPropertyNamedId(EntityType entityType, IMapping factory)
		{
			// NH: Different implementation (removed done "todo" of H3.2.6)
			return factory.HasNonIdentifierPropertyNamedId(entityType.GetAssociatedEntityName());
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
			if (string.IsNullOrEmpty(path))
				return property;
			else
				return StringHelper.Qualify(path, property);
		}

		public string[] GetColumnNames(string propertyName)
		{
			try
			{
				string[] columns;
				columns = columnsByPropertyPath[propertyName];
				return columns;
			}
			catch (KeyNotFoundException)
			{
				throw new MappingException(string.Format("unknown property: {0} of: {1}", propertyName, EntityName));
			}
		}
	}
}
