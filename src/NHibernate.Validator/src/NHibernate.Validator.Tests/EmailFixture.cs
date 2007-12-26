namespace NHibernate.Validator.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class EmailFixture
	{
		private ClassValidator userValidator;

		[Test]
		public void testEmail()
		{
			userValidator = new ClassValidator(typeof(User));
			
			isRightEmail("emmanuel@hibernate.org");
			isRightEmail("");
			isRightEmail(null);
			isWrongEmail("emmanuel.hibernate.org");
			isRightEmail("emmanuel@hibernate");
			isRightEmail("emma-n_uel@hibernate");
			isWrongEmail("emma nuel@hibernate.org");
			isWrongEmail("emma(nuel@hibernate.org");
			isWrongEmail("emmanuel@");
			isRightEmail("emma+nuel@hibernate.org");
			isRightEmail("emma=nuel@hibernate.org");
			isWrongEmail("emma\nnuel@hibernate.org");
			isWrongEmail("emma@nuel@hibernate.org");
			isRightEmail("emmanuel@[123.12.2.11]");
		}

		private void isRightEmail(string email)
		{
			Assert.AreEqual(0, userValidator.GetPotentialInvalidValues("email", email).Length, "Wrong email");
		}

		private void isWrongEmail(string email)
		{
			Assert.AreEqual(1, userValidator.GetPotentialInvalidValues("email", email).Length, "Right email");
		}
	}
}