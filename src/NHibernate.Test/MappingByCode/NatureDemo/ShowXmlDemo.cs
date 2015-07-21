using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Test.MappingByCode.NatureDemo.Naturalness;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.NatureDemo
{
	public class ShowXmlDemo
	{
		[Test, Explicit]
		public void MappingAndShowXmlInConsole()
		{
			Console.Write(GetMapping().AsString());
		}

		public static HbmMapping GetMapping()
		{
			var mapper = new ModelMapper();

			mapper.Component<Address>(comp =>
			{
				comp.Property(address => address.Street);
				comp.Property(address => address.City);
				comp.Property(address => address.PostalCode);
				comp.Property(address => address.Country);
				comp.ManyToOne(address => address.StateProvince);
			});

			mapper.Class<Animal>(rc => 
			{
				rc.Id(x => x.Id, map => map.Generator(Generators.Native));

				rc.Property(animal => animal.Description);
				rc.Property(animal => animal.BodyWeight);
				rc.ManyToOne(animal => animal.Mother);
				rc.ManyToOne(animal => animal.Father);
				rc.ManyToOne(animal => animal.Zoo);
				rc.Property(animal => animal.SerialNumber);
				rc.Set(animal => animal.Offspring, cm => cm.OrderBy(an => an.Father), rel => rel.OneToMany());
			});

			mapper.JoinedSubclass<Reptile>(jsc => { jsc.Property(reptile => reptile.BodyTemperature); });

			mapper.JoinedSubclass<Lizard>(jsc => { });

			mapper.JoinedSubclass<Mammal>(jsc =>
			{
				jsc.Property(mammal => mammal.Pregnant);
				jsc.Property(mammal => mammal.Birthdate);
			});

			mapper.JoinedSubclass<DomesticAnimal>(jsc =>
			                                      {
			                                      	jsc.ManyToOne(domesticAnimal => domesticAnimal.Owner);
			                                      });

			mapper.JoinedSubclass<Cat>(jsc => { });

			mapper.JoinedSubclass<Dog>(jsc => { });

			mapper.JoinedSubclass<Human>(jsc =>
			{
				jsc.Component(human => human.Name, comp =>
				{
					comp.Property(name => name.First);
					comp.Property(name => name.Initial);
					comp.Property(name => name.Last);
				});
				jsc.Property(human => human.NickName);
				jsc.Property(human => human.Height);
				jsc.Property(human => human.IntValue);
				jsc.Property(human => human.FloatValue);
				jsc.Property(human => human.BigDecimalValue);
				jsc.Property(human => human.BigIntegerValue);
				jsc.Bag(human => human.Friends, cm => { }, rel => rel.ManyToMany());
				jsc.Map(human => human.Family, cm => { }, rel => rel.ManyToMany());
				jsc.Bag(human => human.Pets, cm => { cm.Inverse(true); }, rel => rel.OneToMany());
				jsc.Set(human => human.NickNames, cm =>
				{
					cm.Lazy(CollectionLazy.NoLazy);
					cm.Sort();
				}, cer => { });
				jsc.Map(human => human.Addresses, cm => { }, rel => rel.Component(comp => { }));
			});

			mapper.Class<User>(rc =>
			{
				rc.Id(u => u.Id, im => im.Generator(Generators.Foreign<User>(u => u.Human)));

				rc.Property(user => user.UserName);
				rc.OneToOne(user => user.Human, rm => rm.Constrained(true));
				rc.List(user => user.Permissions, cm => { }, cer => { });
			});

			mapper.Class<Zoo>(rc =>
			{
				rc.Id(x => x.Id, map => map.Generator(Generators.Native));
				rc.Property(zoo => zoo.Name);
				rc.Property(zoo => zoo.Classification);
				rc.Map(zoo => zoo.Mammals, cm => { }, rel => rel.OneToMany());
				rc.Map(zoo => zoo.Animals, cm => { cm.Inverse(true); }, rel => rel.OneToMany());
				rc.Component(zoo => zoo.Address, comp => { });
			});

			mapper.Subclass<PettingZoo>(sc => { });

			mapper.Class<StateProvince>(rc =>
			{
				rc.Id(x => x.Id, map => map.Generator(Generators.Native));
				rc.Property(sp => sp.Name);
				rc.Property(sp => sp.IsoCode);
			});
			return mapper.CompileMappingFor(typeof (Animal).Assembly.GetTypes().Where(t => t.Namespace == typeof (Animal).Namespace));
		}
	}
}