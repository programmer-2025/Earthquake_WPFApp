using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
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


        /// <summary>
        /// APIのデータを、色に変換する関数（※参考：気象庁）
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ConvertScaleColor(int color) {
            if (color == 10) return Color.Gray;         // 震度1
            if (color == 20) return Color.AliceBlue;    // 震度2
            if (color == 30) return Color.Blue;         // 震度3
            if (color == 40) return Color.Yellow;       // 震度4
            if (color == 45) return Color.Yellow;       // 震度5弱
            if (color == 50) return Color.Orange;       // 震度5強
            if (color == 55) return Color.Red;          // 震度6弱
            if (color == 60) return Color.DarkRed;      // 震度6強
            if (color == 65) return Color.Purple;       // 震度7
            return Color.White;                         // その他
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
