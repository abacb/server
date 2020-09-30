// LogAnalysisDll.cpp : DLL アプリケーション用にエクスポートされる関数を定義します。
//

#include "stdafx.h"
#include "LogAnalysisDll.h"

#define __file__ (strrchr(__FILE__, '\\') + 1)

#define OK      0   //リターン値：OK
#define NG      -1  //リターン値：NG
#define EVASION     1   //リターン値：回避

#define ANALYSIS_SLOPE_REVERSE_RESULT_OK    "HP"    //ALS逆向き判定処理 判定結果：OK(傾き＞0)
#define ANALYSIS_SLOPE_REVERSE_RESULT_NG    "HF"    //ALS逆向き判定処理 判定結果：NG(傾き≦0 ALスロープが正しく配置されていない懸念があります)
#define ANALYSIS_SLOPE_REVERSE_RESULT_NONE  "NONE"

#define ANALYSIS_UNEVEN_RESULT_OK1          "PP"    //簡易ムラ判定処理 判定結果：OK OK  <通知なし>
#define ANALYSIS_UNEVEN_RESULT_NG1          "PF"    //簡易ムラ判定処理 判定結果：OK NG  装置ムラが発生している懸念があります。
#define ANALYSIS_UNEVEN_RESULT_OK2          "PN"    //簡易ムラ判定処理 判定結果：OK 回避    <通知なし> 
#define ANALYSIS_UNEVEN_RESULT_NG2          "FP"    //簡易ムラ判定処理 判定結果：NG OK  カセッテに対してX線が斜入している懸念があります。
#define ANALYSIS_UNEVEN_RESULT_NG3          "FF"    //簡易ムラ判定処理 判定結果：NG NG  装置ムラが発生している懸念があります
#define ANALYSIS_UNEVEN_RESULT_NG4          "FN"    //簡易ムラ判定処理 判定結果：NG 回避    カセッテに対してX線が斜入している懸念があります。
#define ANALYSIS_UNEVEN_RESULT_NONE         "NONE"

#define ANALYSIS_RESULT_OK "S" //傾き、面積判定処理成功
#define ANALYSIS_RESULT_SLOPE_NG "FS" //傾き判定処理失敗
#define ANALYSIS_RESULT_AREA_NG "FA" //面積判定処理失敗

#define IMAGE_SIZE_500 (500) //画像サイズ 四切 / 1824 / 2430 / DR各種の場合
#define IMAGE_SIZE_750 (750) //画像サイズ 大角 / 半切 画像サイズ750の場合
#define IMAGE_SIZE_375 (375) //画像サイズ 大角 / 半切 画像サイズ375の場合
#define ALIMI_IMAGE_SIZE_MIN_RANGE (10) //画像サイズ範囲下限(アルミスログ)
#define ALIMI_IMAGE_SIZE_MAX_RANGE (10) //画像サイズ範囲上限(アルミログ)
#define BLOC_IMAGE_SIZE_MIN_RANGE (16) //画像サイズ範囲下限(ムラログ)
#define BLOC_IMAGE_SIZE_MAX_RANGE (16) //画像サイズ範囲上限(ムラログ)
#define DATA_NUM_500 (134) //傾き判定個数 四切 / 1824 / 2430 / DR各種の場合
#define DATA_NUM_750 (201) //傾き判定個数 大角 / 半切 画像サイズ750の場合
#define DATA_NUM_375 (100) //傾き判定個数 大角 / 半切 画像サイズ375の場合
#define SIZE_NO_500 (0) //画像サイズNo 四切 / 1824 / 2430 / DR各種の場合
#define SIZE_NO_750 (1) //画像サイズNo 大角 / 半切 画像サイズ750の場合
#define SIZE_NO_375 (2) //画像サイズNo 大角 / 半切 画像サイズ375の場合
#define DATA_SPACE_NUM_500 (0.3) //データ間隔 四切 / 1824 / 2430 / DR各種の場合
#define DATA_SPACE_NUM_750 (0.2) //データ間隔 大角 / 半切 画像サイズ750の場合
#define DATA_SPACE_NUM_375 (0.4) //データ間隔 大角 / 半切 画像サイズ375の場合
#define SAMPLING_NUM_500 (4) //サンプリング間隔 四切 / 1824 / 2430 / DR各種の場合
#define SAMPLING_NUM_750 (6) //サンプリング間隔 大角 / 半切 画像サイズ750の場合
#define SAMPLING_NUM_375 (3) //サンプリング間隔 大角 / 半切 画像サイズ375の場合

