﻿using System;
using System.Runtime.InteropServices;

namespace Senjyouhara.Common.Utils;


// 在我们需要保持屏幕常亮的时候，我们直接调用SystemSleep.PreventForCurrentThread()
// 在我们需要释放的时候，调用SystemSleep.RestoreForCurrentThread()即可
  /// <summary>
    /// 包含控制屏幕关闭以及系统休眠相关的方法。
    /// </summary>
    public static class SystemSleep
    {
        [DllImport("kernel32")]
        private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);
        [Flags]
        private enum ExecutionState : uint
        {
            /// <summary>
            /// Forces the system to be in the working state by resetting the system idle timer.
            /// </summary>
            SystemRequired = 0x01,

            /// <summary>
            /// Forces the display to be on by resetting the display idle timer.
            /// </summary>
            DisplayRequired = 0x02,

            /// <summary>
            /// This value is not supported. If <see cref="UserPresent"/> is combined with other esFlags values, the call will fail and none of the specified states will be set.
            /// </summary>
            [Obsolete("This value is not supported.")]
            UserPresent = 0x04,

            /// <summary>
            /// Enables away mode. This value must be specified with <see cref="Continuous"/>.
            /// <para />
            /// Away mode should be used only by media-recording and media-distribution applications that must perform critical background processing on desktop computers while the computer appears to be sleeping.
            /// </summary>
            AwaymodeRequired = 0x40,

            /// <summary>
            /// Informs the system that the state being set should remain in effect until the next call that uses <see cref="Continuous"/> and one of the other state flags is cleared.
            /// </summary>
            Continuous = 0x80000000,
        }
        /// <summary>
        /// 设置此线程此时开始一直将处于运行状态，此时计算机不应该进入睡眠状态。
        /// 此线程退出后，设置将失效。
        /// 如果需要恢复，请调用 <see cref="RestoreForCurrentThread"/> 方法。
        /// </summary>
        /// <param name="keepDisplayOn">
        /// 表示是否应该同时保持屏幕不关闭。
        /// 对于游戏、视频和演示相关的任务需要保持屏幕不关闭；而对于后台服务、下载和监控等任务则不需要。
        /// </param>
        public static void PreventForCurrentThread(bool keepDisplayOn = true)
        {
            SetThreadExecutionState(keepDisplayOn
                ? ExecutionState.Continuous | ExecutionState.SystemRequired | ExecutionState.DisplayRequired
                : ExecutionState.Continuous | ExecutionState.SystemRequired);
        }

        /// <summary>
        /// 恢复此线程的运行状态，操作系统现在可以正常进入睡眠状态和关闭屏幕。
        /// </summary>
        public static void RestoreForCurrentThread()
        {
            SetThreadExecutionState(ExecutionState.Continuous);
        }

        /// <summary>
        /// 重置系统睡眠或者关闭屏幕的计时器，这样系统睡眠或者屏幕能够继续持续工作设定的超时时间。
        /// </summary>
        /// <param name="keepDisplayOn">
        /// 表示是否应该同时保持屏幕不关闭。
        /// 对于游戏、视频和演示相关的任务需要保持屏幕不关闭；而对于后台服务、下载和监控等任务则不需要。
        /// </param>
        public static void ResetIdle(bool keepDisplayOn = true)
        {
            SetThreadExecutionState(keepDisplayOn
                ? ExecutionState.SystemRequired | ExecutionState.DisplayRequired
                : ExecutionState.SystemRequired);
        }
    }