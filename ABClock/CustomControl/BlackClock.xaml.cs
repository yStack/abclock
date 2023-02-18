using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ABClock
{
    /// <summary>
    /// BlackClock.xaml 的交互逻辑
    /// </summary>
    public partial class BlackClock : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged接口实现
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler? handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        private Rectangle _hourPointer = new Rectangle();
        private Rectangle _minutePointer = new Rectangle();
        private Grid _secondPointer = new Grid();
        private double _centerX;
        private double _centerY;
        private RotateTransform _hourRotation = new RotateTransform();
        private RotateTransform _minuteRotation = new RotateTransform();
        private RotateTransform _secondRotation = new RotateTransform();
        private double _canvasWidth;
        private double _outterCircleDiameter;
        private double _mostOutterCircleDiameter = 380.0;

        public readonly DependencyProperty ClockWidthProperty =
        DependencyProperty.Register("ClockWidth", typeof(double), typeof(UserControl), new PropertyMetadata(400.0));

        public readonly DependencyProperty InnerCircleDiameterProperty =
        DependencyProperty.Register("InnerCircleDiameter", typeof(double), typeof(UserControl), new PropertyMetadata(340.0));

        public readonly DependencyProperty DiameterOffsetProperty =
        DependencyProperty.Register("DiameterOffset", typeof(double), typeof(UserControl), new PropertyMetadata(30.0));


        public double ClockWidth
        {
            get { return (double)GetValue(ClockWidthProperty); }
            set { SetValue(ClockWidthProperty, value); }
        }


        public double InnerCircleDiameter
        {
            get { return (double)GetValue(InnerCircleDiameterProperty); }
            set { SetValue(InnerCircleDiameterProperty, value); }
        }


        public double DiameterOffset
        {
            get { return (double)GetValue(DiameterOffsetProperty); }
            set
            {
                SetValue(DiameterOffsetProperty, value);
                OutterCircleDiameter = InnerCircleDiameter + value;
                MostOutterCircleDiameter = OutterCircleDiameter + 10;
            }
        }


        public double OutterCircleDiameter
        {
            get { return _outterCircleDiameter; }
            set
            {
                if (_outterCircleDiameter != value)
                {
                    _outterCircleDiameter = value;
                    OnPropertyChanged(nameof(OutterCircleDiameter));
                }
            }
        }


        public double MostOutterCircleDiameter
        {
            get { return _outterCircleDiameter; }
            set
            {
                if (_mostOutterCircleDiameter != value)
                {
                    _mostOutterCircleDiameter = value;
                    OnPropertyChanged(nameof(MostOutterCircleDiameter));
                }
            }
        }



        public RotateTransform HourRotation
        {
            get { return _hourRotation; }
            set
            {
                _hourRotation = value;
                OnPropertyChanged(nameof(HourRotation));
            }
        }


        public RotateTransform MinuteRotation
        {
            get { return _minuteRotation; }
            set
            {
                _minuteRotation = value;
                OnPropertyChanged(nameof(MinuteRotation));
            }
        }


        public RotateTransform SecondRotation
        {
            get { return _secondRotation; }
            set
            {
                _secondRotation = value;
                OnPropertyChanged(nameof(SecondRotation));
            }
        }


        public BlackClock()
        {
            InitializeComponent();
            AddElementToCanvas();
            DataContext = this;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            StartUpdate();
        }


        private void AddElementToCanvas()
        {
            DiameterOffset = 30;

            _hourPointer = (Rectangle)FindResource("HourPointer");
            _minutePointer = (Rectangle)FindResource("MinutePointer");
            _secondPointer = (Grid)FindResource("SecondPointer");

            ClockPanel.Children.Add(_hourPointer);
            ClockPanel.Children.Add(_minutePointer);
            ClockPanel.Children.Add(_secondPointer);
        }


        private void Init()
        {
            _canvasWidth = ClockPanel.ActualWidth;
            _centerX = ClockPanel.ActualHeight / 2;
            _centerY = ClockPanel.ActualWidth / 2;

            Canvas.SetLeft(_hourPointer, _centerX - _hourPointer.ActualHeight / 2);
            Canvas.SetTop(_hourPointer, _centerY - _hourPointer.ActualHeight / 2);

            Canvas.SetLeft(_minutePointer, _centerX - _minutePointer.ActualHeight / 2);
            Canvas.SetTop(_minutePointer, _centerY - _minutePointer.ActualHeight / 2);

            Canvas.SetLeft(_secondPointer, _centerX - 30);
            Canvas.SetTop(_secondPointer, _centerY - 20);

            HourRotation.CenterX = _hourPointer.ActualHeight / 2;
            HourRotation.CenterY = _hourPointer.ActualHeight / 2;
            MinuteRotation.CenterX = _minutePointer.ActualHeight / 2;
            MinuteRotation.CenterY = _minutePointer.ActualHeight / 2;
            SecondRotation.CenterX = _secondPointer.Height / 2 + 10;
            SecondRotation.CenterY = _secondPointer.Height / 2;

            GenTickMark();
            GenLabelMark();
        }


        private void Update()
        {
            string now = DateTime.Now.ToString("hh:mm:ss");
            string[] timeInfo = now.Split(":");
            double hour = double.Parse(timeInfo[0]);
            double minute = double.Parse(timeInfo[1]);
            double second = double.Parse(timeInfo[2]);

            HourRotation.Angle = Math.Round(360 / 12 * (hour + minute / 60) - 90, 2);
            MinuteRotation.Angle = Math.Round(360 / 60 * (minute + second / 60) - 90);
            SecondRotation.Angle = Math.Round(360 / 60 * second - 90);
        }



        private void GenTickMark()
        {
            for (int i = 0; i < 60; i++)
            {
                Rectangle rect = new Rectangle() { Width = 8, Height = 1 };

                if (i % 5 == 0)
                {
                    rect.Width = rect.Width * 1.5;
                }
                rect.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C4C3C3"));

                ClockPanel.Children.Add(rect);

                int offset = 2; //留空
                Canvas.SetLeft(rect, _canvasWidth - rect.Width - offset);
                Canvas.SetTop(rect, _canvasWidth / 2 - rect.Height / 2);

                RotateTransform rt = new RotateTransform();
                rt.CenterX = -(_centerX - rect.Width - offset);
                rt.CenterY = rect.Height / 2;

                rt.Angle = i * 6;
                rect.RenderTransform = rt;
            }
        }


        private void GenLabelMark()
        {
            for (int i = 0; i < 12; i++)
            {
                MarkLabel label = new MarkLabel();
                label.Width = 40;
                label.Height = 40;
                label.Padding = new Thickness(0);
                label.Background = new SolidColorBrush(Colors.Transparent);
                label.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C4C3C3"));
                label.Content = (i + 1).ToString();
                label.FontSize = 22;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.VerticalContentAlignment = VerticalAlignment.Center;

                ClockPanel.Children.Add(label);

                double offset = 15; //留空
                Canvas.SetLeft(label, _canvasWidth - label.Width - offset);
                Canvas.SetTop(label, _canvasWidth / 2 - label.Height / 2);

                RotateTransform rt = new RotateTransform();
                rt.CenterX = -(_centerX - label.Width - offset);
                rt.CenterY = label.Height / 2;
                rt.Angle = i * 30 - 60;
                label.RenderTransform = rt;
                label.MarkRotateAngle = -rt.Angle;
            }
        }



        private void StartUpdate()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(10);
                    try
                    {
                        this.Dispatcher.Invoke(() => { Update(); });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });
        }

    }
}
