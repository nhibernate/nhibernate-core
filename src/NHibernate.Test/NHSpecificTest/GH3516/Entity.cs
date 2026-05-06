using System;

namespace NHibernate.Test.NHSpecificTest.GH3516
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual char FirstChar { get; set; }
		public virtual CharEnum CharacterEnum { get; set; } = CharEnum.SimpleChar;
		public virtual Uri UriProperty { get; set; }

		public virtual byte ByteProperty { get; set; }
		public virtual decimal DecimalProperty { get; set; }
		public virtual double DoubleProperty { get; set; }
		public virtual float FloatProperty { get; set; }
		public virtual short ShortProperty { get; set; }
		public virtual int IntProperty { get; set; }
		public virtual long LongProperty { get; set; }
		public virtual sbyte SByteProperty { get; set; }
		public virtual ushort UShortProperty { get; set; }
		public virtual uint UIntProperty { get; set; }
		public virtual ulong ULongProperty { get; set; }

		public virtual DateTime DateTimeProperty { get; set; } = StaticDateProperty;
		public virtual DateTime DateProperty { get; set; } = StaticDateProperty;
		public virtual DateTimeOffset DateTimeOffsetProperty { get; set; } = StaticDateProperty;
		public virtual DateTime TimeProperty { get; set; } = StaticDateProperty;

		public virtual Guid GuidProperty { get; set; } = Guid.Empty;

		public const string NameWithSingleQuote = "'; drop table Entity; --";
		public const string NameWithEscapedSingleQuote = @"\'; drop table Entity; --";

		// Do not switch to property, the feature of referencing static fields in HQL does not work with properties.
		public static string ArbitraryStringValue;

		public const char QuoteInitial = '\'';
		public const char BackslashInitial = '\\';

		public static readonly Uri UriWithSingleQuote = new("https://somewhere/?sql='; drop table Entity; --");
		public static readonly Uri UriWithEscapedSingleQuote = new(@"https://somewhere/?sql=\'; drop table Entity; --");

		public static readonly DateTime StaticDateProperty = DateTime.Today;
		public static readonly DateTimeOffset StaticDateTimeOffsetProperty = DateTimeOffset.Now;
	}

	public enum CharEnum
	{
		SimpleChar = 'A',
		SingleQuote = '\'',
		Backslash = '\\'
	}
}
