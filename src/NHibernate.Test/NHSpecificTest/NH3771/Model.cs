namespace NHibernate.Test.NHSpecificTest.NH3771
{
	using System;
	using System.Collections.Generic;

	public class Singer
	{
        public Singer()
		{
		}

		public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Version { get; set; }
	}

}
