using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// POCO for E
	/// </summary>
	[Serializable]
	public class E
	{
		#region Fields
	
		private Int64 _id;
		private Double _amount;
		private A _reverse;
	
		#endregion

		#region Properties
		/// <summary>
		/// Get/set for id
		/// </summary>
		public virtual Int64 Id
		{
			get { return _id; }
			set { _id = value; }
		}
	
		/// <summary>
		/// Get/set for Amount
		/// </summary>
		public virtual Double Amount
		{
			get { return _amount; }
			set { _amount = value; }
		}

		public virtual A Reverse
		{
			get { return _reverse; }
			set { _reverse = value; }
		}

		#endregion
	}
}