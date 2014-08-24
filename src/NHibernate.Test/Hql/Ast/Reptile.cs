namespace NHibernate.Test.Hql.Ast
{
	public class Reptile: Animal
	{
		private float bodyTemperature;
		public virtual float BodyTemperature
		{
			get { return bodyTemperature; }
			set { bodyTemperature = value; }
		}
	}

	public class Lizard : Reptile { }
}