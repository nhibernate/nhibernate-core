using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3772 {
	public class Entity
	{
		private ICollection<SubEntity> _subEntities;
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<SubEntity> SubEntities {
			get { return this._subEntities ?? (this._subEntities = new HashSet<SubEntity>()); }
			set { this._subEntities = value; }
		}
	}

	public class SubEntity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}
