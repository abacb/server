using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Operation.Abstraction.Repositories;
using Rms.Server.Operation.Azure.Functions.MailSender;
using Rms.Server.Operation.Azure.Functions.StartUp;
using Rms.Server.Operation.Service.Services;
using Rms.Server.Operation.Utility;
using Rms.Server.Operation.Utility.Models;
using Rms.Server.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Azure.Functions.UnitTest
{
    /// <summary>
    /// MailSenderControllerTest
    /// </summary>
    [TestClass]
    public class MailSenderControllerTest
    {
        /// <summary>テスト結果格納先のルートフォルダ</summary>
        private static readonly string TestResultRootDir = string.Format(@"TestResult.{0:yyyyMMddHHmmss}\{1}", DateTime.Now, typeof(MailSenderController).Name);

        /// <summary>テスト設定</summary>
        private static readonly Dictionary<string, string> DefaultAppSettingValues = new Dictionary<string, string>()
        {
            { "ConnectionString", "BlobEndpoint=https://rmsopemujpemain01.blob.core.windows.net/;QueueEndpoint=https://rmsopemujpemain01.queue.core.windows.net/;FileEndpoint=https://rmsopemujpemain01.file.core.windows.net/;TableEndpoint=https://rmsopemujpemain01.table.core.windows.net/;SharedAccessSignature=sv=2019-02-02&ss=bfqt&srt=sco&sp=rwdlacup&se=9999-12-31T14:59:59Z&st=2019-12-31T15:00:00Z&spr=https&sig=df0HxgGoFo3k%2Fk8gIsfYxOPVolY7xCcP%2Be5G%2BpVYJT0%3D" },
            { "SendGridApiKey", "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" },
            { "MailTextFormat", "お客様番号：         {0}\n顧客名：           {1}\n機器シリアル番号：      {2}\n機器管理番号：        {3}\n機器名：           {4}\n機種コード：         {5}\nエラーコード：        {6}\nアラームレベル：       {7}\nイベント日時：        {8}\n説明：            {9}" },
            { "FailureBlobContainerName", "operation-unit-test" },
            { "BlobAccessMaxAttempts", "3" },
            { "BlobAccessDelayDeltaSeconds", "3" },
            { "SendGridAccessMaxAttempts", "3" },
            { "SendGridDelayDeltaSeconds", "3" },
        };

        /// <summary>評価対象外のカラム名</summary>
        private static readonly string[] IgnoreColumns = new string[]
        {
        };

        /// <summary>テスト実行時間</summary>
        private static readonly DateTime TestTime = new DateTime(2030, 1, 1);

        /// <summary>FailureBlob</summary>
        private FailureBlob failureBlob;

        /// <summary>アプリケーション設定</summary>
        private OperationAppSettings settings;

        /// <summary>テスト対象</summary>
        private MailSenderController target;

        /// <summary>
        /// ClassInit
        /// </summary>
        /// <param name="context">TestContext</param>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            if (Directory.Exists(TestResultRootDir))
            {
                Directory.Delete(TestResultRootDir, true);
            }
        }

        /// <summary>
        /// ClassCleanup
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        /// <summary>
        /// TestInitialize
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            DependencyInjection();

            foreach (CloudBlockBlob blockBlob in failureBlob.Client.GetBlockBlobs(DefaultAppSettingValues["FailureBlobContainerName"]))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }
        }

        /// <summary>
        /// TestCleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            DependencyInjection();

            foreach (CloudBlockBlob blockBlob in failureBlob.Client.GetBlockBlobs(DefaultAppSettingValues["FailureBlobContainerName"]))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }
        }

        /// <summary>
        /// DequeueMailInfoTest
        /// </summary>
        /// <param name="no">テスト番号</param>
        /// <param name="mock_MailRepository_ResponseCode">MailRepository.SendMailのモックが返却するHTTPステータスコード</param>
        /// <param name="mock_MailRepository_ResponseException">MailRepository.SendMailのモックが返却するException。Exceptionを返却させない場合はnullを指定する。</param>
        /// <param name="in_QueueItem">インプットアラームキューを記載したjsonファイルのパス</param>
        /// <param name="expected_FailureBlobFileSet">FailureBlobのファイル・フォルダ構成の期待値</param>
        /// <param name="expected_LogMessages">ログ出力の期待値を記載したファイルのパス</param>
        /// <param name="remarks">備考欄</param>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Functions_MailSender_DequeueMailInfo.csv")]
        public async Task DequeueMailInfoTest(string no, string mock_MailRepository_ResponseCode, string mock_MailRepository_ResponseException, string in_QueueItem, string expected_FailureBlobFileSet, string expected_LogMessages, string remarks)
        {
            List<TestLog> actual_logs = new List<TestLog>();

            List<MailInfo> actual_mails = new List<MailInfo>();

            // メールリポジトリのスタブが返すHTTPステータスコード
            HttpStatusCode mockResponseCode = HttpStatusCode.Accepted;
            if (!string.IsNullOrEmpty(mock_MailRepository_ResponseCode))
            {
                int code = int.Parse(mock_MailRepository_ResponseCode);
                mockResponseCode = (HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), code);
            }

            // メールリポジトリのスタブが返すException（Exceptionを返さない場合はnullを代入）
            Exception mockResponseException = null;
            if (!string.IsNullOrEmpty(mock_MailRepository_ResponseException))
            {
                Type type = mock_MailRepository_ResponseException.StartsWith("Rms") ? typeof(RmsException).Assembly.GetType(mock_MailRepository_ResponseException) : Type.GetType(mock_MailRepository_ResponseException);
                mockResponseException = Activator.CreateInstance(type) as Exception;
            }

            // DI
            if (string.IsNullOrEmpty(mock_MailRepository_ResponseCode) && string.IsNullOrEmpty(mock_MailRepository_ResponseException))
            {
                DependencyInjection(DefaultAppSettingValues, actual_logs, null, mockResponseCode, mockResponseException);
            }
            else
            {
                DependencyInjection(DefaultAppSettingValues, actual_logs, actual_mails, mockResponseCode, mockResponseException);
            }

            // テストデータ準備
            {
                in_QueueItem = (in_QueueItem != null && File.Exists(in_QueueItem)) ? File.ReadAllText(in_QueueItem) : in_QueueItem;
            }

            // 期待値
            DirectoryInfo expectedFailureDir = new DirectoryInfo(expected_FailureBlobFileSet);
            string[] expectedFailureFiles = expectedFailureDir.Exists ? expectedFailureDir.GetFiles("*", SearchOption.AllDirectories).Select(x => x.FullName).OrderBy(x => x).ToArray() : new string[] { };
            string[] expected_failure_names = expectedFailureFiles.Select(x => x.Replace(expectedFailureDir.FullName, string.Empty)).ToArray();
            string[] expected_failure_contents = expectedFailureFiles.Select(x => File.ReadAllText(x)).ToArray();
            List<string> expected_log_messages = (expected_LogMessages != null && File.Exists(expected_LogMessages)) ? File.ReadLines(expected_LogMessages).ToList() : new List<string>();

            // テスト実行
            await target.DequeueMailInfo(in_QueueItem, new TestLogger<MailSenderController>(actual_logs));

            // テスト結果
            DirectoryInfo actualDir = new DirectoryInfo(Path.Combine(TestResultRootDir, no));
            string[] actualFailureFiles = failureBlob.Client.GetFiles(settings.FailureBlobContainerName, actualDir).OrderBy(x => x).ToArray();
            string[] actual_failure_names = actualFailureFiles.Select(x => x.Replace(actualDir.FullName, string.Empty)).ToArray();
            string[] actual_failure_contents = actualFailureFiles.Select(x => File.ReadAllText(x)).ToArray();
            List<string> actual_log_messages = actual_logs.Select(x => x.GetSimpleText()).ToList();

            // 確認（Failureブロブ）
            CollectionAssert.AreEqual(expected_failure_names, actual_failure_names);
            CollectionAssert.AreEqual(expected_failure_contents, actual_failure_contents);

            // 確認（ログ）
            foreach (var expected_log_message in expected_log_messages)
            {
                var matching_element = actual_log_messages.FirstOrDefault(actual => actual.Contains(expected_log_message));
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(matching_element, string.Format("「{0}」に一致する要素が見つかりません", expected_log_message));
                if (matching_element != null)
                {
                    actual_log_messages.Remove(matching_element);
                }
            }
        }

        /// <summary>
        /// DIを実行する
        /// </summary>
        /// <param name="appSettings">アプリケーション設定を上書きする場合は指定する</param>
        /// <param name="testLogs">ログの格納先</param>
        /// <param name="testMails">送信したメール情報を格納するリスト</param>
        /// <param name="mockResponseCode">SendMailメソッドが返却するHTTPステータスコード</param>
        /// <param name="mockResponseException">SendMailメソッドが返却するException</param>
        private void DependencyInjection(Dictionary<string, string> appSettings = null, List<TestLog> testLogs = null, List<MailInfo> testMails = null, HttpStatusCode mockResponseCode = HttpStatusCode.Accepted, Exception mockResponseException = null)
        {
            var builder = new TestDiProviderBuilder<OperationAppSettings>(FunctionsHostBuilderExtend.AddUtility);
            builder.ServiceCollection.AddTransient<MailSenderController>();
            builder.ServiceCollection.AddTransient<FailureBlob>();
            builder.AddConfigures(appSettings);

            Mock<DateTimeProvider> timeProviderMock = new Mock<DateTimeProvider>();
            timeProviderMock.SetupGet(tp => tp.UtcNow).Returns(TestTime);
            builder.ServiceCollection.AddSingleton<ITimeProvider>(timeProviderMock.Object);

            if (testLogs != null)
            {
                builder.ServiceCollection.AddSingleton<ILogger<MailSenderService>>(new TestLogger<MailSenderService>(testLogs));
            }

            if (testMails != null)
            {
                builder.ServiceCollection.AddTransient<IMailRepository>(x => new TestMailRepository(testMails, mockResponseCode, mockResponseException));
            }

            ServiceProvider provider = builder.Build();
            failureBlob = provider.GetService<FailureBlob>();
            settings = provider.GetService<AppSettings>() as OperationAppSettings;
            target = provider.GetService<MailSenderController>();
        }

        /// <summary>
        /// メールリポジトリのスタブ
        /// </summary>
        public class TestMailRepository : IMailRepository
        {
            /// <summary>送信したメール情報を格納するリスト</summary>
            private List<MailInfo> mails;

            /// <summary>SendMailメソッドが返却するHTTPステータスコード</summary>
            private HttpStatusCode responseCode = HttpStatusCode.Accepted;

            /// <summary>SendMailメソッドが返却するException</summary>
            private Exception responseException = null;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="mails">送信したメール情報を格納するリスト</param>
            /// <param name="responseCode">SendMailメソッドが返却するHTTPステータスコード</param>
            /// <param name="responseException">SendMailメソッドが返却するException</param>
            public TestMailRepository(List<MailInfo> mails, HttpStatusCode responseCode, Exception responseException)
            {
                if (mails == null)
                {
                    throw new ArgumentNullException("mails");
                }

                this.mails = mails;
                this.responseCode = responseCode;
                this.responseException = responseException;
            }

            /// <summary>
            /// メールを送信する(スタブ)
            /// </summary>
            /// <param name="mailInfo">メール情報</param>
            /// <returns>メール送信結果（HTTPステータスコードとレスポンスボディ）</returns>
            public async Task<KeyValuePair<HttpStatusCode, string>> SendMail(MailInfo mailInfo)
            {
                if (responseException != null)
                {
                    throw responseException;
                }

                mails.Add(mailInfo);

                return new KeyValuePair<HttpStatusCode, string>(responseCode, string.Format("status code is {0}.", responseCode));
            }
        }
    }
}
