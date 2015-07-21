using System;
using NHibernate.Type;

namespace NHibernate.Engine.Query
{
	[Serializable]
	public class OrdinalParameterDescriptor
	{
		private readonly int ordinalPosition;
		private readonly IType expectedType;

		public OrdinalParameterDescriptor(int ordinalPosition, IType expectedType)
		{
			this.ordinalPosition = ordinalPosition;
			this.expectedType = expectedType;
		}

		public int OrdinalPosition
		{
			get { return ordinalPosition; }
		}

		public IType ExpectedType
		{
			get { return expectedType; }
		}
	}
}
