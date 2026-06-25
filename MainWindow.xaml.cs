using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Earthquake_WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            var api = EarthquakeData.GetInstance()[0];
            ScaleText.Text = (api.earthquake.maxScale / 10).ToString();
            DateText.Text = api.issue.time;
            LocationNameText.Text = api.earthquake.hypocenter.name;
            DepthText.Text = api.earthquake.hypocenter.depth.ToString();
            MagnitudeText.Text = api.earthquake.hypocenter.magnitude.ToString();
        }
    }
}