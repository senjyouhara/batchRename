using Senjyouhara.Main.ViewModels;
using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Senjyouhara.Main.Views
{
    /// <summary>
    /// StartLoading.xaml 的交互逻辑
    /// </summary>
    public partial class StartLoadingView : Window
    {
        public StartLoadingView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var loadingVideo = Properties.Resources.loadingVideo;
            //video.Source = new Uri(@"pack://application:,,,/Resources/loading.mp4", UriKind.Absolute);
            //video.Play();

            //img.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/loading.png", UriKind.Absolute));
        }

        public void MediaLoaded(object sender, RoutedEventArgs e)
        {
            double lastTime = 2 * 1000;
            Timer t = new Timer(lastTime);//实例化Timer类，设置间隔时间为200毫秒；   
            t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；    
            t.Enabled = true;  //是否执行System.Timers.Timer.Elapsed事件；  ,调用start()方法也可以将其设置为true

            t.Elapsed += new ElapsedEventHandler((o, e) =>
            {
                App.Current.Dispatcher.Invoke((() =>
                {
                    win.OpacityMask = Resources["ClosedBrush"] as LinearGradientBrush;
                    Storyboard storyboard = (Resources["hideMe"] as Storyboard);
                    storyboard.Completed += (o, a) =>
                    {
                        var ctx = DataContext as StartLoadingViewModel;
                        ctx.CloseHandle();
                    };
                    storyboard.Begin(this);
                }));
            });  //到达时间的时候执行事件； 
        }

        //private void video_MediaOpened(object sender, RoutedEventArgs e)
        //{
        //    var video = sender as MediaElement;
        //    if (video.NaturalDuration.HasTimeSpan)
        //    {
        //        var durationTime = video.NaturalDuration.TimeSpan.TotalSeconds;
        //        double lastTime = durationTime - 1.5;
        //        lastTime = lastTime * 1000;

        //        System.Timers.Timer t = new System.Timers.Timer(lastTime);//实例化Timer类，设置间隔时间为200毫秒；   
        //        t.Elapsed += new System.Timers.ElapsedEventHandler(HideWindow);  //到达时间的时候执行事件； 
        //        t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；    
        //        t.Enabled = true;  //是否执行System.Timers.Timer.Elapsed事件；  ,调用start()方法也可以将其设置为true  
        //        Console.WriteLine($"time: {durationTime}");
        //    }
        //}

        private void HideWindow(object sender, ElapsedEventArgs e)
        {
            //    var video = sender as MediaElement;
            //    win.Dispatcher.Invoke(() =>
            //    {
            //        Storyboard storyboard = (FindResource("hideMe") as System.Windows.Media.Animation.Storyboard);
            //        storyboard.Completed += (o, a) => { DialogResult = true; };
            //        storyboard.Begin(this);
            //    });

        }
    }
}
