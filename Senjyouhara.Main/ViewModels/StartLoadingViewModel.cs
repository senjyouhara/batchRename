using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Threading;

namespace Senjyouhara.Main.ViewModels
{
    public class StartLoadingViewModel : Conductor<IScreen>
    {
        private IWindowManager _WindowManager;
        public StartLoadingViewModel(IWindowManager manager)
        {
            _WindowManager = manager;
        }

        public async void VideoMediaEnded(object sender, RoutedEventArgs e)
        {
            var model = IoC.Get<ShellViewModel>();
            await _WindowManager.ShowWindowAsync(model);
            await CloseForm();
        }

        public void VideoMediaOpened(object sender, RoutedEventArgs e)
        {
            var video = sender as MediaElement;
            if (video.NaturalDuration.HasTimeSpan)
            {
                var durationTime = video.NaturalDuration.TimeSpan.TotalSeconds;
                double lastTime = durationTime - 1.5;
                lastTime = lastTime * 1000;

                System.Timers.Timer t = new System.Timers.Timer(lastTime);//实例化Timer类，设置间隔时间为200毫秒；   
                //t.Elapsed += new System.Timers.ElapsedEventHandler(HideWindow);  //到达时间的时候执行事件； 
                t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；    
                t.Enabled = true;  //是否执行System.Timers.Timer.Elapsed事件；  ,调用start()方法也可以将其设置为true  
                Console.WriteLine($"time: {durationTime}");
            }
        }

        //public void HideWindow(object sender, ElapsedEventArgs e)
        //{
        //    var video = sender as MediaElement;
        //    _win.Dispatcher.Invoke(() =>
        //    {
        //        Storyboard storyboard = (FindResource("hideMe") as System.Windows.Media.Animation.Storyboard);
        //        storyboard.Completed += (o, a) => {
        //            CloseForm();
        //        };
        //        storyboard.Begin(this);
        //    });

        //}

        public Task CloseForm()
        {
            return TryCloseAsync();
        }
    }
}
