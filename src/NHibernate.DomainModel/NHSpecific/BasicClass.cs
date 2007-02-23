using System;
using System.Collections;
using System.Globalization;

using Iesi.Collections;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for BasicClass.
	/// </summary>
	[Serializable]
	public class BasicClass
	{
		private int _id;

		private char _characterProperty;
		private System.Type _classProperty;
		private CultureInfo _cultureInfoProperty;
		private DateTime _dateTimeProperty = DateTime.Today;
		private short _int16Property;
		private int _int32Property;
		private long _int64Property;
		private float _singleProperty;
		private string _stringProperty;
		private DateTime _ticksProperty;
		private bool _trueFalseProperty;
		private bool _yesNoProperty;

		private int _privateField;

		private string[] _stringArray;
		private int[] _int32Array;
		private IList _stringBag;
		private IList _stringList;
		private IDictionary _stringMap;
		private ISet _stringSet;

		public BasicClass()
		{
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public char CharacterProperty
		{
			get { return _characterProperty; }
			set { _characterProperty = value; }
		}

		public System.Type ClassProperty
		{
			get { return _classProperty; }
			set { _classProperty = value; }
		}

		public CultureInfo CultureInfoProperty
		{
			get { return _cultureInfoProperty; }
			set { _cultureInfoProperty = value; }
		}

		public DateTime DateTimeProperty
		{
			get { return _dateTimeProperty; }
			set { _dateTimeProperty = value; }
		}

		public short Int16Property
		{
			get { return _int16Property; }
			set { _int16Property = value; }
		}

		public int Int32Property
		{
			get { return _int32Property; }
			set { _int32Property = value; }
		}

		public long Int64Property
		{
			get { return _int64Property; }
			set { _int64Property = value; }
		}

		public float SingleProperty
		{
			get { return _singleProperty; }
			set { _singleProperty = value; }
		}

		public string StringProperty
		{
			get { return _stringProperty; }
			set { _stringProperty = value; }
		}

		public DateTime TicksProperty
		{
			get { return _ticksProperty; }
			set { _ticksProperty = value; }
		}

		public bool TrueFalseProperty
		{
			get { return _trueFalseProperty; }
			set { _trueFalseProperty = value; }
		}

		public bool YesNoProperty
		{
			get { return _yesNoProperty; }
			set { _yesNoProperty = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// NHibernate knows nothing about this Property.  This Property
		/// is provided so the Test Fixtures can set and get the value of the
		/// field <c>_privateField</c> to make sure that NHibernate is reading
		/// and writing the field correctly.
		/// </remarks>
		public int ValueOfPrivateField
		{
			get { return _privateField; }
			set { _privateField = value; }
		}

		public string[] StringArray
		{
			get { return _stringArray; }
			set { _stringArray = value; }
		}

		public int[] Int32Array
		{
			get { return _int32Array; }
			set { _int32Array = value; }
		}

		public IList StringBag
		{
			get { return _stringBag; }
			set { _stringBag = value; }
		}

		public IList StringList
		{
			get { return _stringList; }
			set { _stringList = value; }
		}

		public IDictionary StringMap
		{
			get { return _stringMap; }
			set { _stringMap = value; }
		}

		public ISet StringSet
		{
			get { return _stringSet; }
			set { _stringSet = value; }
		}

		public void AddToStringSet(string stringValue)
		{
			if (StringSet == null)
			{
				StringSet = new HashedSet();
			}
			StringSet.Add(stringValue);
		}
	}
}