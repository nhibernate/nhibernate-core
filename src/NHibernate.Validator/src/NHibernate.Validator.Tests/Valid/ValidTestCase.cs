namespace NHibernate.Validator.Tests.Valid
{
    using NUnit.Framework;

    [TestFixture]
    public class ValidTest
    {
        [Test]
        public void testDeepValid()
        {
            ClassValidator formValidator = new ClassValidator(typeof(Form));

            Address a = new Address();
            Member m = new Member();
            m.Address = a;
            Form f = new Form();
            f.Member = m;
            InvalidValue[] values = formValidator.GetInvalidValues(f);
            Assert.AreEqual(1, values.Length);

            m.Address.City = "my city";
            InvalidValue[] values2 = formValidator.GetInvalidValues(f);
            Assert.AreEqual(0, values2.Length);
        }
    }
}