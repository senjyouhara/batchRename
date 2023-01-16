using Senjyouhara.Common.Utils;
using Senjyouhara.Main.models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Senjyouhara.Main.ViewModels
{
    public class MainWindowViewModel: NotifycationObject
    {


        private string name;

        public string Name
        {
            get { return  name; }
            set {  name = value; RaisePropertyChanged(nameof(Name)); }
        }

        private ObservableCollection<FileNameItem> fileNameItems = new ObservableCollection<FileNameItem>();

        public ObservableCollection<FileNameItem> FileNameItems
        {
            get { return fileNameItems; }
            set { fileNameItems = value; RaisePropertyChanged(nameof(FileNameItems)); }
        }

        public MainWindowViewModel()
        {

            //fileNameItems.Add(new FileNameItem() { FileName = "111.mkv", PreviewFileName = "", SubtitleFileName = "111.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "222.mkv", PreviewFileName = "", SubtitleFileName = "222.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "333.mkv", PreviewFileName = "", SubtitleFileName = "333.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "444.mkv", PreviewFileName = "", SubtitleFileName = "444.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "444.mkv", PreviewFileName = "", SubtitleFileName = "444.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "444.mkv", PreviewFileName = "", SubtitleFileName = "444.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "444.mkv", PreviewFileName = "", SubtitleFileName = "444.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "444.mkv", PreviewFileName = "", SubtitleFileName = "444.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "444.mkv", PreviewFileName = "", SubtitleFileName = "444.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "444.mkv", PreviewFileName = "", SubtitleFileName = "444.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "444.mkv", PreviewFileName = "", SubtitleFileName = "444.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "444.mkv", PreviewFileName = "", SubtitleFileName = "444.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "444.mkv", PreviewFileName = "", SubtitleFileName = "444.src" }) ;
            //fileNameItems.Add(new FileNameItem() { FileName = "444.mkv", PreviewFileName = "", SubtitleFileName = "444.src" }) ;


        }
    }
}
