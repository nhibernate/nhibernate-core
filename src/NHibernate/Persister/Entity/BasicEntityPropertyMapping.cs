using System;
using NHibernate.Type;

namespace NHibernate.Persister.Entity
{
	public class BasicEntityPropertyMapping : AbstractPropertyMapping
	{
		private readonly AbstractEntityPersister persister;

		public BasicEntityPropertyMapping(AbstractEntityPersister persister)
		{
			this.persister = persister;
		}

		public override string[] IdentifierColumnNames
		{
			get { return persister.IdentifierColumnNames; }
		}

		protected System.Type EntityName
		{
			get { return persister.MappedClass; }
		}

		public override IType Type
		{
			get { return persister.Type; }
		}

		public override string[] ToColumns(string alias, string propertyName)
		{
			return base.ToColumns(
					persister.GenerateTableAlias(alias, persister.GetSubclassPropertyTableNumber(propertyName)),
					propertyName
				);
		}

		public override string ClassName
		{
			get { return Type.Name; }
		}
	}
}
