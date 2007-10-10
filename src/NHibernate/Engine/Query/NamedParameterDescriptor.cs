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
		private readonly int[] sourceLocations;
		private readonly bool jpaStyle;

		public NamedParameterDescriptor(string name, IType expectedType, int[] sourceLocations, bool jpaStyle)
		{
			this.name = name;
			this.expectedType = expectedType;
			this.sourceLocations = sourceLocations;
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

		public int[] SourceLocations
		{
			get { return sourceLocations; }
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
