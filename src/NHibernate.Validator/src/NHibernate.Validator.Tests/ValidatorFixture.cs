namespace NHibernate.Validator.Tests
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using System.Resources;
	using NUnit.Framework;

	[TestFixture]
	public class ValidatorFixture
	{
		public static readonly string ESCAPING_EL = "(escaping #{el})";

		[Test]
		public void AddressValid()
		{
			Address a = new Address();
			Address.blacklistedZipCode = null;
			a.Country = "Australia";
			a.Zip = "1221341234123";
			a.State = "Vic";
			a.Line1 = "Karbarook Ave";
			a.Id = 3;
			ClassValidator classValidator = new ClassValidator(typeof(Address),
			                                                   new ResourceManager(
			                                                   	"NHibernate.Validator.Tests.Resource.Messages",
			                                                   	Assembly.GetExecutingAssembly()),
			                                                   new CultureInfo("en"));
			InvalidValue[] validationMessages = classValidator.GetInvalidValues(a);
			Assert.AreEqual(2, validationMessages.Length); //static field is tested also
			Address.blacklistedZipCode = "323232";
			a.Zip = null;
			a.State = "Victoria";
			validationMessages = classValidator.GetInvalidValues(a);
			Assert.AreEqual(2, validationMessages.Length);
			//validationMessages = classValidator.GetInvalidValues(a, "zip");
			//Assert.AreEqual(1, validationMessages.Length);
			a.Zip = "3181";
			a.State = "NSW";
			validationMessages = classValidator.GetInvalidValues(a);
			Assert.AreEqual(0, validationMessages.Length);
			a.Country = null;
			validationMessages = classValidator.GetInvalidValues(a);
			Assert.AreEqual(1, validationMessages.Length);
			a.InternalValid = false;
			validationMessages = classValidator.GetInvalidValues(a);
			Assert.AreEqual(2, validationMessages.Length);
			a.InternalValid = true;
			a.Country = "France";
			a.floor = 4000;
			validationMessages = classValidator.GetInvalidValues(a);
			Assert.AreEqual(1, validationMessages.Length);
			Assert.AreEqual("Floor cannot " + ESCAPING_EL + " be lower that -2 and greater than 50 " + ESCAPING_EL,
			                validationMessages[0].Message);
		}

		[Test]
		public void Circularity()
		{
			Brother emmanuel = new Brother();
			emmanuel.Name ="Emmanuel";
			Address.blacklistedZipCode = "666";
			Address address = new Address();
			address.InternalValid = true;
			address.Country = "France";
			address.Id = 3;
			address.Line1 = "Rue des rosiers";
			address.State = "NYC";
			address.Zip = "33333";
			address.floor = 4;
			emmanuel.Address = address;
			Brother christophe = new Brother();
			christophe.Name = "Christophe" ;
			christophe.Address = address;
			emmanuel.YoungerBrother = christophe;
			christophe.Elder= emmanuel;
			ClassValidator classValidator = new ClassValidator(typeof(Brother));
			InvalidValue[] invalidValues = classValidator.GetInvalidValues(emmanuel);
			Assert.AreEqual(0, invalidValues.Length);
			christophe.Name = null;
			invalidValues = classValidator.GetInvalidValues(emmanuel);
			Assert.AreEqual(1, invalidValues.Length);
			Assert.AreEqual(emmanuel, invalidValues[0].RootBean);
			Assert.AreEqual("YoungerBrother.Name", invalidValues[0].PropertyPath);
			christophe.Name = "Christophe";
			address = new Address();
			address.InternalValid = true;
			address.Country="France";
			address.Id = 4;
			address.Line1="Rue des plantes";
			address.State="NYC";
			address.Zip="33333";
			address.floor = -100;
			christophe.Address=address;
			invalidValues = classValidator.GetInvalidValues(emmanuel);
			Assert.AreEqual(1, invalidValues.Length);
		}

		[Test]
		public void BeanValidator()
		{
			Suricato s = new Suricato();
			ClassValidator vtor = new ClassValidator(typeof(Suricato));

			Assert.IsTrue(vtor.HasValidationRules);
			Assert.AreEqual(1,vtor.GetInvalidValues(s).Length);
		}

		/// <summary>
		/// Test duplicates annotations.
		/// Hibernate.Validator has diferent behavior: Aggregate Annotations
		/// <example>
		/// example for the field 'info'.
		/// </example>
		/// <code>
		/// [Pattern(Regex = "^[A-Z0-9-]+$", Message = "must contain alphabetical characters only")]
		///	[Pattern(Regex = "^....-....-....$", Message = "must match ....-....-....")]
		/// private string info;
		/// </code> 
		/// </summary>
		[Test]
		public void DuplicateAnnotations()
		{
			Engine eng = new Engine();
			eng.HorsePower = 23;
			eng.SerialNumber = "23-43###4";
			ClassValidator classValidator = new ClassValidator(typeof(Engine));
			InvalidValue[] invalidValues = classValidator.GetInvalidValues(eng);
			Assert.AreEqual( 2, invalidValues.Length);

			//This cannot be tested, the order is random
			//Assert.AreEqual("must contain alphabetical characters only", invalidValues[0].Message);
			//Assert.AreEqual("must match ....-....-....", invalidValues[1].Message);
			//Instead of that, I do this:
			List<string> list_invalidValues = new List<string>();
			list_invalidValues.Add(invalidValues[0].Message);
			list_invalidValues.Add(invalidValues[1].Message);
			Assert.IsTrue(list_invalidValues.Contains("must contain alphabetical characters only"));
			Assert.IsTrue(list_invalidValues.Contains("must match ....-....-...."));

			
			eng.SerialNumber =  "1234-5678-9012";
			invalidValues = classValidator.GetInvalidValues(eng);
			Assert.AreEqual(0, invalidValues.Length);
		}
	}
}