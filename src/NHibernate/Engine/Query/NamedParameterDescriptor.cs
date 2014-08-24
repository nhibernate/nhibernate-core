using System;
using NHibernate.Type;

namespace NHibernate.Engine.Query
{
	/// <summary> Descriptor regarding a named parameter. </summary>
	[Serializable]
	public class NamedParameterDescriptor
	{
		private readonly string name;
		private readonly IType expectedType;
		private readonly bool jpaStyle;

		public NamedParameterDescriptor(string name, IType expectedType, bool jpaStyle)
		{
			this.name = name;
			this.expectedType = expectedType;
			this.jpaStyle = jpaStyle;
		}

		public string Name
		{
			get { return name; }
		}

		public IType ExpectedType
		{
			get { return expectedType; }
		}

		/// <summary>
		/// Not supported yet (AST parse needed)
		/// </summary>
		public bool JpaStyle
		{
			get { return jpaStyle; }
		}
	}
}
