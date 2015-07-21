using System;

namespace NHibernate.Test.Hql
{
	public class MaterialResource
	{
		public enum MaterialState : int
		{
			Available,
			Reserved,
			Discarded
		}

		public MaterialResource()
		{

		}

		public MaterialResource(string description, string serialNumber, MaterialState state)
		{
			_description = description;
			_serialNumber = serialNumber;
			_state = state;
		}

		private int _id;
		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private string _description;
		public virtual string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		private string _serialNumber;
		public virtual string SerialNumber
		{
			get { return _serialNumber; }
			set { _serialNumber = value; }
		}

		private MaterialState _state;
		public virtual MaterialState State
		{
			get { return _state; }
			set { _state = value; }
		}

	}
}
