using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for BasicSerializable.
	/// </summary>
	public class BasicSerializable
	{
		private int _id;
		private SerializableClass _serializableProperty;
		private object _serial;

		public BasicSerializable()
		{
			_serializableProperty = new SerializableClass();
			_serializableProperty._classId = 5;
			_serializableProperty._classString = "serialize me";
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public SerializableClass SerializableProperty
		{
			get { return _serializableProperty; }
			set { _serializableProperty = value; }
		}

		public object Serial
		{
			get { return _serial; }
			set { _serial = value; }
		}
	}

	[Serializable]
	public class SerializableClass
	{
		public int _classId;
		public string _classString;

		public override int GetHashCode()
		{
			// not a good method, but all that is needed for this Class
			// to be used by tests.
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			SerializableClass lhs = obj as SerializableClass;
			if (lhs == null) return false;

			if (this == lhs) return true;

			if (this._classId.Equals(lhs._classId)
			    && this._classString.Equals(lhs._classString)) return true;

			return false;
		}
	}
}
