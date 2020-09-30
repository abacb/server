#pragma once

#define ERROR_MSG_LEN (512)

// �A���~�X���[�v���O��̓f�[�^
#define LUMINANCE_VALUE_LEN (1000)
#define AREA_STANDARD_DATA_LEN LUMINANCE_VALUE_LEN

typedef struct AlmiLogAnalysisData
{
    int LuminanceValue[LUMINANCE_VALUE_LEN];        // �P�x�l
    int LuminanceValueCount;                        // �P�x�l�f�[�^��
    double MinSlopeValue;                           // �X����l�����l
    double MiddleSlopeValue;                        // �X����l���Ԓl
    double MaxSlopeValue;                           // �X����l����l
    int LowVoltageAreaValue;                        // �ʐϊ�l(��Ǔd����)
    int HighVoltageAreaValue;                       // �ʐϊ�l(���Ǔd����)
    int AreaStandardData[AREA_STANDARD_DATA_LEN];   // �ʐώZ�o�p�f�[�^
    int AreaDataNum;                                // �ʐώZ�o�f�[�^��
    double AlsStandardValue;                        // ALS�t�������菈�� �����l
} AlmiLogAnalysisData;

// �����A���~�X���[�v���O��͌���
#define JUDGE_RESULT_LEN (8)
#define SLOPE_REVERSE_RESULT_LEN (8)

typedef struct AlmiLogAnalysisResult
{
    char JudgeResult[JUDGE_RESULT_LEN];                 // ���茋��
    double CulSlopeValue;                               // �X���Z�o�l
    int CulAreaValue;                                   // �ʐώZ�o�l
    int CulStdValue;                                    // �ʐϊ�l
    char SlopeReverseResult[SLOPE_REVERSE_RESULT_LEN];  // ALS�t�������菈�� ���茋��
    double Inclination;                                 // ALS�t�������菈�� �X���Z�o�l
    char ErrorMsg[ERROR_MSG_LEN];                       // ��͎��ɔ��������G���[�̏ڍ�
} AlmiLogAnalysisResult;

// �������O��̓f�[�^
#define PROFILE_VALUE_LEN (1000)

typedef struct BlocLogAnalysisData
{
    double ProfileValue[PROFILE_VALUE_LEN]; // �v���t�@�C���l
    int ProfileValueCount;                  // �v���t�@�C���l�f�[�^��
    int GpValue;                            // GP�l
    int TopUnevennessSkipValue;             // �擪�X�L�b�v�s��
    int BottomUnevennessSkipValue;          // ����X�L�b�v�s��
    double McvStandardValue;                // MCV�����l
    double ScvStandardValue1;               // SCV�����l1(OK�ENG��)
    double ScvStandardValue2;               // SCV�����l2(NG�E�����)
} BlocLogAnalysisData;

// �������O��͌���
#define UNEVEN_RESULT_LEN (8)

typedef struct BlocLogAnalysisResult
{
    char UnevenResult[UNEVEN_RESULT_LEN];   // �ȈՃ������菈�� ���茋��
    double Mcv;                             // �ȈՃ������菈�� MCV�Z�o�l
    double Scv;                             // �ȈՃ������菈�� SCV�Z�o�l
    char ImageClassification;               // ����Ώۉ摜���
    int PixelSize;                          // ��f�T�C�Y
    char ErrorMsg[ERROR_MSG_LEN];           // ��͎��ɔ��������G���[�̏ڍ�
} BlocLogAnalysisResult;

// �O�����J�֐�
extern "C" {
    __declspec(dllexport) int __stdcall AnalyzeAlmiLog(AlmiLogAnalysisData* analysisData, AlmiLogAnalysisResult* analysisResult);
    __declspec(dllexport) int __stdcall AnalyzeBlocLog(BlocLogAnalysisData* analysisData, BlocLogAnalysisResult* analysisResult);
}