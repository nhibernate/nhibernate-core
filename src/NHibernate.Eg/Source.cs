using System;

namespace NHibernate.Eg 
{
	
	public class Source : Vertex 
	{
		private float strength;

		public float SourceStrength 
		{
			get { return strength; }
			set { strength = value; }
		}

		public override float ExcessCapacity 
		{
			get { return base.ExcessCapacity - strength; }
		}
	}
}
