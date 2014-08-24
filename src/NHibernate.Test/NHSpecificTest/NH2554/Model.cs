using System;

namespace NHibernate.Test.NHSpecificTest.NH2554
{
	public class Student
	{
		public virtual Guid Id { get; set; }
		public virtual string FullName { get; set; }
		public virtual byte[] FullNameAsVarBinary { get; set; }
		public virtual byte[] FullNameAsVarBinary512 { get; set; }
		public virtual byte[] FullNameAsBinary { get; set; }
		public virtual byte[] FullNameAsBinary256 { get; set; }
		public virtual string FullNameAsVarChar { get; set; }
		public virtual string FullNameAsVarChar125 { get; set; }
	}
}