typedef struct AnalysisError
{
    char* cErrorMsg;
    int iErrorMsgLen;
} AnalysisError;

int GetImageSize(int iSize, int* iResultSize, bool isAlmiLog, AnalysisError* analysisError);
int JudgeImageSize(int iSize, bool isAlmiLog);
void AdjustLogValue(int* iDataList, int iSize);
int AnalysisSlope(int* iValue, int iImgSizeNo, double dMaxNum, double dMinNum, double* dData, int* iResult, AnalysisError* analysisError);
int AnalysisArea(int* iDatalist, int* iStandardList, int iDataNum, int iImageSize, double dMiddleValue, double dCulValue, int iLowValue, int iHighValue, int* iValue, int* iJudgeNum, int* iResult, AnalysisError* analysisError);
int AnalysisSlopeReverse(int* iDatalist, int iDataNum, int iSize, double *dInclination, AnalysisError* analysisError);
int AnalysisUnevenness(BlocLogAnalysisData *analysisData, double *mcv, double   *scv, AnalysisError *analysisError);

///////////////////////////////////////////////////////
// AnalyzeAlmiLog
// アルミスロープログ解析処理
// アルミスロープログの解析を行う。
// パラメータ
//   analysisData  解析対象データ
//   analysisResult  解析結果
// 戻り値
//   0 : 正常
//  -1 : 異常
///////////////////////////////////////////////////////
int __stdcall AnalyzeAlmiLog(AlmiLogAnalysisData* analysisData, AlmiLogAnalysisResult* analysisResult)
{
    int iRet = -1;
    int iResultSize = 0;                    //画像サイズ判定結果
    double dCulSlopeValue = 0.0;            //傾き算出値
    int iCulAreaValue = 0;                  //面積算出値
    int iCulStdValue = 0;                   //面積基準値
    char cJudgeResult[3] = { 0 };           //判定結果
    int iAnalysisResult = 0;                //解析結果
    char cSlopeReverseResult[5] = { 0 };    //ALS逆向き判定処理 判定結果
    double  dInclination;                   //ALS逆向き判定処理 傾き算出値
    AnalysisError analysisError = { 0 };    //解析エラー詳細

    // 解析エラー詳細初期化
    analysisError.cErrorMsg = analysisResult->ErrorMsg;
    analysisError.iErrorMsgLen = sizeof(analysisResult->ErrorMsg);

    //判定結果初期化
    strcpy_s(&cSlopeReverseResult[0], sizeof(cSlopeReverseResult), ANALYSIS_SLOPE_REVERSE_RESULT_NONE); //ALS逆向き判定処理 判定結果

    //画像サイズ取得処理を行う。
    iRet = GetImageSize(analysisData->LuminanceValueCount, &iResultSize, true, &analysisError);
    if (iRet == -1)
    {
        return -1;
    }

    //アルミスロープログ値調整処理を行う。
    AdjustLogValue(analysisData->LuminanceValue, analysisData->LuminanceValueCount);

    //傾き判定処理を行う。
    iRet = AnalysisSlope(analysisData->LuminanceValue, iResultSize, analysisData->MaxSlopeValue, analysisData->MinSlopeValue, &dCulSlopeValue, &iAnalysisResult, &analysisError);
    //傾き判定でエラーの場合は、処理を終了する。
    if (iRet == -1)
    {
        return -1;
    }
    //傾き判定処理に成功した場合に面積判定処理を行う。
    if (iAnalysisResult == -1)
    {
        //判定処理結果を設定する。
        strcpy_s(&cJudgeResult[0], sizeof(cJudgeResult), ANALYSIS_RESULT_SLOPE_NG);
    }
    else
    {
        //面積判定処理を行う。
        iRet = AnalysisArea(analysisData->LuminanceValue, analysisData->AreaStandardData, analysisData->AreaDataNum, iResultSize, analysisData->MiddleSlopeValue,
            dCulSlopeValue, analysisData->LowVoltageAreaValue, analysisData->HighVoltageAreaValue, &iCulAreaValue, &iCulStdValue, &iAnalysisResult, &analysisError);
        //面積判定でエラーの場合は、処理を終了する。
        if (iRet == -1)
        {
            return -1;
        }
        if (iAnalysisResult == -1)
        {
            //面積判定処理NGを設定する。
            strcpy_s(cJudgeResult, sizeof(cJudgeResult), ANALYSIS_RESULT_AREA_NG);
        }
        else
        {
            //傾き、面積処理OKを設定する。
            strcpy_s(cJudgeResult, sizeof(cJudgeResult), ANALYSIS_RESULT_OK);
        }
    }

    //傾き判定/面積判定いずれかがNGならALS逆向き処理を行う。
    dInclination = 0.0;
    if (iAnalysisResult == -1)
    {
        iRet = AnalysisSlopeReverse(analysisData->LuminanceValue, analysisData->AreaDataNum, iResultSize, &dInclination, &analysisError);
        if (iRet != OK)
        {
            return -1;
        }
        else
        {
            if (dInclination > analysisData->AlsStandardValue)  //傾き＞0
            {
                strcpy_s(cSlopeReverseResult, sizeof(cSlopeReverseResult), ANALYSIS_SLOPE_REVERSE_RESULT_OK);
            }
            else
            {
                strcpy_s(cSlopeReverseResult, sizeof(cSlopeReverseResult), ANALYSIS_SLOPE_REVERSE_RESULT_NG);
            }
        }
    }

    //解析結果設定
    strcpy_s(analysisResult->JudgeResult, JUDGE_RESULT_LEN, cJudgeResult);
    analysisResult->CulSlopeValue = dCulSlopeValue;
    analysisResult->CulAreaValue = iCulAreaValue;
    analysisResult->CulStdValue = iCulStdValue;
    strcpy_s(analysisResult->SlopeReverseResult, SLOPE_REVERSE_RESULT_LEN, cSlopeReverseResult);
    analysisResult->Inclination = dInclination;

    return 0;
}

