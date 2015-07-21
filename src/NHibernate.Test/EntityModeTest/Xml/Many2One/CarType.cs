using System;

namespace NHibernate.Test.EntityModeTest.Xml.Many2One
{
	[Serializable]
	public class CarType
	{
		public virtual long Id { get; set; }
		public virtual string TypeName { get; set; }
	}
}