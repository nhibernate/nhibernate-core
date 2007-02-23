using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for OuterKey.
	/// </summary>
	[Serializable]
	public class OuterKey
	{
		private Middle _master;
		private string _detailId;

		public Middle Master
		{
			get { return _master; }
			set { _master = value; }
		}

		public string DetailId
		{
			get { return _detailId; }
			set { _detailId = value; }
		}

		#region System.Object Members

		public override bool Equals(object obj)
		{
			if (this == obj) return true;

			OuterKey cidDetailId = obj as OuterKey;
			if (cidDetailId == null) return false;

			if (_detailId != null ? !_detailId.Equals(cidDetailId.DetailId) : cidDetailId.DetailId != null) return false;
			if (_master != null ? !_master.Equals(cidDetailId.Master) : cidDetailId.Master != null) return false;

			return true;
		}


		public override int GetHashCode()
		{
			unchecked
			{
				int result;

				result = (_master != null ? _master.GetHashCode() : 0);
				//TODO: string == null???
				result = 29 * result + (_detailId != null ? _detailId.GetHashCode() : 0);

				return result;
			}
		}

		#endregion
	}
}