using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.CompositeElementManyToOneWithFormulaInSet
{
	public class Person : EqualityAndHashCodeProvider<Person, Guid>
	{
		#region Instance Variables (4) 

		private readonly ISet<Todo> todos;
		private readonly IList<Stuff> myStuff;

		#endregion Instance Variables 

		#region Public Properties (8) 
		

		public override Guid Id { get; protected set; }


		public virtual string Name { get; set; }

		public virtual string NickName { get; set; }

		public virtual Person Partner { get; set; }

		public virtual IEnumerable<Todo> Todos { get { return todos; } }
		public virtual IEnumerable<Stuff> MyStuff { get { return myStuff; } }

		#endregion Public Properties 

		#region Constructors (2) 

		protected Person()
		{
			todos = new HashedSet<Todo>();
			myStuff = new List<Stuff>();
		}

		public Person(string name):this()
		{
			Name = name;
		}

		#endregion Constructors 

		#region Public Methods (4) 

		public virtual Todo AddTodo(Todo todo)
		{
			todos.Add(todo);
			return todo;
		}

		#endregion Public Methods 

   

		public virtual Stuff AddStuff(Stuff stuff)
		{
			myStuff.Add(stuff);
			return stuff;
		}

		public virtual void RemoveTodo(Todo myTodo)
		{
			todos.Remove(myTodo);
		}

		public virtual void RemoveStuff(Stuff stuff)
		{
			myStuff.Remove(stuff);
		}
	}
}