using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Inner.
	/// </summary>
	[Serializable]
	public class Inner
	{
		private InnerKey _id;
		private string _dudu;

		public InnerKey Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Dudu
		{
			get { return _dudu; }
			set { _dudu = value; }
		}

		#region object members

		public override bool Equals(object obj)
		{
			if (this == obj) return true;

			Inner cidSuper = obj as Inner;
			if (cidSuper == null) return false;

			if (_id == null)
			{
				return cidSuper._id == null;
			}
			else
			{
				return _id.Equals(cidSuper._id);
			}
		}

		public override int GetHashCode()
		{
			if (_id == null)
			{
				return 0;
			}
			else
			{
				return _id.GetHashCode();
			}
		}

		#endregion
	}
}