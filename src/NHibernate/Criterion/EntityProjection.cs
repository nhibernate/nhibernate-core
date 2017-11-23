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
}
