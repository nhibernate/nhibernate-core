using System;
using System.Collections;

namespace NHibernate.DomainModel
{

	[Serializable]
	public class SerializableClass 
	{
		public int classId;
		public string classString;

		public override int GetHashCode()
		{
			// not a good method, but all that is needed for this Class
			// to be used by tests.
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			SerializableClass lhs = obj as SerializableClass;
			if(lhs==null) return false;

			if(this==lhs) return true;

			if(this.classId.Equals(lhs.classId) 
				&& this.classString.Equals(this.classString)) return true;

			return false;
		}

	}

	/// <summary>
	/// Summary description for BasicClass.
	/// </summary>
	[Serializable]
	public class BasicClass
	{
		private int id;

		private byte[] binaryProperty;
		private bool booleanProperty;
		private byte byteProperty;
		private char characterProperty;
		private System.Type classProperty;
		private System.Globalization.CultureInfo cultureInfoProperty;
		private DateTime dateTimeProperty;
		private decimal decimalProperty;
		private double doubleProperty;
		private short int16Property;
		private int int32Property;
		private long int64Property;
		private SerializableClass serializableProperty;
		private float singleProperty;
		private string stringProperty;
		private DateTime ticksProperty;
		private bool trueFalseProperty;
		private bool yesNoProperty;

		private string[] stringArray;
		private int[] int32Array;
		private IList stringList;
		private IDictionary stringMap;
		private IDictionary stringSet;
		private object dummyObject = new object();

		public BasicClass()
		{
			serializableProperty = new SerializableClass();
			serializableProperty.classId = 5;
			serializableProperty.classString = "serialize me";
		}

		public int Id 
		{
			get { return id;}
			set { id = value;}
		}

		public byte[] BinaryProperty 
		{
			get {return binaryProperty;}
			set {binaryProperty = value;}
		}

		public bool BooleanProperty 
		{
			get {return booleanProperty;}
			set {booleanProperty = value;}
		}

		public byte ByteProperty 
		{
			get {return byteProperty;}
			set {byteProperty = value;}
		}

		public char CharacterProperty 
		{
			get {return characterProperty ;}
			set {characterProperty = value;}
		}

		public System.Type ClassProperty 
		{
			get {return classProperty;}
			set {classProperty = value;}
		}

		public System.Globalization.CultureInfo CultureInfoProperty 
		{
			get {return cultureInfoProperty;}
			set {cultureInfoProperty = value;}
		}

		public DateTime DateTimeProperty 
		{
			get {return dateTimeProperty;}
			set {dateTimeProperty = value;}
		}

		public decimal DecimalProperty 
		{
			get {return decimalProperty;}
			set {decimalProperty = value;}
		}

		public double DoubleProperty 
		{
			get {return doubleProperty;}
			set {doubleProperty = value;}
		}

		public short Int16Property 
		{
			get {return int16Property;}
			set {int16Property = value;}
		}

		public int Int32Property 
		{
			get {return int32Property;}
			set {int32Property = value;}
		}

		public long Int64Property 
		{
			get {return int64Property;}
			set {int64Property = value;}
		}

		public SerializableClass SerializableProperty 
		{
			get {return serializableProperty;}
			set {serializableProperty = value;}
		}

		public float SingleProperty 
		{
			get {return singleProperty;}
			set {singleProperty = value;}
		}

		public string StringProperty 
		{
			get {return stringProperty;}
			set {stringProperty = value;}
		}

		public DateTime TicksProperty 
		{
			get {return ticksProperty;}
			set {ticksProperty = value;}
		}

		public bool TrueFalseProperty 
		{
			get {return trueFalseProperty;}
			set {trueFalseProperty = value;}
		}

		public bool YesNoProperty 
		{
			get {return yesNoProperty;}
			set {yesNoProperty = value;}
		}

		public string[] StringArray 
		{
			get { return stringArray; }
			set { stringArray = value; }
		}

		public int[] Int32Array 
		{
			get { return int32Array; }
			set { int32Array = value; }
		}

		public IList StringList
		{
			get { return stringList; }
			set { stringList = value; }
		}

		public IDictionary StringMap 
		{
			get { return stringMap; }
			set { stringMap = value; }
		}

		public IDictionary StringSet 
		{
			get { return stringSet; }
			set { stringSet = value; }
		}
		
		public void AddToStringSet(string stringValue) 
		{
			if(StringSet==null) StringSet = new Hashtable();
			StringSet[stringValue] = dummyObject;
		}

	}
}
