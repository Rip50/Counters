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

namespace Browser
{
    public class WaterCounterData
    {
        public class Impl
        {
            public string Number;
            public double Current;
            public double Difference;
        }

        public string Name;
        public string Id;

        public Impl CW1, CW2, HW1, HW2;

    }

    /// <summary>
    /// Interaction logic for WaterCounter.xaml
    /// </summary>
    public partial class WaterCounter : Window
    {
        private string UserName { get; set; }
        private string Id { get; set; }
        private WaterCounterData.Impl Impl { get; set; }

        public WaterCounter(string name, string id, WaterCounterData.Impl impl)
        {
            UserName = name;
            Id = id;
            Impl = impl;
            InitializeComponent();
            Browser.Navigate("http://saratovvodokanal.ru/statement/");
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
        
        private void Browser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            DialogResult = true;
        }
    }
}
