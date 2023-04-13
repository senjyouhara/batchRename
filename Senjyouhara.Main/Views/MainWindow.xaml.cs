using Senjyouhara.Main.models;
using Senjyouhara.Main.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Senjyouhara.Main.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
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

        private void listView1_Drop(object sender, DragEventArgs e)
        {

            var FileDrop = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];

            if(FileDrop != null)
            {


                var FileNames = new List<string>(FileDrop);
                var FilterFile = FileNames.Select(v =>
                {

                    var flag = Directory.Exists(v);
                    var file = new FileInfo(v);
                    return flag ? null : file;
                }).Where(v => v != null).ToList();

                if(FilterFile.Count > 0)
                {
                    ObservableCollection<FileNameItem> source = listView1.ItemsSource as ObservableCollection<FileNameItem>;
                    FilterFile.ForEach(v =>
                    {
                        Debug.WriteLine(v.Name.LastIndexOf(".") >= 0 ? v.Name.Substring(v.Name.LastIndexOf(".") + 1) : "");
                        source.Add(new FileNameItem() { FilePath = v.FullName, FileName = v.Name, PreviewFileName = "", SubtitleFileName = "", SuffixName = v.Name.LastIndexOf(".") >= 0 ? v.Name.Substring(v.Name.LastIndexOf(".") + 1) : "" });
                    });
                    var list = source.ToList();
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
                                } else
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
                    } catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    
                }

            } else
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

        private void listView1_PreviewDragOver(object sender, DragEventArgs e)
        {

        }

        private void listView1_PreviewKeyUp(object sender, KeyEventArgs e)
        {

            if(e.Key == Key.Delete)
            {
                var source = listView1.ItemsSource as ObservableCollection<FileNameItem>;
                var items = listView1.SelectedItems;
                var newArr = new FileNameItem[items.Count];
                items.CopyTo(newArr, 0);
                foreach (FileNameItem item in newArr) {
                    source.Remove(item);
                }
                listView1.SelectedItem = null;
            }

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
