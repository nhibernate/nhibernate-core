using System;
using NHibernate.Type;

namespace NHibernate.Tuple
{
	/// <summary>
	/// Defines the basic contract of a Property within the runtime metamodel.
	/// </summary>
	[Serializable]
	public abstract class Property
	{
		private readonly string name;
		private readonly IType type;

		/// <summary>
		/// Constructor for Property instances.
		/// </summary>
		/// <param name="name">The name by which the property can be referenced within its owner.</param>
		/// <param name="type">The Hibernate Type of this property.</param>
		protected Property(string name, IType type)
		{
			this.name = name;
			this.type = type;
		}

		public string Name
		{
			get { return name; }
		}

		public IType Type
		{
			get { return type; }
		}

		public override string ToString()
		{
			return "Property(" + name + ':' + type.Name + ')';
		}
	}
}