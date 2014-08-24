using System;

namespace NHibernate.Test.Criteria
{
	public class MaterialResource
	{
		private long _id;

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private string _description;

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public MaterialResource() : base()
		{
		}

		public MaterialResource(string description) : this()
		{
			_description = description;
		}
	}

	public class MaterialUnitable : MaterialResource
	{
	}

	public class DeviceDef : MaterialUnitable
	{
	}

	public class MaterialUnit
	{
		private long _id;

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private MaterialUnitable _material;

		public MaterialUnitable Material
		{
			get { return _material; }
			set { _material = value; }
		}

		private string _serialNumber;

		public string SerialNumber
		{
			get { return _serialNumber; }
			set { _serialNumber = value; }
		}

		protected MaterialUnit() : base()
		{
		}

		public MaterialUnit(MaterialUnitable material, string serialNumber) : this()
		{
			_material = material;
			_serialNumber = serialNumber;
		}
	}
}