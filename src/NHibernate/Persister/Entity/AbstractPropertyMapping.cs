using System;
using System.Collections.Generic;
using System.Diagnostics;
using NHibernate.Cfg.XmlHbmBinding;
using NHibernate.Engine;
using NHibernate.Mapping;
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
		private readonly HashSet<string> duplicateIncompatiblePaths = new HashSet<string>();
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
			if (!typesByPropertyPath.TryGetValue(propertyName, out var type))
				throw PropertyException(propertyName);
			
			return type;
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
			var result = new string[columns.Length];
			for (int i = 0; i < columns.Length; i++)
			{
				if (columns[i] == null)
					result[i] = Template.ReplacePlaceholder(templates[i], alias);
				else
					result[i] = StringHelper.Qualify(alias, columns[i]);
			}
			return result;
		}

		private string[] GetColumns(string propertyName)
		{
			string[] columns;
			if (!columnsByPropertyPath.TryGetValue(propertyName, out columns))
				throw PropertyException(propertyName);

			return columns;
		}

		public virtual string[] ToColumns(string propertyName)
		{
			string[] columns = GetColumns(propertyName);

			string[] templates;
			formulaTemplatesByPropertyPath.TryGetValue(propertyName, out templates);
			var result = new string[columns.Length];
			for (int i = 0; i < columns.Length; i++)
			{
				if (columns[i] == null)
					result[i] = Template.ReplacePlaceholder(templates[i], string.Empty);
				else
					result[i] = columns[i];
			}
			return result;
		}

		public abstract IType Type { get; }

		#endregion

		[Obsolete] 
		protected void AddPropertyPath(string path, IType type, string[] columns, string[] formulaTemplates)
		{
			AddPropertyPath(path, type, columns, formulaTemplates, null);
		}

		protected void AddPropertyPath(
			string path,
			IType type,
			string[] columns,
			string[] formulaTemplates,
			IMapping factory)
		{
			if (typesByPropertyPath.TryGetValue(path, out var existingType) || duplicateIncompatiblePaths.Contains(path))
			{
				// If types match or the new type is not an association type, there is nothing for us to do
				if (type == existingType || existingType == null || !(type is IAssociationType))
				{
					//TODO: Log duplicated
				}
				else if (!(existingType is IAssociationType))
				{
					// Workaround for org.hibernate.cfg.annotations.PropertyBinder.bind() adding a component for *ToOne ids
					//TODO: Log duplicated
				}
				else
				{
					if (type is AnyType && existingType is AnyType)
					{
						// TODO: not sure how to handle any types. For now we just return and let the first type dictate what type the property has...
					}
					else
					{
						IType commonType = null;
						if (type is CollectionType typeCollection && existingType is CollectionType existingCollection)
						{
							var metadata = (IMetadata) factory;
							var thisCollection = metadata.GetCollection(existingCollection.Role);
							var otherCollection = metadata.GetCollection(typeCollection.Role);

							if (thisCollection.Equals(otherCollection))
							{
								//TODO: Log duplicated
							}
							else
							{
								//TODO: log Incompatible Registration
							}
						}
						else if (type is EntityType entityType1 && existingType is EntityType entityType2)
						{
							if (entityType1.GetAssociatedEntityName() == entityType2.GetAssociatedEntityName())
							{
								//TODO: Log duplicated
								return;
							}
							
							commonType = GetCommonType((IMetadata)factory, entityType1, entityType2);
						}
						else
						{
							//TODO: log Incompatible Registration
						}

						if (commonType == null)
						{
							duplicateIncompatiblePaths.Add(path);
							typesByPropertyPath.Remove(path);
							columnsByPropertyPath[path] = columns;
							
							if ( formulaTemplates != null )
							{
								formulaTemplatesByPropertyPath[path] = formulaTemplates;
							}
						}
						else
						{
							typesByPropertyPath[path] = commonType;
						}
					}
				}
			}
			else
			{
				typesByPropertyPath[path] = type;
				columnsByPropertyPath[path] = columns;
				
				if (formulaTemplates != null)
				{
					formulaTemplatesByPropertyPath[path] = formulaTemplates;
				}
			}
		}

		private static IType GetCommonType(IMetadata metadata, EntityType entityType1, EntityType entityType2)
		{
			var thisClass = metadata.GetPersistentClass(entityType1.GetAssociatedEntityName());
			var otherClass = metadata.GetPersistentClass(entityType2.GetAssociatedEntityName());
			var commonClass = GetCommonPersistent(thisClass, otherClass);

			if (commonClass == null)
			{
				return null;
			}

			switch (entityType1)
			{
				case ManyToOneType many:
					return new ManyToOneType(many, commonClass.EntityName);
				case SpecialOneToOneType special:
						return new SpecialOneToOneType(special, commonClass.EntityName);
				default:
					throw new Exception("Unexpected entity type: " + entityType1);
					
			}
		}

		private static PersistentClass GetCommonPersistent(PersistentClass class1, PersistentClass class2)
		{
			while (class2 != null && class2.MappedClass != null && !class2.MappedClass.IsAssignableFrom(class1.MappedClass))
			{
				class2 = class2.Superclass;
			}

			return class2;
		}

		protected internal void InitPropertyPaths( string path, IType type, string[] columns, string[] formulaTemplates, IMapping factory )
		{
			if (columns.Length != type.GetOwnerColumnSpan(factory))
			{
				throw new MappingException(
					string.Format("broken column mapping for: {0} of: {1}, type {2} expects {3} columns, but {4} were mapped",
								  path, EntityName, type.Name, type.GetColumnSpan(factory), columns.Length));
			}

			if (type.IsAssociationType)
			{
				var actType = (IAssociationType)type;
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
				AddPropertyPath(path, type, columns, formulaTemplates, factory);
			}

			if (type.IsComponentType)
			{
				var actype = (IAbstractComponentType)type;
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
					AddPropertyPath(idpath1, idtype, columns, null, factory);
					InitPropertyPaths(idpath1, idtype, columns, null, factory);
				}
			}

			if (idPropName != null)
			{
				string idpath2 = ExtendPath(path, idPropName);
				AddPropertyPath(idpath2, idtype, columns, null, factory);
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
			
			return StringHelper.Qualify(path, property);
		}

		public string[] GetColumnNames(string propertyName)
		{
			string[] columns;
			if (!columnsByPropertyPath.TryGetValue(propertyName, out columns))
				throw new MappingException(string.Format("unknown property: {0} of: {1}", propertyName, EntityName));

			return columns;
		}
	}
}
