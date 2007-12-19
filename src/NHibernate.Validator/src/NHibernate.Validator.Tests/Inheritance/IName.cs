namespace NHibernate.Validator.Tests.Inheritance
{
    public interface IName
    {
        [NotNull]
        string Name { get; set; }
    }
}