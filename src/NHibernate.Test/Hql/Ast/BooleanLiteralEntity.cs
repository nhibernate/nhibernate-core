namespace NHibernate.Test.Hql.Ast
{
	public class BooleanLiteralEntity
	{
		public virtual long Id { get; set; }
		public virtual bool YesNoBoolean { get; set; }
		public virtual bool TrueFalseBoolean { get; set; }
		public virtual bool ZeroOneBoolean { get; set; }
	}
}