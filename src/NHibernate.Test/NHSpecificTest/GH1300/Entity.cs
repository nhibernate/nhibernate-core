﻿namespace NHibernate.Test.NHSpecificTest.GH1300
{
	class Entity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string AnsiName { get; set; }
		public virtual string FullText { get; set; }
		public virtual string AnsiFullText { get; set; }
	}
}
