using System;
using Nancy;
using Nancy.Hosting.Self;
using System.Collections.Generic;

namespace Meme_Economy_backend
{
	class Program
	{
		public static DB db = new DB();

		static void Main(string[] args)
		{
			db.add_user("Etirps");
			db.user("Etirps").make_purchase("/r/funny/comments/5w91rc/what_it_feels_like_to_get_scammed/", 1);

			string url = "https://localhost:4443";
			HostConfiguration hc = new HostConfiguration();
			hc.UrlReservations.CreateAutomatically = true;
			using (var host = new NancyHost(hc, new Uri(url)))
			{
				StaticConfiguration.DisableErrorTraces = false;
				host.Start();
				Console.WriteLine($"Starting server at {url}");
				Console.ReadLine();
			}
		}

		public static User get_user_info(string username)
		{
			foreach (User u in db.users)
			{
				if (u.username == username)
				{
					u.print_purchases();
					return u;
				}
			}
			throw new Username_Exception("User not found");
		}

		public static void update(string username)
		{
			foreach (User u in db.users)
				if (u.username == username)
				{
					u.update();
					return;
				}
			throw new Username_Exception("User not found");
		}

		public static bool buy(string username, string url, int shares)
		{
			foreach (User u in db.users)
				if (u.username == username)
				{
					try
					{
						u.make_purchase(url, shares);
						return true;
					}
					catch (Purchase_Exception) { }
					return false;
				}
			throw new Username_Exception("User not found");
		}

		public static bool sell(string username, int purchase_id)
		{
			foreach (User u in db.users)
				if (u.username == username)
				{
					try
					{
						u.sell_purchase(purchase_id);
						return true;
					}
					catch (Purchase_Exception) { }
					return false;
				}
			throw new Username_Exception("User not found");
		}

		public static bool add(string username)
		{
			try
			{
				db.add_user(username);
				return true;
			}
			catch (Username_Exception) { }
			return false;
		}

		public static bool remove(string username)
		{
			return db.remove_user(username);
		}
	}
}
