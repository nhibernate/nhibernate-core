namespace NHibernate.Test.NHSpecificTest.NH1789
{
	public class Cat : DomainObject, ICat
	{
		public Cat(string name, long id)
		{
			Name = name;
			_id = id;
		}

		/// <summary>
		/// NHibernate parameterless constructor
		/// </summary>
		protected Cat() {}

		#region ICat Members

		///<summary>
		/// This is a string which uniquely identifies an instance in the case that the ids 
		/// are both transient
		///</summary>
		public override string BusinessKey
		{
			get { return Name; }
		}

		/// <summary>
		/// Name of the cat
		/// </summary>
		public virtual string Name { get; set; }

		#endregion
	}
}