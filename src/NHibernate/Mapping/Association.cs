using System;
using NHibernate.Loader;

namespace NHibernate.Mapping 
{
	
	public abstract class Association : Value 
	{
		private OuterJoinLoaderType joinedFetch;

		protected Association(Table table) : base(table) {}

		public override OuterJoinLoaderType OuterJoinFetchSetting 
		{
			get { return joinedFetch; }
			set { joinedFetch = value; }
		}

		public abstract override void SetTypeByReflection(System.Type propertyClass, string propertyName);
		public abstract override void CreateForeignKey();

	}
}
