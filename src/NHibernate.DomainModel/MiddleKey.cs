using System;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for MiddleKey.
	/// </summary>
	[Serializable]
	public class MiddleKey 
	{
		private Inner _sup;
		private string _two;
		private string _one;
		
		public Inner Sup
		{
			get { return _sup; }
			set { _sup = value; }
		}

		public string One
		{
			get { return _one; }
			set { _one = value; }
		}

		public string Two
		{
			get { return _two; }
			set { _two = value; }
		}

		#region System.Object Members

		public override bool Equals(object obj)
		{
			if(this==obj) return true;

			MiddleKey cidMasterID = obj as MiddleKey;
			if(cidMasterID==null) return false;

			if ( _one!=null ? !_one.Equals(cidMasterID.One) : cidMasterID.One!=null) return false; 
			if ( _sup!=null ? !_sup.Equals(cidMasterID.Sup) : cidMasterID.Sup!=null) return false;
			if ( _two!=null ? !_two.Equals(cidMasterID.Two) : cidMasterID.Two!=null) return false;

			return true;
		}

		public override int GetHashCode()
		{
			unchecked 
			{
				int result;

				//TODO: string can't be null
				result = ( _sup!=null ? _sup.GetHashCode() : 0 );
				result = 29 * result + ( _one!=null ? _one.GetHashCode() : 0 );
				result = 29 * result + ( _two!=null ? _two.GetHashCode() : 0 );

				return result;
			}
		}

		#endregion

	}
}
