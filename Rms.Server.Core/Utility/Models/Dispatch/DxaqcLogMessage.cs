using Newtonsoft.Json;
using Rms.Server.Core.Utility.Models.Entites;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rms.Server.Core.Utility.Models.Dispatch
{
    /// <summary>
    /// DXAQCログ
    /// </summary>
    public class DxaqcLogMessage : IConvertibleModel<DtDxaQcLog>
    {
        /// <summary>
        /// 発生元機器UID
        /// </summary>
        [Required]
        [JsonProperty(nameof(SourceEquipmentUID))]
        public string SourceEquipmentUID { get; set; }

        /// <summary>
        /// 収集日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(CollectDT))]
        public DateTime? CollectDT { get; set; }

        /// <summary>
        /// 検査インスタンスUID
        /// </summary>
        [Required]
        [JsonProperty(nameof(StudyInstanceUID))]
        public string StudyInstanceUID { get; set; }

        /// <summary>
        /// 画像インスタンスUID
        /// </summary>
        [Required]
        [JsonProperty(nameof(SOPInstanceUID))]
        public string SOPInstanceUID { get; set; }

        /// <summary>
        /// 検査日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(StudyDT))]
        public DateTime? StudyDT { get; set; }

        /// <summary>
        /// 計測日時
        /// </summary>
        [Required]
        [JsonProperty(nameof(MeasureDT))]
        public DateTime? MeasureDT { get; set; }

        /// <summary>
        /// パネルのシリアルID
        /// </summary>
        [Required]
        [JsonProperty(nameof(PanelSerialID))]
        public string PanelSerialID { get; set; }

        /// <summary>
        /// 術式
        /// </summary>
        [Required]
        [JsonProperty(nameof(TechniqueCode))]
        public byte? TechniqueCode { get; set; }

        /// <summary>
        /// 計測名称
        /// </summary>
        [Required]
        [JsonProperty(nameof(TypeName))]
        public byte? TypeName { get; set; }

        /// <summary>
        /// 合否判定結果
        /// </summary>
        [JsonProperty(nameof(QcDetailresultPhantomtTest))]
        public long? QcDetailresultPhantomtTest { get; set; }

        /// <summary>
        /// DXA-QCファントムAのBMC
        /// </summary>
        [Required]
        [JsonProperty(nameof(BmcDxaAPhantomttTest))]
        public double? BmcDxaAPhantomttTest { get; set; }

        /// <summary>
        /// DXA-QCファントムBのBMC
        /// </summary>
        [Required]
        [JsonProperty(nameof(BmcDxaBPhantomttTest))]
        public double? BmcDxaBPhantomttTest { get; set; }

        /// <summary>
        /// DXA-QCファントムCのBMC
        /// </summary>
        [Required]
        [JsonProperty(nameof(BmcDxaCPhantomttTest))]
        public double? BmcDxaCPhantomttTest { get; set; }

        /// <summary>
        /// DXA-QCファントムAの面積
        /// </summary>
        [Required]
        [JsonProperty(nameof(AreaDxaAPhantomtTest))]
        public double? AreaDxaAPhantomtTest { get; set; }

        /// <summary>
        /// DXA-QCファントムBの面積
        /// </summary>
        [Required]
        [JsonProperty(nameof(AreaDxaBPhantomtTest))]
        public double? AreaDxaBPhantomtTest { get; set; }

        /// <summary>
        /// DXA-QCファントムCの面積
        /// </summary>
        [Required]
        [JsonProperty(nameof(AreaDxaCPhantomtTest))]
        public double? AreaDxaCPhantomtTest { get; set; }

        /// <summary>
        /// DXA-QCファントムAのBMD
        /// </summary>
        [Required]
        [JsonProperty(nameof(BmdDxaAPhantomtTest))]
        public double? BmdDxaAPhantomtTest { get; set; }

        /// <summary>
        /// DXA-QCファントムBのBMD
        /// </summary>
        [Required]
        [JsonProperty(nameof(BmdDxaBPhantomtTest))]
        public double? BmdDxaBPhantomtTest { get; set; }

        /// <summary>
        /// DXA-QCファントムCのBMD
        /// </summary>
        [Required]
        [JsonProperty(nameof(BmdDxaCPhantomtTest))]
        public double? BmdDxaCPhantomtTest { get; set; }

        /// <summary>
        /// BMD直線性
        /// </summary>
        [Required]
        [JsonProperty(nameof(BmdLinearityPhantomtTest))]
        public double? BmdLinearityPhantomtTest { get; set; }

        /// <summary>
        /// CsI骨部AのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlCsiBoneAPhantomtTest))]
        public double? QlCsiBoneAPhantomtTest { get; set; }

        /// <summary>
        /// CsI骨部BのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlCsiBoneBPhantomtTest))]
        public double? QlCsiBoneBPhantomtTest { get; set; }

        /// <summary>
        /// CsI骨部CのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlCsiBoneCPhantomtTest))]
        public double? QlCsiBoneCPhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部左AのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlCsiSoftLAPhantomtTest))]
        public double? QlCsiSoftLAPhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部左BのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlCsiSoftLBPhantomtTest))]
        public double? QlCsiSoftLBPhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部左CのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlCsiSoftLCPhantomtTest))]
        public double? QlCsiSoftLCPhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部右AのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlCsiSoftRAPhantomtTest))]
        public double? QlCsiSoftRAPhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部右BのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlCsiSoftRBPhantomtTest))]
        public double? QlCsiSoftRBPhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部右CのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlCsiSoftRCPhantomtTest))]
        public double? QlCsiSoftRCPhantomtTest { get; set; }

        /// <summary>
        /// GOS骨部AのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlGosBoneAPhantomtTest))]
        public double? QlGosBoneAPhantomtTest { get; set; }

        /// <summary>
        /// GOS骨部BのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlGosBoneBPhantomtTest))]
        public double? QlGosBoneBPhantomtTest { get; set; }

        /// <summary>
        /// GOS骨部CのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlGosBoneCPhantomtTest))]
        public double? QlGosBoneCPhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部左AのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlGosSoftLAPhantomtTest))]
        public double? QlGosSoftLAPhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部左BのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlGosSoftLBPhantomtTest))]
        public double? QlGosSoftLBPhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部左のQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlGosSoftLCPhantomtTest))]
        public double? QlGosSoftLCPhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部右AのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlGosSoftRAPhantomtTest))]
        public double? QlGosSoftRAPhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部右BのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlGosSoftRBPhantomtTest))]
        public double? QlGosSoftRBPhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部右CのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlGosSoftRCPhantomtTest))]
        public double? QlGosSoftRCPhantomtTest { get; set; }

        /// <summary>
        /// DXA骨部AのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlDxaBoneAPhantomtTest))]
        public double? QlDxaBoneAPhantomtTest { get; set; }

        /// <summary>
        /// DXA骨部BのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlDxaBoneBPhantomtTest))]
        public double? QlDxaBoneBPhantomtTest { get; set; }

        /// <summary>
        /// DXA骨部CのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlDxaBoneCPhantomtTest))]
        public double? QlDxaBoneCPhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部左AのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlDxaSoftLAPhantomtTest))]
        public double? QlDxaSoftLAPhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部左BのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlDxaSoftLBPhantomtTest))]
        public double? QlDxaSoftLBPhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部左CのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlDxaSoftLCPhantomtTest))]
        public double? QlDxaSoftLCPhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部右AのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlDxaSoftRAPhantomtTest))]
        public double? QlDxaSoftRAPhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部右BのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlDxaSoftRBPhantomtTest))]
        public double? QlDxaSoftRBPhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部右CのQL値
        /// </summary>
        [Required]
        [JsonProperty(nameof(QlDxaSoftRCPhantomtTest))]
        public double? QlDxaSoftRCPhantomtTest { get; set; }

        /// <summary>
        /// CsI骨部AのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdCsiBoneAPhantomtTest))]
        public double? StdCsiBoneAPhantomtTest { get; set; }

        /// <summary>
        /// CsI骨部BのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdCsiBoneBPhantomtTest))]
        public double? StdCsiBoneBPhantomtTest { get; set; }

        /// <summary>
        /// CsI骨部CのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdCsiBoneCPhantomtTest))]
        public double? StdCsiBoneCPhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部左AのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdCsiSoftLAPhantomtTest))]
        public double? StdCsiSoftLAPhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部左BのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdCsiSoftLBPhantomtTest))]
        public double? StdCsiSoftLBPhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部左CのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdCsiSoftLCPhantomtTest))]
        public double? StdCsiSoftLCPhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部右AのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdCsiSoftRAPhantomtTest))]
        public double? StdCsiSoftRAPhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部右BのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdCsiSoftRBPhantomtTest))]
        public double? StdCsiSoftRBPhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部右CのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdCsiSoftRCPhantomtTest))]
        public double? StdCsiSoftRCPhantomtTest { get; set; }

        /// <summary>
        /// GOS骨部AのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdGosBoneAPhantomtTest))]
        public double? StdGosBoneAPhantomtTest { get; set; }

        /// <summary>
        /// GOS骨部BのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdGosBoneBPhantomtTest))]
        public double? StdGosBoneBPhantomtTest { get; set; }

        /// <summary>
        /// GOS骨部CのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdGosBoneCPhantomtTest))]
        public double? StdGosBoneCPhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部左AのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdGosSoftLAPhantomtTest))]
        public double? StdGosSoftLAPhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部左BのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdGosSoftLBPhantomtTest))]
        public double? StdGosSoftLBPhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部左CのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdGosSoftLCPhantomtTest))]
        public double? StdGosSoftLCPhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部右AのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdGosSoftRAPhantomtTest))]
        public double? StdGosSoftRAPhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部右BのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdGosSoftRBPhantomtTest))]
        public double? StdGosSoftRBPhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部右CのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdGosSoftRCPhantomtTest))]
        public double? StdGosSoftRCPhantomtTest { get; set; }

        /// <summary>
        /// DXA骨部AのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdDxaBoneAPhantomtTest))]
        public double? StdDxaBoneAPhantomtTest { get; set; }

        /// <summary>
        /// DXA骨部BのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdDxaBoneBPhantomtTest))]
        public double? StdDxaBoneBPhantomtTest { get; set; }

        /// <summary>
        /// DXA骨部CのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdDxaBoneCPhantomtTest))]
        public double? StdDxaBoneCPhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部左AのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdDxaSoftLAPhantomtTest))]
        public double? StdDxaSoftLAPhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部左BのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdDxaSoftLBPhantomtTest))]
        public double? StdDxaSoftLBPhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部左CのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdDxaSoftLCPhantomtTest))]
        public double? StdDxaSoftLCPhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部右AのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdDxaSoftRAPhantomtTest))]
        public double? StdDxaSoftRAPhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部右BのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdDxaSoftRBPhantomtTest))]
        public double? StdDxaSoftRBPhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部右CのQL値の標準偏差
        /// </summary>
        [Required]
        [JsonProperty(nameof(StdDxaSoftRCPhantomtTest))]
        public double? StdDxaSoftRCPhantomtTest { get; set; }

        /// <summary>
        /// DXA-QCファントムAのBMD（基礎値）
        /// </summary>
        [JsonProperty(nameof(BmdDxaABasicvaluePhantomtTest))]
        public double? BmdDxaABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA-QCファントムBのBMD（基礎値）
        /// </summary>
        [JsonProperty(nameof(BmdDxaBBasicvaluePhantomtTest))]
        public double? BmdDxaBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA-QCファントムCのBMD（基礎値）
        /// </summary>
        [JsonProperty(nameof(BmdDxaCBasicvaluePhantomtTest))]
        public double? BmdDxaCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI骨部AのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlCsiBoneABasicvaluePhantomtTest))]
        public double? QlCsiBoneABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI骨部BのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlCsiBoneBBasicvaluePhantomtTest))]
        public double? QlCsiBoneBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI骨部CのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlCsiBoneCBasicvaluePhantomtTest))]
        public double? QlCsiBoneCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部左AのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlCsiSoftLABasicvaluePhantomtTest))]
        public double? QlCsiSoftLABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部左BのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlCsiSoftLBBasicvaluePhantomtTest))]
        public double? QlCsiSoftLBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部左CのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlCsiSoftLCBasicvaluePhantomtTest))]
        public double? QlCsiSoftLCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部右AのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlCsiSoftRABasicvaluePhantomtTest))]
        public double? QlCsiSoftRABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部右BのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlCsiSoftRBBasicvaluePhantomtTest))]
        public double? QlCsiSoftRBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部右CのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlCsiSoftRCBasicvaluePhantomtTest))]
        public double? QlCsiSoftRCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS骨部AのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlGosBoneABasicvaluePhantomtTest))]
        public double? QlGosBoneABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS骨部BのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlGosBoneBBasicvaluePhantomtTest))]
        public double? QlGosBoneBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS骨部CのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlGosBoneCBasicvaluePhantomtTest))]
        public double? QlGosBoneCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部左AのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlGosSoftLABasicvaluePhantomtTest))]
        public double? QlGosSoftLABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部左BのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlGosSoftLBBasicvaluePhantomtTest))]
        public double? QlGosSoftLBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部左CのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlGosSoftLCBasicvaluePhantomtTest))]
        public double? QlGosSoftLCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部右AのQL値（基礎値)
        /// </summary>
        [JsonProperty(nameof(QlGosSoftRABasicvaluePhantomtTest))]
        public double? QlGosSoftRABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部右BのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlGosSoftRBBasicvaluePhantomtTest))]
        public double? QlGosSoftRBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部右CのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlGosSoftRCBasicvaluePhantomtTest))]
        public double? QlGosSoftRCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA骨部AのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlDxaBoneABasicvaluePhantomtTest))]
        public double? QlDxaBoneABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA骨部BのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlDxaBoneBBasicvaluePhantomtTest))]
        public double? QlDxaBoneBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA骨部CのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlDxaBoneCBasicvaluePhantomtTest))]
        public double? QlDxaBoneCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部左AのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlDxaSoftLABasicvaluePhantomtTest))]
        public double? QlDxaSoftLABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部左BのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlDxaSoftLBBasicvaluePhantomtTest))]
        public double? QlDxaSoftLBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部左CのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlDxaSoftLCBasicvaluePhantomtTest))]
        public double? QlDxaSoftLCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部右AのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlDxaSoftRABasicvaluePhantomtTest))]
        public double? QlDxaSoftRABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部右BのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlDxaSoftRBBasicvaluePhantomtTest))]
        public double? QlDxaSoftRBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部右CのQL値（基礎値）
        /// </summary>
        [JsonProperty(nameof(QlDxaSoftRCBasicvaluePhantomtTest))]
        public double? QlDxaSoftRCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI骨部AのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdCsiBoneABasicvaluePhantomtTest))]
        public double? StdCsiBoneABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI骨部BのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdCsiBoneBBasicvaluePhantomtTest))]
        public double? StdCsiBoneBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI骨部CのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdCsiBoneCBasicvaluePhantomtTest))]
        public double? StdCsiBoneCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部左AのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdCsiSoftLABasicvaluePhantomtTest))]
        public double? StdCsiSoftLABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部左BのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdCsiSoftLBBasicvaluePhantomtTest))]
        public double? StdCsiSoftLBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部左CのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdCsiSoftLCBasicvaluePhantomtTest))]
        public double? StdCsiSoftLCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部右AのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdCsiSoftRABasicvaluePhantomtTest))]
        public double? StdCsiSoftRABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部右BのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdCsiSoftRBBasicvaluePhantomtTest))]
        public double? StdCsiSoftRBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// CsI軟部右CのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdCsiSoftRCBasicvaluePhantomtTest))]
        public double? StdCsiSoftRCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS骨部AのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdGosBoneABasicvaluePhantomtTest))]
        public double? StdGosBoneABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS骨部BのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdGosBoneBBasicvaluePhantomtTest))]
        public double? StdGosBoneBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS骨部CのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdGosBoneCBasicvaluePhantomtTest))]
        public double? StdGosBoneCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部左AのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdGosSoftLABasicvaluePhantomtTest))]
        public double? StdGosSoftLABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部左BのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdGosSoftLBBasicvaluePhantomtTest))]
        public double? StdGosSoftLBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部左CのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdGosSoftLCBasicvaluePhantomtTest))]
        public double? StdGosSoftLCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部右AのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdGosSoftRABasicvaluePhantomtTest))]
        public double? StdGosSoftRABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部右BのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdGosSoftRBBasicvaluePhantomtTest))]
        public double? StdGosSoftRBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// GOS軟部右CのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdGosSoftRCBasicvaluePhantomtTest))]
        public double? StdGosSoftRCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA骨部AのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdDxaBoneABasicvaluePhantomtTest))]
        public double? StdDxaBoneABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA骨部BのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdDxaBoneBBasicvaluePhantomtTest))]
        public double? StdDxaBoneBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA骨部CのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdDxaBoneCBasicvaluePhantomtTest))]
        public double? StdDxaBoneCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部左AのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdDxaSoftLABasicvaluePhantomtTest))]
        public double? StdDxaSoftLABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部左BのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdDxaSoftLBBasicvaluePhantomtTest))]
        public double? StdDxaSoftLBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部左CのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdDxaSoftLCBasicvaluePhantomtTest))]
        public double? StdDxaSoftLCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部右AのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdDxaSoftRABasicvaluePhantomtTest))]
        public double? StdDxaSoftRABasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部右BのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdDxaSoftRBBasicvaluePhantomtTest))]
        public double? StdDxaSoftRBBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// DXA軟部右CのQL値の標準偏差（基礎値）
        /// </summary>
        [JsonProperty(nameof(StdDxaSoftRCBasicvaluePhantomtTest))]
        public double? StdDxaSoftRCBasicvaluePhantomtTest { get; set; }

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="deviceId">デバイスSID</param>
        /// <param name="eventData">イベント情報</param>
        /// <returns>DtDxaQcLog</returns>
        public DtDxaQcLog Convert(long deviceId, RmsEvent eventData)
        {
            return new DtDxaQcLog()
            {
                //// Sid
                DeviceSid = deviceId,
                SourceEquipmentUid = SourceEquipmentUID,
                StudyInstanceUid = StudyInstanceUID,
                SopInstanceUid = SOPInstanceUID,
                StudyDatetime = StudyDT,
                MeasureDatetime = MeasureDT,
                PanelSerialId = PanelSerialID,
                TechniqueCode = TechniqueCode,
                TypeName = TypeName,
                QcResult = QcDetailresultPhantomtTest,
                BmcDxaATest = BmcDxaAPhantomttTest,
                BmcDxaBTest = BmcDxaBPhantomttTest,
                BmcDxaCTest = BmcDxaCPhantomttTest,
                AreaDxaATest = AreaDxaAPhantomtTest,
                AreaDxaBTest = AreaDxaBPhantomtTest,
                AreaDxaCTest = AreaDxaCPhantomtTest,
                BmdDxaATest = BmdDxaAPhantomtTest,
                BmdDxaBTest = BmdDxaBPhantomtTest,
                BmdDxaCTest = BmdDxaCPhantomtTest,
                BmdLinearityTest = BmdLinearityPhantomtTest,
                QlCsiBoneATest = QlCsiBoneAPhantomtTest,
                QlCsiBoneBTest = QlCsiBoneBPhantomtTest,
                QlCsiBoneCTest = QlCsiBoneCPhantomtTest,
                QlCsiSoftLaTest = QlCsiSoftLAPhantomtTest,
                QlCsiSoftLbTest = QlCsiSoftLBPhantomtTest,
                QlCsiSoftLcTest = QlCsiSoftLCPhantomtTest,
                QlCsiSoftRaTest = QlCsiSoftRAPhantomtTest,
                QlCsiSoftRbTest = QlCsiSoftRBPhantomtTest,
                QlCsiSoftRcTest = QlCsiSoftRCPhantomtTest,
                QlGosBoneATest = QlGosBoneAPhantomtTest,
                QlGosBoneBTest = QlGosBoneBPhantomtTest,
                QlGosBoneCTest = QlGosBoneCPhantomtTest,
                QlGosSoftLaTest = QlGosSoftLAPhantomtTest,
                QlGosSoftLbTest = QlGosSoftLBPhantomtTest,
                QlGosSoftLcTest = QlGosSoftLCPhantomtTest,
                QlGosSoftRaTest = QlGosSoftRAPhantomtTest,
                QlGosSoftRbTest = QlGosSoftRBPhantomtTest,
                QlGosSoftRcTest = QlGosSoftRCPhantomtTest,
                QlDxaBoneATest = QlDxaBoneAPhantomtTest,
                QlDxaBoneBTest = QlDxaBoneBPhantomtTest,
                QlDxaBoneCTest = QlDxaBoneCPhantomtTest,
                QlDxaSoftLaTest = QlDxaSoftLAPhantomtTest,
                QlDxaSoftLbTest = QlDxaSoftLBPhantomtTest,
                QlDxaSoftLcTest = QlDxaSoftLCPhantomtTest,
                QlDxaSoftRaTest = QlDxaSoftRAPhantomtTest,
                QlDxaSoftRbTest = QlDxaSoftRBPhantomtTest,
                QlDxaSoftRcTest = QlDxaSoftRCPhantomtTest,
                StdCsiBoneATest = StdCsiBoneAPhantomtTest,
                StdCsiBoneBTest = StdCsiBoneBPhantomtTest,
                StdCsiBoneCTest = StdCsiBoneCPhantomtTest,
                StdCsiSoftLaTest = StdCsiSoftLAPhantomtTest,
                StdCsiSoftLbTest = StdCsiSoftLBPhantomtTest,
                StdCsiSoftLcTest = StdCsiSoftLCPhantomtTest,
                StdCsiSoftRaTest = StdCsiSoftRAPhantomtTest,
                StdCsiSoftRbTest = StdCsiSoftRBPhantomtTest,
                StdCsiSoftRcTest = StdCsiSoftRCPhantomtTest,
                StdGosBoneATest = StdGosBoneAPhantomtTest,
                StdGosBoneBTest = StdGosBoneBPhantomtTest,
                StdGosBoneCTest = StdGosBoneCPhantomtTest,
                StdGosSoftLaTest = StdGosSoftLAPhantomtTest,
                StdGosSoftLbTest = StdGosSoftLBPhantomtTest,
                StdGosSoftLcTest = StdGosSoftLCPhantomtTest,
                StdGosSoftRaTest = StdGosSoftRAPhantomtTest,
                StdGosSoftRbTest = StdGosSoftRBPhantomtTest,
                StdGosSoftRcTest = StdGosSoftRCPhantomtTest,
                StdDxaBoneATest = StdDxaBoneAPhantomtTest,
                StdDxaBoneBTest = StdDxaBoneBPhantomtTest,
                StdDxaBoneCTest = StdDxaBoneCPhantomtTest,
                StdDxaSoftLaTest = StdDxaSoftLAPhantomtTest,
                StdDxaSoftLbTest = StdDxaSoftLBPhantomtTest,
                StdDxaSoftLcTest = StdDxaSoftLCPhantomtTest,
                StdDxaSoftRaTest = StdDxaSoftRAPhantomtTest,
                StdDxaSoftRbTest = StdDxaSoftRBPhantomtTest,
                StdDxaSoftRcTest = StdDxaSoftRCPhantomtTest,
                BmdDxaABasvalTest = BmdDxaABasicvaluePhantomtTest,
                BmdDxaBBasvalTest = BmdDxaBBasicvaluePhantomtTest,
                BmdDxaCBasvalTest = BmdDxaCBasicvaluePhantomtTest,
                QlCsiBoneABasvalTest = QlCsiBoneABasicvaluePhantomtTest,
                QlCsiBoneBBasvalTest = QlCsiBoneBBasicvaluePhantomtTest,
                QlCsiBoneCBasvalTest = QlCsiBoneCBasicvaluePhantomtTest,
                QlCsiSoftLaBasvalTest = QlCsiSoftLABasicvaluePhantomtTest,
                QlCsiSoftLbBasvalTest = QlCsiSoftLBBasicvaluePhantomtTest,
                QlCsiSoftLcBasvalTest = QlCsiSoftLCBasicvaluePhantomtTest,
                QlCsiSoftRaBasvalTest = QlCsiSoftRABasicvaluePhantomtTest,
                QlCsiSoftRbBasvalTest = QlCsiSoftRBBasicvaluePhantomtTest,
                QlCsiSoftRcBasvalTest = QlCsiSoftRCBasicvaluePhantomtTest,
                QlGosBoneABasvalTest = QlGosBoneABasicvaluePhantomtTest,
                QlGosBoneBBasvalTest = QlGosBoneBBasicvaluePhantomtTest,
                QlGosBoneCBasvalTest = QlGosBoneCBasicvaluePhantomtTest,
                QlGosSoftLaBasvalTest = QlGosSoftLABasicvaluePhantomtTest,
                QlGosSoftLbBasvalTest = QlGosSoftLBBasicvaluePhantomtTest,
                QlGosSoftLcBasvalTest = QlGosSoftLCBasicvaluePhantomtTest,
                QlGosSoftRaBasvalTest = QlGosSoftRABasicvaluePhantomtTest,
                QlGosSoftRbBasvalTest = QlGosSoftRBBasicvaluePhantomtTest,
                QlGosSoftRcBasvalTest = QlGosSoftRCBasicvaluePhantomtTest,
                QlDxaBoneABasvalTest = QlDxaBoneABasicvaluePhantomtTest,
                QlDxaBoneBBasvalTest = QlDxaBoneBBasicvaluePhantomtTest,
                QlDxaBoneCBasvalTest = QlDxaBoneCBasicvaluePhantomtTest,
                QlDxaSoftLaBasvalTest = QlDxaSoftLABasicvaluePhantomtTest,
                QlDxaSoftLbBasvalTest = QlDxaSoftLBBasicvaluePhantomtTest,
                QlDxaSoftLcBasvalTest = QlDxaSoftLCBasicvaluePhantomtTest,
                QlDxaSoftRaBasvalTest = QlDxaSoftRABasicvaluePhantomtTest,
                QlDxaSoftRbBasvalTest = QlDxaSoftRBBasicvaluePhantomtTest,
                QlDxaSoftRcBasvalTest = QlDxaSoftRCBasicvaluePhantomtTest,
                StdCsiBoneABasvalTest = StdCsiBoneABasicvaluePhantomtTest,
                StdCsiBoneBBasvalTest = StdCsiBoneBBasicvaluePhantomtTest,
                StdCsiBoneCBasvalTest = StdCsiBoneCBasicvaluePhantomtTest,
                StdCsiSoftLaBasvalTest = StdCsiSoftLABasicvaluePhantomtTest,
                StdCsiSoftLbBasvalTest = StdCsiSoftLBBasicvaluePhantomtTest,
                StdCsiSoftLcBasvalTest = StdCsiSoftLCBasicvaluePhantomtTest,
                StdCsiSoftRaBasvalTest = StdCsiSoftRABasicvaluePhantomtTest,
                StdCsiSoftRbBasvalTest = StdCsiSoftRBBasicvaluePhantomtTest,
                StdCsiSoftRcBasvalTest = StdCsiSoftRCBasicvaluePhantomtTest,
                StdGosBoneABasvalTest = StdGosBoneABasicvaluePhantomtTest,
                StdGosBoneBBasvalTest = StdGosBoneBBasicvaluePhantomtTest,
                StdGosBoneCBasvalTest = StdGosBoneCBasicvaluePhantomtTest,
                StdGosSoftLaBasvalTest = StdGosSoftLABasicvaluePhantomtTest,
                StdGosSoftLbBasvalTest = StdGosSoftLBBasicvaluePhantomtTest,
                StdGosSoftLcBasvalTest = StdGosSoftLCBasicvaluePhantomtTest,
                StdGosSoftRaBasvalTest = StdGosSoftRABasicvaluePhantomtTest,
                StdGosSoftRbBasvalTest = StdGosSoftRBBasicvaluePhantomtTest,
                StdGosSoftRcBasvalTest = StdGosSoftRCBasicvaluePhantomtTest,
                StdDxaBoneABasvalTest = StdDxaBoneABasicvaluePhantomtTest,
                StdDxaBoneBBasvalTest = StdDxaBoneBBasicvaluePhantomtTest,
                StdDxaBoneCBasvalTest = StdDxaBoneCBasicvaluePhantomtTest,
                StdDxaSoftLaBasvalTest = StdDxaSoftLABasicvaluePhantomtTest,
                StdDxaSoftLbBasvalTest = StdDxaSoftLBBasicvaluePhantomtTest,
                StdDxaSoftLcBasvalTest = StdDxaSoftLCBasicvaluePhantomtTest,
                StdDxaSoftRaBasvalTest = StdDxaSoftRABasicvaluePhantomtTest,
                StdDxaSoftRbBasvalTest = StdDxaSoftRBBasicvaluePhantomtTest,
                StdDxaSoftRcBasvalTest = StdDxaSoftRCBasicvaluePhantomtTest,
                CollectDatetime = CollectDT,
                MessageId = eventData?.MessageId
                //// CreateDatetime
                //// DtDevice
            };
        }
    }
}