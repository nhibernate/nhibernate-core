using System;
using System.Collections;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for BasicClass.
	/// </summary>
	[Serializable]
	public class BasicClass
	{
		private int _id;

		private bool _booleanProperty;
		private byte _byteProperty;
		private char _characterProperty;
		private System.Type _classProperty;
		private System.Globalization.CultureInfo _cultureInfoProperty;
		private DateTime _dateTimeProperty;
		private decimal _decimalProperty;
		private short _int16Property;
		private int _int32Property;
		private long _int64Property;
		private float _singleProperty;
		private string _stringProperty;
		private DateTime _ticksProperty;
		private bool _trueFalseProperty;
		private bool _yesNoProperty;
		
		private string[] _stringArray;
		private int[] _int32Array;
		private IList _stringBag;
		private IList _stringList;
		private IDictionary _stringMap;
		private IDictionary _stringSet;
		private object _dummyObject = new object();

		public BasicClass()
		{
			
		}

		public int Id 
		{
			get { return _id;}
			set { _id = value;}
		}

		public bool BooleanProperty 
		{
			get {return _booleanProperty;}
			set {_booleanProperty = value;}
		}

		public byte ByteProperty 
		{
			get {return _byteProperty;}
			set {_byteProperty = value;}
		}

		public char CharacterProperty 
		{
			get {return _characterProperty ;}
			set {_characterProperty = value;}
		}

		public System.Type ClassProperty 
		{
			get {return _classProperty;}
			set {_classProperty = value;}
		}

		public System.Globalization.CultureInfo CultureInfoProperty 
		{
			get {return _cultureInfoProperty;}
			set {_cultureInfoProperty = value;}
		}

		public DateTime DateTimeProperty 
		{
			get {return _dateTimeProperty;}
			set {_dateTimeProperty = value;}
		}

		public decimal DecimalProperty 
		{
			get {return _decimalProperty;}
			set {_decimalProperty = value;}
		}

		public short Int16Property 
		{
			get {return _int16Property;}
			set {_int16Property = value;}
		}

		public int Int32Property 
		{
			get {return _int32Property;}
			set {_int32Property = value;}
		}

		public long Int64Property 
		{
			get {return _int64Property;}
			set {_int64Property = value;}
		}

		public float SingleProperty 
		{
			get {return _singleProperty;}
			set {_singleProperty = value;}
		}

		public string StringProperty 
		{
			get {return _stringProperty;}
			set {_stringProperty = value;}
		}

		public DateTime TicksProperty 
		{
			get {return _ticksProperty;}
			set {_ticksProperty = value;}
		}

		public bool TrueFalseProperty 
		{
			get {return _trueFalseProperty;}
			set {_trueFalseProperty = value;}
		}

		public bool YesNoProperty 
		{
			get {return _yesNoProperty;}
			set {_yesNoProperty = value;}
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

		public IDictionary StringSet 
		{
			get { return _stringSet; }
			set { _stringSet = value; }
		}
		
		public void AddToStringSet(string stringValue) 
		{
			if(StringSet==null) StringSet = new Hashtable();
			StringSet[stringValue] = _dummyObject;
		}

	}
}
