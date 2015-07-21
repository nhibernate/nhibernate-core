using System;

namespace NHibernate.Test.NHSpecificTest.NH2266
{
	public abstract class Token { public virtual int Id { get; set; } }

	public class SecurityToken : Token { public virtual string Owner { get; set; } }

	public abstract class TemporaryToken : Token { public virtual DateTime ExpiryDate { get; set; } } 
}