#pragma once

#define ERROR_MSG_LEN (512)

// アルミスロープログ解析データ
#define LUMINANCE_VALUE_LEN (1000)
#define AREA_STANDARD_DATA_LEN LUMINANCE_VALUE_LEN

typedef struct AlmiLogAnalysisData
{
    int LuminanceValue[LUMINANCE_VALUE_LEN];        // 輝度値
    int LuminanceValueCount;                        // 輝度値データ数
    double MinSlopeValue;                           // 傾き基準値下限値
    double MiddleSlopeValue;                        // 傾き基準値中間値
    double MaxSlopeValue;                           // 傾き基準値上限値
    int LowVoltageAreaValue;                        // 面積基準値(低管電圧側)
    int HighVoltageAreaValue;                       // 面積基準値(高管電圧側)
    int AreaStandardData[AREA_STANDARD_DATA_LEN];   // 面積算出用データ
    int AreaDataNum;                                // 面積算出データ個数
    double AlsStandardValue;                        // ALS逆向き判定処理 判定基準値
} AlmiLogAnalysisData;

// 骨塩アルミスロープログ解析結果
#define JUDGE_RESULT_LEN (8)
#define SLOPE_REVERSE_RESULT_LEN (8)

typedef struct AlmiLogAnalysisResult
{
    char JudgeResult[JUDGE_RESULT_LEN];                 // 判定結果
    double CulSlopeValue;                               // 傾き算出値
    int CulAreaValue;                                   // 面積算出値
    int CulStdValue;                                    // 面積基準値
    char SlopeReverseResult[SLOPE_REVERSE_RESULT_LEN];  // ALS逆向き判定処理 判定結果
    double Inclination;                                 // ALS逆向き判定処理 傾き算出値
    char ErrorMsg[ERROR_MSG_LEN];                       // 解析時に発生したエラーの詳細
} AlmiLogAnalysisResult;

// ムラログ解析データ
#define PROFILE_VALUE_LEN (1000)

typedef struct BlocLogAnalysisData
{
    double ProfileValue[PROFILE_VALUE_LEN]; // プロファイル値
    int ProfileValueCount;                  // プロファイル値データ数
    int GpValue;                            // GP値
    int TopUnevennessSkipValue;             // 先頭スキップ行数
    int BottomUnevennessSkipValue;          // 後尾スキップ行数
    double McvStandardValue;                // MCV判定基準値
    double ScvStandardValue1;               // SCV判定基準値1(OK・NG間)
    double ScvStandardValue2;               // SCV判定基準値2(NG・回避間)
} BlocLogAnalysisData;

// ムラログ解析結果
#define UNEVEN_RESULT_LEN (8)

typedef struct BlocLogAnalysisResult
{
    char UnevenResult[UNEVEN_RESULT_LEN];   // 簡易ムラ判定処理 判定結果
    double Mcv;                             // 簡易ムラ判定処理 MCV算出値
    double Scv;                             // 簡易ムラ判定処理 SCV算出値
    char ImageClassification;               // 判定対象画像種別
    int PixelSize;                          // 画素サイズ
    char ErrorMsg[ERROR_MSG_LEN];           // 解析時に発生したエラーの詳細
} BlocLogAnalysisResult;

// 外部公開関数
extern "C" {
    __declspec(dllexport) int __stdcall AnalyzeAlmiLog(AlmiLogAnalysisData* analysisData, AlmiLogAnalysisResult* analysisResult);
    __declspec(dllexport) int __stdcall AnalyzeBlocLog(BlocLogAnalysisData* analysisData, BlocLogAnalysisResult* analysisResult);
}