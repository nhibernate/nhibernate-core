using System;
using NHibernate.Type;

namespace NHibernate.Engine.Query
{
	[Serializable]
	public class OrdinalParameterDescriptor
	{
		private readonly int ordinalPosition;
		private readonly IType expectedType;
		private readonly int sourceLocation;

		public OrdinalParameterDescriptor(int ordinalPosition, IType expectedType, int sourceLocation)
		{
			this.ordinalPosition = ordinalPosition;
			this.expectedType = expectedType;
			this.sourceLocation = sourceLocation;
		}

		public int OrdinalPosition
		{
			get { return ordinalPosition; }
		}

		public IType ExpectedType
		{
			get { return expectedType; }
		}

		public int SourceLocation
		{
			get { return sourceLocation; }
		}
	}
}
