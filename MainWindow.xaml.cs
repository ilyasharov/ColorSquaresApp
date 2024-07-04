using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ColorSquaresApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<SolidColorBrush> colors = new List<SolidColorBrush>
        {
            Brushes.White, Brushes.Black, Brushes.Brown, Brushes.Blue, Brushes.LightBlue,
            Brushes.Red, Brushes.Yellow, Brushes.Green, Brushes.Gray, Brushes.Pink
        };

        private SolidColorBrush freeColor = Brushes.Orange; // Свободный цвет
        private int clickCounter = 0;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSquares();
            StartTimer();
            UpdateTimeAndTemperature();
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentTimeText.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private Dictionary<SolidColorBrush, string> colorNames = new Dictionary<SolidColorBrush, string>
        {
            { Brushes.White, "белый" },
            { Brushes.Black, "чёрный" },
            { Brushes.Brown, "коричневый" },
            { Brushes.Blue, "синий" },
            { Brushes.LightBlue, "голубой" },
            { Brushes.Red, "красный" },
            { Brushes.Yellow, "жёлтый" },
            { Brushes.Green, "зелёный" },
            { Brushes.Gray, "серый" },
            { Brushes.Pink, "розовый" },
            { Brushes.Orange, "оранжевый" }
        };

        private void InitializeSquares()
        {
            var random = new Random();
            var shuffledColors = colors.OrderBy(x => random.Next()).ToList();

            for (int i = 0; i < 9; i++)
            {
                var button = new Button
                {
                    Background = shuffledColors[i],
                    Content = colorNames[shuffledColors[i]],
                    Margin = new Thickness(5)
                };
                button.Click += Square_Click;
                SquaresGrid.Children.Add(button);
            }
        }

        private void Square_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var currentColor = button.Background;

            button.Background = freeColor;
            button.Content = colorNames[freeColor];

            freeColor = currentColor as SolidColorBrush;

            clickCounter++;
            ClickCounterText.Text = clickCounter.ToString();
        }

        private async void UpdateTimeAndTemperature()
        {
            while (true)
            {
                CurrentTimeText.Text = DateTime.Now.ToString("HH:mm:ss");

                var temperature = await GetTemperatureAsync();
                TemperatureText.Text = $"{temperature} °C";

                await Task.Delay(60000); // Обновление каждую минуту
            }
        }

        private async Task<double> GetTemperatureAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetStringAsync("https://api.open-meteo.com/v1/forecast?latitude=55.7558&longitude=37.6176&current_weather=true");
                    var data = JObject.Parse(response);
                    return data["current_weather"]["temperature"].Value<double>();
                }
                catch
                {
                    return 0.0;
                }
            }
        }
    }
}