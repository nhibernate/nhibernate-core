using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class IntegerValue : SimpleValue
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public IntegerValue( Table table ) : base( table )
		{
		}

		/// <summary></summary>
		public override IType Type
		{
			get { return NHibernateUtil.Int32; }
		}
	}
}