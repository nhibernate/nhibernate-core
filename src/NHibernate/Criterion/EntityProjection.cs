﻿using System;

namespace NHibernate.Criterion
{
	[Serializable]
	public class EntityProjection : BaseEntityProjection
	{
		public EntityProjection(System.Type rootEntity, String alias) : base(rootEntity, alias)
		{
		}
	}

	[Serializable]
	public class EntityProjection<T> : EntityProjection
	{
		public EntityProjection(String alias) : base(typeof(T), alias)
		{
		}
	}
}