///////////////////////////////////////////////////////
// GetImageSize
// 画像サイズ取得処理
// 引数のログについて画像サイズ判定処理を行う。
// パラメータ
//   iSize  画像サイズ
//   iResultSize  画像サイズ判定結果
//   isAlmiLog  対象ログ種別(true:アルミログ false:それ以外)
//   analysisError 解析エラー詳細
//   
// 戻り値
//   0 : 正常
//  -1 : 異常
///////////////////////////////////////////////////////
int GetImageSize(int iSize, int* iResultSize, bool isAlmiLog, AnalysisError* analysisError)
{
    int iRet = -1;              //戻り値
    int iImageSize = -1;        //画像サイズ判定結果

    //画像サイズ判定処理を行う。
    iImageSize = JudgeImageSize(iSize, isAlmiLog);
    //画像サイズが対象外の場合は、エラーを返す。
    if (iImageSize == -1)
    {
        sprintf_s(analysisError->cErrorMsg, analysisError->iErrorMsgLen, "[%s(%d)][%s] ログファイルの画像サイズが正しくありません。(画像サイズ:%d)", __file__, __LINE__, __func__, iSize);
        return -1;
    }

    //画像判定結果を設定する。
    *iResultSize = iImageSize;

    //正常に終了した場合は、0を返す。
    return 0;
}

///////////////////////////////////////////////////////
// JudgeImageSize
// 画像サイズ判定関数
// 引数の画像サイズから、画像サイズ判定処理を行う。
// 存在していない場合は-1を返す。
// パラメータ
//   iSize 画像サイズ
//   isAlmiLog  対象ログ種別(true:アルミログ false:それ以外)
// 戻り値
//   0 : 四切 / 1824 / 2430 / DR各種(画像サイズ500の場合)
//   1 : 大角 / 半切 (画像サイズ750の場合)
//   2 : 大角 / 半切 (画像サイズ375の場合)
//  -1 : 対象外の場合
///////////////////////////////////////////////////////
int JudgeImageSize(int iSize, bool isAlmiLog)
{
    int iImageSizeMinRange = ALIMI_IMAGE_SIZE_MIN_RANGE;
    int iImageSizeMaxRange = ALIMI_IMAGE_SIZE_MAX_RANGE;

    if (!isAlmiLog)
    {
        iImageSizeMinRange = BLOC_IMAGE_SIZE_MIN_RANGE;
        iImageSizeMaxRange = BLOC_IMAGE_SIZE_MAX_RANGE;
    }

    //画像サイズ比較を行う。
    //四切 / 1824 / 2430 / DR各種の場合
    if ((IMAGE_SIZE_500 - iImageSizeMinRange) <= iSize && iSize <= (IMAGE_SIZE_500 + iImageSizeMaxRange))
    {
        return 0;
    }
    //大角 / 半切 画像サイズ750の場合
    else if ((IMAGE_SIZE_750 - iImageSizeMinRange) <= iSize && iSize <= (IMAGE_SIZE_750 + iImageSizeMaxRange))
    {
        return 1;
    }
    //大角 / 半切 画像サイズ375の場合
    else if ((IMAGE_SIZE_375 - iImageSizeMinRange) <= iSize && iSize <= (IMAGE_SIZE_375 + iImageSizeMaxRange))
    {
        return 2;
    }
    else
    {
        //上記以外は、異常なので、エラー
        return -1;
    }
}

