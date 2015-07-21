using System;

namespace NHibernate.Test.EntityModeTest.Xml.Many2One
{
	[Serializable]
	public class CarPart
	{
		public virtual long Id { get; set; }
		public virtual string PartName { get; set; }
	}
}