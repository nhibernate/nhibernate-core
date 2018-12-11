﻿using System;

namespace NHibernate.Test.NHSpecificTest.GH1921
{
	class Entity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}

	class MultiTableEntity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string OtherName { get; set; }
	}
}
