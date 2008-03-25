using System;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class FumCompositeID
	{
		private String _string;
		private DateTime _date;
		private short _short;

		public string String
		{
			get { return _string; }
			set { _string = value; }
		}

		public DateTime Date
		{
			get { return _date; }
			set { _date = value; }
		}

		public short Short
		{
			get { return _short; }
			set { _short = value; }
		}

		#region System.Object Members

		public override bool Equals(object obj)
		{
			FumCompositeID that = (FumCompositeID) obj;
			return this._string.Equals(that._string) && this._short == that._short;
		}

		public override int GetHashCode()
		{
			return _string.GetHashCode();
		}

		#endregion
	}
}