using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rms.Server.Core.Azure.Utility.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityTest.Validations
{
    [TestClass()]
    public class RequestValidatorTest
    {
        [DataTestMethod]
        [DynamicData(nameof(GetDynamicDataTestAllNullData), DynamicDataSourceType.Method)]
        public void IsBadRequestParameterIsNullAllInstancePropertyTest(string testTarget, bool expected, object target)
        {
            bool result = RequestValidator.IsBadRequestParameter(target, out string message);
            Assert.AreEqual(expected, result, testTarget);
        }

        public static IEnumerable<object[]> GetDynamicDataTestAllNullData()
        {
            // GoodRequest
            yield return new object[] { "【false】継承したプロパティの値が一つでもnull以外", false,
                new InheritPropertiesClass()
                {
                    ReqParam = 0,
                    NotReqParam = null
                }
            };
            yield return new object[] { "【False】継承したプロパティの値と追加したプロパティの値が一つでもnull以外", false,
                new InheritAddPropertiesClass()
                {
                    ReqParam = 0,
                    NotReqParam = null,
                    AddProperties = null
                }
            };
            // badRequest
            yield return new object[] { "【true】継承したプロパティの値がnull", true,
                new InheritPropertiesClass()
                {
                    ReqParam = null,
                    NotReqParam = null
                }
            };
            yield return new object[] { "【true】継承したプロパティの値と追加したプロパティの値がnull", true,
                new InheritAddPropertiesClass()
                {
                    ReqParam = null,
                    NotReqParam = null,
                    AddProperties = null
                }
            };
        }

        public class InheritPropertiesClass : IsNullAllInstancePropertiesClass { }
        public class InheritAddPropertiesClass : IsNullAllInstancePropertiesClass
        {
            public string AddProperties { get; set; }
        }
        public class IsNullAllInstancePropertiesClass
        {
            [Required]
            public virtual int? ReqParam { get; set; }
            public virtual int? NotReqParam { get; set; }
        }

        /// <summary>
        /// 引数がnullの場合、BadRequestとすること。
        /// </summary>
        [TestMethod()]
        public void IsBadRequestParameterNullParamTest()
        {
            FlatPropertiesClass param = null;

            bool result = RequestValidator.IsBadRequestParameter(param, out string message);
            Assert.IsTrue(result);
            // messageには何か入っていること
            Assert.IsNotNull(message);
        }

        /// <summary>
        /// 必須でない内部クラスがnullの場合、バリデーションを通ること。
        /// </summary>
        [TestMethod()]
        public void IsBadRequestParameterNullParamTestNestedClass()
        {
            NestedClassWithRequired param = new NestedClassWithRequired
            {
                Inner = null,
                DoNotAllNull = "doNotAllNull"
            };

            bool result = RequestValidator.IsBadRequestParameter(param, out string message);
            Assert.IsFalse(result);
            // messageには何か入っていること
            Assert.IsNotNull(message);
        }

        public class NestedClassWithRequired
        {
            [NestedValidate]
            public FlatPropertiesClass Inner { get; set; }

            // IsBadRequestParameter()はすべて初期値の場合trueを返すため、
            // その避けのためにdoNotAllNullを追加したクラスでテストを行う。
            public string DoNotAllNull{ get; set; }
        }

        /// <summary>
        /// IsBadRequestParameterメソッドに正常値を渡す
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        [DataTestMethod]
        [DataRow(0, 0, "1", "123")] // arg3, arg4最低文字数
        [DataRow(0, 0, "1", "1234567890")] // arg4最大文字数
        [DataRow(0, 0, "1", null)]
        [DataRow(0, 0, "1234567890", "123")] // arg3最大文字数
        [DataRow(0, null, "1", "123")]
        [DataRow(0, int.MaxValue, "1", "123")]
        [DataRow(0, int.MinValue, "1", "123")]
        [DataRow(int.MaxValue, 0, "1", "123")]
        [DataRow(int.MinValue, 0, "1", "123")]
        public void IsBadRequestParameterValidParam(int? arg1, int? arg2, string arg3, string arg4)
        {
            FlatPropertiesClass param = new FlatPropertiesClass()
            {
                ReqParam = arg1,
                NotReqParam = arg2,
                Max10LenReqParam = arg3,
                Max10Min3LenParam = arg4
            };

            bool result = RequestValidator.IsBadRequestParameter(param, out string message);
            // BadRequestなし
            Assert.IsFalse(result);
            // messageには何か入っていること
            Assert.IsNotNull(message);
        }

        /// <summary>
        /// IsBadRequestParameterメソッドに異常値を渡す
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        [DataTestMethod]
        [DataRow(null, 0, "1", "123")]
        [DataRow(0, 0, null, "123")]
        [DataRow(0, 0, "1", "12345678901")] // arg4最大文字数 + 1
        [DataRow(0, 0, "12345678901", "123")] // arg3最大文字数 + 1
        [DataRow(0, 0, "1", "12")] // arg4最小文字数 - 1
        [DataRow(0, 0, "", "123")] // arg3最小文字数 - 1
        [DataRow(null, null, null, null)] // 全てのプロパティがNullの場合
        public void IsBadRequestParameterInvalidParam(int? arg1, int? arg2, string arg3, string arg4)
        {
            FlatPropertiesClass param = new FlatPropertiesClass()
            {
                ReqParam = arg1,
                NotReqParam = arg2,
                Max10LenReqParam = arg3,
                Max10Min3LenParam = arg4
            };

            bool result = RequestValidator.IsBadRequestParameter(param, out string message);
            // BadRequestあり
            Assert.IsTrue(result);
            // messageには何か入っていること
            Assert.IsNotNull(message);
        }

        public class FlatPropertiesClass
        {
            [Required]
            public int? ReqParam { get; set; }

            public int? NotReqParam { get; set; }

            [Required]
            [StringLength(10)]
            public string Max10LenReqParam { get; set; }

            [StringLength(10, MinimumLength = 3)]
            public string Max10Min3LenParam { get; set; }
        }

        /// <summary>
        /// IsBadRequestParameterメソッドに正常値を渡す(NestedClass)
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        [DataTestMethod]
        [DataRow(0, 0, "1", "123")] // arg3, arg4最低文字数
        [DataRow(0, 0, "1", "1234567890")] // arg4最大文字数
        [DataRow(0, 0, "1", null)]
        [DataRow(0, 0, "1234567890", "123")] // arg3最大文字数
        [DataRow(0, null, "1", "123")]
        [DataRow(0, int.MaxValue, "1", "123")]
        [DataRow(0, int.MinValue, "1", "123")]
        [DataRow(int.MaxValue, 0, "1", "123")]
        [DataRow(int.MinValue, 0, "1", "123")]
        public void IsBadRequestParameterValidParamNestedClass(int? arg1, int? arg2, string arg3, string arg4)
        {
            NestedClass param = new NestedClass()
            {
                Inner = new FlatPropertiesClass()
                {
                    ReqParam = arg1,
                    NotReqParam = arg2,
                    Max10LenReqParam = arg3,
                    Max10Min3LenParam = arg4
                }
            };

            bool result = RequestValidator.IsBadRequestParameter(param, out string message);
            // BadRequestなし
            Assert.IsFalse(result);
            // messageには何か入っていること
            Assert.IsNotNull(message);
        }

        [DataTestMethod]
        [DataRow(null, 0, "1", "123")]
        [DataRow(0, 0, null, "123")]
        [DataRow(0, 0, "1", "12345678901")] // arg4最大文字数 + 1
        [DataRow(0, 0, "12345678901", "123")] // arg3最大文字数 + 1
        [DataRow(0, 0, "1", "12")] // arg4最小文字数 - 1
        [DataRow(0, 0, "", "123")] // arg3最小文字数 - 1
        [DataRow(null, null, null, null)] // 全てのプロパティがNullの場合
        public void IsBadRequestParameterInvalidParamNestedClass(int? arg1, int? arg2, string arg3, string arg4)
        {
            NestedClass param = new NestedClass()
            {
                Inner = new FlatPropertiesClass()
                {
                    ReqParam = arg1,
                    NotReqParam = arg2,
                    Max10LenReqParam = arg3,
                    Max10Min3LenParam = arg4
                }
            };

            bool result = RequestValidator.IsBadRequestParameter(param, out string message);
            // BadRequestあり
            Assert.IsTrue(result);
            // messageには何か入っていること
            Assert.IsNotNull(message);
        }

        public class NestedClass
        {
            [NestedValidate]
            public FlatPropertiesClass Inner { get; set; }
        }

        /// <summary>
        /// IsBadRequestParameterメソッドに正常値を渡す(DeepNestedClass)
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        [DataTestMethod]
        [DataRow(0, 0, "1", "123")] // arg3, arg4最低文字数
        [DataRow(0, 0, "1", "1234567890")] // arg4最大文字数
        [DataRow(0, 0, "1", null)]
        [DataRow(0, 0, "1234567890", "123")] // arg3最大文字数
        [DataRow(0, null, "1", "123")]
        [DataRow(0, int.MaxValue, "1", "123")]
        [DataRow(0, int.MinValue, "1", "123")]
        [DataRow(int.MaxValue, 0, "1", "123")]
        [DataRow(int.MinValue, 0, "1", "123")]
        public void IsBadRequestParameterValidParamDeepNestedClass(int? arg1, int? arg2, string arg3, string arg4)
        {
            DeepNestedClass param = new DeepNestedClass()
            {
                Nested = new NestedClass()
                {
                    Inner = new FlatPropertiesClass()
                    {
                        ReqParam = arg1,
                        NotReqParam = arg2,
                        Max10LenReqParam = arg3,
                        Max10Min3LenParam = arg4
                    }
                }
            };

            bool result = RequestValidator.IsBadRequestParameter(param, out string message);
            // BadRequestなし
            Assert.IsFalse(result);
            // messageには何か入っていること
            Assert.IsNotNull(message);
        }

        [DataTestMethod]
        [DataRow(null, 0, "1", "123")]
        [DataRow(0, 0, null, "123")]
        [DataRow(0, 0, "1", "12345678901")] // arg4最大文字数 + 1
        [DataRow(0, 0, "12345678901", "123")] // arg3最大文字数 + 1
        [DataRow(0, 0, "1", "12")] // arg4最小文字数 - 1
        [DataRow(0, 0, "", "123")] // arg3最小文字数 - 1
        [DataRow(null, null, null, null)] // 全てのプロパティがNullの場合
        public void IsBadRequestParameterInvalidParamDeepNestedClass(int? arg1, int? arg2, string arg3, string arg4)
        {
            DeepNestedClass param = new DeepNestedClass() {
                Nested = new NestedClass()
                {
                    Inner = new FlatPropertiesClass()
                    {
                        ReqParam = arg1,
                        NotReqParam = arg2,
                        Max10LenReqParam = arg3,
                        Max10Min3LenParam = arg4
                    }
                }
            };

            bool result = RequestValidator.IsBadRequestParameter(param, out string message);
            // BadRequestあり
            Assert.IsTrue(result);
            // messageには何か入っていること
            Assert.IsNotNull(message);
        }

        public class DeepNestedClass
        {
            [NestedValidate]
            public NestedClass Nested { get; set; }
        }

        /// <summary>
        /// IsBadRequestParameterメソッドに正常値を渡す(NestedClass)
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        [DataTestMethod]
        [DataRow(0, 0, "1", "123")] // arg3, arg4最低文字数
        [DataRow(0, 0, "1", "1234567890")] // arg4最大文字数
        [DataRow(0, 0, "1", null)]
        [DataRow(0, 0, "1234567890", "123")] // arg3最大文字数
        [DataRow(0, null, "1", "123")]
        [DataRow(0, int.MaxValue, "1", "123")]
        [DataRow(0, int.MinValue, "1", "123")]
        [DataRow(int.MaxValue, 0, "1", "123")]
        [DataRow(int.MinValue, 0, "1", "123")]
        public void IsBadRequestParameterValidParamInnerList(int? arg1, int? arg2, string arg3, string arg4)
        {
            InnerListClass param = new InnerListClass()
            {
                InnerClasses = new List<FlatPropertiesClass>() {
                    new FlatPropertiesClass()
                    {
                        ReqParam = 100,
                        NotReqParam = 200,
                        Max10LenReqParam = "123",
                        Max10Min3LenParam = "500"
                    },
                    new FlatPropertiesClass()
                    {
                        ReqParam = arg1,
                        NotReqParam = arg2,
                        Max10LenReqParam = arg3,
                        Max10Min3LenParam = arg4
                    },
                }
            };

            bool result = RequestValidator.IsBadRequestParameter(param, out string message);
            // BadRequestなし
            Assert.IsFalse(result);
            // messageには何か入っていること
            Assert.IsNotNull(message);
        }

        [DataTestMethod]
        [DataRow(null, 0, "1", "123")]
        [DataRow(0, 0, null, "123")]
        [DataRow(0, 0, "1", "12345678901")] // arg4最大文字数 + 1
        [DataRow(0, 0, "12345678901", "123")] // arg3最大文字数 + 1
        [DataRow(0, 0, "1", "12")] // arg4最小文字数 - 1
        [DataRow(0, 0, "", "123")] // arg3最小文字数 - 1
        [DataRow(null, null, null, null)] // 全てのプロパティがNullの場合
        public void IsBadRequestParameterInvalidParamInnerList(int? arg1, int? arg2, string arg3, string arg4)
        {
            InnerListClass param = new InnerListClass()
            {
                InnerClasses = new List<FlatPropertiesClass>() {
                    new FlatPropertiesClass()
                    {
                        ReqParam = 100,
                        NotReqParam = 200,
                        Max10LenReqParam = "123",
                        Max10Min3LenParam = "500"
                    },
                    new FlatPropertiesClass()
                    {
                        ReqParam = arg1,
                        NotReqParam = arg2,
                        Max10LenReqParam = arg3,
                        Max10Min3LenParam = arg4
                    },
                }
            };

            bool result = RequestValidator.IsBadRequestParameter(param, out string message);
            // BadRequestあり
            Assert.IsTrue(result);
            // messageには何か入っていること
            Assert.IsNotNull(message);
        }

        public class InnerListClass
        {
            [NestedValidate]
            public IEnumerable<FlatPropertiesClass> InnerClasses { get; set; }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetDynamicTestData), DynamicDataSourceType.Method)]
        public void IsBadRequestParameterInvalidParamInnerEmptyList(string testTarget, bool expected, RequiredInnerListClass target)
        {
            bool result = RequestValidator.IsBadRequestParameter(target, out string message);
            Assert.AreEqual(expected, result, testTarget);
        }

        public static IEnumerable<object[]> GetDynamicTestData()
        {
            // GoodRequest
            yield return new object[] { "【Flase】InnerListのプロパティの配列が要素1", false,
                new RequiredInnerListClass()
                {
                    InnerList = new InnerListOneOrMore[1]{ new InnerListOneOrMore() {RequiredOneOrMore = new int[] { 1 } } }
                }
            };
            yield return new object[] { "【Flase】InnerListのプロパティの配列が要素2", false,
                new RequiredInnerListClass()
                {
                    InnerList = new InnerListOneOrMore[1]{ new InnerListOneOrMore() {RequiredOneOrMore = new int[] { 1, 2 } } }
                }
            };
            // BadRequest
            yield return new object[] { "【True】InnerListがnull", true,
                new RequiredInnerListClass()
                {
                    InnerList = null
                }
            };
            yield return new object[] { "【True】InnerListが空配列", true,
                new RequiredInnerListClass()
                {
                    InnerList = new InnerListOneOrMore[0]
                }
            };
            yield return new object[] { "【True】InnerListのプロパティがnull", true,
                new RequiredInnerListClass()
                {
                    InnerList = new InnerListOneOrMore[1]{ new InnerListOneOrMore() }
                }
            };
            yield return new object[] { "【True】InnerListのプロパティが空配列", true,
                new RequiredInnerListClass()
                {
                    InnerList = new InnerListOneOrMore[1]{ new InnerListOneOrMore() {RequiredOneOrMore = new int[0] } }
                }
            };
        }

        public class RequiredInnerListClass
        {
            [RequiredAtLeastOneElement]
            [NestedValidate]
            public InnerListOneOrMore[] InnerList { get; set; }
        }

        public class InnerListOneOrMore
        {
            [RequiredAtLeastOneElement]
            public int[] RequiredOneOrMore { get; set; }
        }

        /// <summary>
        /// 「ASCII文字のみ許可」の制約をテストする
        /// "^[\x20-\x7e]+$" の正規表現にマッチすればOK
        /// </summary>
        /// <param name="paramater">投入文字列</param>
        /// <param name="expectedAssert">BadRequestであればtrue / なければfalse</param>
        [DataTestMethod]
        [DataRow("\x19", true)]  // 下限Over
        [DataRow("\x7f", true)]  // 上限Over
        [DataRow(null, false)]   // 正規表現NGだがRegularExpressionでOKとなる
        [DataRow("", false)]     // 正規表現NGだがRegularExpressionでOKとなる
        [DataRow("\x20", false)] // 下限値
        [DataRow("\x4f", false)] // 中間値
        [DataRow("\x7e", false)] // 上限値
        [DataRow("!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~", false)] // 半角英数字記号すべて
        public void AsciiRegularExpressionTest(string paramater, bool expectedAssert)
        {
            ASCII param = new ASCII()
            {
                RequiredAscii = paramater
            };
            bool isBadReq = RequestValidator.IsBadRequestParameter(param, out string message);
            // BadRequestあり
            Assert.AreEqual(expectedAssert, isBadReq);
            // messageには何か入っていること
            Assert.IsNotNull(message);
        }

        /// <summary>
        /// 「ASCII文字のみ許可」の制約のメンバをもつクラス
        /// </summary>
        public class ASCII
        {
            [RegularExpression(Rms.Server.Core.Utility.Const.AsciiCodeCharactersReg)]
            public string RequiredAscii { get; set; }

            // All Null回避用のNotNullパラメータ
            public int notNull { get; set; }
        }
    }
}