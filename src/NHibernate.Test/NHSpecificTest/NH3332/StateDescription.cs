using System;

namespace NHibernate.Test.NHSpecificTest.NH3332
{
	public class StateDescription
	{
		private Culture _culture;
		private State _state;
		private String _description;
		private Byte[] _rowVersionId;

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
