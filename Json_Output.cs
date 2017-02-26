using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meme_Economy_backend
{
	class Purchase_Json
	{
		public int purchase_id;
		public string segment_url;
		public int score;
		public string time;
		public int shares;
		public bool data_available;
		public double multiplier;
		public double purchased_price_per_share;
		public double value;

		public Purchase_Json() { }
	}

	class Json_Output
	{
		public string username;
		public bool user_exist;
		public double bank;
		public bool changed = false;
		public Purchase_Json[] purchase_arr;

		public Json_Output() { }
	
		public string to_json()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
