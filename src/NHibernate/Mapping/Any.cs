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
		}

		/// <summary>
		/// Get or set the identifier type name
		/// </summary>
		public virtual string IdentifierTypeName
		{
			get { return identifierTypeName; }
			set { identifierTypeName = value; }
		}

		private IType type;
		public override IType Type
		{
			get
			{
				if (type == null)
				{
					type =
						new AnyType(
							metaValues == null
							? ("class".Equals(metaTypeName) ? new ClassMetaType(): TypeFactory.HeuristicType(metaTypeName))
								: new MetaType(metaValues, TypeFactory.HeuristicType(metaTypeName)),
							TypeFactory.HeuristicType(identifierTypeName));
				}
				return type;
			}
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