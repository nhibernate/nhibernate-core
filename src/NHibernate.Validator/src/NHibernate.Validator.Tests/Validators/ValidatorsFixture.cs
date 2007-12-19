namespace NHibernate.Validator.Tests.Validators
{
    using NUnit.Framework;

    /// <summary>
    /// Fixture to validate the diferent existing validators
    /// </summary>
    [TestFixture]
    public class ValidatorsFixture
    {
        [Test]
        public void LengthTest()
        {
            FooLength f1 = new FooLength(1,"hola");
            ClassValidator validator = new ClassValidator(typeof(FooLength));

            InvalidValue[] res = validator.GetInvalidValues(f1);
            Assert.AreEqual(0, res.Length);
            
            FooLength f2 = new FooLength(1, string.Empty);

            InvalidValue[] res2 = validator.GetInvalidValues(f2);
            Assert.AreEqual(1, res2.Length);

            FooLength f3 = new FooLength(1, null);
            InvalidValue[] res3 = validator.GetInvalidValues(f3);
            Assert.AreEqual(1, res3.Length);
        }

        [Test]
        public void NotEmptyTest()
        {
            ClassValidator validator = new ClassValidator(typeof(FooNotEmpty));

            FooNotEmpty f1 = new FooNotEmpty("hola");
            FooNotEmpty f2 = new FooNotEmpty(string.Empty);

            validator.AssertValid(f1);

            InvalidValue[] res = validator.GetInvalidValues(f2);
            Assert.AreEqual(1, res.Length);
        }

        //[Test]
        //public void MinTest()
        //{
        //}
    }
}