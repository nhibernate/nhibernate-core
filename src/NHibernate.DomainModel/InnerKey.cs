using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for InnerKey.
	/// </summary>
	[Serializable]
	public class InnerKey
	{
		private string _aKey;
		private string _bKey;

		public string AKey
		{
			get { return _aKey; }
			set { _aKey = value; }
		}

		public string BKey
		{
			get { return _bKey; }
			set { _bKey = value; }
		}

		#region object Members

		public override bool Equals(object obj)
		{
			if (this == obj) return true;

			InnerKey cidSuperID = obj as InnerKey;
			if (cidSuperID == null) return false;

			if (_aKey != null ? !_aKey.Equals(cidSuperID._aKey) : cidSuperID._aKey != null) return false;
			if (_bKey != null ? !_bKey.Equals(cidSuperID._bKey) : cidSuperID._bKey != null) return false;

			return true;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result;
				//TODO: string won't be null - verify
				result = (_aKey != null ? _aKey.GetHashCode() : 0);
				result = 29 * result + (_bKey != null ? _bKey.GetHashCode() : 0);

				return result;
			}
		}

		#endregion
	}
}