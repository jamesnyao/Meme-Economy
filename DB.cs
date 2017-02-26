using System;
using System.Collections.Generic;

namespace Meme_Economy_backend
{
	class DB
	{
		public LinkedList<User> users { get; private set; }

		public DB()
		{
			users = new LinkedList<User>();
		}

		public User user(string username)
		{
			foreach (User u in users)
				if (u.username == username) return u;
			throw new Username_Exception("User not found");
		}

		public void make_purchase(string username, string segment_url, int shares)
		{
			foreach (User u in users)
				if (u.username == username)
				{
					u.make_purchase(segment_url, shares);
					return;
				}
			throw new Username_Exception("User not found");
		}

		public void sell_purchase(string username, int purchase_id)
		{
			foreach (User u in users)
				if (u.username == username)
				{
					u.sell_purchase(purchase_id);
					return;
				}
			throw new Username_Exception("User not found");
		}

		public void add_user(string username)
		{
			if (!check_username_format(username))
				throw new Username_Exception("Invalid format");
			if (!check_username_dupe(username))
				throw new Username_Exception("Duplicate username");
			users.AddLast(new User(username));
		}

		public bool remove_user(string username)
		{
			foreach (User u in users)
				if (u.username == username)
				{
					users.Remove(u);
					return true;
				}
			return false;
		}

		bool check_username_format(string username)
		{
			if (username.Length > 16 && username.Length < 3) return false;
			foreach (char c in username)
				if (!(Char.IsLetter(c) || Char.IsDigit(c))) return false;
			return true;
		}

		bool check_username_dupe(string username)
		{
			foreach (User u in users)
				if (u.username == username) return false;
			return true;
		}
	}

	class Username_Exception : Exception
	{
		public Username_Exception(string msg) : base(msg) { }
	}
}
