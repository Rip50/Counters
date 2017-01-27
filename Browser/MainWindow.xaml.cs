using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using mshtml;
using NavigationEventArgs = System.Windows.Navigation.NavigationEventArgs;

namespace Browser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Config Config
        {
            get { return App.Config; }
        }

        public Config.Session PreviousSession => Config.Sessions.Count > 1 ? Config.Sessions[Config.Sessions.Count - 2] : new Config.Session();
        public Config.Session CurrentSession => Config.Sessions.Count != 0 ? Config.Sessions[Config.Sessions.Count - 1] : new Config.Session();

        public double LastGaz
        {
            get { return PreviousSession.Gaz?.Current ?? 0.0; }
            set { throw new NotImplementedException(); }
        }

        public double LastCW1
        {
            get { return PreviousSession.Water?.CW1.Current ?? 0.0; }
            set { throw new NotImplementedException(); }
        }

        public double LastCW2
        {
            get { return PreviousSession.Water?.CW2.Current ?? 0.0; }
            set { throw new NotImplementedException(); }
        }

        public double LastHW1
        {
            get { return PreviousSession.Water?.HW1.Current ?? 0.0; }
            set { throw new NotImplementedException(); }
        }

        public double LastHW2
        {
            get { return PreviousSession.Water?.HW2.Current ?? 0.0; }
            set { throw new NotImplementedException(); }
        }

        public double CurrentGaz
        {
            get { return CurrentSession.Gaz.Current; }
            set
            {
                CurrentSession.Gaz.Current = value;
                CurrentSession.Gaz.Difference = value - LastGaz;
                OnPropertyChanged("DifferenceGaz");
            }
        }

        public double CurrentCW1
        {
            get { return CurrentSession.Water.CW1.Current; }
            set
            {
                CurrentSession.Water.CW1.Current = value;
                CurrentSession.Water.CW1.Difference = value - LastCW1;
                OnPropertyChanged("DifferenceCW1");
            }
        }

        public double CurrentCW2
        {
            get { return CurrentSession.Water.CW2.Current; }
            set
            {
                CurrentSession.Water.CW2.Current = value;
                CurrentSession.Water.CW2.Difference = value - LastCW2;
                OnPropertyChanged("DifferenceCW2");
            }
        }

        public double CurrentHW1
        {
            get { return CurrentSession.Water.HW1.Current; }
            set
            {
                CurrentSession.Water.HW1.Current = value;
                CurrentSession.Water.HW1.Difference = value - LastHW1;
                OnPropertyChanged("DifferenceHW1");
            }
        }

        public double CurrentHW2
        {
            get { return CurrentSession.Water.HW2.Current; }
            set
            {
                CurrentSession.Water.HW2.Current = value;
                CurrentSession.Water.HW2.Difference = value - LastHW2;
                OnPropertyChanged("DifferenceHW2");
            }
        }

        public double DifferenceGaz
        {
            get { return CurrentSession.Gaz.Difference; }
            set
            {
                CurrentSession.Gaz.Difference = value;
                CurrentSession.Gaz.Current = LastGaz + value;
                OnPropertyChanged("CurrentGaz");
            }
        }

        public double DifferenceCW1
        {
            get { return CurrentSession.Water.CW1.Difference; }
            set
            {
                CurrentSession.Water.CW1.Difference = value;
                CurrentSession.Water.CW1.Current = LastCW1 + value;
                OnPropertyChanged("CurrentCW1");
            }
        }

        public double DifferenceCW2
        {
            get { return CurrentSession.Water.CW2.Difference; }
            set
            {
                CurrentSession.Water.CW2.Difference = value;
                CurrentSession.Water.CW2.Current = LastCW2 + value;
                OnPropertyChanged("CurrentCW2");
            }
        }

        public double DifferenceHW1
        {
            get { return CurrentSession.Water.HW1.Difference; }
            set
            {
                CurrentSession.Water.HW1.Difference = value;
                CurrentSession.Water.HW1.Current = LastHW1 + value;
                OnPropertyChanged("CurrentHW1");
            }
        }

        public double DifferenceHW2
        {
            get { return CurrentSession.Water.HW2.Difference; }
            set
            {
                CurrentSession.Water.HW2.Difference = value;
                CurrentSession.Water.HW2.Current = LastHW2 + value;
                OnPropertyChanged("CurrentHW2");
            }
        }

        public string PreviousSessionInfo => Config.Sessions?.Count > 1 ? 
                                             $"Предыдущие показания были переданы {Config.Sessions[Config.Sessions.Count - 2].SessionTime:G}" : 
                                             "Предыдущие показания отсутствуют";

        public event Action SessionSucceeded;
        public event Action SessionFailed;

        public MainWindow()
        {
            DifferenceGaz = PreviousSession?.Gaz?.Difference ?? 0.0;
            DifferenceCW1 = PreviousSession?.Water?.CW1.Difference ?? 0.0;
            DifferenceCW2 = PreviousSession?.Water?.CW2.Difference ?? 0.0;
            DifferenceHW1 = PreviousSession?.Water?.HW1.Difference ?? 0.0;
            DifferenceHW2 = PreviousSession?.Water?.HW2.Difference ?? 0.0;

            CurrentGaz = LastGaz + DifferenceGaz;
            CurrentCW1 = LastCW1 + DifferenceCW1;
            CurrentCW2 = LastCW2 + DifferenceCW2;
            CurrentHW1 = LastHW1 + DifferenceHW1;
            CurrentHW2 = LastHW2 + DifferenceHW2;


            InitializeComponent();
        }
        

        protected virtual void OnSessionSucceeded()
        {
            SessionSucceeded?.Invoke();
        }

        protected virtual void OnSessionFailed()
        {
            SessionFailed?.Invoke();
        }
        

        private void Confirm(object sender, RoutedEventArgs e)
        {
            CurrentSession.Gaz.Id = PreviousSession.Gaz.Id;
            CurrentSession.Gaz.Name = PreviousSession.Gaz.Name;

            CurrentSession.Water.Id = PreviousSession.Water.Id;
            CurrentSession.Water.Name = PreviousSession.Water.Name;

            CurrentSession.Water.CW1.Number = PreviousSession.Water.CW1.Number;
            CurrentSession.Water.CW2.Number = PreviousSession.Water.CW2.Number;
            CurrentSession.Water.HW1.Number = PreviousSession.Water.HW1.Number;
            CurrentSession.Water.HW2.Number = PreviousSession.Water.HW2.Number;

            var wnds = new List<Window>()
            {
                new GazCounter(CurrentSession.Gaz),
                new WaterCounter(CurrentSession.Water.Name, CurrentSession.Water.Id, CurrentSession.Water.CW1),
                new WaterCounter(CurrentSession.Water.Name, CurrentSession.Water.Id, CurrentSession.Water.CW2),
                new WaterCounter(CurrentSession.Water.Name, CurrentSession.Water.Id, CurrentSession.Water.HW1),
                new WaterCounter(CurrentSession.Water.Name, CurrentSession.Water.Id, CurrentSession.Water.HW2)
            };

            foreach (var wnd in wnds)
            {
                var showDialog = wnd.ShowDialog();
                if (showDialog != null && showDialog.Value) continue;
                OnSessionFailed();
                return;
            }

            CurrentSession.SessionTime = DateTime.Now;
            OnSessionSucceeded();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            OnSessionFailed();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
