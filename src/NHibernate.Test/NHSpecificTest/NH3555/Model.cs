using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3555
{
	public class MapEntity
	{
		public MapEntity()
		{
			Dic = new Dictionary<KeyEntity, ValueEntity>();
		}

		public virtual int Id { get; set; }
		public virtual IDictionary<KeyEntity, ValueEntity> Dic { get; set; }
	}

	public class KeyEntity
	{
		public virtual int Id { get; set; }
		public virtual int Number { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as KeyEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Number == casted.Number);
		}

		public override int GetHashCode()
		{
			return Id ^ Number;
		}
	}

	public class ValueEntity
	{
		public virtual int Id { get; set; }
		public virtual int Number { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as KeyEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Number == casted.Number);
		}

		public override int GetHashCode()
		{
			return Id ^ Number;
		}
	}
}