using Senjyouhara.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Senjyouhara.Common.Log
{
    /// <summary>
    /// 写日志类
    /// </summary>
    internal class LogUtil
    {
        #region 字段

        private static LogWriter _logWriter = new LogWriter();
        private static List<LogType> LogLevelList = new List<LogType>();

        #endregion


        #region 静态构造函数
        static LogUtil()
        {
            var values = LogType.GetValues(typeof(LogType));
            var level = LogConfig.LogLevel;

            var index = 0;
            var flag = false;

            foreach (var value in values)
            {
                if (value.Equals(level))
                {
                    index = (int)value;
                    flag = true;
                }
                if (flag)
                {
                    LogLevelList.Add((LogType) value);
                }
            }
        }
        #endregion

        #region 写操作日志
        /// <summary>
        /// 写操作日志
        /// </summary>
        public static void Info(string log)
        {
            Write(LogType.Info, log);
        }
        #endregion

        #region 写调试日志
        /// <summary>
        /// 写调试日志
        /// </summary>
        public static void Debug(string log)
        {
            Write(LogType.Debug, log);
        }
        #endregion

        #region 写警告日志
        /// <summary>
        /// 写调试日志
        /// </summary>
        public static void Warn(string log)
        {
            Write(LogType.Warn, log);
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
            Write(LogType.Error, log);
        }

        public static void Write(LogType type, string log)
        {
            Write(type, log, null);
        }

        private static bool FindList<T>(List<T> list, string t)
        {
            foreach (var item in list)
            {
                if (item.ToString().Equals(t))
                {
                    return true;
                }
            }

            return false;
        }

        public static void Write(LogType type, string log, Exception ex)
        {

            var find = FindList<LogType>(LogLevelList, type.ToString());
            if(!find)
            {
                return;
            }

            _logWriter.WriteLog(type, string.IsNullOrEmpty(log) ? ex?.ToString() : log + "：" + ex?.ToString());
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
