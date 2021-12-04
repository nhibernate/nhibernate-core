namespace NHibernate.Test.OneToOneType
{
	public class Owner
	{
		private Details _details;

		public virtual int Id { get; protected set; }
		public virtual string Name { get; set; }
		public virtual Details Details
		{
			get
			{
				return _details;
			}
			set
			{
				_details = value;

				if (_details != null)
				{
					_details.Owner = this;
				}
			}
		}
	}
}