///////////////////////////////////////////////////////
// AdjustLogValue
// アルミスロープログ値調整処理
// 解析対象となるアルミスロープログ値を調整する。
// 先頭の値を0に調整し、先頭以降の値も、調整前の先頭値を
// 引くことで調整する。
// パラメータ
//   iDataList 調整ログデータリスト
//   iSize 画像サイズ
// 戻り値
//   なし
///////////////////////////////////////////////////////
void AdjustLogValue(int* iDataList, int iSize)
{
    int iChgNum = 0;        //先頭値(アルミスロープログ調整値)
    int iCount = 0;         //カウンタ

    //アルミスロープログ値を調整する。先頭の値を0に調整し、それ以降の値は先頭の値を引く。
    iChgNum = iDataList[0];

    for (iCount = 0; iCount < iSize; iCount++)
    {
        iDataList[iCount] = iDataList[iCount] - iChgNum;
    }
}

///////////////////////////////////////////////////////
// AnalysisSlope()
// 傾き判定処理を行う。
// パラメータで受け取るログデータリストから、傾き値を
// 算出する。
// パラメータ
//   iValue  算出用データリスト
//   iImgSizeNo  画像サイズNO
//   dMaxValue  傾き基準値(上限値)のデータ
//   dMinValue  傾き基準値(下限値)のデータ
//   dData  傾き算出データ
//   iResult 判定結果  (OK:0 , NG:-1 )
//   analysisError 解析エラー詳細
// 戻り値
//   0 : 正常
//  -1 : 異常
///////////////////////////////////////////////////////
int AnalysisSlope(int* iValue, int iImgSizeNo, double dMaxNum, double dMinNum, double* dData, int* iResult, AnalysisError* analysisError)
{
    int iCount = 0;         //カウンタ
    int iDataNum = 0;       //算出個数
    double dMolecule = 0;   //傾き判定分子算出値
    double dDenominator = 0;//傾き判定分母算出値
    double dNum = 0;        //データ間隔値
    double dSpaceCount = 0; //データ間隔

    //データ個数から、データ間隔を取得する。
    //画像サイズがDR/四切/1824/2430の場合
    if (iImgSizeNo == SIZE_NO_500)
    {
        //データ間隔0.3 、算出データ個数133を設定
        dSpaceCount = DATA_SPACE_NUM_500;
        iDataNum = DATA_NUM_500;
    }
    else if (iImgSizeNo == SIZE_NO_750)
    {
        //データ間隔0.2 、算出データ個数200を設定
        dSpaceCount = DATA_SPACE_NUM_750;
        iDataNum = DATA_NUM_750;
    }
    else if (iImgSizeNo == SIZE_NO_375)
    {
        //データ間隔0.4 、算出データ個数100を設定
        dSpaceCount = DATA_SPACE_NUM_375;
        iDataNum = DATA_NUM_375;
    }
    else
    {
        //上記以外の場合は、エラー
        sprintf_s(analysisError->cErrorMsg, analysisError->iErrorMsgLen, "[%s(%d)][%s] アルミスロープログファイルの画像サイズが正しくありません。(画像サイズNo:%d)", __file__, __LINE__, __func__, iImgSizeNo);
        return -1;
    }

    //傾き解析処理を行う。
    for (iCount = 0; iCount < iDataNum; iCount++)
    {
        dMolecule += (dNum * (double)iValue[iCount]);
        dDenominator += (dNum * dNum);
        //データ間隔の更新を行う。
        dNum += dSpaceCount;
    }
    //傾き値を算出し、設定する。
    *dData = dMolecule / dDenominator;

    //傾き判定を行う。
    if (*dData < dMinNum || *dData> dMaxNum)
    {
        //傾き判定がNGの場合
        *iResult = -1;
    }
    else
    {
        //傾き判定がNGの場合
        *iResult = 0;
    }
    return 0;
}

