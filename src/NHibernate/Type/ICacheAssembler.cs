using NHibernate.Engine;

namespace NHibernate.Type
{
	public interface ICacheAssembler
	{
		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.Disassemble"]/*'
		/// /> 
		object Disassemble(object value, ISessionImplementor session);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.Assemble"]/*'
		/// /> 
		object Assemble(object cached, ISessionImplementor session, object owner);
	}
}