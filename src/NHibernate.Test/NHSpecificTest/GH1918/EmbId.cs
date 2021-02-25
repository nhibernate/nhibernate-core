namespace NHibernate.Test.NHSpecificTest.GH1918
{
	public class EmbId
	{
		public virtual int X { get; set; }
		public virtual int Y { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as EmbId;
			if (casted == null)
				return false;
			return X == casted.X && Y == casted.Y;
		}

		public override int GetHashCode()
		{
			return X ^ Y;
		}
	}
}
