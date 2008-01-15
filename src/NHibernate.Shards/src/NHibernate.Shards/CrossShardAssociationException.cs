using System;
using System.Runtime.Serialization;

namespace NHibernate.Shards
{
	/// <summary>
	/// Exception thrown when someone attempts to create a cross-shard association.
	/// Here's an example of a cross-shard association.  Let's say we have
	/// AccountManager and Account.  There is an owned, one-to-many relationship
	/// between AccountManagers and Accounts, and we are sharding by AccountManager.
	/// That means an AccountManager and all her Accounts live on the same shard.
	/// Now suppose you did the following:
	/// <code>
	/// public void reassignLeastProfitableAccount(AccountManager mgr1, AccountManager mgr2) 
	/// {
	///		Account acct = mgr1.removeLeastProfitableAccount();
	///		acct.setAccountManager(mgr2);
	///		mgr2.addAccount(acct);
	/// }
	/// </code>
	/// If the 2 managers happen to live on different shards and you were to then
	/// attempt to save the second manager you would receive a
	/// CrossShardAssociationException because the account lives on a different shard
	/// than the manager with which you're attempting to associate it.
	///
	/// Now you'll notice a few things about this example.  First, it doesn't really
	/// respect the constraints of an owned one-to-many relationship.  If AccountManagers
	/// truly own Accounts (as opposed to just being associated with them), it doesn't
	/// makes sense to reassign an account because the AccountManager is part of that
	/// object's identity.  And If the relationship is an association then you
	/// probably shouldn't be using Hibernate Sharding to manage this relationship
	/// because Accounts are going to pass between AccountManagers, which means
	/// Accounts are going to need to pass between shards, which means you're better
	/// off just letting Hibernate manage the relationship between AccountManagers
	/// and account ids and loading the objects uniquely identified by those ids
	/// on your own.
	///
	/// The other thing you'll notice is that if the two managers happen to live on
	/// the same shard this will work just fine.  Yup, it will.  We can detect
	/// cross-shard relationships.  We can't detect risky code.  You just need to
	/// be careful here.
	/// </summary>
	public class CrossShardAssociationException : HibernateException
	{
		public CrossShardAssociationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public CrossShardAssociationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public CrossShardAssociationException(Exception innerException) : base(innerException)
		{
		}

		public CrossShardAssociationException(string message) : base(message)
		{
		}

		public CrossShardAssociationException()
		{
		}
	}
}