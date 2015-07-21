using System;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmOneToMany: IRelationship
	{
		#region Implementation of IRelationship

		public string EntityName
		{
			get { return entityname; }
		}

		public string Class
		{
			get { return @class; }
		}

		public HbmNotFoundMode NotFoundMode
		{
			get { return notfound; }
		}

		#endregion	
	}
}