using System;
using System.Collections.Generic;

namespace Meme_Economy_backend
{
	class User
	{
		public string username { get; private set; }
		public double bank { get; private set; }
		public LinkedList<Purchase> purchases { get; private set; }

		public User(string username)
		{
			this.username = username;
			bank = 1000;
			purchases = new LinkedList<Purchase>();
		}

		public void update()
		{
			foreach (Purchase p in purchases)
				p.update();
		}

		public void make_purchase(string segment_url, int shares)
		{
			int id;
			bool repeat = false;
			Random rand = new Random();
			do
			{
				id = rand.Next(int.MinValue, int.MaxValue);
				foreach (Purchase p in purchases)
					if (id == p.purchase_id || id == 0)
					{
						repeat = true;
						break;
					}
			} while (repeat);
			var new_purchase = new Purchase(segment_url, shares, id);
			if (bank - new_purchase.value() < 0)
				throw new Purchase_Exception("Not enough money");
			bank -= new_purchase.value();
			purchases.AddLast(new_purchase);
		}

		public void sell_purchase(int purchase_id)
		{
			foreach (Purchase p in purchases)
				if (p.purchase_id == purchase_id)
				{
					bank += p.value();
					purchases.Remove(p);
					return;
				}
			throw new ID_Exception("Purchase ID not found");
		}

		public void print_purchases()
		{
			foreach (Purchase p in purchases)
				Console.WriteLine(
					$"URL: {p.post_url}, SHARES: {p.shares}, MULTIPLIER: {p.multiplier()}, "
					+ $"VALUE: {p.formatted_value()}, TIME: {p.time_since_posted()}");
		}
	}

	class Purchase_Exception: Exception
	{
		public Purchase_Exception(string msg): base(msg) { }
	}

	class ID_Exception: Exception
	{
		public ID_Exception(string msg): base(msg) { }
	}
}
