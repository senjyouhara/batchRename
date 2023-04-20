using Senjyouhara.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Log
{
    /// <summary>
    /// 写日志类
    /// </summary>
    internal class LogUtil
    {
        #region 字段

        private static LogWriter _logWriter = new LogWriter();

        #endregion


        #region 静态构造函数
        static LogUtil()
        {
            //_logWriter = new LogWriter();
        }
        #endregion

        #region 写操作日志
        /// <summary>
        /// 写操作日志
        /// </summary>
        public static void Info(string log)
        {
            _logWriter.WriteLog(LogType.Info, log);
        }
        #endregion

        #region 写调试日志
        /// <summary>
        /// 写调试日志
        /// </summary>
        public static void Debug(string log)
        {
            _logWriter.WriteLog(LogType.Debug, log);
        }
        #endregion

        #region 写错误日志
        /// <summary>
        /// 写错误日志
        /// </summary>
        public static void Error(string log, Exception ex)
        {
            Error(string.IsNullOrEmpty(log) ? ex.ToString() : log + "：" + ex.ToString());
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        public static void Error(string log)
        {
            _logWriter.WriteLog(LogType.Error, log);
        }
        #endregion

        #region Dispose
        public static void Dispose()
        {
            _logWriter?.Dispose();
        }
        #endregion

    }
}
