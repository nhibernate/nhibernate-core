using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class ManyToOne : Association
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public ManyToOne( Table table ) : base( table )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyClass"></param>
		/// <param name="propertyName"></param>
		public override void SetTypeByReflection( System.Type propertyClass, string propertyName )
		{
			try
			{
				if( Type == null )
				{
					Type = TypeFactory.ManyToOne( ReflectHelper.GetGetter( propertyClass, propertyName ).ReturnType );
				}
			}
			catch( HibernateException he )
			{
				throw new MappingException( "Problem trying to set association type by reflection", he );
			}
		}

		/// <summary></summary>
		public override void CreateForeignKey()
		{
			CreateForeignKeyOfClass( ( ( EntityType ) Type ).PersistentClass );
		}
	}
}