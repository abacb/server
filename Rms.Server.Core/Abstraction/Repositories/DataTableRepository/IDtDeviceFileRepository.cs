using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Collections.Generic;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtDeviceFileRepository
    /// </summary>
    public interface IDtDeviceFileRepository
    {
        /// <summary>
        /// 引数に指定したDtDeviceFileをDT_DEVICE_FILEテーブルへ登録する
        /// 既にレコードが存在する場合は登録ではなく当該レコードの更新処理を行う
        /// また当該レコードに紐づいたDT_DEVICE_FILE_ATTRIBUTEテーブルの更新を行う
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        DtDeviceFile CreateOrUpdateDtDeviceFile(DtDeviceFile inData);

        /// <summary>
        /// DT_DEVICE_FILEテーブルからDtDeviceFileを取得する
        /// </summary>
        /// <param name="sid">取得するデータのSID</param>
        /// <returns>取得したデータ</returns>
        DtDeviceFile ReadDtDeviceFile(long sid);

        /// <summary>
        /// 引数に指定したパスに、ファイルパスが先頭一致するDtDeviceFileを取得する
        /// </summary>
        /// <param name="containerName">コンテナ名</param>
        /// <param name="path">パス。指定したパスに先頭一致するDtDeviceFileを取得する。</param>
        /// <param name="endDateTime">期間(終了)</param>
        /// <returns>DtDeviceFileのリスト</returns>
        IEnumerable<DtDeviceFile> FindByFilePathStartingWithAndUpdateDatetimeLessThan(string containerName, string path, DateTime endDateTime);

        //// HACK: DIの兼ね合いで、自動生成部分で定義されている基本的なCRUDのInterfaceとして記述している。
 
        /// <summary>
        /// テーブルからDtDeviceFileを削除する
        /// </summary>
        /// <param name="sid">削除するデータのSID</param>
        /// <returns>削除したデータ</returns>
        DtDeviceFile DeleteDtDeviceFile(long sid);
    }
}
