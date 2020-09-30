using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Rms.Server.Utility.Utility
{
    /// <summary>
    /// ログ解析DLLラッパークラス
    /// </summary>
    public class LogAnalysisDllWrapper
    {
        /// <summary>
        /// 解析データ最大数
        /// </summary>
        public const int AnalysisDataMax = 1000;

        /// <summary>
        /// 面積算出データ個数(既存モジュールのALMI_AREA_DATA_NUM)
        /// 流用元の基本設設計書に「使用するデータ個数は画像サイズによらず、109個とする」と記載がある。
        /// ※DIP性能監視機能システム基本設計書(ムラ監視改善)_20161115.xlsの5.アルミスロープログ判定処理シート
        /// </summary>
        public static readonly int AlmiAreaDataNum = 109;

        /// <summary>
        /// GP値最小値
        /// </summary>
        public static readonly int GpMinValue = 100;

        /// <summary>
        /// GP値最大値
        /// </summary>
        public static readonly int GpMaxValue = 800;

        /// <summary>
        /// アルミスロープログ解析データ
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct AlmiLogAnalysisData
        {
            /// <summary>
            /// 輝度値
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = AnalysisDataMax)]
            public int[] LuminanceValue;

            /// <summary>
            /// 輝度値データ数
            /// </summary>
            public int LuminanceValueCount;

            /// <summary>
            /// 傾き基準値下限値
            /// </summary>
            public double MinSlopeValue;

            /// <summary>
            /// 傾き基準値中間値
            /// </summary>
            public double MiddleSlopeValue;

            /// <summary>
            /// 傾き基準値上限値
            /// </summary>
            public double MaxSlopeValue;

            /// <summary>
            /// 面積基準値(低管電圧側)
            /// </summary>
            public int LowVoltageAreaValue;

            /// <summary>
            /// 面積基準値(高管電圧側)
            /// </summary>
            public int HighVoltageAreaValue;

            /// <summary>
            /// 面積算出用データ
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = AnalysisDataMax)]
            public int[] AreaStandardData;

            /// <summary>
            /// 面積算出データ個数
            /// </summary>
            public int AlmiAreaDataNum;

            /// <summary>
            /// ALS逆向き判定処理 判定基準値
            /// </summary>
            public double AlsStandardValue;
        }

        /// <summary>
        /// アルミスロープログ解析結果
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct AlmiLogAnalysisResult
        {
            /// <summary>
            /// 判定結果
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string JudgeResult;

            /// <summary>
            /// 傾き算出値
            /// </summary>
            public double CulSlopeValue;

            /// <summary>
            /// 面積算出値
            /// </summary>
            public int CulAreaValue;

            /// <summary>
            /// 面積基準値
            /// </summary>
            public int CulStdValue;

            /// <summary>
            /// ALS逆向き判定処理 判定結果
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string SlopeReverseResult;

            /// <summary>
            /// ALS逆向き判定処理 傾き算出値
            /// </summary>
            public double Inclination;

            /// <summary>
            /// 解析時に発生したエラーの詳細
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string ErrorMsg;
        }

        /// <summary>
        /// ムラログ解析データ
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct BlocLogAnalysisData
        {
            /// <summary>
            /// プロファイル値
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = AnalysisDataMax)]
            public double[] ProfileValue;

            /// <summary>
            /// プロファイル値データ数
            /// </summary>
            public int ProfileValueCount;

            /// <summary>
            /// GP値
            /// </summary>
            public int GpValue;

            /// <summary>
            /// 先頭スキップ行数
            /// </summary>
            public int TopUnevennessSkipValue;

            /// <summary>
            /// 後尾スキップ行数
            /// </summary>
            public int BottomUnevennessSkipValue;

            /// <summary>
            /// MCV判定基準値
            /// </summary>
            public double McvStandardValue;

            /// <summary>
            /// SCV判定基準値1(OK・NG間)
            /// </summary>
            public double ScvStandardValue1;

            /// <summary>
            /// SCV判定基準値2(NG・回避間)
            /// </summary>
            public double ScvStandardValue2;
        }

        /// <summary>
        /// ムラログ解析結果
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct BlocLogAnalysisResult
        {
            /// <summary>
            /// 簡易ムラ判定処理 判定結果
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string UnevenResult;

            /// <summary>
            /// 簡易ムラ判定処理 MCV算出値
            /// </summary>
            public double Mcv;

            /// <summary>
            /// 簡易ムラ判定処理 Scv算出値
            /// </summary>
            public double Scv;

            /// <summary>
            /// 判定対象画像種別
            /// </summary>
            public sbyte ImageClassification;

            /// <summary>
            /// 画素サイズ
            /// </summary>
            public int PixelSize;

            /// <summary>
            /// 解析時に発生したエラーの詳細
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string ErrorMsg;
        }

        /// <summary>
        /// DllImport実行用クラス
        /// </summary>
        public static class NativeMethods
        {
            /// <summary>
            /// ログ解析DLL名
            /// </summary>
            //// [TODO]:DLLのパスは実環境の配置に合わせて修正する
            private const string dllName = @"..\..\..\..\..\lib\LogAnalysisDll.dll";

            /// <summary>
            /// アルミスロープログ解析実行
            /// </summary>
            /// <param name="analysisData">解析対象データ</param>
            /// <param name="analysisResult">解析結果</param>
            /// <returns>0:正常 -1:異常</returns>
            [DllImport(dllName)]
            public static extern int AnalyzeAlmiLog(ref AlmiLogAnalysisData analysisData, ref AlmiLogAnalysisResult analysisResult);

            /// <summary>
            /// ムラログ解析実行
            /// </summary>
            /// <param name="analysisData">解析対象データ</param>
            /// <param name="analysisResult">解析結果</param>
            /// <returns>0:正常 -1:異常</returns>
            [DllImport(dllName)]
            public static extern int AnalyzeBlocLog(ref BlocLogAnalysisData analysisData, ref BlocLogAnalysisResult analysisResult);
        }
    }
}
