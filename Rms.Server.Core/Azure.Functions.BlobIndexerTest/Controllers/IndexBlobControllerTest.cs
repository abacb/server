using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Models;
using Rms.Server.Core.Abstraction.Pollies;
using Rms.Server.Core.Abstraction.Repositories;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Azure.Functions.BlobIndexer;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Core.Utility.Exceptions;
using Rms.Server.Core.Utility.Models.Entites;
using Rms.Server.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestHelper;

namespace Azure.Functions.BlobIndexerTest
{
    /// <summary>
    /// BlobIndexer�����e�X�g
    /// </summary>
    [TestClass]
    public class IndexBlobControllerTest
    {
        /// <summary>
        /// �����f�[�^������̃t�H�[�}�b�g��`
        /// </summary>
        private static readonly string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fff";

        /// <summary>
        /// �e�X�g�ΏۃR���e�i���ꗗ�iPrimaryBlob�j
        /// </summary>
        private static readonly string[] PrimaryBlobContainerNameList = new string[] { "log", "error", "device" };

        /// <summary>
        /// �e�X�g�ΏۃR���e�i���ꗗ�iCollectingBlob�j
        /// </summary>
        private static readonly string[] CollectingBlobContainerNameList = new string[]
        {
            "collect", "unknown", "changedcollect", "changedunknown"
        };

        /// <summary>
        /// BlobCleanController
        /// </summary>
        private static IndexBlobController _controller = null;

        /// <summary>
        /// ServiceProvider
        /// </summary>
        private static ServiceProvider _provider = null;

        /// <summary>
        /// AppSettings
        /// </summary>
        private static AppSettings _appSettings = null;

        /// <summary>
        /// DBPolly
        /// </summary>
        private static DBPolly _dBPolly = null;

        /// <summary>
        /// BlobPolly
        /// </summary>
        private static BlobPolly _blobPolly = null;

        /// <summary>
        /// PrimaryBlob
        /// </summary>
        private static PrimaryBlob _primaryBlob;

        /// <summary>
        /// CollectingBlob
        /// </summary>
        private static CollectingBlob _collectingBlob;

        /// <summary>
        /// PrimaryBlobRepositoryMock
        /// </summary>
        private static PrimaryBlobRepositoryMock _primaryBlobRepositoryMock = null;

        /// <summary>
        /// CollectingBlobRepositoryMock
        /// </summary>
        private static CollectingBlobRepositoryMock _collectingBlobRepositoryMock;

        /// <summary>
        /// DtDeviceFileRepositoryMock
        /// </summary>
        private static DtDeviceFileRepositoryMock _dtDeviceFileRepositoryMock;

        /// <summary>
        /// DateTimeProvider
        /// </summary>
        private static DateTimeProvider _dateTimeProvider = null;

        /// <summary>
        /// ���K�[
        /// </summary>
        private static TestLogger<IndexBlobService> _serviceLogger = null;

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="context">�R���e�L�X�g</param>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // �֘ADB�f�[�^��S�폜
            DbTestHelper.DeleteAllReseed();

            // DB�ݒ�
            // �}�X�^�e�[�u���f�[�^���쐬����
            DbTestHelper.ExecSqlFromFilePath(@"TestData\Sqls\MakeMasterTableData.sql");
        }

