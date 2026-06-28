using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Application = System.Windows.Application;
using Color = System.Drawing.Color;

namespace Earthquake_WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            DataContext = new MainViewModel();

            var api = EarthquakeData.GetInstance()[0];
            ScaleText.Text = (api.earthquake.maxScale / 10).ToString();
            DateText.Text = api.issue.time;
            LocationNameText.Text = api.earthquake.hypocenter.name;
            DepthText.Text = api.earthquake.hypocenter.depth.ToString();
            MagnitudeText.Text = api.earthquake.hypocenter.magnitude.ToString();

            setImageAsync();
        }

        /// <summary>
        /// 住所等から市区町村名だけを取り出す関数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string ConvertLocationName(string name) {
            int index = -1;
            if (name.IndexOf('市') > 0)  index = name.IndexOf('市');
            else if (name.IndexOf('村') > 0) index = name.IndexOf('村');
            else if (name.IndexOf('町') > 0) index = name.IndexOf('町');
            else if (name.IndexOf('区') > 0) index = name.IndexOf('区');

            if (index == -1) {
                return null;
            }
            else {
                return name.AsSpan(0, index + 1).ToString();
            }    
        }

        /// <summary>
        /// IFeatureからシェイプファイルの属性（regionname）に実際にあるかを返す関数
        /// </summary>
        /// <param name="feature"></param>
        /// <returns>実際にある場合は実際のキーを返し、無い場合はnullを返す。</returns>
        public static string FindKey(string fieldName, IFeature feature) {
            var key = feature.Attributes
                            .GetNames().FirstOrDefault(a => 
                            a.Equals(fieldName, StringComparison.OrdinalIgnoreCase));

            if (key == null) return null;
            if (!feature.Attributes.Exists(key)) return null;

            var name = feature.Attributes[key]?.ToString();
            if (string.IsNullOrEmpty(name)) return null;

            return feature.Attributes[key]?.ToString();
        }

        public async Task setImageAsync() {
            MapRender backDrawMap = new("./Resource/市町村等.shp", 1000, 1000);
            MapRender forwardDrawMap = new("./Resource/府県予報区等.shp", 1000, 1000);
            DateTime beforeTime = DateTime.Now;
            await Task.Run(() => {
                var earthquakeData = EarthquakeData.GetInstance()[0];
                var earthquake = earthquakeData.earthquake;
                var hypocenter = earthquake.hypocenter;

                // 各震度地点ごとに色設定
                foreach (var point in earthquakeData.points) {
                    Func<IFeature, bool> filter = feature => {
                        string key = FindKey("regionname", feature);
                        if (key == null) return false;
                        return key.Contains($"{point.pref}{ConvertLocationName(point.addr)}");
                    };
                    backDrawMap.SetFill(EarthquakeData.ConvertScaleColor(point.scale), filter);  // 地域ごとに個別色設定
                }
          
                backDrawMap.AddLine(Color.Black, 1, f => true); // 境界線は黒色
                forwardDrawMap.AddLine(Color.Black, 3, f => true);

                // 描画範囲
                Func<IFeature, bool> backDrawFilter = feature => {
                    return earthquakeData.points.Any(p => {
                        string key = FindKey("regionname", feature);
                        if (key == null) return false;
                        return key.Contains($"{p.pref}");
                    });
                };

                Func<IFeature, bool> forwardDrawFilter = feature => {
                    return earthquakeData.points.Any(p => {
                        string key = FindKey("name", feature);
                        if (key == null) return false;
                        return key.Contains($"{p.pref}");
                    });
                };



                backDrawMap.SetDrawArea(backDrawFilter);   // 描画範囲を設定する
                forwardDrawMap.SetDrawArea(forwardDrawFilter);

                var backDrawMapBuild = backDrawMap.Build();
                var forwardDrawMapBuild = forwardDrawMap.Build();
                //forwardDrawMapBuild.Save("debug2.png");

                using Bitmap image = new(1000, 1000);
                using (Graphics graphics = Graphics.FromImage(image)) {
                    graphics.DrawImage(backDrawMapBuild, 0, 0);
                    graphics.DrawImage(forwardDrawMapBuild, 0, 0);
                }

                var hBitmap = image.GetHbitmap();
                //image.Save("debug.png");

                Application.Current.Dispatcher.Invoke(() => {   //WPF側のスレッドで実行する
                    var imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            hBitmap,
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());

                    EarthquakeImage.Source = imageSource;   // WPF側に画像をセットする

                    DateTime afterTime = DateTime.Now;
                    TimeSpan subtractTime = beforeTime.Subtract(afterTime);
                    Debug.Write("描画に成功しました：" + subtractTime.ToString("hh\\:mm\\:ss"));
                });
            });
        }

    }
}