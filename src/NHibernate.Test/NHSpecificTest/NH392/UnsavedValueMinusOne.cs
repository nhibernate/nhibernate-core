using System;

namespace NHibernate.Test.NHSpecificTest.NH392
{
	/// <summary>
	/// This class initializes the unsaved value to -1.
	/// </summary>
	public class UnsavedValueMinusOne
	{
		private int _id;
		private string _name;
		private DateTime _updateTimestamp;

		public int Id
		{
			get { return this._id; }
			set { this._id = value; }
		}

		public string Name
		{
			get { return this._name; }
			set { this._name = value; }
		}

		public DateTime UpdateTimestamp
		{
			get { return this._updateTimestamp; }
			set { this._updateTimestamp = value; }
		}

		public UnsavedValueMinusOne()
		{
			this._id = -1;
		}
	}
}