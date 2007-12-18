using System;
using NHibernate.Search.Attributes;

namespace NHibernate.Search.Tests.Bridge
{
	[Indexed]
	public class Cloud
	{
		private int id;
		private long? long1;
		private long long2;
		private int? int1;
		private int int2;
		private double? double1;
		private double double2;
		private float? float1;
		private float float2;
		private string string1;
		private DateTime? dateTime;
		private DateTime? dateTimeYear;
		private DateTime? dateTimeMonth;
		private DateTime? dateTimeDay;
		private DateTime? dateTimeHour;
		private DateTime? dateTimeMinute;
		private DateTime? dateTimeSecond;
		private DateTime? dateTimeMillisecond;
		private String customFieldBridge;
		private String customStringBridge;
		private CloudType type;
		private bool storm;

		[Field(Index.Tokenized, Store = Attributes.Store.Yes)]
		[FieldBridge(typeof (TruncateFieldBridge))]
		public virtual  string CustomFieldBridge
		{
			get { return customFieldBridge; }
			set { this.customFieldBridge = value; }
		}

		[Field(Index.Tokenized, Store= Attributes.Store.Yes)]
		[FieldBridge(typeof (TruncateStringBridge), 4)]
		public virtual  string CustomStringBridge
		{
			get { return customStringBridge; }
			set { this.customStringBridge = value; }
		}

		[DocumentId]
		public virtual  int Id
		{
			get { return id; }
			set { this.id = value; }
		}

		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		public virtual  long? Long1
		{
			get { return long1; }
			set { this.long1 = value; }
		}

		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		public virtual  long Long2
		{
			get { return long2; }
			set { this.long2 = value; }
		}

		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		public virtual  int? Int1
		{
			get { return int1; }
			set { this.int1 = value; }
		}

		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		public virtual  int Int2
		{
			get { return int2; }
			set { this.int2 = value; }
		}

		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		public virtual  double? Double1
		{
			get { return double1; }
			set { this.double1 = value; }
		}

		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		public virtual  double Double2
		{
			get { return double2; }
			set { this.double2 = value; }
		}

		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		public virtual  float? Float1
		{
			get { return float1; }
			set { this.float1 = value; }
		}

		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		public virtual  float Float2
		{
			get { return float2; }
			set { this.float2 = value; }
		}

		[Field(Index.Tokenized, Store= Attributes.Store.Yes)]
		public virtual  string String1
		{
			get { return string1; }
			set { this.string1 = value; }
		}

		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		public virtual  DateTime? DateTime
		{
			get { return dateTime; }
			set { this.dateTime = value; }
		}

		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		[DateBridge(Resolution.Year)]
		public virtual  DateTime? DateTimeYear
		{
			get { return dateTimeYear; }
			set { this.dateTimeYear = value; }
		}


		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		[DateBridge(Resolution.Month)]
		public virtual  DateTime? DateTimeMonth
		{
			get { return dateTimeMonth; }
			set { this.dateTimeMonth = value; }
		}


		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		[DateBridge(Resolution.Day)]
		public virtual  DateTime? DateTimeDay
		{
			get { return dateTimeDay; }
			set { this.dateTimeDay = value; }
		}


		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		[DateBridge(Resolution.Hour)]
		public virtual  DateTime? DateTimeHour
		{
			get { return dateTimeHour; }
			set { this.dateTimeHour = value; }
		}


		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		[DateBridge(Resolution.Minute)]
		public virtual  DateTime? DateTimeMinute
		{
			get { return dateTimeMinute; }
			set { this.dateTimeMinute = value; }
		}


		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		[DateBridge(Resolution.Second)]
		public virtual  DateTime? DateTimeSecond
		{
			get { return dateTimeSecond; }
			set { this.dateTimeSecond = value; }
		}


		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		[DateBridge(Resolution.Millisecond)]
		public virtual  DateTime? DateTimeMillisecond
		{
			get { return dateTimeMillisecond; }
			set { this.dateTimeMillisecond = value; }
		}

		[Field(Index.Tokenized)]
		public virtual  CloudType Type
		{
			get { return type; }
			set { this.type = value; }
		}


		[Field(Index.Tokenized)]
		public virtual  bool Storm
		{
			get { return storm; }
			set { this.storm = value; }
		}
	}
}