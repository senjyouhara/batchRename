using Caliburn.Micro;
using HandyControl.Controls;
using HandyControl.Tools;
using HandyControl.Tools.Extension;
using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using PropertyChanged;
using Senjyouhara.Common.Exceptions;
using Senjyouhara.Common.Log;
using Senjyouhara.Common.Utils;
using Senjyouhara.Main.Comparer;
using Senjyouhara.Main.Config;
using Senjyouhara.Main.Core.Manager.Dialog;
using Senjyouhara.Main.models;
using Senjyouhara.Main.Views;
using Senjyouhara.UI.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace Senjyouhara.Main.ViewModels
{
    internal class FileNameComparison : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var nameA = x!;
            var nameB = y!;

            if (Regex.IsMatch(nameA, @"\d+\.\d+"))
            {
                if (!Regex.IsMatch(nameB, @"\d+\.\d+"))
                {
                    nameB = Regex.Replace(nameB, @"(\d+)", "$1.0");
                }
            }
            else if (Regex.IsMatch(nameB, @"\d+\.\d+"))
            {
                if (!Regex.IsMatch(nameA, @"\d+\.\d+"))
                {
                    nameA = Regex.Replace(nameA, @"(\d+)", "$1.0");
                }
            }

            return new FileComparer().Compare(nameA, nameB);
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : Screen
    {
        public bool IsSubMergeVideo { get; set; } = true;
        public string Tips { get; set; }
        private List<FileNameItem> OriginFileNameItems { get; set; } = new();
        public ObservableCollection<FileNameItem> FileNameItems { get; set; } = new();

        private FormData formData;

        [OnChangedMethod(nameof(FileNameItemsHandle))]
        public string Rename { get; set; } = string.Empty;

        private FormData FormData = new();

        private void FileNameItemsHandle()
        {
            var subUidList = (from item in FileNameItems select item.SubUidList).SelectMany(v => v).ToList();
            var otherList = FileNameItems.Where(v => !subUidList.Contains(v.Uid)).ToList();

            var appendList = formData?.AppendNumberList;
            var step = string.IsNullOrWhiteSpace(formData?.Step) ? 10 : (int)(double.Parse(formData?.Step) * 10);

            var firstNumber = string.IsNullOrWhiteSpace(formData?.FirstNumber)
                ? 1
                : (int.Parse(formData?.FirstNumber));
            firstNumber *= 10;
            AppendNumber select = null;

            for (int i = 0; i < otherList.Count; i++)
            {
                var item = otherList[i];
                var f = new FileInfo(item.FilePath);
                var name = Rename;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    if (name?.IndexOf("#") != -1)
                    {
                        // 根据文件数量获取前面填充0数量
                        var count = otherList.Count.ToString();
                        var prevIndex = (firstNumber - step) / 10;
                        var tmp = count;
                        Log.Debug(tmp);
                        var digitsNumber = string.IsNullOrWhiteSpace(formData?.DigitsNumber)
                            ? tmp.Length
                            : int.Parse(formData?.DigitsNumber);
                        var index = double.Parse((firstNumber / 10.0).ToString());
                        var indexStr = index.ToString("#.#");

                        name = name.Replace("#", $"{indexStr.PadLeft(digitsNumber, '0')}");
                        Log.Debug(
                            $"index: {indexStr},prevIndex: {prevIndex}, tmp: {tmp}, name: {name}, itemFile: {item.FileName}");
                        Log.Debug($"appendList: {appendList}");
                        if (appendList?.Count > 0)
                        {
                            var append = appendList.Where(v =>
                            {
                                Log.Debug(
                                    $"SerialNumber: {(v.SerialNumber)},prevIndex: {prevIndex}, compare: {(v.SerialNumber).Equals(prevIndex.ToString())}");
                                if (prevIndex > -1)
                                {
                                    return (v.SerialNumber).Equals(prevIndex.ToString());
                                }

                                return false;
                            }).FirstOrDefault();
                            Log.Debug($"select: {select}");
                            if (append != null && append != select)
                            {
                                select = append;
                                name = Rename.Replace("#",
                                    $"{prevIndex.ToString("#.#").PadLeft(digitsNumber, '0')}{(string.IsNullOrWhiteSpace(append.DecimalNumber) ? string.Empty : '.' + append.DecimalNumber)}");
                                Log.Debug($"name: {name}");
                            }
                            else
                            {
                                select = null;
                            }
                        }
                    }
                }
                else
                {
                    name = item.OriginFileName;
                }

                if (select == null)
                {
                    firstNumber += step;
                }

                item.PreviewFileName = name + (!string.IsNullOrEmpty(item.SuffixName)
                    ? $".{item.SuffixName}"
                    : string.Empty);
                item.PreviewFilePath = $"{f.DirectoryName}\\{name}" + (!string.IsNullOrEmpty(item.SuffixName)
                    ? $".{item.SuffixName}"
                    : string.Empty);

                if (item.SubUidList.Count > 0)
                {
                    var filter = FileNameItems.Where(v => item.SubUidList.Contains(v.Uid)).ToList();
                    foreach (var fileNameItem in filter)
                    {
                        string langSubffix =
                            PatternUtil.GetPatternResultFirst(@".[a-zA-Z-]+$", fileNameItem.OriginFileName) ??
                            string.Empty;
                        fileNameItem.PreviewFileName = name + (!string.IsNullOrEmpty(fileNameItem.SuffixName)
                            ? $"{(string.IsNullOrEmpty(langSubffix) ? "" : langSubffix)}.{fileNameItem.SuffixName}"
                            : string.Empty);
                        fileNameItem.PreviewFilePath = $"{f.DirectoryName}\\{name}" +
                                                       (!string.IsNullOrEmpty(fileNameItem.SuffixName)
                                                           ? $"{(string.IsNullOrEmpty(langSubffix) ? "" : langSubffix)}.{fileNameItem.SuffixName}"
                                                           : string.Empty);
                    }
                }
            }


            foreach (var originFileNameItem in OriginFileNameItems)
            {
                var find = FileNameItems.FirstOrDefault(item => item.Uid.Equals(originFileNameItem.Uid));
                if (find != null)
                {
                    originFileNameItem.PreviewFileName = find.PreviewFileName;
                    originFileNameItem.PreviewFilePath = find.PreviewFilePath;
                }
            }
        }


        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private readonly IDialogManager _dialogManager;

        public MainViewModel(IEventAggregator eventAggregator, IWindowManager windowManager,
            IDialogManager dialogManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            this._dialogManager = dialogManager;
            _eventAggregator.SubscribeOnUIThread(this);
            formData = new FormData();
            formData.AppendNumberList.Add(new());
            AddUpdateModal();
        }

        public void AddUpdateModal()
        {
            Task.Run(async () =>
            {
                if (UpdateConfig.IsEnableUpdate)
                {
                    var updateInfo = await UpdateConfig.GetUpdateData();
                    if (updateInfo.Version != AppConfig.Version)
                    {
                        await Application.Current.Dispatcher.BeginInvoke(async () =>
                        {
                            var update = IoC.Get<UpdateViewModel>();
                            await _windowManager.ShowDialogAsync(update);
                        });
                    }
                }
            });
        }

        public void AddLog()
        {
            var t = DateTime.Now;
            //Debug.WriteLine(t.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            Log.Debug($"添加日志~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
        }

        public void SelectFileHandle()
        {
            Tips = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\desktop"; //初始的文件夹
            openFileDialog.Filter = "所有文件(*.*)|*.*"; //在对话框中显示的文件类型
            openFileDialog.Title = "请选择文件";

            openFileDialog.Multiselect = true;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                var fileList = new List<string>(openFileDialog.FileNames);
                AddFilesHandle(fileList);
            }
        }

        public void ClearListHandle()
        {
            Tips = string.Empty;
            FileNameItems.Clear();
        }

        private static readonly string[] SubFileSubfixList = new string[] { "ass", "ssa", "srt" };

        private static readonly string[] VideoSubfixList = new string[]
            { "mkv", "mp4", "flv", "f4v", "avi", "rm", "rmvb", "mov", "wmv" };

        private void SubFileMerge()
        {
            var subList = FileNameItems.Where(v =>
                SubFileSubfixList.FirstOrDefault(s => s.Trim().ToLower().Equals(v.SuffixName)) != null).ToList();
            var otherList = FileNameItems.Where(v => !subList.Contains(v)).ToList();
            foreach (var fileNameItem in subList)
            {
                if (fileNameItem.FileName.StartsWith(" └─  "))
                {
                    fileNameItem.FileName = fileNameItem.FileName.Replace(" └─  ", "");
                }
            }

            foreach (var fileNameItem in otherList)
            {
                fileNameItem.SubUidList = new();
            }

            if (IsSubMergeVideo)
            {
                try
                {
                    var subSortList = subList.OrderBy(f => f.OriginFileName, new FileNameComparison()).ToList();
                    var newList = new List<FileNameItem>();
                    if (otherList.Count > 0)
                    {
                        var tmpSubList = new List<FileNameItem>();
                        foreach (var item in otherList)
                        {
                            // 如果为视频文件
                            if (VideoSubfixList.Contains(item.SuffixName.ToLower()))
                            {
                                newList.Add(item);
                                var number =
                                    PatternUtil.GetPatternResultFirst(@"[0-9]+(\.[0-9]+)*", item.OriginFileName);
                                Log.Info($"number: {number}");
                                if (number is { })
                                {
                                    // 寻找视频文件对应的字幕
                                    var subFilter = subSortList.Where(v =>
                                    {
                                        var tmp = PatternUtil.GetPatternResultFirst(@"[0-9]+(\.[0-9]+)*",
                                            v.OriginFileName);
                                        return tmp == number;
                                    }).Select(v =>
                                    {
                                        if (!v.FileName.StartsWith(" └─  "))
                                        {
                                            v.FileName = " └─  " + v.FileName;
                                        }

                                        return v;
                                    }).ToList();
                                    Log.Info($" subFilter : {subFilter}");
                                    item.SubUidList.AddRange(subFilter.Select(v => v.Uid));
                                    tmpSubList.AddRange(subFilter);
                                    newList.AddRange(subFilter);
                                }
                            }
                            else
                            {
                                newList.Add(item);
                            }
                        }

                        if (tmpSubList.Count < subList.Count)
                        {
                            var tmp = (from subItem in subList
                                    where !(from item in tmpSubList select item).ToList().Contains(subItem)
                                    select subItem
                                ).ToList();

                            Log.Info($"tmp: {tmp}");

                            if (tmp.Count > 0)
                            {
                                newList.AddRange(tmp);
                            }
                        }
                    }
                    else
                    {
                        newList = subList;
                    }

                    FileNameItems = new ObservableCollection<FileNameItem>(newList);
                }
                catch (Exception ex)
                {
                    Log.Error("error", ex);
                }
            }
            else
            {
                FileNameItems = new(OriginFileNameItems);
            }
        }

        private void AddFilesHandle(List<string> files)
        {
            var time = DateTime.Now;
            TimeSpan ts = time - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var Timestamp = Convert.ToInt64(ts.TotalMilliseconds); //精确到毫秒
            var selectList = files.Where(file =>
            {
                var flag = Directory.Exists(file);
                if (flag)
                {
                    return false;
                }

                return true;
            }).Select(file =>
            {
                var v = new FileInfo(file);
                return new FileNameItem()
                {
                    Uid = Guid.NewGuid().ToString(),
                    OriginFileName = v.Name.LastIndexOf(".", StringComparison.Ordinal) >= 0
                        ? v.Name.Substring(0, v.Name.LastIndexOf(".", StringComparison.Ordinal))
                        : v.Name,
                    FileName = v.Name,
                    FilePath = v.FullName,
                    PreviewFileName = string.Empty,
                    SubtitleFileName = string.Empty,
                    SuffixName = v.Name.LastIndexOf(".", StringComparison.Ordinal) >= 0
                        ? v.Name.Substring(v.Name.LastIndexOf(".", StringComparison.Ordinal) + 1)
                        : string.Empty
                };
            }).ToList();
            var sortList = new List<FileNameItem>(FileNameItems.ToList());
            selectList = selectList.OrderBy(v => v.OriginFileName, new FileNameComparison()).ToList();
            foreach (var item in selectList)
            {
                item.Timestamp = Timestamp++;
            }

            sortList.AddRange(selectList);
            OriginFileNameItems.AddRange(selectList);

            FileNameItems = new ObservableCollection<FileNameItem>(sortList);
            SubFileMerge();

            FileNameItemsHandle();
        }

        public void RenameFileHandle()
        {
            var count = FileNameItems.Count;
            if (count <= 0)
            {
                return;
            }

            var lisDup = FileNameItems.GroupBy(x => x.PreviewFileName).Where(x => x.Count() > 1).Select(x => x.Key)
                .ToList();
            if (lisDup.Count > 0)
            {
                System.Windows.MessageBox.Show("消息提示", "文件名重命名项有重复，请检查！", MessageBoxButton.OK);
                return;
            }


            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < FileNameItems.Count; i++)
                {
                    var item = FileNameItems[i];
                    var f = new FileInfo(item.FilePath);
                    if (f.Exists)
                    {
                        try
                        {
                            f.MoveTo(item.PreviewFilePath);
                            Application.Current.Dispatcher.BeginInvoke(() =>
                            {
                                item.FilePath = item.PreviewFilePath;
                                item.FileName = item.PreviewFileName;
                                item.OriginFileName = item.FileName.LastIndexOf(".", StringComparison.Ordinal) >= 0
                                    ? item.FileName.Substring(0,
                                        item.FileName.LastIndexOf(".", StringComparison.Ordinal))
                                    : item.FileName;
                                item.SuffixName = item.FileName.LastIndexOf(".", StringComparison.Ordinal) >= 0
                                    ? item.FileName.Substring(item.FileName.LastIndexOf(".", StringComparison.Ordinal) +
                                                              1)
                                    : string.Empty;
                                item.PreviewFileName = "成功！";
                            });
                            // Thread.Sleep(20);
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
            });
        }

        public void OnListViewDrop(object sender, DragEventArgs e)
        {
            var listView1 = sender as ListView;
            if (e.Data.GetData(System.Windows.DataFormats.FileDrop) is string[] fileDrop)
            {
                var fileNames = new List<string>(fileDrop);
                AddFilesHandle(fileNames);
            }
            else
            {
                var position = e.GetPosition(listView1!); //获取位置
                var result = VisualTreeHelper.HitTest(listView1, position); //根据位置得到result
                if (result == null)
                {
                    return; //找不到 返回
                }

                #region 查找元数据

                var sourcePerson = e.Data.GetData(typeof(FileNameItem)) as FileNameItem;
                if (sourcePerson == null)
                {
                    return;
                }

                #endregion

                #region 查找目标数据

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


                var sourceIndex = listView1.Items.IndexOf(sourcePerson);
                var targetIndex = listView1.Items.IndexOf(targetPerson!);

                if (listView1.ItemsSource is ObservableCollection<FileNameItem> source)
                {
                    var sourceItem = source[sourceIndex];
                    var targetItem = source[targetIndex];
                    (targetItem.Timestamp, sourceItem.Timestamp) = (sourceItem.Timestamp, targetItem.Timestamp);

                    if (SubFileSubfixList.Contains(sourceItem.SuffixName) ||
                        SubFileSubfixList.Contains(targetItem.SuffixName))
                    {
                        return;
                    }

                    source!.Move(sourceIndex, targetIndex);

                    var findIndex = OriginFileNameItems.FindIndex(item => item.Uid == sourceItem.Uid);
                    var findIndex2 = OriginFileNameItems.FindIndex(item => item.Uid == targetItem.Uid);
                    OriginFileNameItems[findIndex2] = sourceItem;
                    OriginFileNameItems[findIndex] = targetItem;

                    listView1.SelectedItem = source[targetIndex];
                }
            }
        }

        public void OnListViewPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var listView1 = sender as ListView;
                var items = listView1!.SelectedItems;
                var newArr = new FileNameItem[items.Count];
                items.CopyTo(newArr, 0);
                foreach (FileNameItem item in newArr)
                {
                    OriginFileNameItems.Remove(item);
                    FileNameItems.Remove(item);
                }

                listView1.SelectedItem = null;
            }
        }


        public void OnListViewPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var listView1 = sender as ListView;

                #region 源位置

                var pos = e.GetPosition(listView1); // 获取位置
                HitTestResult result = VisualTreeHelper.HitTest(listView1!, pos); //根据位置得到result
                if (result == null)
                {
                    return; //找不到 返回
                }

                var listBoxItem = Utils.FindVisualParent<ListBoxItem>(result.VisualHit);
                if (listBoxItem == null || listBoxItem.Content != listView1.SelectedItem)
                {
                    return;
                }

                DataObject dataObj =
                    new DataObject(listBoxItem.Content as FileNameItem ?? throw new InvalidOperationException());

                #endregion

                //DataObject dataObj = new DataObject(sender as FileNameItem);
                DragDrop.DoDragDrop(listView1, dataObj, DragDropEffects.Move); //调用方法
            }
        }

        public void CheckSubCommand()
        {
            SubFileMerge();
        }

        public void ShowGenerateRuleModal()
        {
            var tmp = JSONUtil.ToData<FormData>(JSONUtil.ToJSON(formData));
            var parm = new DialogParameters();
            parm.Add("detail", tmp);

            //MessageBoxHelper.Info("打开弹框", "消息提示", r =>
            //{
            //}, UI.Styles.MessageBoxWindow.ButtonType.OKCancel);
            _dialogManager.ShowMyDialogAsync(IoC.Get<GenerateRuleViewModel>(), parm, (r) =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    formData = r.Parameters.GetValue<FormData>("detail");
                    FileNameItemsHandle();
                }
            });
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