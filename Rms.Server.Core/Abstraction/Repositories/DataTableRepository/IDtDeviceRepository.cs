using Rms.Server.Core.Utility.Models.Dispatch;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.Collections.Generic;

namespace Rms.Server.Core.Abstraction.Repositories
{
    /// <summary>
    /// IDtDeviceRepository : IRepository
    /// </summary>
    public interface IDtDeviceRepository : IRepository
    {
        /// <summary>
        /// 引数に指定したDtDeviceをDT_DEVICEテーブルへ登録する
        /// </summary>
        /// <param name="inData">登録するデータ</param>
        /// <returns>処理結果</returns>
        DtDevice CreateDtDevice(DtDevice inData);

        /// <summary>
        /// 引数に指定したDtDeviceでDT_DEVICEテーブルを更新する
        /// </summary>
        /// <param name="inData">更新するデータ</param>
        /// <returns>更新したデータ</returns>
        DtDevice UpdateDtDevice(DtDevice inData);

        /// <summary>
        /// DT_DEVICEテーブルからDtDeviceを取得する
        /// </summary>
        /// <param name="sid">端末SID</param>
        /// <returns>取得したデータ</returns>
        DtDevice ReadDtDevice(long sid);

        /// <summary>
        /// テーブルからDtDeviceを取得する
        /// DT_DEVICEテーブルからDtDeviceを取得する
        /// </summary>
        /// <param name="equipmentUidOrEdgeId">取得するデータの機器UIDまたはエッジID</param>
        /// <returns>取得したデータ</returns>
        DtDevice ReadDtDevice(string equipmentUidOrEdgeId);

        /// <summary>
        /// DT_DEVICEテーブルからDtDeviceを取得する
        /// </summary>
        /// <param name="edgeId">エッジID</param>
        /// <returns>取得したデータ</returns>
        DtDevice ReadDtDevice(Guid edgeId);

        /// <summary>
        /// 端末テーブルの接続ステータスを更新する
        /// </summary>
        /// <param name="sid">端末SID</param>
        /// <param name="connectionEventTimeInfo">接続/切断イベント時刻情報</param>
        /// <returns>更新したデータ。ステータスに変更がなかった場合にはnullを返す</returns>
        DtDevice UpdateDeviceConnectionStatus(long sid, ConnectionEventTimeInfo connectionEventTimeInfo);

        /// <summary>
        /// デバイスツイン更新イベントを受信して端末テーブルの「リモート接続UID」と「RMSソフトバージョン」を更新する
        /// </summary>
        /// <param name="sid">端末SID</param>
        /// <param name="data">更新データ</param>
        /// <returns>更新したデータ。テーブルが更新されなかった場合にはnullを返す</returns>
        DtDevice UpdateDeviceInfoByTwinChanged(long sid, DtTwinChanged data);

        /// <summary>
        /// DT_DEVICEテーブルからオンラインなゲートウェイ機器のDtDeviceを取得する
        /// </summary>
        /// <param name="groupData">配信グループ</param>
        /// <returns>オンラインなゲートウェイ機器のDtDeviceデータリスト</returns>
        IEnumerable<DtDevice> ReadDtDeviceOnlineGateway(DtDeliveryGroup groupData);
    }
}