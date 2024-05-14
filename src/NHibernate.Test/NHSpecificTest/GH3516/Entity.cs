using System;

namespace NHibernate.Test.NHSpecificTest.GH3516
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public const string NameWithSingleQuote = "'; drop table Entity; --";
		public const string NameWithEscapedSingleQuote = @"\'; drop table Entity; --";
	}
}
