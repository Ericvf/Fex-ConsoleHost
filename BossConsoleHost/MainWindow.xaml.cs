using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BossConsoleHost
{
    public partial class MainWindow : Window
    {
        public class ConsoleVM : INotifyPropertyChanged
        {
            public ObservableCollection<string> Lines
            {
                get { return _Lines; }
                set
                {
                    if (_Lines != value)
                    {
                        _Lines = value;
                    }
                }
            }
            private ObservableCollection<string> _Lines = new ObservableCollection<string>();
            public const string LinesPropertyName = "Lines";

            public string Input
            {
                get { return _Input; }
                set
                {
                    if (_Input != value)
                    {
                        _Input = value;
                        OnPropertyChanged(InputPropertyName);
                    }
                }
            }

            private void OnPropertyChanged(string PropertyName)
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
            private string _Input;
            public const string InputPropertyName = "Input";

            public event PropertyChangedEventHandler PropertyChanged;
        }

        private ProcessStartService service;
        public ConsoleVM vm { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.vm = new ConsoleVM();
            this.DataContext = this.vm;

            this.Title = "Boss Console Host";
            this.Background = new SolidColorBrush(Color.FromArgb(192, 50, 50, 50));
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.AllowsTransparency = true;
            this.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
            this.MouseDown += MainWindow_MouseDown;
            this.LayoutRoot.Margin = new Thickness(10);
            this.Opacity = 0.75;

            RunProcess();


            this.KeyDown += MainWindow_KeyDown;
            this.Closed += MainWindow_Closed;

            this.Activate();
        }

        void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var keyConverter = new KeyConverter();
            var keyString = keyConverter.ConvertToString(e.Key);

            if (keyString.Length == 1)
                this.vm.Input += keyString;

            else if (e.Key == Key.Back)
            {
                var currentStringLength = this.vm.Input.Length;
                if (currentStringLength > 0)
                    this.vm.Input = this.vm.Input.Remove(currentStringLength - 1);
            }
            else if (e.Key == Key.Enter)
            {
                var input = this.vm.Input;
                this.vm.Input = null;
                this.vm.Lines.Add(input);
                this.service.Send(input);
            }
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (this.service != null)
                this.service.Kill();
        }

        private void RunProcess()
        {
            var args = Environment.GetCommandLineArgs().AsEnumerable().Skip(1);
            if (args.Count() >= 1)
            {
                this.service = new ProcessStartService();
                this.service.RunProcessAsync(
                    args.First(),
                    args.Skip(1),
                    this.handleConsoleOutput);
            }
        }

        private void handleConsoleOutput(string output, bool isError)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.vm.Lines.Add(output);
                this.cv.ScrollToEnd();

            }));
        }
    }
}
