using System;
using System.Collections.Generic;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A NHibernate <c>any</c> type.
	/// </summary>
	/// <remarks>
	/// Polymorphic association to one of several tables.
	/// </remarks>
	[Serializable]
	public class Any : SimpleValue
	{
		private string identifierTypeName;
		private string metaTypeName = NHibernateUtil.String.Name;
		private IDictionary<object, string> metaValues;

		public Any(Table table) : base(table)
		{
			_type = GetLazyType();
		}

		/// <summary>
		/// Get or set the identifier type name
		/// </summary>
		public virtual string IdentifierTypeName
		{
			get { return identifierTypeName; }
			set { identifierTypeName = value; }
		}

		private Lazy<IType> GetLazyType()
		{
			return new Lazy<IType>(
				() =>
				{
					var identifierType = TypeFactory.HeuristicType(identifierTypeName);
					if (identifierType == null)
						throw new MappingException($"Identifier type '{identifierTypeName}' is invalid");

					var hasMetaValues = metaValues != null && metaValues.Count > 0;

					var metaType = !hasMetaValues && "class" == metaTypeName
						? NHibernateUtil.String
						: TypeFactory.HeuristicType(metaTypeName);

					if (metaType == null)
						throw new MappingException($"Meta type '{metaTypeName}' is invalid");

					if (!hasMetaValues && metaType.ReturnedClass != typeof(string))
						throw new MappingException(
							$"Using a non-string meta type ('{metaTypeName}') without providing meta values is invalid");

					return new AnyType(
						new MetaType(metaValues, metaType),
						identifierType);
				});
		}

		// Types may be used by many threads, we must use a thread safe lazy initialization.
		// (Or we should build this class in another way, but this would likely be a breaking change.)
		private Lazy<IType> _type;

		public override IType Type => _type.Value;

		public void ResetCachedType()
		{
			// this is required if the user is programatically modifying the Any instance
			// and need to reset the cached type instance;
			_type = GetLazyType();
		}

		public override void SetTypeUsingReflection(string className, string propertyName, string access)
		{
		}

		/// <summary>
		/// Get or set the metatype 
		/// </summary>
		public virtual string MetaType
		{
			get { return metaTypeName; }
			set { metaTypeName = value; }
		}

		/// <summary>
		/// Represent the relation between a meta-value and the related entityName
		/// </summary>
		public IDictionary<object, string> MetaValues
		{
			get { return metaValues; }
			set { metaValues = value; }
		}
	}
}
