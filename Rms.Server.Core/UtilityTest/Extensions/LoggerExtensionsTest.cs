using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Properties;
using Rms.Server.Test;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityTest.Extensions
{
    /// <summary>
    /// ログ出力拡張メソッドテスト
    /// </summary>
    /// <remarks>
    /// WarnおよびErrorのテストにはUtilityのプロパティに設定したエラーメッセージ用のメッセージコードを使用する。
    /// 引数を取らないメッセージとして[CO_API_DFC_001]を、引数を取るメッセージとして[CO_API_DFC_002]を使用する。
    /// メッセージの文言まではテストしないため、メッセージ修正は本テストコードには影響しない。
    /// ただし、引数を取る/取らないを含めてメッセージコードを変更する場合には、本テストに影響が出るため注意すること。
    /// </remarks>
    [TestClass]
    public class LoggerExtensionsTest
    {
        #region Debug

        /// <summary>
        /// Debugテスト（引数なし）
        /// </summary>
        /// <param name="targetMessage">出力するログメッセージ</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(DebugDynamicDataWithoutArguments), DynamicDataSourceType.Method)]
        public void DebugTestWithoutArguments(string targetMessage, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Debug(logger, targetMessage);

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, null);
        }

        /// <summary>
        /// Debugテスト（引数あり）
        /// </summary>
        /// <param name="targetMessage">出力するログメッセージ</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(DebugDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void DebugTestWithArguments(string targetMessage, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Debug(
                    logger, targetMessage, 
                    new object[] { LoggerExtensionsTestObject.GetJsonString(targetArgs) }
                );

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// DebugJsonテスト
        /// </summary>
        /// <param name="targetMessage">出力するログメッセージ</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(DebugDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void DebugJsonTest(string targetMessage, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.DebugJson(logger, targetMessage, targetArgs);

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// 引数なしDebugLog用テストデータ
        /// </summary>
        /// <returns>テストデータリスト</returns>
        public static IEnumerable<object[]> DebugDynamicDataWithoutArguments() {
            yield return new object[] {
                // 出力するログメッセージ
                "TestMessage", 

                // 期待するログ
                new ExpectedLog { MessageFormat = "TestMessage", Exception = null, LogLevel = LogLevel.Debug },
            };
        }

        /// <summary>
        /// 引数ありDebugLog用テストデータ
        /// </summary>
        /// <returns>テストデータリスト</returns>
        public static IEnumerable<object[]> DebugDynamicDataWithArguments()
        {
            // 引数ありのテスト
            {
                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject("value1", "value2"),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "TestMessage: {0}", Exception = null, LogLevel = LogLevel.Debug },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject(null, null),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "TestMessage: {0}", Exception = null, LogLevel = LogLevel.Debug },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject(string.Empty, string.Empty),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "TestMessage: {0}", Exception = null, LogLevel = LogLevel.Debug },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    null,

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "TestMessage: {0}", Exception = null, LogLevel = LogLevel.Debug },
                };
            }

            // 引数なしのテスト
            {
                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject("value1", "value2"),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "TestMessage", Exception = null, LogLevel = LogLevel.Debug },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage", 

                    // 出力するログに渡す引数オブジェクト
                    null,

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "TestMessage", Exception = null, LogLevel = LogLevel.Debug },
                };
            }
        }

        #endregion

        #region Info

        /// <summary>
        /// Infoテスト（引数なし）
        /// </summary>
        /// <param name="targetMessage">出力するログメッセージ</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(InfoDynamicDataWithoutArguments), DynamicDataSourceType.Method)]
        public void InfoTestWithoutArguments(string targetMessage, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Info(logger, targetMessage);

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, null);
        }

        /// <summary>
        /// Infoテスト（引数あり）
        /// </summary>
        /// <param name="targetMessage">出力するログメッセージ</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(InfoDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void InfoTestWithArguments(string targetMessage, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Info(
                    logger, targetMessage,
                    new object[] { LoggerExtensionsTestObject.GetJsonString(targetArgs) }
                );

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// InfoJsonテスト
        /// </summary>
        /// <param name="targetMessage">出力するログメッセージ</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(InfoDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void InfoJsonTest(string targetMessage, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.InfoJson(logger, targetMessage, targetArgs);

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// 引数なしInfoLog用テストデータ
        /// </summary>
        /// <returns>テストデータリスト</returns>
        public static IEnumerable<object[]> InfoDynamicDataWithoutArguments()
        {
            yield return new object[] {
                // 出力するログメッセージ
                "TestMessage", 

                // 期待するログ
                new ExpectedLog { MessageFormat = "TestMessage", Exception = null, LogLevel = LogLevel.Information },
            };
        }

        /// <summary>
        /// 引数ありInfoLog用テストデータ
        /// </summary>
        /// <returns>テストデータリスト</returns>
        public static IEnumerable<object[]> InfoDynamicDataWithArguments()
        {
            // 引数ありのテスト
            {
                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject("value1", "value2"),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "TestMessage: {0}", Exception = null, LogLevel = LogLevel.Information },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject(null, null),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "TestMessage: {0}", Exception = null, LogLevel = LogLevel.Information },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject(string.Empty, string.Empty),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "TestMessage: {0}", Exception = null, LogLevel = LogLevel.Information },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    null,

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "TestMessage: {0}", Exception = null, LogLevel = LogLevel.Information },
                };
            }

            // 引数なしのテスト
            {
                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject("value1", "value2"),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "TestMessage", Exception = null, LogLevel = LogLevel.Information },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage", 

                    // 出力するログに渡す引数オブジェクト
                    null,

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "TestMessage", Exception = null, LogLevel = LogLevel.Information },
                };
            }
        }

        #endregion

        #region Warn

        /// <summary>
        /// Warnテスト（引数なし, Exceptionなし）
        /// </summary>
        /// <param name="targetException">出力するException</param>
        /// <param name="targetMessageCode">出力するログのメッセージコード</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(WarnDynamicDataWithoutArguments), DynamicDataSourceType.Method)]
        public void WarnTestWithoutArguments(Exception targetException, string targetMessageCode, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Warn(logger, targetMessageCode);

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, null);
        }

        /// <summary>
        /// Warnテスト（引数なし, Exceptionあり）
        /// </summary>
        /// <param name="targetException">出力するException</param>
        /// <param name="targetMessageCode">出力するログのメッセージコード</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(WarnDynamicDataWithoutArguments), DynamicDataSourceType.Method)]
        public void WarnTestWithoutArgumentsWithException(Exception targetException, string targetMessageCode, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Warn(logger, targetException, targetMessageCode);

            // ログを確認する
            CheckEqualityLogLevelsAndExceptionsAndMessages(logs, expectedLog, null);
        }

        /// <summary>
        /// Warnテスト（引数あり, Exceptionなし）
        /// </summary>
        /// <param name="targetException">出力するException</param>
        /// <param name="targetMessageCode">出力するログのメッセージコード</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(WarnDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void WarnTestWithArguments(
            Exception targetException, string targetMessageCode, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Warn(
                logger, targetMessageCode, new object[] { LoggerExtensionsTestObject.GetJsonString(targetArgs) });

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// Warnテスト（引数あり, Exceptionあり）
        /// </summary>
        /// <param name="targetException">出力するException</param>
        /// <param name="targetMessageCode">出力するログのメッセージコード</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(WarnDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void WarnTestWithArgumentsWithException(
            Exception targetException, string targetMessageCode, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Warn(
                logger, targetException, targetMessageCode, new object[] { LoggerExtensionsTestObject.GetJsonString(targetArgs) });

            // ログを確認する
            CheckEqualityLogLevelsAndExceptionsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// WarnJsonテスト（Exceptionなし）
        /// </summary>
        /// <param name="targetException">出力するException</param>
        /// <param name="targetMessageCode">出力するログのメッセージコード</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(WarnDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void WarnJsonTest(
            Exception targetException, string targetMessageCode, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.WarnJson(logger, targetMessageCode, targetArgs);

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// WarnJsonテスト（Exceptionあり）
        /// </summary>
        /// <param name="targetException">出力するException</param>
        /// <param name="targetMessageCode">出力するログのメッセージコード</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(WarnDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void WarnJsonWithException(
            Exception targetException, string targetMessageCode, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.WarnJson(logger, targetException, targetMessageCode, targetArgs);

            // ログを確認する
            CheckEqualityLogLevelsAndExceptionsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// 引数なしWarnLog用テストデータ
        /// </summary>
        /// <returns>テストデータリスト</returns>
        public static IEnumerable<object[]> WarnDynamicDataWithoutArguments()
        {
            yield return new object[] {
                // 出力するException
                new RmsException("WarnLog"),

                // メッセージコード
                nameof(Resources.CO_API_DFC_001), 

                // 期待するログ
                new ExpectedLog { MessageFormat = nameof(Resources.CO_API_DFC_001), Exception = new RmsException("WarnLog"), LogLevel = LogLevel.Warning },
            };
        }

        /// <summary>
        /// 引数ありWarnLog用テストデータ
        /// </summary>
        /// <returns>テストデータリスト</returns>
        public static IEnumerable<object[]> WarnDynamicDataWithArguments()
        {
            // 引数ありのテスト
            {
                yield return new object[] {
                    // 出力するException
                    new RmsException("WarnLog"),

                    // メッセージコード
                    nameof(Resources.CO_API_DFC_002),

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject("value1", "value2"),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "{0}", Exception = new RmsException("WarnLog"), LogLevel = LogLevel.Warning },
                };

                yield return new object[] {
                    // 出力するException
                    new RmsException("WarnLog"),

                    // メッセージコード
                    nameof(Resources.CO_API_DFC_002),

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject(null, null),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "{0}", Exception = new RmsException("WarnLog"), LogLevel = LogLevel.Warning },
                };

                yield return new object[] {
                    // 出力するException
                    new RmsException("WarnLog"),

                    // メッセージコード
                    nameof(Resources.CO_API_DFC_002),

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject(string.Empty, string.Empty),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "{0}", Exception = new RmsException("WarnLog"), LogLevel = LogLevel.Warning },
                };

                yield return new object[] {
                    // 出力するException
                    new RmsException("WarnLog"),

                    // メッセージコード
                    nameof(Resources.CO_API_DFC_002),

                    // 出力するログに渡す引数オブジェクト
                    null,

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "{0}", Exception = new RmsException("WarnLog"), LogLevel = LogLevel.Warning },
                };
            }

            // 引数なしのテスト
            {
                yield return new object[] {
                    // 出力するException
                    new RmsException("WarnLog"),

                    // メッセージコード
                    nameof(Resources.CO_API_DFC_001),

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject("value1", "value2"),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = nameof(Resources.CO_API_DFC_001), Exception = new RmsException("WarnLog"), LogLevel = LogLevel.Warning },
                };

                yield return new object[] {
                    // 出力するException
                    new RmsException("WarnLog"),

                    // メッセージコード
                    nameof(Resources.CO_API_DFC_001),

                    // 出力するログに渡す引数オブジェクト
                    null,

                    // 期待するログ
                    new ExpectedLog { MessageFormat = nameof(Resources.CO_API_DFC_001), Exception = new RmsException("WarnLog"), LogLevel = LogLevel.Warning },
                };
            }
        }

        #endregion

        #region Error

        /// <summary>
        /// Errorテスト（引数なし, Exceptionなし）
        /// </summary>
        /// <param name="targetException">出力するException</param>
        /// <param name="targetMessageCode">出力するログのメッセージコード</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(ErrorDynamicDataWithoutArguments), DynamicDataSourceType.Method)]
        public void ErrorTestWithoutArguments(Exception targetException, string targetMessageCode, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Error(logger, targetMessageCode);

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, null);
        }

        /// <summary>
        /// Errorテスト（引数なし, Exceptionあり）
        /// </summary>
        /// <param name="targetException">出力するException</param>
        /// <param name="targetMessageCode">出力するログのメッセージコード</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(ErrorDynamicDataWithoutArguments), DynamicDataSourceType.Method)]
        public void ErrorTestWithoutArgumentsWithException(Exception targetException, string targetMessageCode, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Error(logger, targetException, targetMessageCode);

            // ログを確認する
            CheckEqualityLogLevelsAndExceptionsAndMessages(logs, expectedLog, null);
        }

        /// <summary>
        /// Errorテスト（引数あり, Exceptionなし）
        /// </summary>
        /// <param name="targetException">出力するException</param>
        /// <param name="targetMessageCode">出力するログのメッセージコード</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(ErrorDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void ErrorTestWithArguments(
            Exception targetException, string targetMessageCode, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Error(
                logger, targetMessageCode, new object[] { LoggerExtensionsTestObject.GetJsonString(targetArgs) });

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// Errorテスト（引数あり, Exceptionあり）
        /// </summary>
        /// <param name="targetException">出力するException</param>
        /// <param name="targetMessageCode">出力するログのメッセージコード</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(ErrorDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void ErrorTestWithArgumentsWithException(
            Exception targetException, string targetMessageCode, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Error(
                logger, targetException, targetMessageCode, new object[] { LoggerExtensionsTestObject.GetJsonString(targetArgs) });

            // ログを確認する
            CheckEqualityLogLevelsAndExceptionsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// ErrorJsonテスト（Exceptionなし）
        /// </summary>
        /// <param name="targetException">出力するException</param>
        /// <param name="targetMessageCode">出力するログのメッセージコード</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(ErrorDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void ErrorJsonTest(
            Exception targetException, string targetMessageCode, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.ErrorJson(logger, targetMessageCode, targetArgs);

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// ErrorJsonテスト（Exceptionあり）
        /// </summary>
        /// <param name="targetException">出力するException</param>
        /// <param name="targetMessageCode">出力するログのメッセージコード</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(ErrorDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void ErrorJsonWithException(
            Exception targetException, string targetMessageCode, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.ErrorJson(logger, targetException, targetMessageCode, targetArgs);

            // ログを確認する
            CheckEqualityLogLevelsAndExceptionsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// 引数なしErrorLog用テストデータ
        /// </summary>
        /// <returns>テストデータリスト</returns>
        public static IEnumerable<object[]> ErrorDynamicDataWithoutArguments()
        {
            yield return new object[] {
                // 出力するException
                new RmsException("ErrorLog"),

                // メッセージコード
                nameof(Resources.CO_API_DFC_001), 

                // 期待するログ
                new ExpectedLog { MessageFormat = nameof(Resources.CO_API_DFC_001), Exception = new RmsException("ErrorLog"), LogLevel = LogLevel.Error },
            };
        }

        /// <summary>
        /// 引数ありErrorLog用テストデータ
        /// </summary>
        /// <returns>テストデータリスト</returns>
        public static IEnumerable<object[]> ErrorDynamicDataWithArguments()
        {
            // 引数ありのテスト
            {
                yield return new object[] {
                    // 出力するException
                    new RmsException("ErrorLog"),

                    // メッセージコード
                    nameof(Resources.CO_API_DFC_002),

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject("value1", "value2"),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "{0}", Exception = new RmsException("ErrorLog"), LogLevel = LogLevel.Error },
                };

                yield return new object[] {
                    // 出力するException
                    new RmsException("ErrorLog"),

                    // メッセージコード
                    nameof(Resources.CO_API_DFC_002),

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject(null, null),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "{0}", Exception = new RmsException("ErrorLog"), LogLevel = LogLevel.Error },
                };

                yield return new object[] {
                    // 出力するException
                    new RmsException("ErrorLog"),

                    // メッセージコード
                    nameof(Resources.CO_API_DFC_002),

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject(string.Empty, string.Empty),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "{0}", Exception = new RmsException("ErrorLog"), LogLevel = LogLevel.Error },
                };

                yield return new object[] {
                    // 出力するException
                    new RmsException("ErrorLog"),

                    // メッセージコード
                    nameof(Resources.CO_API_DFC_002),

                    // 出力するログに渡す引数オブジェクト
                    null,

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "{0}", Exception = new RmsException("ErrorLog"), LogLevel = LogLevel.Error },
                };
            }

            // 引数なしのテスト
            {
                yield return new object[] {
                    // 出力するException
                    new RmsException("ErrorLog"),

                    // メッセージコード
                    nameof(Resources.CO_API_DFC_001),

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject("value1", "value2"),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = nameof(Resources.CO_API_DFC_001), Exception = new RmsException("ErrorLog"), LogLevel = LogLevel.Error },
                };

                yield return new object[] {
                    // 出力するException
                    new RmsException("ErrorLog"),

                    // メッセージコード
                    nameof(Resources.CO_API_DFC_001),

                    // 出力するログに渡す引数オブジェクト
                    null,

                    // 期待するログ
                    new ExpectedLog { MessageFormat = nameof(Resources.CO_API_DFC_001), Exception = new RmsException("ErrorLog"), LogLevel = LogLevel.Error },
                };
            }
        }

        #endregion

        #region Enter

        /// <summary>
        /// Enterテスト（引数なし）
        /// </summary>
        /// <param name="targetMessage">出力するログメッセージ</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(EnterDynamicDataWithoutArguments), DynamicDataSourceType.Method)]
        public void EnterTestWithoutArguments(string targetMessage, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Enter(logger, targetMessage);

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, null);
        }

        /// <summary>
        /// Enterテスト（引数あり）
        /// </summary>
        /// <param name="targetMessage">出力するログメッセージ</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(EnterDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void EnterTestWithArguments(string targetMessage, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Enter(
                    logger, targetMessage,
                    new object[] { LoggerExtensionsTestObject.GetJsonString(targetArgs) }
                );

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// EnterJsonテスト
        /// </summary>
        /// <param name="targetMessage">出力するログメッセージ</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(EnterDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void EnterJsonTest(string targetMessage, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.EnterJson(logger, targetMessage, targetArgs);

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// 引数なしEnterLog用テストデータ
        /// </summary>
        /// <returns>テストデータリスト</returns>
        public static IEnumerable<object[]> EnterDynamicDataWithoutArguments()
        {
            yield return new object[] {
                // 出力するログメッセージ
                "TestMessage", 

                // 期待するログ
                new ExpectedLog { MessageFormat = "(START) TestMessage", Exception = null, LogLevel = LogLevel.Debug },
            };
        }

        /// <summary>
        /// 引数ありEnterLog用テストデータ
        /// </summary>
        /// <returns>テストデータリスト</returns>
        public static IEnumerable<object[]> EnterDynamicDataWithArguments()
        {
            // 引数ありのテスト
            {
                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject("value1", "value2"),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "(START) TestMessage: {0}", Exception = null, LogLevel = LogLevel.Debug },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject(null, null),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "(START) TestMessage: {0}", Exception = null, LogLevel = LogLevel.Debug },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject(string.Empty, string.Empty),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "(START) TestMessage: {0}", Exception = null, LogLevel = LogLevel.Debug },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    null,

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "(START) TestMessage: {0}", Exception = null, LogLevel = LogLevel.Debug },
                };
            }

            // 引数なしのテスト
            {
                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject("value1", "value2"),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "(START) TestMessage", Exception = null, LogLevel = LogLevel.Debug },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage", 

                    // 出力するログに渡す引数オブジェクト
                    null,

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "(START) TestMessage", Exception = null, LogLevel = LogLevel.Debug },
                };
            }
        }

        #endregion

        #region Leave 

        /// <summary>
        /// Leaveテスト（引数なし）
        /// </summary>
        /// <param name="targetMessage">出力するログメッセージ</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(LeaveDynamicDataWithoutArguments), DynamicDataSourceType.Method)]
        public void LeaveTestWithoutArguments(string targetMessage, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Leave(logger, targetMessage);

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, null);
        }

        /// <summary>
        /// Leaveテスト（引数あり）
        /// </summary>
        /// <param name="targetMessage">出力するログメッセージ</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(LeaveDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void LeaveTestWithArguments(string targetMessage, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.Leave(
                    logger, targetMessage,
                    new object[] { LoggerExtensionsTestObject.GetJsonString(targetArgs) }
                );

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// LeaveJsonテスト
        /// </summary>
        /// <param name="targetMessage">出力するログメッセージ</param>
        /// <param name="targetArgs">出力するログメッセージに渡す引数オブジェクト</param>
        /// <param name="expectedLog">期待するログ</param>
        [DataTestMethod]
        [DynamicData(nameof(LeaveDynamicDataWithArguments), DynamicDataSourceType.Method)]
        public void LeaveJsonTest(string targetMessage, LoggerExtensionsTestObject targetArgs, ExpectedLog expectedLog)
        {
            List<TestLog> logs = new List<TestLog>();
            TestLogger<string> logger = new TestLogger<string>(logs);

            // ログ出力
            Rms.Server.Core.Utility.Extensions.LoggerExtensions.LeaveJson(logger, targetMessage, targetArgs);

            // ログを確認する
            CheckEqualityLogLevelsAndMessages(logs, expectedLog, targetArgs);
        }

        /// <summary>
        /// 引数なしLeaveLog用テストデータ
        /// </summary>
        /// <returns>テストデータリスト</returns>
        public static IEnumerable<object[]> LeaveDynamicDataWithoutArguments()
        {
            yield return new object[] {
                // 出力するログメッセージ
                "TestMessage", 

                // 期待するログ
                new ExpectedLog { MessageFormat = "(END) TestMessage", Exception = null, LogLevel = LogLevel.Debug },
            };
        }

        /// <summary>
        /// 引数ありLeaveLog用テストデータ
        /// </summary>
        /// <returns>テストデータリスト</returns>
        public static IEnumerable<object[]> LeaveDynamicDataWithArguments()
        {
            // 引数ありのテスト
            {
                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject("value1", "value2"),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "(END) TestMessage: {0}", Exception = null, LogLevel = LogLevel.Debug },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject(null, null),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "(END) TestMessage: {0}", Exception = null, LogLevel = LogLevel.Debug },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject(string.Empty, string.Empty),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "(END) TestMessage: {0}", Exception = null, LogLevel = LogLevel.Debug },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage: {0}", 

                    // 出力するログに渡す引数オブジェクト
                    null,

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "(END) TestMessage: {0}", Exception = null, LogLevel = LogLevel.Debug },
                };
            }

            // 引数なしのテスト
            {
                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage", 

                    // 出力するログに渡す引数オブジェクト
                    new LoggerExtensionsTestObject("value1", "value2"),

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "(END) TestMessage", Exception = null, LogLevel = LogLevel.Debug },
                };

                yield return new object[] {
                    // 出力するログメッセージ
                    "TestMessage", 

                    // 出力するログに渡す引数オブジェクト
                    null,

                    // 期待するログ
                    new ExpectedLog { MessageFormat = "(END) TestMessage", Exception = null, LogLevel = LogLevel.Debug },
                };
            }
        }

        #endregion 

        #region 共通テストメソッド

        /// <summary>
        /// 出力したログをチェックする
        /// </summary>
        /// <param name="actualLogs">出力したログのリスト</param>
        /// <param name="expectedLog">テスト結果として期待するログ</param>
        /// <param name="expectedArgs">テスト結果として期待するログに渡す引数</param>
        private void CheckEqualityLogLevelsAndMessages(List<TestLog> actualLogs, ExpectedLog expectedLog, LoggerExtensionsTestObject expectedArgs)
        {
            // ログは1つだけ出力
            Assert.AreEqual(1, actualLogs.Count);

            // ログオブジェクトがnullでない
            Assert.IsNotNull(actualLogs[0]);

            // ログレベルを確認
            Assert.AreEqual(expectedLog.LogLevel, actualLogs[0].LogLevel);

            // ログオブジェクトのメッセージが期待した値になっていることを確認
            Assert.AreEqual(true, actualLogs[0].GetSimpleText().Contains(ExpectedLog.GetMessageWithArguments(expectedLog, expectedArgs)));
        }

        /// <summary>
        /// 出力した例外付きログをチェックする
        /// </summary>
        /// <param name="actualLogs">出力したログのリスト</param>
        /// <param name="expectedLog">テスト結果として期待するログ</param>
        /// <param name="expectedArgs">テスト結果として期待するログに渡す引数</param>
        private void CheckEqualityLogLevelsAndExceptionsAndMessages(List<TestLog> actualLogs, ExpectedLog expectedLog, LoggerExtensionsTestObject expectedArgs)
        {
            // ログレベルとメッセージの確認
            CheckEqualityLogLevelsAndMessages(actualLogs, expectedLog, expectedArgs);

            // Exceptionの確認
            Exception exception = actualLogs[0].Exception;
            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedLog.Exception.GetType(), exception.GetType());
        }

        #endregion

        #region Test用クラス

        /// <summary>
        /// テスト結果として期待するログ
        /// </summary>
        public class ExpectedLog
        {
            /// <summary>
            /// Exception
            /// </summary>
            public Exception Exception { get; set; }

            /// <summary>
            /// LogLevel
            /// </summary>
            public LogLevel LogLevel { get; set; }

            /// <summary>
            /// ログメッセージフォーマット
            /// </summary>
            /// <remarks>
            /// フォーマットに埋め込むプレースホルダーは0または1個を前提とする
            /// </remarks>
            public string MessageFormat { get; set; }

            /// <summary>
            /// 指定したメッセージフォーマットに引数を渡してログメッセージを生成する
            /// </summary>
            /// <param name="jsonObject">引数に渡すJSONオブジェクト</param>
            /// <returns>引数つきのログメッセージ</returns>
            /// <remarks>
            /// メッセージフォーマットにプレースホルダーが無い場合には引数として渡したJSONオブジェクトは無視される。
            /// 引数に渡すオブジェクトの数とプレースホルダーの数が一致しないとFormatExceptionが発生するので注意
            /// </remarks>
            public static string GetMessageWithArguments(ExpectedLog expectedLog, LoggerExtensionsTestObject jsonObject)
            {
                return string.Format(expectedLog.MessageFormat, LoggerExtensionsTestObject.GetJsonString(jsonObject));
            }
        }

        /// <summary>
        /// ログ出力テスト用クラス
        /// </summary>
        public class LoggerExtensionsTestObject
        {
            /// <summary>
            /// 値1
            /// </summary>
            [Required]
            [JsonProperty(nameof(Value1))]
            public string Value1;

            /// <summary>
            /// 値2
            /// </summary>
            [Required]
            [JsonProperty(nameof(Value2))]
            public string Value2;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="value1">値1</param>
            /// <param name="value2">値2</param>
            public LoggerExtensionsTestObject(string value1, string value2)
            {
                Value1 = value1;
                Value2 = value2;
            }

            /// <summary>
            /// 本クラスをシリアライズした文字列を取得する
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static string GetJsonString(LoggerExtensionsTestObject obj)
            {
                return JsonConvert.SerializeObject(obj);
            }
        }

        #endregion
    }
}
