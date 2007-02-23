using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class ManyToOne : ToOne
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public ManyToOne(Table table) : base(table)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyClass"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyAccess"></param>
		public override void SetTypeByReflection(System.Type propertyClass, string propertyName, string propertyAccess)
		{
			try
			{
				if (Type == null)
				{
					Type = TypeFactory.ManyToOne(
						ReflectHelper.ReflectedPropertyClass(propertyClass, propertyName, propertyAccess),
						ReferencedPropertyName, IsLazy, isIgnoreNotFound);
				}
			}
			catch (HibernateException he)
			{
				throw new MappingException("Problem trying to set association type by reflection", he);
			}
		}

		/// <summary></summary>
		public override void CreateForeignKey()
		{
			// TODO: Handle the case of a foreign key to something other than the pk
			// NH-268 isIgnoreNotFound work with <not-found> tag to determine the cration of FK
			if (ReferencedPropertyName == null && !isIgnoreNotFound)
			{
				CreateForeignKeyOfClass(((EntityType) Type).AssociatedClass);
			}
		}

		private bool isIgnoreNotFound = false; // NH-268

		public bool IsIgnoreNotFound
		{
			get { return isIgnoreNotFound; }
			set { isIgnoreNotFound = value; }
		}
	}
}