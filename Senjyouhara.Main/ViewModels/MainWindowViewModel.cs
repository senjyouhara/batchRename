using Microsoft.Win32;
using Senjyouhara.Common.Utils;
using Senjyouhara.Main.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
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

        private string tips;

        public string Tips
        {
            get { return tips; }
            set { tips = value; RaisePropertyChanged(nameof(Tips)); }
        }

        private ObservableCollection<FileNameItem> fileNameItems = new ObservableCollection<FileNameItem>();

        public ObservableCollection<FileNameItem> FileNameItems
        {
            get { return fileNameItems; }
            set { fileNameItems = value; RaisePropertyChanged(nameof(FileNameItems)); }
        }


        private string rename;

        public string Rename
        {
            get { return rename; }
            set { rename = value; RaisePropertyChanged(nameof(Rename)); FileNameItemsHandle();  }
        }

        private void FileNameItemsHandle()
        {

            for (int i = 0; i < FileNameItems.Count; i++)
            {
                var item = FileNameItems[i];
                var f = new FileInfo(item.FilePath);
                var name = Rename;
                if (name.IndexOf("#") != -1)
                {
                    var count = FileNameItems.Count + "";
                    var buqi = count.Substring(0, count.Length - (i+1 + "").Length);
                    name = name.Replace("#", $"{"".PadLeft(buqi.Length, '0')}{i+1}");
                }
                item.PreviewFileName = name + (!string.IsNullOrEmpty(item.SuffixName) ? $".{item.SuffixName}" : "");
                item.PreviewFilePath = $"{f.DirectoryName}\\{name}" + (!string.IsNullOrEmpty(item.SuffixName) ? $".{item.SuffixName}" : "");
            }
         
        }

        public DelegateCommand ClearListCommand { get; }
        public DelegateCommand SelectFileCommand { get; }
        public DelegateCommand RenameFileCommand { get; }

        public MainWindowViewModel()
        {
            ClearListCommand = new DelegateCommand(() =>
            {
                Tips = "";
                FileNameItems.Clear();
            });

            SelectFileCommand = new DelegateCommand(() =>
            {
                Tips = "";
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = "c:\\desktop";    //初始的文件夹
                openFileDialog.Filter = "所有文件(*.*)|*.*";//在对话框中显示的文件类型
                openFileDialog.Title = "请选择文件";
                openFileDialog.Multiselect = true;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == true)
                {

                    var list = new List<string>(openFileDialog.FileNames);
                    list.ForEach(file => {
                        var v = new FileInfo(file);
                        FileNameItems.Add(new FileNameItem() { 
                            FilePath = v.FullName,
                            FileName = v.Name,
                            PreviewFileName = "",
                            SubtitleFileName = "",
                            SuffixName = v.Name.LastIndexOf(".") >= 0 ? v.Name.Substring(v.Name.LastIndexOf(".") + 1) : ""
                        });
                    });
                    
                }

            });


            RenameFileCommand = new DelegateCommand(() =>
            {
                var count = FileNameItems.Count;
                foreach (var item in FileNameItems)
                {
                    var f = new FileInfo(item.FilePath);
                    if (f.Exists)
                    {
                        try
                        {
                            f.MoveTo(item.PreviewFilePath);
                        } catch (Exception ex)
                        {
                            if (ex.Message.Contains("当文件已存在时，无法创建该文件。"))
                            {
                                Tips = "当文件已存在时，无法重命名";
                                return;
                            } else
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        
                    } else
                    {
                        Tips = $"文件{item.FileName}不存在！";
                        return;
                    }
                }
                Tips = "重命名成功!";

            });
        }
    }
}
