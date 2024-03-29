﻿using Caliburn.Micro;
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
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : Screen
    {
        public bool IsSubMergeVideo { get; set; } = true;
        public string Tips { get; set; }
        public ObservableCollection<FileNameItem> FileNameItems { get; set; } = new ObservableCollection<FileNameItem>();

        private FormData formData;

        [OnChangedMethod(nameof(FileNameItemsHandle))]
        public string Rename { get; set; } = string.Empty;

        private FormData FormData = new();

        private void FileNameItemsHandle()
        {

            var subList = FileNameItems.Where(v => SUB_FILE_SUBFIX_LIST.Where(s => s.Trim().ToLower().Equals(v.SuffixName)).FirstOrDefault() != null).ToList();
            var otherList = FileNameItems.Where(v => !subList.Contains(v)).ToList();

            var appendList = formData?.AppendNumberList;

            var step = string.IsNullOrWhiteSpace(formData?.Step) ? 10 : (int)(double.Parse(formData?.Step) * 10);

            if (otherList.Count > 0)
            {
                var otherCount = string.IsNullOrWhiteSpace(formData?.FirstNumber) ? 1 : (int.Parse(formData?.FirstNumber));
                otherCount *= 10;
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
                            var prevIndex = (otherCount - step) / 10;
                            var count = otherList.Count.ToString();
                            var tmp = count;
                            Log.Debug(tmp);
                            var DigitsNumber = string.IsNullOrWhiteSpace(formData?.DigitsNumber) ? tmp.Length : int.Parse(formData?.DigitsNumber);
                            var index = double.Parse((otherCount / 10.0).ToString());
                            var indexStr = index.ToString("#.#");

                            name = name.Replace("#", $"{indexStr.PadLeft(DigitsNumber, '0')}");
                            Log.Debug($"index: {indexStr},prevIndex: {prevIndex}, tmp: {tmp}, name: {name}, itemFile: {item.FileName}");
                            Log.Debug($"appendList: {appendList}");
                            if (appendList?.Count > 0)
                            {
                                var append = appendList.Where(v =>
                                {
                                    Log.Debug($"SerialNumber: {(v.SerialNumber)},prevIndex: {prevIndex}, compare: {(v.SerialNumber).Equals(prevIndex.ToString())}");
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
                                    name = Rename.Replace("#", $"{prevIndex.ToString("#.#").PadLeft(DigitsNumber, '0')}{(string.IsNullOrWhiteSpace(append.DecimalNumber) ? string.Empty : '.' + append.DecimalNumber)}");
                                    Log.Debug($"name: {name}");
                                } else
                                {
                                    select = null;
                                }
                            }

                        }
                    } else
                    {
                        name = item.OriginFileName;
                    }
                   
                    item.PreviewFileName = name + (!string.IsNullOrEmpty(item.SuffixName) ? $".{item.SuffixName}" : string.Empty);
                    item.PreviewFilePath = $"{f.DirectoryName}\\{name}" + (!string.IsNullOrEmpty(item.SuffixName) ? $".{item.SuffixName}" : string.Empty);
                    if (select == null)
                    {
                        otherCount += step;
                    }
                }
            }


            // 如果有字幕文件则进入该项 为了匹配对应视频文件序号
            if(subList.Count > 0)
            {
                var Count = string.IsNullOrWhiteSpace(formData?.FirstNumber) ? 1 : int.Parse(formData?.FirstNumber);
                Count *= 10;
                AppendNumber select = null;

                for (int i = 0; i < subList.Count; i++)
                {
                    var item = subList[i];
                    var name = Rename;
                    var f = new FileInfo(item.FilePath);
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        if (name.IndexOf("#") != -1)
                        {
                            // 根据文件数量获取前面填充0数量
                            var count = subList.Count.ToString();
                            var prevIndex = (Count - step) / 10;
                            var tmp = count;
                            var DigitsNumber = string.IsNullOrWhiteSpace(formData?.DigitsNumber) ? tmp.Length : int.Parse(formData?.DigitsNumber);
                            var index = double.Parse((Count / 10.0).ToString());
                            var indexStr = index.ToString("#.#");
                            name = name.Replace("#", $"{indexStr.PadLeft(DigitsNumber, '0')}");

                            if (appendList?.Count > 0)
                            {
                                var append = appendList.Where(v =>
                                {
                                    Log.Debug($"SerialNumber: {(v.SerialNumber)},prevIndex: {prevIndex}, compare: {(v.SerialNumber).Equals(prevIndex.ToString())}");
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
                                    name = Rename.Replace("#", $"{prevIndex.ToString("#.#").PadLeft(DigitsNumber, '0')}{(string.IsNullOrWhiteSpace(append.DecimalNumber) ? string.Empty : '.' + append.DecimalNumber)}");
                                    Log.Debug($"name: {name}");
                                }
                                else
                                {
                                    select = null;
                                }
                            }
                        }
                    } else
                    {
                        name = item.OriginFileName;
                    }
                    if(select == null)
                    {
                        Count += step;
                    }
                    item.PreviewFileName = name + (!string.IsNullOrEmpty(item.SuffixName) ? $".{item.SuffixName}" : string.Empty);
                    item.PreviewFilePath = $"{f.DirectoryName}\\{name}" + (!string.IsNullOrEmpty(item.SuffixName) ? $".{item.SuffixName}" : string.Empty);
                }
            }
           
        }


        private IEventAggregator _eventAggregator;
        private IWindowManager _windowManager;
        private readonly IDialogManager dialogManager;

        public MainViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, IDialogManager dialogManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            this.dialogManager = dialogManager;
            _eventAggregator.SubscribeOnUIThread(this);
            formData = new FormData();
            formData.AppendNumberList.Add(new ());
            AddUpdateModal();
        }

        public void AddUpdateModal()
        {
            Task.Run(async () =>
            {
                if (UpdateConfig.IsEnableUpdate)
                {
                    var _updateInfo = await UpdateConfig.GetUpdateData();
                    if (_updateInfo.Version != AppConfig.Version)
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
            Tips = string.Empty;
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
                    OriginFileName = v.Name.LastIndexOf(".") >= 0 ? v.Name.Substring(0, v.Name.LastIndexOf(".")) : v.Name,
                    FileName = v.Name,
                    FilePath = v.FullName,
                    PreviewFileName = string.Empty,
                    SubtitleFileName = string.Empty,
                    SuffixName = v.Name.LastIndexOf(".") >= 0 ? v.Name.Substring(v.Name.LastIndexOf(".") + 1) : string.Empty
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
                //groupList.Sort(MySort);
                groupList = groupList.OrderBy(f => f.FileName, new FileComparer()).ToList();
                sortList = sortList.Concat(groupList).ToList();
            }

            Log.Debug(group.ToString());

            FileNameItems = new ObservableCollection<FileNameItem>(sortList);
            FileNameItemsHandle();

            if (IsSubMergeVideo)
            {
                try
                {
                    var subList = FileNameItems.Where(v => SUB_FILE_SUBFIX_LIST.Where(s => s.Trim().ToLower().Equals(v.SuffixName)).FirstOrDefault() != null).ToList();
                    var otherList = FileNameItems.Where(v => !subList.Contains(v)).ToList();
                    
                    var subSortList = subList.OrderBy(f => f.OriginFileName, new FileComparer()).ToList();
                    var newList = new List<FileNameItem>();
                    foreach (var item in otherList)
                    {
                        // 如果为视频文件
                        if (VIDEO_SUBFIX_LIST.Contains(item.SuffixName.ToLower()))
                        {
                            newList.Add(item);
                            // 寻找视频文件对应的字幕
                            var FilterSub = subSortList;
                                newList = newList.Concat(FilterSub.Select(v =>
                                {
                                    v.FileName = " └─  " + v.FileName;
                                    return v;
                                })).ToList();
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
        }

        public void RenameFileHandle()
        {

            var count = FileNameItems.Count;
            if (count <= 0)
            {
                return;
            }

            var lisDup = FileNameItems.GroupBy(x => x.PreviewFileName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if(lisDup.Count > 0)
            {
                System.Windows.MessageBox.Show("消息提示", "文件名重命名项有重复，请检查！", MessageBoxButton.OK);
                return;
            }


            Task.Factory.StartNew(() => {
                for (int i = 0; i < FileNameItems.Count; i++)
                {
                    var item = FileNameItems[i];
                    var f = new FileInfo(item.FilePath);
                    if (f.Exists)
                    {
                        try
                        {
                            f.MoveTo(item.PreviewFilePath);
                            Application.Current.Dispatcher.Invoke(() => {
                                item.FilePath = item.PreviewFilePath;
                                item.FileName = item.PreviewFileName;
                                item.OriginFileName = item.FileName.LastIndexOf(".") >= 0 ? item.FileName.Substring(0, item.FileName.LastIndexOf(".")) : item.FileName;
                                item.SuffixName = item.FileName.LastIndexOf(".") >= 0 ? item.FileName.Substring(item.FileName.LastIndexOf(".") + 1) : string.Empty;
                                item.PreviewFileName = "成功！";
                            });
                            Thread.Sleep(80);
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

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var listView1 = sender as ListView;

                #region 源位置
                var pos = e.GetPosition(listView1);  // 获取位置
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
                DataObject dataObj = new DataObject(listBoxItem.Content as FileNameItem);
                #endregion


                //DataObject dataObj = new DataObject(sender as FileNameItem);
                DragDrop.DoDragDrop(listView1, dataObj, DragDropEffects.Move);  //调用方法
            }

        }

        public void ShowGenerateRuleModal()
        {

            var tmp = JSONUtil.ToData<FormData>(JSONUtil.ToJSON(formData));
            var parm = new DialogParameters();
            parm.Add("detail", tmp);
            
            //MessageBoxHelper.Info("打开弹框", "消息提示", r =>
            //{
            //}, UI.Styles.MessageBoxWindow.ButtonType.OKCancel);
            dialogManager.ShowMyDialogAsync(IoC.Get<GenerateRuleViewModel>(), parm, (r) =>
            {
                if(r.Result == ButtonResult.OK)
                {
                    formData = r.Parameters.GetValue<FormData>("detail");
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
