using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text;

namespace Earthquake_WPFApp {

    /// <summary>
    /// 地震情報を取得するクラス
    /// </summary>
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

            /// <summary>
            /// 国内への津波の有無
            /// </summary>
            public string domesticTsunami { get; set; }

            /// <summary>
            /// 地震の発生日時
            /// </summary>
            public string time { get; set; }

            /// <summary>
            /// 地震の最大規模
            /// </summary>
            public int maxScale { get; set; }

            public Hypocenter hypocenter { get; set; }

            public class Hypocenter {

                /// <summary>
                /// 地震の深さ
                /// </summary>
                public int depth { get; set; }

                /// <summary>
                /// 地震の緯度
                /// </summary>
                public float latitude { get; set; }

                /// <summary>
                /// 地震の経度
                /// </summary>
                public float longitude { get; set; }

                /// <summary>
                /// 地震のマグニチュード
                /// </summary>
                public float magnitude { get; set; }

                /// <summary>
                /// 地震の震源地名
                /// </summary>
                public string name { get; set; }
            }
        }

        /// <summary>
        /// 地震の各地点の情報を保持するクラス
        /// </summary>
        public class Point {

            /// <summary>
            /// 観測した地点の名前
            /// </summary>
            public string addr;

            /// <summary>
            /// 観測した都道府県
            /// </summary>
            public string pref;

            /// <summary>
            /// 地震の規模
            /// </summary>
            public int scale;
        }

        public class Issue {

            /// <summary>
            /// 発表日時
            /// </summary>
            public string time { get; set; }

            /// <summary>
            /// 地震の発表種類
            /// </summary>
            public string type { get; set; }
        }
    }
}
