namespace NHibernate.Test.NHSpecificTest.NH3069
{
    public abstract class VersionableAbstract
    {
        public long Id { get; set; }
        public int Version { get; set; }
    }

    public class VersionableConcreate : VersionableAbstract
    {
        public string Name { get; set; }
    }
}
