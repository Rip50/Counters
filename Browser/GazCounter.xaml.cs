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
using System.Windows.Navigation;
using System.Windows.Shapes;
using mshtml;

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
            Browser.Navigate("http://www.sargc.ru/counters.html");
        }

        private dynamic GetProperty(dynamic form, string name)
        {
            if (form?.all == null)
                return null;
            var property = ((IEnumerable)form.all).Cast<dynamic>().FirstOrDefault(o => o.GetType().GetProperty("name") != null && o.name!=null && o.name.Equals(name));
            return property;
        }

        private void SetProperty(dynamic form, string name, string value)
        {
            var prop = GetProperty(form, name);
            if (prop != null)
                prop.value = value;
        }

        private void Browser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            var doc = Browser.Document as IHTMLDocument2;
            var counterForm = doc?.forms?.Cast<dynamic>().FirstOrDefault(form => form.id != null && form.id.Equals("form_cnt"));
            if (counterForm == null) return;
            if (counterForm.all == null) return;

            SetProperty(counterForm, "account", Data.Id);
            SetProperty(counterForm, "fam", Data.Name);
            SetProperty(counterForm, "indication", $"{Data.Current}");
        }
    }
}
