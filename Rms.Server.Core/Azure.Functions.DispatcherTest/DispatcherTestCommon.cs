using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Newtonsoft.Json;
using Rms.Server.Core.Abstraction.Repositories.Blobs;
using Rms.Server.Core.Azure.Functions.Dispatcher;
using Rms.Server.Core.Azure.Functions.Startup;
using Rms.Server.Core.Service.Services;
using Rms.Server.Core.Utility;
using Rms.Server.Test;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using TestHelper;

namespace Azure.Functions.DispatcherTest
{
    /// <summary>
    /// DispatcherTestCommon
    /// </summary>
    public static class DispatcherTestCommon
    {
        /// <summary>
        /// マスタテーブルデータを削除する
        /// </summary>
        public static void DeleteMasterTableData()
        {
            DbTestHelper.ExecSqlFromFilePath(@"TestData\DeleteMastersReseed.sql");
        }

        /// <summary>
        /// マスタテーブルデータを生成する
        /// </summary>
        public static void MakeMasterTableData()
        {
            DbTestHelper.ExecSqlFromFilePath(@"TestData\MakeMasterTableData.sql");
        }

        /// <summary>
        /// 試験対象DBのデータを削除する
        /// </summary>
        public static void DeleteDbData()
        {
            DbTestHelper.ExecSqlFromFilePath(@"TestData\DeleteDispatcherData.sql");
        }

        /// <summary>
        /// FailureBlobのファイルを削除する
        /// <param name="failureBlob">FailureBlob</param>
        /// <param name="containerName">コンテナ名</param>
        /// </summary>
        public static void DeleteFailureBlobFile(FailureBlob failureBlob, string containerName)
        {
            foreach (CloudBlockBlob blockBlob in failureBlob.Client.GetBlockBlobs(containerName))
            {
                blockBlob.DeleteIfExistsAsync().Wait();
            }
        }
    }
}
