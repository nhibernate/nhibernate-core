using System;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for Middle.
	/// </summary>
	[Serializable]
	public class Middle 
	{
		
		private MiddleKey _id;
		private string _bla;
		
		public MiddleKey Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Bla
		{
			get { return _bla; }
			set { _bla = value; }
		}

		#region System.Object Members

		public override bool Equals(object obj)
		{
			if(this==obj) return true;

			Middle cidMaster = obj as Middle;
			if(cidMaster==null) return false;

			if(_id!=null ? !_id.Equals(cidMaster.Id) : cidMaster.Id!=null) return false;

			return true;
		}

		public override int GetHashCode()
		{
			return ( _id!=null ? _id.GetHashCode() : 0 );
		}

		#endregion
	}
}
