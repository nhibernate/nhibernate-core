using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2245
{
public class Foo 
{ 
	public Foo() {}
	public virtual Guid Id { get; protected set; } 
	public virtual string Name {get; set;} 
	public virtual string Description {get; set;} 
	public virtual int    Version{get; set;} 
} 

}