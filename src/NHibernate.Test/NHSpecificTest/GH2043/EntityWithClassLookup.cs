﻿using System;

namespace NHibernate.Test.NHSpecificTest.IlogsProxyTest
{
	public class EntityWithClassLookup
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }

		public virtual EntityWithClassProxyDefinition EntityLookup { get; set; }
	}
}