///////////////////////////////////////////////////////
// AnalysisArea()
// 面積判定処理を行う。
// パラメータのデータリストから面積判定処理を行う。
// パラメータ
//   iDatalist  算出用データリスト
//   iStandardList  面積基準値データリスト
//   iDataNum  面積算出データ個数
//   iImageSize  画像サイズNo
//   dMiddleValue  傾き中間値
//   dCulValue  傾き算出値
//   iLowValue  面積基準値低管電圧側
//   iHighValue  面積基準値高管電圧側
//   iValue  面積算出値
//   iJudgeNum 算出に使用する面積基準値
//   iResult 判定結果  (OK:0 , NG:-1 )
//   analysisError 解析エラー詳細
// 戻り値
//   0 : 正常
//  -1 : 異常
///////////////////////////////////////////////////////
int AnalysisArea(int* iDatalist, int* iStandardList, int iDataNum, int iImageSize, double dMiddleValue,
    double dCulValue, int iLowValue, int iHighValue, int* iValue, int* iJudgeNum, int* iResult, AnalysisError* analysisError)
{
    int iSamplingNum = 0;   //サンプリング間隔
    int iTmpNum = 0;        //一時格納値
    int iCulNum = 0;        //算出値
    int iCount = 0;         //カウンタ
    int iExtractNum = 0;    //データ間隔値

    //画像サイズより、サンプリング間隔が異なるため、サンプリング間隔を決定する。
    if (iImageSize == SIZE_NO_500)
    {
        //サンプリング間隔4を設定する。
        iSamplingNum = SAMPLING_NUM_500;
    }
    else if (iImageSize == SIZE_NO_750)
    {
        //サンプリング間隔6を設定する。
        iSamplingNum = SAMPLING_NUM_750;
    }
    else if (iImageSize == SIZE_NO_375)
    {
        //サンプリング間隔3を設定する。
        iSamplingNum = SAMPLING_NUM_375;
    }
    else
    {
        //上記以外の場合は、エラー
        sprintf_s(analysisError->cErrorMsg, analysisError->iErrorMsgLen, "[%s(%d)][%s] アルミスロープログファイルの画像サイズが正しくありません。(画像サイズ:%d)", __file__, __LINE__, __func__, iImageSize);
        return -1;
    }
    //傾き算出値、傾き中間値より、判定値の面積基準値を決める。
    if (dCulValue <= dMiddleValue)
    {
        //面積基準値に高管電圧側の基準値を設定する。
        *iJudgeNum = iHighValue;
    }
    else
    {
        //面積基準値に低管電圧側の基準値を設定する。
        *iJudgeNum = iLowValue;
    }
    //面積計算を行う。
    for (iCount = 0; iCount < iDataNum; iCount++)
    {
        iTmpNum = (iDatalist[iExtractNum] - iStandardList[iCount]);
        iCulNum += (iTmpNum * iTmpNum);
        //抽出するデータの間隔を更新する。
        iExtractNum += iSamplingNum;
    }
    //基準値範囲内外に関わらず、算出値を設定する。
    *iValue = iCulNum;
    //算出を行った結果から基準値の範囲内か確認を行う。
    if (*iJudgeNum < iCulNum)
    {
        //基準値を超えている場合は、NG。
        *iResult = -1;
    }
    else
    {
        *iResult = 0;
    }

    return 0;
}

