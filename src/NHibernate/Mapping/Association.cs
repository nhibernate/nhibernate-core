using NHibernate.Loader;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public abstract class Association : Value
	{
		private OuterJoinLoaderType joinedFetch;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		protected Association( Table table ) : base( table )
		{
		}

		/// <summary></summary>
		public override OuterJoinLoaderType OuterJoinFetchSetting
		{
			get { return joinedFetch; }
			set { joinedFetch = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyClass"></param>
		/// <param name="propertyName"></param>
		public abstract override void SetTypeByReflection( System.Type propertyClass, string propertyName );

		/// <summary></summary>
		public abstract override void CreateForeignKey();

	}
}