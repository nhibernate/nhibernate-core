namespace NHibernate.Test.Extralazy
{
	public class SessionAttribute
	{
#pragma warning disable 169
		private long id;
#pragma warning restore 169
		private string name;
		private string stringData;
		private object objectData;
		protected SessionAttribute() {}
		public SessionAttribute(string name, string stringData)
		{
			this.name = name;
			this.stringData = stringData;
		}

		public SessionAttribute(string name, object objectData)
		{
			this.name = name;
			this.objectData = objectData;
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual string StringData
		{
			get { return stringData; }
			set { stringData = value; }
		}

		public virtual object ObjectData
		{
			get { return objectData; }
			set { objectData = value; }
		}
	}
}