namespace NHibernate.Test.NHSpecificTest.NH3932.Model
{
	public interface IParent
	{
		IParent Clone();
		void ReverseChildren();
		void ClearChildren();
		void RemoveLastChild();
	}
}
