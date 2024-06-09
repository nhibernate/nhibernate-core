using System;

namespace NHibernate.Test.NHSpecificTest.GH3516
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual char FirstChar { get; set; }
		public virtual CharEnum CharacterEnum { get; set; } = CharEnum.SimpleChar;

		public const string NameWithSingleQuote = "'; drop table Entity; --";
		public const string NameWithEscapedSingleQuote = @"\'; drop table Entity; --";

		// Do not switch to property, the feature of referencing static fields in HQL does not work with properties.
		public static string ArbitraryStringValue;

		public const char QuoteInitial = '\'';
		public const char BackslashInitial = '\\';

	}

	public enum CharEnum
	{
		SimpleChar = 'A',
		SingleQuote = '\'',
		Backslash = '\\'
	}
}
