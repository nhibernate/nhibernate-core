namespace NHibernate.Validator.Tests.Validators
{
	using System;
	using NUnit.Framework;

    /// <summary>
    /// Fixture to validate the existing validators
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

		[Test]
		public void PastAndFuture()
		{
			ClassValidator validator = new ClassValidator(typeof(FooDate));
			FooDate f = new FooDate();
			
			f.Past = DateTime.MinValue;
			f.Future = DateTime.MaxValue;
			Assert.AreEqual(0, validator.GetInvalidValues(f).Length);

			f.Future = DateTime.Today.AddDays(1); //tomorrow
			f.Past = DateTime.Today.AddDays(-1); //yesterday
			Assert.AreEqual(0, validator.GetInvalidValues(f).Length);

			f.Future = DateTime.Today.AddDays(-1); //yesterday
			Assert.AreEqual(1, validator.GetInvalidValues(f).Length);

			f.Future = DateTime.Today.AddDays(1); //tomorrow
			f.Past = DateTime.Today.AddDays(1); //tomorrow
			Assert.AreEqual(1, validator.GetInvalidValues(f).Length);

			f.Future = DateTime.Now;
			f.Past = DateTime.Now;
			Assert.AreEqual(2, validator.GetInvalidValues(f).Length);
		}


    }
}