///////////////////////////////////////////////////////
// AnalysisSlopeReverse
// アルミスロープ逆向き判定処理
// パラメータ
//   iDatalist  算出用データリスト
//   iDataNum  面積算出データ個数
//   iImageSize 画像サイズ
//   double *Inclination 傾き
//   analysisError 解析エラー詳細
// 戻り値
//   0 : 正常
//  -1 : 異常
///////////////////////////////////////////////////////
int AnalysisSlopeReverse(int* iDatalist, int iDataNum, int iImageSize, double *dInclination, AnalysisError* analysisError)
{
    double  stdDevX;                //標準偏差X
    double  stdDevY;                //標準偏差Y

    double  averageX;               //平均値X
    double  averageY;               //平均値Y
    double  deviationX;             //偏差X
    double  deviationY;             //偏差Y
    double  deviationMul;           //偏差積
    double  deviationX2;            //偏差X^2
    double  deviationY2;            //偏差Y^2
    double  distributionX;          //分散X
    double  distributionY;          //分散Y

    double  Covariance;             //共分散
    double  CorCoe;                 //相関係数

    int     iSamplingNum = 0;       //サンプリング間隔
    int     iExtractNum = 0;        //データ間隔値
    int     i;

    //画像サイズより、サンプリング間隔が異なるため、サンプリング間隔を決定する。
    switch (iImageSize)
    {
    case SIZE_NO_500:   iSamplingNum = SAMPLING_NUM_500;    break;      //サンプリング間隔4を設定する。
    case SIZE_NO_750:   iSamplingNum = SAMPLING_NUM_750;    break;      //サンプリング間隔6を設定する。
    case SIZE_NO_375:   iSamplingNum = SAMPLING_NUM_375;    break;      //サンプリング間隔3を設定する。
    default:
        //上記以外の場合は、エラー
        sprintf_s(analysisError->cErrorMsg, analysisError->iErrorMsgLen, "[%s(%d)][%s] アルミスロープログファイルの画像サイズが正しくありません。(画像サイズNo:%d)", __file__, __LINE__, __func__, iImageSize);
        return NG;
    }

    //平均値算出
    averageY = 0.0;
    averageX = 0.0;
    iExtractNum = 0;
    for (i = 0; i < iDataNum; i++)
    {
        //データ総和
        averageX += iExtractNum + 1.0;          //X
        averageY += iDatalist[iExtractNum];     //Y

        //抽出するデータの間隔を更新する。
        iExtractNum += iSamplingNum;
    }
    //平均値(QL)算出
    averageX = averageX / iDataNum;  // 平均値X
    averageY = averageY / iDataNum;  // 平均値Y

    //偏差
    iExtractNum = 0;
    deviationX = 0.0;           //偏差X
    deviationY = 0.0;           //偏差Y
    deviationMul = 0.0;         //偏差積
    deviationX2 = 0.0;          //偏差X^2
    deviationY2 = 0.0;          //偏差Y^2
    distributionX = 0.0;        //分散X
    distributionY = 0.0;        //分散Y
    for (i = 0; i < iDataNum; i++)
    {
        deviationX = iExtractNum + 1.0 - averageX;      //偏差X (X - 平均値X)
        deviationY = iDatalist[iExtractNum] - averageY; //偏差Y (Y - 平均値Y)

        deviationMul += (deviationX * deviationY);      //偏差積

        deviationX2 = deviationX * deviationX;          //偏差X^2
        deviationY2 = deviationY * deviationY;          //偏差Y^2

        distributionX += deviationX2;           //分散X
        distributionY += deviationY2;           //分散Y

        //抽出するデータの間隔を更新する。
        iExtractNum += iSamplingNum;
    }

    //アルミスロープの長手方向(x)の標準偏差を算出する。(σx)
    stdDevX = sqrt(distributionX);
    //アルミスロープ値(y)の標準偏差を算出する。(σy)
    stdDevY = sqrt(distributionY);

    Covariance = deviationMul / iDataNum;               //共分散(偏差積の平均値)
    CorCoe = Covariance / (stdDevX * stdDevY);          //相関係数(共分散/(標準偏差X*標準偏差Y))
    *dInclination = CorCoe * (stdDevY / stdDevX);       //傾き(相関係数*(標準偏差Y/標準偏差X))

    return OK;
}

