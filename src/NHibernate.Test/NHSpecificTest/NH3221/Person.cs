using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3221
{
	public class Person : EqualityAndHashCodeProvider<Person, Guid>
	{
		private readonly ISet<Todo> todos;

		private readonly IList<Stuff> myStuff;

		public override Guid Id { get; protected set; }

		public virtual string Name { get; set; }

		public virtual string NickName { get; set; }

		public virtual Person Partner { get; set; }

		public virtual IEnumerable<Todo> Todos { get { return todos; } }

		public virtual IEnumerable<Stuff> MyStuff { get { return myStuff; } }

		protected Person()
		{
			todos = new HashSet<Todo>();
			myStuff = new List<Stuff>();
		}

		public Person(string name):this()
		{
			Name = name;
		}

		public virtual Todo AddTodo(Todo todo)
		{
			todos.Add(todo);
			return todo;
		}

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