using System;
using Nancy;
using Newtonsoft.Json;
using System.Text;

namespace Meme_Economy_backend
{
	public class RootRoutes : NancyModule
	{
		public RootRoutes()
		{
			Get["/{flag}/username={username}/url-segment={url_segment}/shares={shares}"]
				= Service;

			Get["/{flag}/username={username}"] = User_Info;
		}

		dynamic User_Info(dynamic parameters)
		{
			return service_handler(parameters.flag, parameters.username, "NA", 0);
		}

		dynamic Service(dynamic parameters)
		{
			return service_handler(parameters.flag, parameters.username, ((string)parameters.url_segment).Replace("|", "/"), parameters.shares);
		}

		string service_handler(string flag, string username, string url_segment, int shares)
		{
			Console.WriteLine($"{username} makes a <{flag}> operation");

			User user = null;
			bool user_exist = true;
			double bank = -1;
			bool changed = false;
			Purchase_Json[] purchases_arr;
			var json_obj = new Json_Output();
			bool post = (flag == "post");
			try
			{
				if (flag == "add") user_exist = Program.add(username);
				if (flag == "remove") user_exist = Program.remove(username);
				Program.update(username);
				if (flag == "buy") changed = Program.buy(username, url_segment, shares);
				else if (flag == "sell") changed = Program.sell(username, shares);
				else if ((flag != "request") && !post) return "Invalid request";
				user = Program.get_user_info(username);
			}
			catch (Username_Exception)
			{
				user_exist = false;
			}
			if (user_exist)
			{
				bank = user.bank;
				if (post)
				{
					var p = new Purchase(url_segment, shares, 0);
					purchases_arr = new Purchase_Json[1];
					purchases_arr[0] = new Purchase_Json();
					purchases_arr[0].purchase_id = p.purchase_id;
					purchases_arr[0].segment_url = p.segment_url;
					purchases_arr[0].score = p.current_score;
					purchases_arr[0].time = p.time_since_posted();
					purchases_arr[0].shares = p.shares;
					purchases_arr[0].data_available = p.data_available;
					purchases_arr[0].multiplier = p.multiplier();
					purchases_arr[0].purchased_price_per_share = p.price_per_share;
					purchases_arr[0].value = p.value();
				}
				else
				{
					purchases_arr = new Purchase_Json[user.purchases.Count];
					int i = 0;
					foreach (Purchase p in user.purchases)
					{
						purchases_arr[i] = new Purchase_Json();
						purchases_arr[i].purchase_id = p.purchase_id;
						purchases_arr[i].segment_url = p.segment_url;
						purchases_arr[i].score = p.current_score;
						purchases_arr[i].time = p.time_since_posted();
						purchases_arr[i].shares = p.shares;
						purchases_arr[i].data_available = p.data_available;
						purchases_arr[i].multiplier = p.multiplier();
						purchases_arr[i].purchased_price_per_share = p.price_per_share;
						purchases_arr[i].value = p.value();
						i++;
					}
				}
				json_obj.username = username;
				json_obj.user_exist = true;
			}
			else
			{
				json_obj.username = "[Invalid]";
				json_obj.user_exist = false;
				purchases_arr = new Purchase_Json[0];
			}

			json_obj.changed = changed;
			json_obj.bank = bank;
			json_obj.purchase_arr = purchases_arr;
			return JsonConvert.SerializeObject(json_obj);
		}
	}
}