///////////////////////////////////////////////////////
// AnalyzeBlocLog
// ムラログ解析処理
// ムラログの解析を行う。
// パラメータ
//   analysisData  解析対象データ
//   analysisResult  解析結果
// 戻り値
//   0 : 正常
//  -1 : 異常
///////////////////////////////////////////////////////
int __stdcall AnalyzeBlocLog(BlocLogAnalysisData* analysisData, BlocLogAnalysisResult* analysisResult)
{
    int     iRet = -1;
    int     iResultSize = 0;                //画像サイズ判定結果
    int     mcvResult = OK;                 //MCV値判定結果
    int     scvResult = OK;                 //SCV値判定結果
    char    cUnevenResult[5] = { 0 };       //簡易ムラ判定処理 判定結果
    double  dMcv = 0.0;                     //簡易ムラ判定処理 MCV算出値
    double  dScv = 0.0;                     //簡易ムラ判定処理 SCV算出値
    char    cImageClassification;           //判定対象画像種別
    int     iPixelSize;                     //画素サイズ
    AnalysisError analysisError = { 0 };    //解析エラー詳細

    // 解析エラー詳細初期化
    analysisError.cErrorMsg = analysisResult->ErrorMsg;
    analysisError.iErrorMsgLen = sizeof(analysisResult->ErrorMsg);

    //判定結果初期化
    strcpy_s(&cUnevenResult[0], sizeof(cUnevenResult), ANALYSIS_UNEVEN_RESULT_NONE);

    //画像サイズ取得処理を行う。
    iRet = GetImageSize(analysisData->ProfileValueCount, &iResultSize, false, &analysisError);
    if (iRet == -1)
    {
        return -1;
    }

    // 簡易ムラ判定処理
    iRet = AnalysisUnevenness(analysisData, &dMcv, &dScv, &analysisError);

    if (iRet == OK)
    {
        //MCV判定
        if (dMcv > analysisData->McvStandardValue)   //MCV > 基準値
        {
            mcvResult = NG;
        }
        else
        {
            mcvResult = OK;
        }

        if (dScv > analysisData->ScvStandardValue2)  // SCV > 基準値2
        {
            scvResult = EVASION;        //回避
        }
        else
        {
            if (dScv > analysisData->ScvStandardValue1)  // SCV > 基準値1
            {
                scvResult = NG;
            }
            else
            {
                scvResult = OK;
            }
        }

        if (mcvResult == OK)
        {
            if (scvResult == OK) {
                strcpy_s(cUnevenResult, sizeof(cUnevenResult), ANALYSIS_UNEVEN_RESULT_OK1);      //<通知なし>
            }
            if (scvResult == NG) {
                strcpy_s(cUnevenResult, sizeof(cUnevenResult), ANALYSIS_UNEVEN_RESULT_NG1);      //装置ムラが発生している懸念があります。
            }
            if (scvResult == EVASION) {
                strcpy_s(cUnevenResult, sizeof(cUnevenResult), ANALYSIS_UNEVEN_RESULT_OK2);      //<通知なし>
            }
        }
        else
        {
            if (scvResult == OK) {
                strcpy_s(cUnevenResult, sizeof(cUnevenResult), ANALYSIS_UNEVEN_RESULT_NG2);      //カセッテに対してX線が斜入している懸念があります。
            }
            if (scvResult == NG) {
                strcpy_s(cUnevenResult, sizeof(cUnevenResult), ANALYSIS_UNEVEN_RESULT_NG3);      //装置ムラが発生している懸念があります
            }
            if (scvResult == EVASION) {
                strcpy_s(cUnevenResult, sizeof(cUnevenResult), ANALYSIS_UNEVEN_RESULT_NG4);      //カセッテに対してX線が斜入している懸念があります。
            }
        }
    }

    //画素サイズ決定
    switch (iResultSize)
    {
    case 0:iPixelSize = 300;    break;
    case 1:iPixelSize = 200;    break;
    case 2:iPixelSize = 400;    break;
    default:iPixelSize = -1;    break;
    }

    //判定対象画像種別の設定
    if (analysisData->GpValue == 0)
    {
        cImageClassification = 0;
    }
    else {
        cImageClassification = 1;
    }

    strcpy_s(analysisResult->UnevenResult, sizeof(analysisResult->UnevenResult), cUnevenResult);
    analysisResult->Mcv = dMcv;
    analysisResult->Scv = dScv;
    analysisResult->ImageClassification = cImageClassification;
    analysisResult->PixelSize = iPixelSize;

    return 0;
}

