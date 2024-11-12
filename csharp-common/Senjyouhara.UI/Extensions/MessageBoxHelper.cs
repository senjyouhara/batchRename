using Senjyouhara.UI.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Senjyouhara.UI.Styles.MessageBoxWindow;

namespace Senjyouhara.UI.Extensions
{
    /// <summary>
    /// ClassName：  MessageBoxHelper
    /// Description：自定义消息弹窗
    /// Author：     luc
    /// CreatTime：  2022-12-26 21:43:23  
    /// </summary>
    public class MessageBoxHelper
    {
        /// <summary>
        /// 信息提示
        /// </summary>
        /// <param name="content">提示信息</param>
        /// <param name="caption">标题</param>
        /// <param name="callback">返回结果</param>
        public static void Info(object content, string caption, Action<MessageBoxResult> callback = null, ButtonType button = ButtonType.OKCancel) => Show(content, caption, MessageBoxType.Info, button, callback);

        /// <summary>
        /// 成功信息提示
        /// </summary>
        /// <param name="content">提示信息</param>
        /// <param name="caption">标题</param>
        /// <param name="callback">返回结果</param>
        public static void Success(object content, string caption, Action<MessageBoxResult> callback = null, ButtonType button = ButtonType.OKCancel) => Show(content, caption, MessageBoxType.Success, button, callback);

        /// <summary>
        /// 警告信息提示
        /// </summary>
        /// <param name="content">提示信息</param>
        /// <param name="caption">标题</param>
        /// <param name="callback">返回结果</param>
        public static void Warning(object content, string caption, Action<MessageBoxResult> callback = null, ButtonType button = ButtonType.OKCancel) => Show(content, caption, MessageBoxType.Warning, button, callback);

        /// <summary>
        /// 错误信息提示
        /// </summary>
        /// <param name="content">提示信息</param>
        /// <param name="caption">标题</param>
        /// <param name="callback">返回结果</param>
        public static void Error(object content, string caption, Action<MessageBoxResult> callback = null, ButtonType button = ButtonType.OKCancel) => Show(content, caption, MessageBoxType.Error, button, callback);

        /// <summary>
        /// 询问信息提示
        /// </summary>
        /// <param name="content">提示信息</param>
        /// <param name="caption">标题</param>
        /// <param name="callback">返回结果</param>
        public static void Ask(object content, string caption, Action<MessageBoxResult> callback = null, ButtonType button = ButtonType.OKCancel) => Show(content, caption, MessageBoxType.Ask, button, callback);


        public static void Show(object messageBoxText, string caption, MessageBoxType type, ButtonType button = ButtonType.OKCancel, Action<MessageBoxResult> callback = null, string confirmName = "", string cannelName = "")
        {
            MessageBoxWindow window = new MessageBoxWindow();
            window.Show(messageBoxText, caption, type, button, callback, confirmName, cannelName);
        }
    }
}
