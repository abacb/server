using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rms.Server.Core.Azure.Service.Workers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestHelper;
using static Rms.Server.Core.Azure.Service.Workers.SingleTaskWorker;

namespace ServiceTest.Workers
{
    [TestClass]
    public class SingleTaskWorkerTest
    {
        /*
         * HACK:クロージャで呼び出す処理で使用しているオブジェクトが勝手に破棄されないよう参照を保持する修正をいれたが、
         * テストでうまいこと効果が確認できなかった。
         * 何か思いついたら追加してほしい。
         * */

        [TestMethod]
        public void RunningCountDoesNotExceedOne()
        {
            int sleepMilliSeconds = 3 * 1000;
            int count = 0;
            var worker = new SingleTaskWorker("Test");

            var mock = new Mock<ILogger>();
            ILogger logger = mock.Object;
            Action work = () =>
            {
                count++;
                Thread.Sleep(sleepMilliSeconds);
                count++;
            };
            // 非同期でがーっと投げて問題ないことを確認
            var tasks = new List<Task>();
            var results = new ConcurrentQueue<Result>();

            for (int i = 0; i < 50; i++)
            {
                tasks.Add(Task.Run(() => {
                    results.Enqueue(worker.StartTask(logger, work, count));
                    Assert.AreEqual(1, worker.RunningCount);
                }));
            }
            // Workerを叩き終わるのを待つ。
            // Workerが実行する非同期処理は待たない点に注意。
            Task t = Task.WhenAll(tasks);
            t.Wait();

            Assert.IsFalse(t.IsFaulted);
            Assert.AreEqual(1, results.Count((x) => x == Result.Started));
            Assert.AreEqual(49, results.Count((x) => x == Result.AlreadyRunning));

            // Workerが実行する非同期処理の完了するのを待つ
            Thread.Sleep(sleepMilliSeconds * 2);
            Assert.AreEqual(0, worker.RunningCount);

            // 再度実行できるはず。
            var result = worker.StartTask(logger, work, count);
            Assert.AreEqual(Result.Started, result);
            Assert.AreEqual(3, count);
            Assert.AreEqual(1, worker.RunningCount);
        }

        [TestMethod]
        public void ErrorOnWorkerTest()
        {
            int sleepMilliSeconds = 3 * 1000;
            int count = 0;
            var worker = new SingleTaskWorker("Test");

            var mock = new Mock<ILogger>();
            ILogger logger = mock.Object;

            Action workError = () =>
            {
                throw new Exception();
            };

            // タスクを開始する。
            var result = worker.StartTask(logger, workError);

            Thread.Sleep(500);
            // work内で例外が投げられても、ログ処理がされるだけでそれ以上は何もしない。

            // エラー後も正常に動作する
            Action work = () =>
            {
                count++;
                Thread.Sleep(sleepMilliSeconds);
                count++;
            };
            result = worker.StartTask(logger, work);
            Thread.Sleep(500);
            Assert.AreEqual(Result.Started, result);
            Assert.AreEqual(1, count);
        }

        // 戻り値でErrorが返るのは、Task.Runで例外が走ったときと、lockが取得できないケースのみのため、意図的に動作させることはできない。
    }
}