        /// <summary>
        /// �㏈��
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // �֘ADB�f�[�^��S�폜
            DbTestHelper.DeleteAllReseed();
        }

        /// <summary>
        /// BlobIndexer�����e�X�g
        /// </summary>
        /// <param name="no">�e�X�g�ԍ�</param>
        /// <param name="in_TestDateTime">�e�X�g���{���F���̒l�����DB�ɐݒ肳���쐬�E�X�V���������܂�̂Œ���</param>
        /// <param name="in_AppSettings">AppSettings�i�ǉ��܂��͊����̐ݒ�l���X�V�j</param>
        /// <param name="in_InsertNewDataSqlPath">SQL�����L�ڂ����t�@�C���p�X�i�ǉ���DB�ɐݒ肪�K�v�ȏꍇ�Ɏg�p�j</param>
        /// <param name="in_UploadedBlobInfo">�R���e�i�ɃA�b�v���[�h����t�@�C���̏����܂Ƃ߂�CSV�t�@�C���̃p�X</param>
        /// <param name="expected_BlobStatusPath">BlobIndexer���s��̃R���e�i��Blob��Ԃ��L�q����CSV�t�@�C���̃p�X</param>
        /// <param name="expected_DBStatusPath">BlobIndexer���s���DB��Ԃ��L�q����JSON�t�@�C���̃p�X</param>
        /// <param name="expected_BlobContents">���҂���Blob�t�@�C���̓��e</param>
        /// <param name="expected_LogResource">���҂��郍�O���\�[�X�i�G���[�ԍ��j</param>
        /// <param name="expected_LogInfo">���҂��郍�O�ɏo�͂��������܂Ƃ߂�CSV�t�@�C���̃p�X</param>
        /// <param name="expected_CheckNotAddedDeviceFile">�[���t�@�C�������f�[�^�e�[�u���Ƀf�[�^�ǉ����Ȃ��������Ƃ��m�F����ꍇ�ɂ�true�ɂ���</param>
        /// <param name="error_Copy">CollectingBlobRepositoryMock�N���X��Copy���\�b�h�ŗ�O�𔭐�������ꍇ��true�ɂ���</param>
        /// <param name="error_PrimaryDelete">PrimaryBlobRepositoryMock�N���X��Delete���\�b�h�ŗ�O�𔭐�������ꍇ��true�ɂ���</param>
        /// <param name="error_CollectingDelete">CollectingBlobRepositoryMock�N���X��Delete���\�b�h�ŗ�O�𔭐�������ꍇ��true�ɂ���</param>
        /// <param name="error_CreateDtDeviceFile">DtDeviceFileRepositoryMock�N���X��CreateDtDeviceFile���\�b�h�ŗ�O�𔭐�������ꍇ��true�ɂ���</param>
        /// <param name="error_Index">IndexBlobServiceMock�N���X��Index���\�b�h�ŗ�O�𔭐�������ꍇ�ɂ�true�ɂ���</param>
        /// <param name="remark">���l</param>
        /// <returns>�񓯊������^�X�N</returns>
        [DataTestMethod]
        [CsvDataSourece(@"TestCases\Controllers_IndexBlobContoroller.csv")]
        public async Task IndexBlobTest(
            string no,
            string in_TestDateTime,
            string in_AppSettings,
            string in_InsertNewDataSqlPath,
            string in_UploadedBlobInfo,
            string expected_BlobStatusPath,
            string expected_DBStatusPath,
            string expected_BlobContents,
            string expected_LogResource,
            string expected_LogInfo,
            string expected_CheckNotAddedDeviceFile,
            string error_Copy,
            string error_PrimaryDelete,
            string error_CollectingDelete,
            string error_CreateDtDeviceFile,
            string error_Index,
            string remark)
        {
            // AppSettings
            Dictionary<string, string> appSettingsConfigures = null;
            if (!string.IsNullOrEmpty(in_AppSettings))
            {
                appSettingsConfigures = JsonConvert.DeserializeObject<Dictionary<string, string>>(in_AppSettings);
            }

            // DB�ɒ[���t�@�C����񂪒ǉ�����Ă��Ȃ����Ƃ��m�F����
            bool.TryParse(expected_CheckNotAddedDeviceFile, out bool isCheckedDeviceFileNotAdded);

            // ��O�����𔭐������郁�\�b�h�������t���O
            bool.TryParse(error_Copy, out bool isErrorOnCopy);
            bool.TryParse(error_PrimaryDelete, out bool isErrorOnPrimaryDelete);
            bool.TryParse(error_CollectingDelete, out bool isErrorOnCollectingDelete);
            bool.TryParse(error_CreateDtDeviceFile, out bool isErrorCreateDtDeviceFile);
            bool.TryParse(error_Index, out bool isErrorOnIndex);

            // ���O�i�[�p���K�[�̗p��
            // Controller�p
            var actualControllerLogs = new List<TestLog>();
            var logger = new TestLogger<IndexBlobController>(actualControllerLogs);

            // Service�p
            var actualServiceLogs = new List<TestLog>();
            _serviceLogger = new TestLogger<IndexBlobService>(actualServiceLogs);

            // �e�X�g���s�����i�t�@�C���폜臒l�̊�ƂȂ�l�j
            DateTime testDateTime = ConvertStringToDateTime(in_TestDateTime);

            // DI
            DependencyInjection(testDateTime, _serviceLogger, isErrorOnIndex, appSettingsConfigures);

            // DI���Repository�̗�O�����ݒ���s��
            _dtDeviceFileRepositoryMock.Init(isErrorCreateDtDeviceFile);
            _primaryBlobRepositoryMock.Init(isErrorOnPrimaryDelete);
            _collectingBlobRepositoryMock.Init(isErrorOnCopy, isErrorOnCollectingDelete);

            // �e�X�g�p�f�[�^��DB�ɑ}������
            // �[���f�[�^�e�[�u���f�[�^���쐬����i�O��̃f�[�^���N���A����@�\�����˂Ă���j
            DbTestHelper.ExecSqlFromFilePath(@"TestData\Sqls\MakeDeviceData.sql");

            // �[���t�@�C���f�[�^�͓���̃e�X�g�P�[�X�ł̂ݒǉ�����
            if (!string.IsNullOrEmpty(in_InsertNewDataSqlPath))
            {
                DbTestHelper.ExecSqlFromFilePath(in_InsertNewDataSqlPath);
            }

            // PrimaryBlob�̐ڑ�������
            var primaryBlobConnectionString = _appSettings.PrimaryBlobConnectionString;
            var collectingBlobConnectionString = _appSettings.CollectingBlobConnectionString;

            // Blob�t�@�C���A�b�v���[�h
            // �e�X�g�p�̃t�@�C����CollectingBlob�ɃA�b�v���[�h����
            if (!string.IsNullOrEmpty(in_UploadedBlobInfo))
            {
                List<BlobInfo> uploadedBlobInfo = GetUploadedBlobInfo(in_UploadedBlobInfo);

                // Blob�A�b�v���[�h����
                foreach (BlobInfo info in uploadedBlobInfo)
                {
                    // �ڑ�������
                    string connectionString = string.Empty;
                    if (PrimaryBlobContainerNameList.Contains(info.ContainerName))
                    {
                        connectionString = primaryBlobConnectionString;
                    }
                    else if (CollectingBlobContainerNameList.Contains(info.ContainerName))
                    {
                        connectionString = collectingBlobConnectionString;
                    }

                    // �R���e�i�擾
                    CloudBlobContainer container = GetContainer(connectionString, info.ContainerName);

                    // �w�肵�����[�J���t�@�C���p�X�̃t�@�C�����A�b�v���[�h
                    await Upload(container, info.BlobPath, info.LocalFilePath, info.MetaData);
                }
            }

            // BlobIndexer���s
            {
                Rms.Server.Core.Utility.Assert.IfNull(_controller);
                _controller.IndexBlob(null, logger);
            }

            // �e�X�g���ʃt���O
            bool isDatabaseStatusMatched = true;
            bool isDeviceFileNotAdded = true;
            bool isBlobStatusMatched = true;
            bool isBlobUpdated = true;
            bool isExpectedLogContained = false;

            // DB��Ԋm�F
            if (!string.IsNullOrEmpty(expected_DBStatusPath))
            {
                // ���҂���DB�̏�Ԃ��L�q����JSON�t�@�C����ǂݏo���ăf�V���A���C�Y
                string inputJson;

                StreamReader sr = new StreamReader(expected_DBStatusPath);
                inputJson = sr.ReadToEnd();
                sr.Close();

                ExpectedDeviceFile[] expectedDeviceFiles = JsonConvert.DeserializeObject<ExpectedDeviceFile[]>(inputJson);

                // �[���t�@�C����
                int countDeviceFiles = expectedDeviceFiles.Length;

                // DB�Ɋi�[����Ă��錋�ʂ����X�g������
                DtDeviceFile[] actualDeviceFiles = new DtDeviceFile[countDeviceFiles];

                // DB�̓��e��ǂݏo��
                for (int i = 0; i < countDeviceFiles; i++)
                {
                    var model = _dtDeviceFileRepositoryMock.ReadDtDeviceFile(i + 1);    // SID��1����n�܂�
                    actualDeviceFiles[i] = model;
                }

                // ���҂��錋�ʂ��������R�[�h�̐����J�E���g����
                int expectedRecordFoundCount = 0;

                // DB�Ɣ�r����
                foreach (var expected in expectedDeviceFiles)
                {
                    foreach (var actual in actualDeviceFiles)
                    {
                        if (actual == null)
                        {
                            continue;
                        }

                        // �t�@�C���p�X�ƃR���e�i���őΏۃ��R�[�h���ǂ����m�F����
                        // �[���t�@�C�����̓R���e�i���ƃp�X�̑g�ݍ��킹�ň�Ӑ������܂�
                        if (!expected.Path.Equals(actual.FilePath) || !expected.ContainerName.Equals(actual.Container))
                        {
                            continue;
                        }

                        if (expected.CreatedTime == actual.CreateDatetime)
                        {
                            // �[���t�@�C���������̔�r���s��
                            var actualAttributes = actual.DtDeviceFileAttribute;
                            var expectedAttributes = expected.DeviceFileAttributes;

                            int attributesMatchCount = 0;

                            // Name-Value�̃y�A������v���邩�m�F����
                            if (expectedAttributes.Length != actualAttributes.Count())
                            {
                                break;
                            }

                            // Name-Value�����ׂĈ�v���邩�m�F����
                            foreach (var expectedAttribute in expectedAttributes)
                            {
                                foreach (var actualAttribute in actualAttributes)
                                {
                                    if (actualAttribute.Name.Equals(expectedAttribute.Name)
                                        && actualAttribute.Value.Equals(expectedAttribute.Value))
                                    {
                                        attributesMatchCount += 1;
                                        break;
                                    }
                                }
                            }

                            // �[���t�@�C�����̔�r���ʂ�true���[���t�@�C��������񂪂��ׂ�DB�Ɋi�[����Ă���
                            if (attributesMatchCount == expectedAttributes.Length)
                            {
                                expectedRecordFoundCount += 1;
                                break;
                            }
                        }
                    }
                }

                // ��v���郌�R�[�h�̐������҂��郌�R�[�h�̐��ƈ�v�����DB�̏�Ԃ͐�����
                if (expectedRecordFoundCount != expectedDeviceFiles.Length)
                {
                    isDatabaseStatusMatched = false;
                }
            }

            // DB�m�F�i�[���t�@�C���f�[�^���ǉ�����Ă��Ȃ����Ƃ̂݊m�F�j
            if (isCheckedDeviceFileNotAdded)
            {
                var model = _dtDeviceFileRepositoryMock.ReadDtDeviceFile(1);

                // �[���t�@�C���f�[�^�e�[�u���̐擪�Ƀ��R�[�h�����݂���ꍇ�A
                // �f�[�^���ǉ����ꂽ���ƂɂȂ�̂�NG
                if (model != null)
                {
                    isDeviceFileNotAdded = false;
                }
            }

            // Blob��Ԋm�F
            if (!string.IsNullOrEmpty(expected_BlobStatusPath))
            {
                var expectedBlobStatus = GetExpectedBlobInfoList(expected_BlobStatusPath);

                foreach (var status in expectedBlobStatus)
                {
                    string connectionString;
                    if (PrimaryBlobContainerNameList.Contains(status.ContainerName))
                    {
                        connectionString = primaryBlobConnectionString;
                    }
                    else
                    {
                        connectionString = collectingBlobConnectionString;
                    }

                    var actualBlobs = await GetBlobList(connectionString, status.ContainerName);

                    bool contains = ContainsExpectedBlobInfo(actualBlobs, status);
                    if (status.Deleted == contains)
                    {
                        // �폜�t���Otrue�ł����Blob���R���e�i�ɑ��݂��Ȃ����Ƃ����҂����
                        isBlobStatusMatched = false;
                        break;
                    }
                }
            }

            // Blob���e�m�F
            if (!string.IsNullOrEmpty(expected_BlobContents))
            {
                // �����Ǘ��N���X�Ɋi�[
                ExpectedBlobContentsInfo info = JsonConvert.DeserializeObject<ExpectedBlobContentsInfo>(expected_BlobContents);

                // �R���e�i����Blob�擾
                string connectionString = PrimaryBlobContainerNameList.Contains(info.ContainerName) ? primaryBlobConnectionString : collectingBlobConnectionString;
                var container = GetContainer(connectionString, info.ContainerName);
                var blob = container.GetBlockBlobReference(info.Path);
                ExpectedBlobContents actualBlobContent = await GetBlobContents(blob);

                // ���҂������e�ɂȂ��Ă��邩�ǂ������m�F����
                if (!info.Contents.FileName.Equals(actualBlobContent.FileName) || !info.Contents.No.Equals(actualBlobContent.No))
                {
                    // ���e����v���ĂȂ����NG
                    isBlobUpdated = false;
                }
            }
            
            // ���O�̊m�F
            if (!string.IsNullOrEmpty(expected_LogInfo))
            {
                // ���҂��郍�O
                var logInfo = GetLogInfo(expected_LogInfo);
                List<string> expectedLogMessages = GetLogMessages(expected_LogResource, logInfo);

                int expectedLogMessagesCount = expectedLogMessages.Count;
                int countMatchLogs = 0;

                foreach (string expectedLog in expectedLogMessages)
                {
                    // Controller
                    foreach (TestLog eachLog in actualControllerLogs)
                    {
                        string actualLog = eachLog.GetSimpleText();
                        if (actualLog.Contains(expectedLog))
                        {
                            // ���҂��Ă������O����������
                            countMatchLogs += 1;
                            break;
                        }
                    }

                    // Service
                    foreach (TestLog eachLog in actualServiceLogs)
                    {
                        string actualLog = eachLog.GetSimpleText();
                        if (actualLog.Contains(expectedLog))
                        {
                            // ���҂��Ă������O����������
                            countMatchLogs += 1;
                            break;
                        }
                    }

                    // ���҂��郍�O�����ׂČ�����΃t���O�𗧂Ă�
                    if (countMatchLogs == expectedLogMessagesCount)
                    {
                        isExpectedLogContained = true;
                    }
                }
            }
            else
            {
                // ���O���m�F���Ȃ��e�X�g�̏ꍇ�͋����I��true�ɂ���
                isExpectedLogContained = true;
            }

            // Blob�폜�i�R���e�i�͍폜���Ȃ��j: �e�X�g�P�[�X1�񂲂Ƃ�Blob���폜����
            // ���҂���Blob��ԃ��X�g�ɋL�ڂ���Ă���Blob�݂̂�I��I�ɍ폜����
            if (!string.IsNullOrEmpty(expected_BlobStatusPath))
            {
                var expectedBlobList = GetExpectedBlobInfoList(expected_BlobStatusPath);
                
                foreach (var expectedBlob in expectedBlobList)
                {
                    // �폜�Ώ�Blob���擾
                    string connectionString
                        = PrimaryBlobContainerNameList.Contains(expectedBlob.ContainerName) 
                            ? primaryBlobConnectionString : collectingBlobConnectionString;

                    var container = GetContainer(connectionString, expectedBlob.ContainerName);
                    var blob = container.GetBlockBlobReference(expectedBlob.BlobPath);

                    try
                    {
                        await blob.DeleteIfExistsAsync();
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            // �������������ُ�W�σR���e�i���폜����
            // �e�X�g�Ƃ͊֌W�Ȃ����ߗ�O�͖�������
            try
            {
                await DeleteContainer(collectingBlobConnectionString, "changedunknown");
            }
            catch (Exception)
            {
            }

            // �e�X�g���ʁFBlob�폜��Ɋm�F����
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isDatabaseStatusMatched);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isDeviceFileNotAdded);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isBlobStatusMatched);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isBlobUpdated);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, isExpectedLogContained);
        }

        #region Blob����

        /// <summary>
        /// �w�肵���X�g���[�W�A�J�E���g�̎w�肵�����̂�Blob�R���e�i���擾����
        /// </summary>
        /// <param name="connectionString">�ڑ�������</param>
        /// <param name="containerName">�R���e�i��</param>
        /// <returns>�R���e�i</returns>
        private static CloudBlobContainer GetContainer(string connectionString, string containerName)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            return client.GetContainerReference(containerName);
        }

        /// <summary>
        /// �w�肵���R���e�i���폜����
        /// </summary>
        /// <param name="connectionString">�ڑ�������</param>
        /// <param name="containerName">�폜�ΏۃR���e�i��</param>
        /// <returns>�R���e�i�폜�񓯊��^�X�N</returns>
        private static async Task DeleteContainer(string connectionString, string containerName)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerName);

            // Delete the specified container and handle the exception.
            await container.DeleteAsync();
        }

        /// <summary>
        /// �w�肵�����[�J���p�X�̃t�@�C����Blob�X�g���[�W�ɃA�b�v���[�h����
        /// </summary>
        /// <param name="container">�R���e�i</param>
        /// <param name="blobName">Blob��</param>
        /// <param name="localFilePath">���[�J���t�@�C���p�X</param>
        /// <param name="metaData">Blob�ɐݒ肷�郁�^�f�[�^</param>
        /// <returns>�񓯊������^�X�N</returns>
        private static async Task Upload(CloudBlobContainer container, string blobName, string localFilePath, Dictionary<string, string> metaData)
        {
            // ���[�J���̃e�X�g�p�t�@�C����Blob�ɕϊ�����
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            // ���^�f�[�^�ǉ�
            foreach (var (key, value) in metaData)
            {
                blob.Metadata.Add(key, Uri.EscapeDataString(value));
            }

            // �w�肵�����[�J���t�@�C���p�X�̃t�@�C�����A�b�v���[�h
            await blob.UploadFromFileAsync(localFilePath);
        }

        /// <summary>
        /// �w�肵��Blob�̓��e���擾����
        /// </summary>
        /// <param name="blob">Blob</param>
        /// <returns>Blob�t�@�C���̓��e�iJson�e�L�X�g�j</returns>
        private static async Task<ExpectedBlobContents> GetBlobContents(CloudBlockBlob blob)
        {
            string jsonText;
            using (var memoryStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memoryStream);
                jsonText = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return JsonConvert.DeserializeObject<ExpectedBlobContents>(jsonText);
        }

        /// <summary>
        /// �w�肵���X�g���[�W�A�J�E���g�̎w�肵���R���e�i����Blob���X�g���擾����
        /// </summary>
        /// <param name="connectionString">�ڑ�������</param>
        /// <param name="containerName">�R���e�i��</param>
        /// <returns>Blob���X�g</returns>
        private static async Task<List<CloudBlob>> GetBlobList(string connectionString, string containerName)
        {
            List<CloudBlob> blobList = new List<CloudBlob>();

            var _account = CloudStorageAccount.Parse(connectionString);
            var client = _account.CreateCloudBlobClient();

            var container = client.GetContainerReference(containerName);

            if (!await container.ExistsAsync())
            {
                return blobList;
            }

            // Blob�ꗗ���擾
            BlobContinuationToken token = null;
            do
            {
                // Blob�ꗗ���擾
                var segment = await container.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, null, token, null, null);

                token = segment.ContinuationToken;

                var blobs = segment.Results.OfType<CloudBlockBlob>();

                // Blob�����X�g�ɒǉ�
                foreach (var blob in blobs)
                {
                    blobList.Add(blob);
                }
            }
            while (token != null);

            return blobList;
        }

        #endregion

        /// <summary>
        /// DI
        /// </summary>
        /// <param name="utcNow">�e�X�g���{����</param>
        /// <param name="serviceLogger">DI����Service�p�̃��K�[</param>
        /// <param name="isErrorOnIndex">Service�̃��\�b�h�ŗ�O�𔭐������邩?</param>
        /// <param name="configures">AppSettings�ɒǉ�����ݒ�</param>
        private void DependencyInjection(
            DateTime utcNow, 
            TestLogger<IndexBlobService> serviceLogger, 
            bool isErrorOnIndex = false,
            Dictionary<string, string> configures = null)
        {
            TestDiProviderBuilder builder = new TestDiProviderBuilder();

            // Blob
            builder.ServiceCollection.AddTransient<PrimaryBlob>();
            builder.ServiceCollection.AddTransient<CollectingBlob>();

            // Polly
            builder.ServiceCollection.AddSingleton(s => new BlobPolly(s.GetService<AppSettings>()));
            builder.ServiceCollection.AddSingleton(s => new DBPolly(s.GetService<AppSettings>()));

            // Logger
            builder.ServiceCollection.AddSingleton<ILogger<IndexBlobService>>(serviceLogger);

            // Service
            if (isErrorOnIndex)
            {
                builder.ServiceCollection.AddTransient<IIndexBlobService, IndexBlobServiceMock>();
            }

            // Controller����
            builder.ServiceCollection.AddTransient<IndexBlobController>();

            // Repository����
            builder.ServiceCollection.AddTransient<IPrimaryRepository, PrimaryBlobRepositoryMock>();
            builder.ServiceCollection.AddTransient<ICollectingRepository, CollectingBlobRepositoryMock>();
            builder.ServiceCollection.AddTransient<IDtDeviceFileRepository, DtDeviceFileRepositoryMock>();

            // TimeProvider
            builder.ServiceCollection.AddSingleton<ITimeProvider>(UnitTestHelper.CreateTimeProvider(utcNow));

            // �ǉ��̐ݒ荀��
            if (configures != null)
            {
                builder.AddConfigures(configures);
            }

            _provider = builder.Build();

            // AppSettings
            _appSettings = _provider.GetService<AppSettings>();

            // DateTimeProvider
            _dateTimeProvider = _provider.GetService<ITimeProvider>() as DateTimeProvider;

            // Primary & CollectingBlob
            _primaryBlob = _provider.GetService<PrimaryBlob>();
            _collectingBlob = _provider.GetService<CollectingBlob>();

            // DBPolly
            _dBPolly = _provider.GetService<DBPolly>();

            // BlobPolly
            _blobPolly = _provider.GetService<BlobPolly>();

            // BlobCleanController
            _controller = _provider.GetService<IndexBlobController>();

            // PrimaryBlobRepositoryMock
            _primaryBlobRepositoryMock = _provider.GetService<IPrimaryRepository>() as PrimaryBlobRepositoryMock;

            // CollectingBlobRepositoryMock
            _collectingBlobRepositoryMock = _provider.GetService<ICollectingRepository>() as CollectingBlobRepositoryMock;

            // DtDeviceFileRepositoryMock
            _dtDeviceFileRepositoryMock = _provider.GetService<IDtDeviceFileRepository>() as DtDeviceFileRepositoryMock;
        }

        /// <summary>
        /// �e�L�X�g�t�@�C������1�s���ǂݏo�������ʂ����X�g�ɕϊ����Ď擾����
        /// �擪�s�̓w�b�_�ƌ��Ȃ����߃��X�g�ɂ͒ǉ����Ȃ�
        /// </summary>
        /// <param name="filePath">�e�L�X�g�t�@�C���p�X</param>
        /// <returns>�ǂݏo�������ʂ�1�s���i�[�������X�g</returns>
        private List<string> GetLinesFromTextFile(string filePath)
        {
            List<string> result = new List<string>();

            StreamReader sr = new StreamReader(filePath);
            _ = sr.ReadLine();  // �擪�s�̓w�b�_�Ȃ̂œǂݔ�΂�

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    result.Add(line);
                }
            }

            sr.Close();

            return result;
        }

        /// <summary>
        /// �������DateTime�^�ɕϊ�����
        /// </summary>
        /// <param name="dateTime">������</param>
        /// <returns>DateTime</returns>
        private DateTime ConvertStringToDateTime(string dateTime)
        {
            return string.IsNullOrEmpty(dateTime) ? default(DateTime) : DateTime.ParseExact(dateTime, DateTimeFormat, null);
        }

        #region Log

        /// <summary>
        /// CSV�t�@�C���Ɋi�[���ꂽ�f�[�^���烍�O�ɏo�͂���f�[�^���X�g���쐬����
        /// </summary>
        /// <param name="logInfoCsvFilePath">���O�����i�[����CSV�t�@�C���̃p�X</param>
        /// <returns>���O�o�͏�񃊃X�g</returns>
        private List<LogInfo> GetLogInfo(string logInfoCsvFilePath)
        {
            List<LogInfo> result = new List<LogInfo>();

            if (string.IsNullOrEmpty(logInfoCsvFilePath))
            {
                return result;
            }

            // CSV�t�@�C���ǂݍ���
            List<string> lines = GetLinesFromTextFile(logInfoCsvFilePath);
            foreach (string line in lines)
            {
                string[] columns = line.Split(",");
                if (columns.Length == 2)
                {
                    string containerName = columns[0];
                    string targetFilePath = columns[1];

                    LogInfo info = new LogInfo
                    {
                        ContainerName = containerName,
                        Path = targetFilePath
                    };

                    result.Add(info);
                }
            }

            return result;
        }

        /// <summary>
        /// ���҂��郍�O���b�Z�[�W���X�g���擾����
        /// </summary>
        /// <param name="errorCode">�G���[�R�[�h������</param>
        /// <param name="logInfo">���O�Ɋi�[������̃��X�g</param>
        /// <returns>Logger�ɋL�^���ꂽ�G���[���b�Z�[�W</returns>
        private List<string> GetLogMessages(string errorCode, List<LogInfo> logInfo)
        {
            List<string> result = new List<string>();

            var actualLogs = new List<TestLog>();
            var logger = new TestLogger<CleanBlobService>(actualLogs);

            if (logInfo.Count <= 0)
            {
                result.Add(errorCode);
            }
            else
            {
                foreach (LogInfo info in logInfo)
                {
                    // ���O�o��
                    Rms.Server.Core.Utility.Extensions.LoggerExtensions.Error(
                        logger,
                        new RmsException(errorCode), 
                        errorCode, 
                        new object[] { info.ContainerName, info.Path });
                }

                foreach (var log in actualLogs)
                {
                    result.Add(log.GetSimpleText());
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        /// ���҂���Blob��Ԃ�Blob���X�g���ɑ��݂��邩�m�F����
        /// </summary>
        /// <param name="actualBlobs">�R���e�i�Ɋi�[���ꂽBlob���X�g</param>
        /// <param name="expectedBlobInfo">���҂���Blob���</param>
        /// <returns>���҂���Blob��Ԃ�Blob���X�g���ɑ��݂����true��Ԃ�</returns>
        private bool ContainsExpectedBlobInfo(List<CloudBlob> actualBlobs, ExpectedBlobInfo expectedBlobInfo)
        {
            bool result = false;

            foreach (var blob in actualBlobs)
            {
                string path = blob.Name;
                var metadata = blob.Metadata;

                // �p�X�m�F
                if (!path.Equals(expectedBlobInfo.BlobPath))
                {
                    // �p�X����v���Ȃ��̂ł���Ύ��̗v�f���`�F�b�N
                    continue;
                }

                // �p�X����v�����v�f�ɂ��ă��^�f�[�^���m�F
                bool allKeyValuePairMatches = true;

                foreach (var (expectedKey, expectedValue) in expectedBlobInfo.Metadata)
                {
                    if (!metadata.ContainsKey(expectedKey))
                    {
                        // ���^�f�[�^�̃L�[����v���Ȃ��̂�NG
                        allKeyValuePairMatches = false;
                        break;
                    }

                    string actualValue = Uri.UnescapeDataString(metadata[expectedKey]);

                    if (!actualValue.Equals(expectedValue))
                    {
                        // ���^�f�[�^�̃L�[�ƒl����v���Ȃ��̂�NG
                        allKeyValuePairMatches = false;
                        break;
                    }
                }

                if (allKeyValuePairMatches)
                {
                    // NG�����������ꍇ�ɂ́A�p�X�ƃ��^�f�[�^�����S�Ɉ�v�����̂�
                    // �w�肵��Blob���R���e�i�ɑ��݂����Ɣ���
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// �A�b�v���[�h����Blob�̏���ݒ�t�@�C������擾����
        /// </summary>
        /// <param name="csvPath">�ݒ�t�@�C��</param>
        /// <returns>�A�b�v���[�h����Blob��񃊃X�g</returns>
        private List<BlobInfo> GetUploadedBlobInfo(string csvPath)
        {
            List<BlobInfo> result = new List<BlobInfo>();

            if (string.IsNullOrEmpty(csvPath))
            {
                return result;
            }

            // CSV�t�@�C���ǂݍ���
            List<string> lines = GetLinesFromTextFile(csvPath);
            foreach (string line in lines)
            {
                string[] columns = line.Split(",");
                if (columns.Length == 9)
                {
                    string containerName = columns[0];
                    string localPath = columns[1];
                    string blobPath = columns[2];

                    string sys_container = columns[3];
                    string sys_owner = columns[4];
                    string sys_sub_directory = columns[5];
                    string sys_file_datetime = columns[6];
                    string user = columns[7];
                    string user2 = columns[8];

                    BlobInfo status = new BlobInfo();
                    Dictionary<string, string> metaData = new Dictionary<string, string>();

                    if (!string.IsNullOrEmpty(sys_container))
                    {
                        metaData["sys_container"] = sys_container;
                    }

                    if (!string.IsNullOrEmpty(sys_owner))
                    {
                        metaData["sys_owner"] = sys_owner;
                    }

                    if (!string.IsNullOrEmpty(sys_sub_directory))
                    {
                        metaData["sys_sub_directory"] = sys_sub_directory;
                    }

                    if (!string.IsNullOrEmpty(sys_file_datetime))
                    {
                        metaData["sys_file_datetime"] = sys_file_datetime;
                    }

                    if (!string.IsNullOrEmpty(user))
                    {
                        metaData["user"] = user;
                    }

                    if (!string.IsNullOrEmpty(user2))
                    {
                        metaData["user2"] = user2;
                    }

                    status.ContainerName = containerName;
                    status.LocalFilePath = localPath;
                    status.BlobPath = blobPath;
                    status.MetaData = metaData;

                    result.Add(status);
                }
            }

            return result;
        }

        /// <summary>
        /// ���҂���Blob���̃��X�g��ݒ�t�@�C������擾����
        /// </summary>
        /// <param name="expectedBlobStatusCsvPath">�ݒ�t�@�C��</param>
        /// <returns>���҂���Blob��񃊃X�g</returns>
        private List<ExpectedBlobInfo> GetExpectedBlobInfoList(string expectedBlobStatusCsvPath)
        {
            List<ExpectedBlobInfo> infoList = new List<ExpectedBlobInfo>();

            if (string.IsNullOrEmpty(expectedBlobStatusCsvPath))
            {
                return infoList;
            }

            // CSV�t�@�C���ǂݍ���
            List<string> lines = GetLinesFromTextFile(expectedBlobStatusCsvPath);
            foreach (string line in lines)
            {
                string[] columns = line.Split(",");
                if (columns.Length == 9)
                {
                    string containerName = columns[0];
                    string path = columns[1];
                    string deleted = columns[2];
                    string sys_container = columns[3];
                    string sys_owner = columns[4];
                    string sys_sub_directory = columns[5];
                    string sys_file_datetime = columns[6];
                    string user = columns[7];
                    string user2 = columns[8];

                    Dictionary<string, string> metadata = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(sys_container))
                    {
                        metadata.Add("sys_container", sys_container);
                    }

                    if (!string.IsNullOrEmpty(sys_owner))
                    {
                        metadata.Add("sys_owner", sys_owner);
                    }

                    if (!string.IsNullOrEmpty(sys_sub_directory))
                    {
                        metadata.Add("sys_sub_directory", sys_sub_directory);
                    }

                    if (!string.IsNullOrEmpty(sys_file_datetime))
                    {
                        metadata.Add("sys_file_datetime", sys_file_datetime);
                    }

                    if (!string.IsNullOrEmpty(user))
                    {
                        metadata.Add("user", user);
                    }

                    if (!string.IsNullOrEmpty(user2))
                    {
                        metadata.Add("user2", user2);
                    }

                    bool.TryParse(deleted, out bool isDeleted);

                    ExpectedBlobInfo info = new ExpectedBlobInfo
                    {
                        ContainerName = containerName,
                        BlobPath = path,
                        Deleted = isDeleted,
                        Metadata = metadata
                    };

                    infoList.Add(info);
                }
            }

            return infoList;
        }

        #region �e�X�g�p���Ǘ��N���X

        /// <summary>
        /// �A�b�v���[�h����Blob�̏����Ǘ�����N���X
        /// </summary>
        public class BlobInfo
        {
            /// <summary>
            /// �R���e�i��
            /// </summary>
            public string ContainerName { get; set; }

            /// <summary>
            /// �A�b�v���[�h�p�e�X�g�f�[�^�̃��[�J���t�@�C���p�X
            /// </summary>
            public string LocalFilePath { get; set; }

            /// <summary>
            /// �A�b�v���[�h��R���e�i���Blob�p�X
            /// </summary>
            public string BlobPath { get; set; }

            /// <summary>
            /// ���^�f�[�^
            /// </summary>
            public Dictionary<string, string> MetaData { get; set; }
        }

        /// <summary>
        /// �R���e�i�Ɋi�[����Ă��邱�Ƃ����҂����Blob�̏�Ԃ��Ǘ�����N���X
        /// </summary>
        public class ExpectedBlobInfo
        {
            /// <summary>
            /// �Ώ�Blob���폜����Ă��邱�Ƃ��m�F���邩?
            /// </summary>
            public bool Deleted { get; set; }

            /// <summary>
            /// �R���e�i��
            /// </summary>
            public string ContainerName { get; set; }

            /// <summary>
            /// �R���e�i���Blob�p�X
            /// </summary>
            public string BlobPath { get; set; }

            /// <summary>
            /// ���^�f�[�^
            /// </summary>
            public Dictionary<string, string> Metadata { get; set; }
        }

        /// <summary>
        /// Blob�̓��e
        /// </summary>
        public class ExpectedBlobContents
        {
            /// <summary>
            /// �e�X�g�ԍ�
            /// </summary>
            public string No { get; set; }

            /// <summary>
            /// �t�@�C����
            /// </summary>
            public string FileName { get; set; }
        }

        /// <summary>
        /// �X�V���s��ꂽ���Ƃ��m�F����Blob�t�@�C���̏��ƃt�@�C���̓��e���܂Ƃ߂��N���X
        /// </summary>
        public class ExpectedBlobContentsInfo
        {
            /// <summary>
            /// Blob�̓��e
            /// </summary>
            public ExpectedBlobContents Contents { get; set; }

            /// <summary>
            /// �R���e�i��
            /// </summary>
            public string ContainerName { get; set; }

            /// <summary>
            /// Blob�p�X
            /// </summary>
            public string Path { get; set; }
        }

        /// <summary>
        /// ���҂���[���t�@�C���f�[�^�e�[�u���̏�Ԃ��Ǘ�����N���X
        /// </summary>
        public class ExpectedDeviceFile
        {
            /// <summary>
            /// �R���e�i��
            /// </summary>
            public string ContainerName { get; set; }

            /// <summary>
            /// Blob�p�X
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// �X�V����
            /// </summary>
            public DateTime? UpdateTime { get; set; }

            /// <summary>
            /// �쐬����
            /// </summary>
            public DateTime? CreatedTime { get; set; }

            /// <summary>
            /// �t�@�C�������f�[�^
            /// </summary>
            public ExpectedDeviceFileAttribute[] DeviceFileAttributes { get; set; }
        }

        /// <summary>
        /// ���҂���[���t�@�C�������f�[�^�e�[�u���̏�Ԃ��Ǘ�����N���X
        /// </summary>
        public class ExpectedDeviceFileAttribute
        {
            /// <summary>
            /// Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Value
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// �X�V����
            /// </summary>
            public DateTime? UpdateTime { get; set; }

            /// <summary>
            /// �쐬����
            /// </summary>
            public DateTime? CreatedTime { get; set; }
        }

        /// <summary>
        /// ���O�ɏo�͂�������܂Ƃ߂��N���X
        /// </summary>
        public class LogInfo
        {
            /// <summary>
            /// ���O�ɏo�͂���R���e�i��
            /// </summary>
            public string ContainerName { get; set; }

            /// <summary>
            /// ���O�ɏo�͂���t�@�C���p�X
            /// </summary>
            public string Path { get; set; }
        }

        #endregion

        #region Mock�N���X

        /// <summary>
        /// IndexBlobService�̃��b�N�N���X�i��O�����p�j
        /// </summary>
        public class IndexBlobServiceMock : IIndexBlobService
        {
            /// <summary>
            /// Service�̃��\�b�h�������I�ɗ�O�ɂ���
            /// </summary>
            public void Index()
            {
                throw new RmsException("IndexBlobControllerTest");
            }
        }

        /// <summary>
        /// DtDeviceFileRepository Mock�N���X
        /// </summary>
        public class DtDeviceFileRepositoryMock : IDtDeviceFileRepository
        {
            /// <summary>
            /// Log���X�g
            /// </summary>
            private static readonly List<TestLog> DtDeviceFileActualLogs = new List<TestLog>();

            /// <summary>
            /// ��O�������s�t���O
            /// CreateDtDeviceFile�p
            /// </summary>
            /// <remarks>
            /// ��O�������N�����ꍇ�ɂ�true�A���K�̋������N�����ꍇ�ɂ�false
            /// </remarks>
            private static bool _failedToCreateDtDeviceFile = false;

            /// <summary>
            /// ���K�̏��������s����ꍇ�ɎQ�Ƃ��郊�|�W�g��
            /// </summary>
            private static DtDeviceFileRepository _repo = null;

            /// <summary>
            /// ���������� : ��O�����ݒ���s��
            /// </summary>
            /// <param name="failedToCreateDtDeviceFile">��O�����t���O</param>
            public void Init(bool failedToCreateDtDeviceFile)
            {
                // ��O���������邩?
                _failedToCreateDtDeviceFile = failedToCreateDtDeviceFile;

                // ���|�W�g��DI
                var logger = new TestLogger<DtDeviceFileRepository>(DtDeviceFileActualLogs);
                _repo = new DtDeviceFileRepository(logger, _dateTimeProvider, _dBPolly, _appSettings);
            }

            /// <summary>
            /// �����Ɏw�肵��DtDeviceFile��DT_DEVICE_FILE�e�[�u���֓o�^����
            /// </summary>
            /// <param name="inData">�o�^����f�[�^</param>
            /// <returns>��������</returns>
            public DtDeviceFile CreateOrUpdateDtDeviceFile(DtDeviceFile inData)
            {
                if (_failedToCreateDtDeviceFile)
                {
                    throw new RmsException("IndexBlobControllerTest");
                }

                return _repo.CreateOrUpdateDtDeviceFile(inData);
            }

            /// <summary>
            /// DT_DEVICE_FILE�e�[�u������DtDeviceFile���擾����
            /// </summary>
            /// <param name="sid">�擾����f�[�^��SID</param>
            /// <returns>�擾�����f�[�^</returns>
            public DtDeviceFile ReadDtDeviceFile(long sid)
            {
                return _repo.ReadDtDeviceFile(sid);
            }

            /// <summary>
            /// �e�[�u������DtDeviceFile���폜����
            /// </summary>
            /// <param name="sid">�폜����f�[�^��SID</param>
            /// <returns>�폜�����f�[�^</returns>
            public DtDeviceFile DeleteDtDeviceFile(long sid)
            {
                return _repo.DeleteDtDeviceFile(sid);
            }

            /// <summary>
            /// �����Ɏw�肵���p�X�ɁA�t�@�C���p�X���擪��v����DtDeviceFile���擾����
            /// </summary>
            /// <param name="containerName">�R���e�i��</param>
            /// <param name="path">�p�X�B�w�肵���p�X�ɐ擪��v����DtDeviceFile���擾����B</param>
            /// <param name="endDateTime">����(�I��)</param>
            /// <returns>DtDeviceFile�̃��X�g</returns>
            public IEnumerable<DtDeviceFile> FindByFilePathStartingWithAndUpdateDatetimeLessThan(string containerName, string path, DateTime endDateTime)
            {
                return _repo.FindByFilePathStartingWithAndUpdateDatetimeLessThan(containerName, path, endDateTime);
            }
        }

        /// <summary>
        /// PrimaryBlobRepository Mock�N���X
        /// </summary>
        public class PrimaryBlobRepositoryMock : IPrimaryRepository
        {
            /// <summary>
            /// ���K�[
            /// </summary>
            private static readonly List<TestLog> TestLogList = new List<TestLog>();

            /// <summary>
            /// ��O�������s�t���O
            /// </summary>
            private static bool _failedToDelete = false;

            /// <summary>
            /// ���K�̏��������s����ꍇ�ɎQ�Ƃ��郊�|�W�g��
            /// </summary>
            private static PrimaryBlobRepository _repo = null;

            /// <summary>
            /// ����������
            /// ��O���������s���郁�\�b�h�̃t���O��ݒ肷��
            /// </summary>
            /// <param name="failedToDelete">��O�������N�������i��O�������N�����ꍇ�ɂ�true�j</param>
            public void Init(bool failedToDelete)
            {
                // ��O�𔭐������邩?
                _failedToDelete = failedToDelete;

                // ���|�W�g��DI
                DBPolly dbPolly = new DBPolly(_appSettings);
                var logger = new TestLogger<PrimaryBlobRepository>(TestLogList);

                _repo = new PrimaryBlobRepository(_appSettings, _primaryBlob, _blobPolly, logger);
            }

            /// <summary>
            /// Blob���폜����
            /// </summary>
            /// <param name="file">�폜����t�@�C��</param>
            public void Delete(ArchiveFile file)
            {
                if (_failedToDelete)
                {
                    throw new RmsException("IndexBlobControllerTest");
                }

                _repo.Delete(file);
            }
        }

        /// <summary>
        /// CollectingBlobRepository Mock�N���X
        /// </summary>
        public class CollectingBlobRepositoryMock : ICollectingRepository
        {
            /// <summary>
            /// ���K�[
            /// </summary>
            private static readonly List<TestLog> TestLogList = new List<TestLog>();

            /// <summary>
            /// Copy���\�b�h�ŗ�O�𔭐������邩?
            /// </summary>
            private static bool _isErrorOnCopy = false;

            /// <summary>
            /// Delete���\�b�h�ŗ�O�𔭐������邩?
            /// </summary>
            private static bool _isErrorOnDelete = false;

            /// <summary>
            /// ���K�������s�p�̃��|�W�g���C���X�^���X
            /// </summary>
            private static CollectingBlobRepository _repo = null;

            /// <summary>
            /// ���������� : ��O�����ݒ���s��
            /// </summary>
            /// <param name="isErrorOnCopy">Copy���\�b�h�ŗ�O�𔭐������邩?</param>
            /// <param name="isErrorOnDelete">Delete���\�b�h�ŗ�O�𔭐������邩?</param>
            public void Init(bool isErrorOnCopy, bool isErrorOnDelete)
            {
                // ��O���������邩?
                _isErrorOnCopy = isErrorOnCopy;
                _isErrorOnDelete = isErrorOnDelete;

                // ���|�W�g��DI
                var logger = new TestLogger<PrimaryBlobRepository>(TestLogList);
                _repo = new CollectingBlobRepository(_appSettings, _primaryBlob, _collectingBlob, _blobPolly, logger);
            }

            /// <summary>
            /// �Ώۂ̃t�@�C�����폜����B
            /// </summary>
            /// <param name="file">�t�@�C��</param>
            public void Delete(ArchiveFile file)
            {
                if (_isErrorOnDelete)
                {
                    throw new RmsException("IndexBlobControllerTest");
                }

                _repo.Delete(file);
            }

            /// <summary>
            /// �w��directory���̃t�@�C�����擾����
            /// </summary>
            /// <param name="directory">�f�B���N�g��</param>
            /// <returns>�t�@�C��</returns>
            public IEnumerable<ArchiveFile> GetArchiveFiles(ArchiveDirectory directory)
            {
                return _repo.GetArchiveFiles(directory);
            }

            /// <summary>
            /// �R���N�e�B���OBlob����v���C�}��Blob�ɃR�s�[����
            /// </summary>
            /// <param name="source">�R�s�[��</param>
            /// <param name="destination">�R�s�[��</param>
            public void CopyToPrimary(ArchiveFile source, ArchiveFile destination)
            {
                _repo.CopyToPrimary(source, destination);
            }

            /// <summary>
            /// ����Blob��̃R���e�i�Ƀt�@�C�����R�s�[����B
            /// </summary>
            /// <param name="source">CollectingBlob�̃R�s�[��</param>
            /// <param name="destination">CollectingBlob�̃R�s�[��</param>
            public void Copy(ArchiveFile source, ArchiveFile destination)
            {
                if (_isErrorOnCopy)
                {
                    throw new RmsException("IndexBlobControllerTest");
                }

                _repo.Copy(source, destination);
            }
        }

        #endregion
    }
}
