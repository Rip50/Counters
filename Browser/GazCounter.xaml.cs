using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Awesomium.Core;

namespace Browser
{

    public class GazCounterData
    {
        public string Id;
        public string Name;

        public double Current;
        public double Difference;
    }

    /// <summary>
    /// Interaction logic for GazCounter.xaml
    /// </summary>
    public partial class GazCounter : Window
    {
        private GazCounterData Data { get; set; }
        public GazCounter(GazCounterData data)
        {
            Data = data;
            InitializeComponent();
        }

        private dynamic GetProperty(dynamic form, string name)
        {
            if (form?.all == null)
                return null;
            var property = ((IEnumerable)form.all).Cast<dynamic>().FirstOrDefault(o => o.GetType().GetProperty("name") != null && o.name?.Equals(name));
            return property;
        }

        private void SetProperty(dynamic form, string name, string value)
        {
            var prop = GetProperty(form, name);
            if (prop != null)
                prop.value = value;
        }

        private void Browser_OnLoadingFrameComplete(object sender, FrameEventArgs e)
        {
            Browser.WebSession.
        }
    }
}
