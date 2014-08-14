using System;

namespace NHibernate.Criterion
{
	[Serializable]
	public class RootEntityProjection : BaseEntityProjection
	{
		public RootEntityProjection() : base(null, null)
		{
		}
	}
}