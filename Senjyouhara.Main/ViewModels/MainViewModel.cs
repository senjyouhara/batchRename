using Caliburn.Micro;
using Microsoft.Win32;
using PropertyChanged;
using Senjyouhara.Common.Utils;
using Senjyouhara.Main.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Senjyouhara.Main.ViewModels
{

    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : Screen
    {


        public string Tips { get; set; }
        public ObservableCollection<FileNameItem> FileNameItems { get; set; } = new ObservableCollection<FileNameItem>();


        [OnChangedMethod(nameof(FileNameItemsHandle))]
        public string Rename { get; set; }

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
                    var buqi = count.Substring(0, count.Length - (i + 1 + "").Length);
                    name = name.Replace("#", $"{"".PadLeft(buqi.Length, '0')}{i + 1}");
                }
                item.PreviewFileName = name + (!string.IsNullOrEmpty(item.SuffixName) ? $".{item.SuffixName}" : "");
                item.PreviewFilePath = $"{f.DirectoryName}\\{name}" + (!string.IsNullOrEmpty(item.SuffixName) ? $".{item.SuffixName}" : "");
            }
        }


        private IEventAggregator _eventAggregator;
        private IWindowManager _windowManager;
        public MainViewModel(IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;


        }


        public void SelectFileHandle()
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
                list.ForEach(file =>
                {
                    var v = new FileInfo(file);
                    FileNameItems.Add(new FileNameItem()
                    {
                        FilePath = v.FullName,
                        FileName = v.Name,
                        PreviewFileName = "",
                        SubtitleFileName = "",
                        SuffixName = v.Name.LastIndexOf(".") >= 0 ? v.Name.Substring(v.Name.LastIndexOf(".") + 1) : ""
                    });
                });

            }
        }

        public void ClearListHandle()
        {
            //_windowManager.ShowDialogAsync(IoC.Get<StartLoadingViewModel>());

            Tips = "";
            FileNameItems.Clear();
        }

        public void RenameFileHandle()
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
                        item.FilePath = item.PreviewFilePath;
                        item.FileName = item.PreviewFileName;
                        item.SuffixName = item.FileName.LastIndexOf(".") >= 0 ? item.FileName.Substring(item.FileName.LastIndexOf(".") + 1) : "";
                        item.PreviewFileName = "成功！";
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("当文件已存在时，无法创建该文件。"))
                        {
                            Tips = "当文件已存在时，无法重命名";
                            return;
                        }
                        else
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }

                }
                else
                {
                    Tips = $"文件{item.FileName}不存在！";
                    return;
                }
            }
            Tips = "重命名成功!";
        }


        public void OnListViewDrop(object sender, DragEventArgs e)
        {

            var FileDrop = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
            var listView1 = sender as ListView;

            if (FileDrop != null)
            {


                var FileNames = new List<string>(FileDrop);
                var FilterFile = FileNames.Select(v =>
                {

                    var flag = Directory.Exists(v);
                    var file = new FileInfo(v);
                    return flag ? null : file;
                }).Where(v => v != null).ToList();

                if (FilterFile.Count > 0)
                {
                    FilterFile.ForEach(v =>
                    {
                        Debug.WriteLine(v.Name.LastIndexOf(".") >= 0 ? v.Name.Substring(v.Name.LastIndexOf(".") + 1) : "");
                        FileNameItems.Add(new FileNameItem() { FilePath = v.FullName, FileName = v.Name, PreviewFileName = "", SubtitleFileName = "", SuffixName = v.Name.LastIndexOf(".") >= 0 ? v.Name.Substring(v.Name.LastIndexOf(".") + 1) : "" });
                    });
                    var list = FileNameItems.ToList();
                    try
                    {
                        //list.Sort();
                        list.Sort(delegate (FileNameItem a, FileNameItem b)
                        {
                            var matchesA = new Regex(@"[0-9]+\.[0-9]+|[0-9]+").Matches(a.FileName);
                            var matchesB = new Regex(@"[0-9]+\.[0-9]+|[0-9]+").Matches(b.FileName);


                            if (matchesA.Count < matchesB.Count)
                            {
                                (matchesA, matchesB) = (matchesB, matchesA);
                            }
                            var arrA = new object[matchesA.Count];
                            matchesA.CopyTo(arrA, 0);

                            var arrB = new object[matchesB.Count];
                            matchesB.CopyTo(arrB, 0);
                            Debug.WriteLine($"matchesA: {arrA.Select(v => v.ToString())}, matchesB: {arrB.Select(v => v.ToString())}");

                            for (int i = 0; i < matchesA.Count; i++)
                            {
                                var aValue = matchesA[i].Value;

                                var bValue = "";

                                if (matchesB.Count <= i)
                                {
                                }
                                else
                                {
                                    bValue = matchesB[i]?.Value;
                                }


                                Debug.WriteLine($"aValue : {aValue}, bValue : {bValue}");

                                if (string.IsNullOrEmpty(bValue))
                                {
                                    break;
                                }
                                if (aValue == bValue) continue;

                                Int32 aDouble = Int32.Parse((Double.Parse(aValue) * 100).ToString());
                                Int32 bDouble = Int32.Parse((Double.Parse(bValue) * 100).ToString());

                                Debug.WriteLine($"aDouble : {aDouble}, bDouble : {bDouble}");


                                Debug.WriteLine($"Abs aDouble : {aDouble}, bDouble : {bDouble}");
                                return aDouble - bDouble;
                            }

                            Debug.WriteLine(11111);
                            return -1;
                        });


                        listView1.ItemsSource = new ObservableCollection<FileNameItem>(list);
                        Console.WriteLine(list.ToString());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                }

            }
            else
            {

                var pos = e.GetPosition(listView1);   //获取位置
                var result = VisualTreeHelper.HitTest(listView1, pos);   //根据位置得到result
                if (result == null)
                {
                    return;   //找不到 返回
                }
                #region 查找元数据
                var sourcePerson = e.Data.GetData(typeof(FileNameItem)) as FileNameItem;
                if (sourcePerson == null)
                {
                    return;
                }
                #endregion

                #region  查找目标数据
                var listBoxItem = Utils.FindVisualParent<ListViewItem>(result.VisualHit);
                if (listBoxItem == null)
                {
                    return;
                }
                var targetPerson = listBoxItem.Content as FileNameItem;
                if (ReferenceEquals(targetPerson, sourcePerson))
                {
                    return;
                }
                #endregion


                int sourceIndex = listView1.Items.IndexOf(sourcePerson);
                int targetIndex = listView1.Items.IndexOf(targetPerson);

                var source = listView1.ItemsSource as ObservableCollection<FileNameItem>;

                source.Move(sourceIndex, targetIndex);

                listView1.SelectedItem = source[targetIndex];
            }
        }
        public void OnListViewPreviewKeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Delete)
            {
                var listView1 = sender as ListView;
                var items = listView1.SelectedItems;
                var newArr = new FileNameItem[items.Count];
                items.CopyTo(newArr, 0);
                foreach (FileNameItem item in newArr)
                {
                    FileNameItems.Remove(item);
                }
                listView1.SelectedItem = null;
            }

        }


        public void OnListViewPreviewMouseMove(object sender, MouseEventArgs e)
        {

            var listView1 = sender as ListView;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(listView1);  // 获取位置

                #region 源位置
                HitTestResult result = VisualTreeHelper.HitTest(listView1, pos);  //根据位置得到result
                if (result == null)
                {
                    return;    //找不到 返回
                }
                var listBoxItem = Utils.FindVisualParent<ListBoxItem>(result.VisualHit);
                if (listBoxItem == null || listBoxItem.Content != listView1.SelectedItem)
                {
                    return;
                }
                #endregion


                DataObject dataObj = new DataObject(listBoxItem.Content as FileNameItem);
                DragDrop.DoDragDrop(listView1, dataObj, DragDropEffects.Move);  //调用方法
            }

        }

        public void OnListViewPreviewDragOver(object sender, MouseEventArgs e)
        {


        }
    }

    internal static class Utils
    {
        //根据子元素查找父元素
        public static T FindVisualParent<T>(DependencyObject obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T)
                    return obj as T;

                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }
    }

}
