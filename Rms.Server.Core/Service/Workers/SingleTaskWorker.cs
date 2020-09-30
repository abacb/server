using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Rms.Server.Core.Utility.Extensions;

namespace Rms.Server.Core.Azure.Service.Workers
{
    /// <summary>
    /// 一度に一つだけ走るタスクを管理する
    /// </summary>
    public class SingleTaskWorker
    {
        /// <summary>
        /// サービス名
        /// </summary>
        private readonly string _serviceName;

        /// <summary>
        /// ロックオブジェクト
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// タスク
        /// </summary>
        private Task runningTask;

        /// <summary>
        /// ロガー
        /// </summary>
        /// <remarks>
        /// ロガーはクロージャで受け取るため、その内部で使用しているオブジェクトが勝手に開放されないように参照を保持しておく。
        /// </remarks>
        private ILogger _log;

        /// <summary>
        /// 実行中のWork関数の中で実行するオブジェクトの参照
        /// </summary>
        /// <remarks>
        /// workはクロージャで受け取るため、その内部で使用しているオブジェクトが勝手に開放されないように参照を保持しておく。
        /// </remarks>
        private object[] _objectsUsedInWork;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="serviceName">サービス名</param>
        public SingleTaskWorker(string serviceName)
        {
            _serviceName = serviceName;
        }

        /// <summary>
        /// 結果
        /// </summary>
        public enum Result
        {
            /// <summary>
            /// 開始
            /// </summary>
            Started,

            /// <summary>
            /// すでに実行中
            /// </summary>
            AlreadyRunning,

            /// <summary>
            /// エラー
            /// </summary>
            Error
        }

        /// <summary>
        /// 実行中のタスク数
        /// </summary>
        public int RunningCount { get; private set; } = 0;

        /// <summary>
        /// タスクを開始する
        /// </summary>
        /// <param name="log">ロガー</param>
        /// <param name="work">実行する処理</param>
        /// <param name="objectsUsedInWork">workで参照するオブジェクト</param>
        /// <returns>true: タスクを開始可能な状態,false: すでにタスク実行中</returns>
        public Result StartTask(ILogger log, Action work, params object[] objectsUsedInWork)
        {
            try
            {
                var canRunTasks = InitStratWork();

                if (canRunTasks == false)
                {
                    log.Debug("Already started task.");
                    return Result.AlreadyRunning;
                }

                _log = log;
                _objectsUsedInWork = objectsUsedInWork;

                runningTask = Task.Run(() => DoWork(log, work));
                log.Debug("Kicked task.");
            }
            catch (Exception ex)
            {
                log.Error(ex, "Kicked task failed.");
                return Result.Error;
            }
            finally
            {
                log.Debug("End StartTask.");
            }

            return Result.Started;
        }

        /// <summary>
        /// タスクを実行可能か確認する
        /// </summary>
        /// <returns>true: 実行可能</returns>
        private bool InitStratWork()
        {
            bool canRunTasks = false;

            lock (lockObj)
            {
                if (RunningCount == 0)
                {
                    RunningCount++;
                    canRunTasks = true;
                }
            }

            return canRunTasks;
        }

        /// <summary>
        /// 処理を実行する
        /// </summary>
        /// <param name="log">ロガー</param>
        /// <param name="work">実行する処理</param>
        private void DoWork(ILogger log, Action work)
        {
            log.Debug("Start work.");

            try
            {
                work();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed work.");
            }
            finally
            {
                lock (lockObj)
                {
                    RunningCount--;
                }

                log.Debug("Finished work.");

                _objectsUsedInWork = null;
                _log = null;
            }
        }
    }
}
