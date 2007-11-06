using System;
using System.Collections.Generic;
using System.Text;

using NHibernate.Engine;

namespace NHibernate.Mapping
{
	public class JoinedSubclass : Subclass, ITableOwner
	{
		private Table table;
		private IKeyValue key;

		public JoinedSubclass(PersistentClass superclass)
			: base(superclass) { }

		public override Table Table
		{
		    get { return table; }
		}

		Table ITableOwner.Table
		{
			set
			{
				table = value;
				Superclass.AddSubclassTable(table);
			}
		}

		public override IKeyValue Key
		{
			get { return key; }
			set { key = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		public override void Validate(IMapping mapping)
		{
			base.Validate(mapping);
			if (Key != null && !Key.IsValid(mapping))
			{
				throw new MappingException(string.Format("subclass key has wrong number of columns: {0} type: {1}", MappedClass.Name, Key.Type.Name));
			}
		}

		public override IEnumerable<Property> ReferenceablePropertyIterator
		{
			get { return PropertyIterator; }
		}
	}
}
