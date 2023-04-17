using Caliburn.Micro;
using HandyControl.Controls;
using HandyControl.Tools;
using Microsoft.Win32;
using PropertyChanged;
using Senjyouhara.Common.Utils;
using Senjyouhara.Main.models;
using Senjyouhara.Main.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace Senjyouhara.Main.ViewModels
{

    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : Screen
    {
        public bool IsSubMergeVideo { get; set; } = true;
        public string Tips { get; set; }
        public ObservableCollection<FileNameItem> FileNameItems { get; set; } = new ObservableCollection<FileNameItem>();


        [OnChangedMethod(nameof(FileNameItemsHandle))]
        public string Rename { get; set; }

        private GenerateRuleViewModel genetateRuleViewModel;

        private void FileNameItemsHandle()
        {

            var subList = FileNameItems.Where(v => SUB_FILE_SUBFIX_LIST.Where(s => s.Trim().ToLower().Equals(v.SuffixName)).FirstOrDefault() != null).ToList();
            var otherList = FileNameItems.Where(v => !subList.Contains(v)).ToList();

            for (int i = 0; i < otherList.Count; i++)
            {
                var item = otherList[i];
                var f = new FileInfo(item.FilePath);
                var name = Rename;
                if (name.IndexOf("#") != -1)
                {
                    var count = otherList.Count + "";
                    var tmp = count.Substring(0, count.Length - (i + 1 + "").Length);
                    name = name.Replace("#", $"{"".PadLeft(tmp.Length, '0')}{i + 1}");
                }
                item.PreviewFileName = name + (!string.IsNullOrEmpty(item.SuffixName) ? $".{item.SuffixName}" : "");
                item.PreviewFilePath = $"{f.DirectoryName}\\{name}" + (!string.IsNullOrEmpty(item.SuffixName) ? $".{item.SuffixName}" : "");
            }


            var Count = 0;
            for (int i = 0; i < subList.Count; i++)
            {
                var prevItemNumber = "";
                var prevlist = PatternUtil.GetPatternResult(@"(\s[0-9]+\.[0-9]+|[0-9]+\s)|(\[[0-9]+\.[0-9]+|[0-9]+\])", subList[i - 1]?.FileName);
                if(prevlist.Count > 0)
                {
                    prevItemNumber = prevlist[0];
                }

                var itemNumber = "";
                var item = subList[i];
                var list = PatternUtil.GetPatternResult(@"(\s[0-9]+\.[0-9]+|[0-9]+\s)|(\[[0-9]+\.[0-9]+|[0-9]+\])", item.FileName);
                if (list.Count > 0)
                {
                   itemNumber = list[0];
                }

                var name = Rename;
                var f = new FileInfo(item.FilePath);
                if (name.IndexOf("#") != -1)
                {
                    var count = subList.Count + "";
                    var tmp = count.Substring(0, count.Length - (i + 1 + "").Length);
                    name = name.Replace("#", $"{"".PadLeft(tmp.Length, '0')}{Count + 1}");
                }
                item.PreviewFileName = name + (!string.IsNullOrEmpty(item.SuffixName) ? $".{item.SuffixName}" : "");
                item.PreviewFilePath = $"{f.DirectoryName}\\{name}" + (!string.IsNullOrEmpty(item.SuffixName) ? $".{item.SuffixName}" : "");
                if (!itemNumber.Equals(prevItemNumber))
                {
                    Count++;
                }
            }
        }


        private IEventAggregator _eventAggregator;
        private IWindowManager _windowManager;
        public MainViewModel(IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            genetateRuleViewModel = new GenerateRuleViewModel();
            genetateRuleViewModel.Parent = this;
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
                var FileList = new List<string>(openFileDialog.FileNames);
                AddFilesHandle(FileList);
            }
        }

        public void ClearListHandle()
        {
            Tips = "";
            FileNameItems.Clear();
        }

        public static string[] SUB_FILE_SUBFIX_LIST = new string[] { "ass", "ssa", "srt" };
        public static string[] VIDEO_SUBFIX_LIST = new string[] { "mkv", "mp4", "flv", "f4v", "avi", "rm", "rmvb", "mov", "wmv" };
        private void AddFilesHandle(List<string> files)
        {
            files.ForEach(file =>
            {
                var v = new FileInfo(file);
                var flag = Directory.Exists(file);
                if (flag)
                {
                    return;
                }

                FileNameItems.Add(new FileNameItem()
                {
                    FilePath = v.FullName,
                    FileName = v.Name,
                    PreviewFileName = "",
                    SubtitleFileName = "",
                    SuffixName = v.Name.LastIndexOf(".") >= 0 ? v.Name.Substring(v.Name.LastIndexOf(".") + 1) : ""
                });
            });

            var group = FileNameItems.GroupBy((item) =>
            {
                return item.SuffixName.ToLower();
            });

            var sortList = new List<FileNameItem>();
            foreach (var groupItem in group)
            {
                var key = groupItem.Key;
                var groupList = groupItem.ToList();
                groupList.Sort(MySort);
                sortList = sortList.Concat(groupList).ToList();
            }

            Debug.WriteLine(group);

            if (IsSubMergeVideo)
            {

                try
                {

                    var subList = sortList.Where(v => SUB_FILE_SUBFIX_LIST.Where(s => s.Trim().ToLower().Equals(v.SuffixName)).FirstOrDefault() != null).ToList();
                    var otherList = sortList.Where(v => !subList.Contains(v)).ToList();
                    var newList = new List<FileNameItem>();
                    foreach (var item in otherList)
                    {
                        if (VIDEO_SUBFIX_LIST.Contains(item.SuffixName.ToLower()))
                        {
                            newList.Add(item);
                            var FilterSub = subList.Where(v =>
                            {
                                var matchesA = PatternUtil.GetPatternResult(@"(\s[0-9]+\.[0-9]+|[0-9]+\s)|(\[[0-9]+\.[0-9]+|[0-9]+\])", item.FileName);
                                var matchesB = PatternUtil.GetPatternResult(@"(\s[0-9]+\.[0-9]+|[0-9]+\s)|(\[[0-9]+\.[0-9]+|[0-9]+\])", v.FileName);

                                if (matchesA.Count < matchesB.Count)
                                {
                                    (matchesA, matchesB) = (matchesB, matchesA);
                                }
                           
                                var unionData = matchesA.Intersect(matchesB).ToList();

                                //if (unionData.Count > 1)
                                //{
                                //    var m = unionData.Where(v => (v as string).Length <= 2).FirstOrDefault();
                                //    return m[0] == ;
                                //}


                                for (int i = 0; i < matchesA.Count; i++)
                                {
                
                                    var aValue = matchesA[i].Replace(" ", "").Replace("[", "").Replace("]", "");

                                    var bValue = "";

                                    if (matchesB[i] != null)
                                    {
                                        bValue = matchesB[i];
                                    }

                                    bValue = bValue.Replace(" ", "").Replace("[", "").Replace("]", "");

                                    Debug.WriteLine($"aValue : {aValue}, bValue : {bValue}");

                                    if (string.IsNullOrEmpty(bValue))
                                    {
                                        break;
                                    }
                                    if (aValue.Equals(bValue)) return true;
                                }

                                return false;
                            }).ToList();
                            if (FilterSub.Count > 0)
                            {
                                newList = newList.Concat(FilterSub.Select(v=>
                                {
                                    v.FileName = " └─  " + v.FileName;
                                    return v;
                                })).ToList();
                            }
                        }
                        else
                        {
                            newList.Add(item);
                        }
                    }
                    FileNameItems = new ObservableCollection<FileNameItem>(newList);
                }
                catch (Exception ex)
                {

                }


            }
            else
            {
                FileNameItems = new ObservableCollection<FileNameItem>(sortList);
            }
        }

        public void RenameFileHandle()
        {

            var count = FileNameItems.Count;

            if (count <= 0)
            {
                return;
            }

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
                            System.Windows.MessageBox.Show(ex.Message);
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

        private int MySort (FileNameItem a, FileNameItem b)
        {
            var matchesA = new Regex(@"(\s[0-9]+\.[0-9]+|[0-9]+\s)|(\[[0-9]+\.[0-9]+|[0-9]+\])").Matches(a.FileName);
            var matchesB = new Regex(@"(\s[0-9]+\.[0-9]+|[0-9]+\s)|(\[[0-9]+\.[0-9]+|[0-9]+\])").Matches(b.FileName);


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
                var aValue = matchesA[i].Value.Replace(" ", "").Replace("[", "").Replace("]", "");

                var bValue = "";

                if (matchesB.Count <= i)
                {
                }
                else
                {
                    bValue = matchesB[i]?.Value;
                }

                bValue = bValue.Replace(" ", "").Replace("[", "").Replace("]", "");
                Debug.WriteLine($"aValue : {aValue}, bValue : {bValue}");

                if (string.IsNullOrEmpty(bValue))
                {
                    break;
                }
                if (aValue == bValue) continue;

                int aDouble = int.Parse((double.Parse(aValue) * 100).ToString());
                int bDouble = int.Parse((double.Parse(bValue) * 100).ToString());

                Debug.WriteLine($"aDouble : {aDouble}, bDouble : {bDouble}");

                return aDouble - bDouble;
            }

            return -1;
        }

        public void OnListViewDrop(object sender, DragEventArgs e)
        {

            var FileDrop = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
            var listView1 = sender as ListView;

            if (FileDrop != null)
            {
                var FileNames = new List<string>(FileDrop);
                AddFilesHandle(FileNames);
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

        public void ShowGenerateRuleModal()
        {
            _windowManager.ShowDialogAsync(genetateRuleViewModel);
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
