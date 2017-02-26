using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Meme_Economy_backend
{
	class Purchase
	{
		public int purchase_id { get; private set; }
		public string segment_url { get; set; }
		public string post_url { get; private set; }
		public int shares { get; private set; }
		public int current_score { get; private set; }
		public bool data_available { get; set; }
		public double price_per_share { get; set; }

		double total_value;
		double value_multiplier;
		double previous_multiplier;
		int initial_score;
		double initial_hours;
		double current_hours;
		JArray post_json;

		const double LOG_BASE = 1.00008;

		public Purchase(string segment_url, int shares, int purchase_id)
		{
			this.purchase_id = purchase_id;
			this.shares = shares;
			this.segment_url = segment_url;
			post_url = "https://www.reddit.com" + segment_url;
			fetch_json();
			update_hours();
			initial_hours = current_hours;
			update_score();
			initial_score = current_score;
			price_per_share = (initial_score / 2.86445) + 10;
			total_value = shares * price_per_share;
			value_multiplier = previous_multiplier = 1;
			data_available = false;
		}

		void fetch_json()
		{
			try
			{
				using (WebClient wc = new WebClient())
				{
					wc.Headers.Add("user-agent", 
						"Mozilla / 4.0(compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
					post_json = (JArray) JsonConvert.DeserializeObject(
						wc.DownloadString(post_url + ".json"));
				}
			}
			catch (WebException ex)
			{
				Console.WriteLine(ex.StackTrace);
			}
		}

		double update_hours()
		{
			current_hours = ((DateTime.UtcNow - new DateTime(
				1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
				- (double)post_json[0]["data"]["children"][0]["data"]["created_utc"])
				/ 3600;
			data_available = ((current_hours - initial_hours) > ((double)1 / 120));
			return current_hours;
		}

		int update_score()
		{
			current_score = (int)post_json[0]["data"]["children"][0]["data"]["score"];
			return current_score;
		}

		public void update()
		{
			fetch_json();
			update_hours();
			update_score();
			double temp = previous_multiplier;
			previous_multiplier = value_multiplier;
			value_multiplier = (0.7 * temp) + 0.3 * (15.63692 + 
				(-15.69055231 / (1 + Math.Pow(((current_score - initial_score) 
				/ (current_hours - initial_hours)) / 71973.32, 0.3968562)) 
				/ (Math.Sqrt(Math.Log10(initial_score)))));
		}

		public string formatted_value()
		{
			return string.Format("{0:0.00}", value());
		}

		public string time_since_posted()
		{
			string hr, min;
			if (((int)current_hours) == 1) hr = "hour";
			else hr = "hours";
			if (((int)((current_hours % 1) * 60)) == 1) min = "minute";
			else min = "minutes";
			return string.Format(
				$"{(int)current_hours} {hr} {(int)((current_hours % 1) * 60)} {min}");
		}

		public double value()
		{
			return (data_available) ?
				total_value * (((value_multiplier > 0) && (value_multiplier < 10)) 
					? value_multiplier : 0) : 0;
		}

		public double multiplier()
		{
			return (((value_multiplier > 0) && (value_multiplier < 10)) ? value_multiplier : 0) * 100;
		}
	}
}