///////////////////////////////////////////////////////
// AnalysisUnevenness
// 簡易ムラ判定処理
// パラメータ
// BlocLogAnalysisData  *analysisData   解析対象データ
// double   *mcv                MCV値
// double   *scv                SCV値
// AnalysisError    *analysisError  解析エラー詳細
// 戻り値
//   0 : 正常
//  -1 : 異常
///////////////////////////////////////////////////////
int AnalysisUnevenness(BlocLogAnalysisData *analysisData, double *mcv, double *scv, AnalysisError *analysisError)
{
    int     maxCount;           //ファイル行数
    int     i;
    
    double  *blocValueX;            //データ領域(X
    double  *blocValueY;            //データ領域(Y
    int     data_num;               //有効件数(ファイル行数-スキップ行数)
    int     topSkip;                //先頭スキップ行数
    int     bottomSkip;             //後尾スキップ行数

    double  stdDevX;                //標準偏差X
    double  stdDevY;                //標準偏差Y

    double  averageX;               //平均値X
    double  averageY;               //平均値Y
    double  deviationX;             //偏差X
    double  deviationY;             //偏差Y
    double  deviationMul;           //偏差積
    double  deviationX2;            //偏差X^2
    double  deviationY2;            //偏差Y^2
    double  distributionX;          //分散X
    double  distributionY;          //分散Y

    double  Covariance;             //共分散
    double  CorCoe;                 //相関係数
    double  Inclination;            //傾き
    double  Section;                //切片

    double  approximationLine;          //近似直線座標
    double  approximationLineDiff;      //近似座標との差
    double  approximationLineDiff_2;    //偏差(近似座標との差)^2

    double  averageSCV;             //平均値
    double  distributionSCV;        //分散
    double  stdDevSCV;              //標準偏差

    maxCount = analysisData->ProfileValueCount;
    topSkip = analysisData->TopUnevennessSkipValue;
    bottomSkip = analysisData->BottomUnevennessSkipValue;

    //計算用領域取得
    blocValueX = (double*)malloc(sizeof(double) * maxCount);
    if (blocValueX == NULL)
    {
        sprintf_s(analysisError->cErrorMsg, analysisError->iErrorMsgLen, "ベタプロファイル領域(X)の取得に失敗しました。[%d]", maxCount);
        return NG;
    }
    blocValueY = (double*)malloc(sizeof(double) * maxCount);
    if (blocValueY == NULL)
    {
        free(blocValueX);
        sprintf_s(analysisError->cErrorMsg, analysisError->iErrorMsgLen, "ベタプロファイル領域(Y)の取得に失敗しました。[%d]", maxCount);
        return NG;
    }

    averageY = 0.0;
    averageX = 0.0;
    for (i = topSkip; i < maxCount - bottomSkip; i++)
    {
        //値設定
        blocValueY[i] = analysisData->ProfileValue[i];
        blocValueX[i] = (double)i - topSkip + 1.0;

        //データ総和
        averageX += blocValueX[i];              //X
        averageY += blocValueY[i];              //Y
    }

    //有効件数
    data_num = (maxCount - (topSkip + bottomSkip)); //ベタプロファイル件数-(前後スキップ行数)

    //平均値(QL)算出
    averageX = averageX / data_num;  // 平均値X
    averageY = averageY / data_num;  // 平均値Y

    //偏差
    deviationX = 0.0;           //偏差X
    deviationY = 0.0;           //偏差Y
    deviationMul = 0.0;         //偏差積
    deviationX2 = 0.0;          //偏差X^2
    deviationY2 = 0.0;          //偏差Y^2
    distributionX = 0.0;        //分散X
    distributionY = 0.0;        //分散Y
    for (i = topSkip; i < maxCount - bottomSkip; i++)
    {
        deviationX = blocValueX[i] - averageX;          //偏差X (X - 平均値X)
        deviationY = blocValueY[i] - averageY;          //偏差Y (Y - 平均値Y)

        deviationMul += (deviationX * deviationY);      //偏差積

        deviationX2 = deviationX * deviationX;          //偏差X^2
        deviationY2 = deviationY * deviationY;          //偏差Y^2

        distributionX += deviationX2;           //分散X
        distributionY += deviationY2;           //分散Y
    }
    distributionX = distributionX / data_num;       //分散X
    distributionY = distributionY / data_num;       //分散Y

    //////////////////////////////////////////
    //MCV算出処理
    stdDevY = sqrt(distributionY);  //標準偏差

    //変動係数(MCV値)算出(標準偏差を平均で割ったもの
    *mcv = stdDevY / averageY;

    //////////////////////////////////////////
    //SCV算出処理

    stdDevX = sqrt(distributionX);  //標準偏差X
    stdDevY = sqrt(distributionY);  //標準偏差Y

    Covariance = deviationMul / data_num;               //共分散(偏差積の平均値)
    CorCoe = Covariance / (stdDevX * stdDevY);          //相関係数(共分散/(標準偏差X*標準偏差Y))
    Inclination = CorCoe * (stdDevY / stdDevX);         //傾き(相関係数*(標準偏差Y/標準偏差X))
    Section = averageY - (Inclination * averageX);      //切片(平均値Y-(傾き*平均値X))

    //近似座標
    averageSCV = 0.0;               //平均値
    for (i = topSkip; i < maxCount - bottomSkip; i++)
    {
        approximationLine = Section + blocValueX[i] * Inclination;      //近似直線座標(切片+X*傾き)
        approximationLineDiff = blocValueY[i] - approximationLine;      //近似座標との差(Y-近似直線座標)

        averageSCV += approximationLineDiff;                //平均値
    }
    averageSCV = averageSCV / data_num;

    //偏差(近似座標との差)^2、分散
    distributionSCV = 0.0;          //分散
    for (i = topSkip; i < maxCount - bottomSkip; i++)
    {
        approximationLine = Section + blocValueX[i] * Inclination;      //近似直線座標(切片+X*傾き)
        approximationLineDiff = blocValueY[i] - approximationLine;      //近似座標との差(Y-近似直線座標)

        approximationLineDiff_2 = approximationLineDiff - averageSCV;   //偏差(近似座標との差)^2 (近似座標との差-(近似座標との差の平均値))^2
        approximationLineDiff_2 = approximationLineDiff_2 * approximationLineDiff_2;

        distributionSCV += approximationLineDiff_2;         //分散
    }
    distributionSCV = distributionSCV / data_num;           //分散

    stdDevSCV = sqrt(distributionSCV);                      //標準偏差

    *scv = stdDevSCV / averageY;        //SCV(標準偏差/平均値Y)

    free(blocValueX);
    free(blocValueY);

    return OK;
}
