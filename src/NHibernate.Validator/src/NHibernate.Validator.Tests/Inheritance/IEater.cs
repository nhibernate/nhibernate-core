namespace NHibernate.Validator.Tests.Inheritance
{
    public interface IEater
    {
        [Min(2)]
        int Frequency { get; set; }
    }
}