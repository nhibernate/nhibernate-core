using System.Collections;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class OneToOne : Association
	{
		private bool constrained;
		private ForeignKeyType foreignKeyType;
		private Value identifier;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		/// <param name="identifier"></param>
		public OneToOne( Table table, Value identifier ) : base( table )
		{
			this.identifier = identifier;
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
					Type = TypeFactory.OneToOne( ReflectHelper.GetGetter( propertyClass, propertyName ).ReturnType, foreignKeyType );
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
			if( constrained )
			{
				CreateForeignKeyOfClass( ( ( EntityType ) Type ).PersistentClass );
			}
		}

		/// <summary></summary>
		public override IList ConstraintColumns
		{
			get
			{
				ArrayList list = new ArrayList();
				foreach( object obj in identifier.ColumnCollection )
				{
					list.Add( obj );
				}
				return list;
			}
		}

		/// <summary></summary>
		public bool IsConstrained
		{
			get { return constrained; }
			set { constrained = value; }
		}

		/// <summary></summary>
		public ForeignKeyType ForeignKeyType
		{
			get { return foreignKeyType; }
			set { foreignKeyType = value; }
		}

		/// <summary></summary>
		public Value Identifier
		{
			get { return identifier; }
			set { identifier = value; }
		}


	}
}