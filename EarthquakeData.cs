using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Earthquake_WPFApp {
    class EarthquakeData {

        /// <summary>
        /// 地震情報を取得するためのインスタンス
        /// </summary>
        public static List<EarthquakeData>? GetInstance() {
            using HttpClient client = new HttpClient();
            string url = "https://api.p2pquake.net/v2/history?codes=551";
            HttpResponseMessage responseMessage = client.GetAsync(url).Result;
            if (responseMessage.IsSuccessStatusCode) {
                string jsonRaw = responseMessage.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<EarthquakeData>>(jsonRaw);
            }
            return null;
        }


        public Issue issue { get; set; }
        public Earthquake earthquake { get; set; }
        public Point[]? points { get; set; }

        public class Earthquake {
            public string domesticTsunami { get; set; }
            public string time { get; set; }
            public int maxScale { get; set; }
            public Hypocenter hypocenter { get; set; }

            public class Hypocenter {
                public int depth { get; set; }
                public float latitude { get; set; }
                public float longitude { get; set; }
                public float magnitude { get; set; }
                public string name { get; set; }
            }
        }

        public class Point {
            public string addr;
            public string pref;
            public int scale;
        }

        public class Issue {
            public string time { get; set; }
            public string type { get; set; }
        }
    }
}
