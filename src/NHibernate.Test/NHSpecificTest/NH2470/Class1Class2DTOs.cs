namespace NHibernate.Test.NHSpecificTest.NH2470
{
	public class Class2DTO : DTO { }

    public class Class1DTO : DTO
    {
        public Class2DTO[] Class2Ary { get; set; }
    }
}