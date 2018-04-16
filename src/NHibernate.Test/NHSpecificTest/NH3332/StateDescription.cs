using System;

namespace NHibernate.Test.NHSpecificTest.NH3332
{
	public class StateDescription
	{
		private Culture _culture;
		private State _state;
		private String _description;
		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		private Byte[] _rowVersionId;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj);
		}

		public virtual String Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public virtual Byte[] RowVersionId
		{
			get { return _rowVersionId; }
		}

		public virtual Culture Culture
		{
			get { return _culture; }
			set { _culture = value; }
		}

		public virtual State State
		{
			get { return _state; }
			set { _state = value; }
		}
	}
}
