using Rms.Server.Core.Azure.Functions.WebApi.Dto;
using Rms.Server.Core.Utility;

namespace Rms.Server.Core.Azure.Functions.WebApi.Utility
{
    /// <summary>
    /// RequestValidator
    /// </summary>
    public static class RequestConverter
    {
        /// <summary>
        /// Dtoをファイル種別ごとに適切なクラスに変換する。
        /// </summary>
        /// <param name="source">変換元</param>
        /// <returns>変換先。リクエストがnullの場合にはそのままnullを返す</returns>
        public static DeliveryFileAddRequestDto Convert(DeliveryFileAddRequestDto source)
        {
            if (source == null)
            {
                return null;
            }

            switch (source.DeliveryFileType.DeliveryFileTypeCode)
            {
                case Const.DeliveryFileType.AlSoft:
                    return new DeliveryFileAddRequestTypeAlSoft(source);
                case Const.DeliveryFileType.HotFixConsole:
                case Const.DeliveryFileType.HotFixHobbit:
                    return new DeliveryFileAddRequestTypeHotFix(source);
                case Const.DeliveryFileType.Package:
                default:
                    return new DeliveryFileAddRequestTypePackage(source);
            }
        }

        /// <summary>
        /// Dtoをファイル種別ごとに適切なクラスに変換する。
        /// </summary>
        /// <param name="source">変換元</param>
        /// <returns>変換先。リクエストがnullの場合にはそのままnullを返す</returns>
        public static DeliveryFileUpdateRequestDto Convert(DeliveryFileUpdateRequestDto source)
        {
            if (source == null)
            {
                return null;
            }

            switch (source.DeliveryFileType.DeliveryFileTypeCode)
            {
                case Const.DeliveryFileType.AlSoft:
                    return new DeliveryFileUpdateRequestTypeAlSoft(source);
                case Const.DeliveryFileType.HotFixConsole:
                case Const.DeliveryFileType.HotFixHobbit:
                    return new DeliveryFileUpdateRequestTypeHotFix(source);
                case Const.DeliveryFileType.Package:
                default:
                    return new DeliveryFileUpdateRequestTypePackage(source);
            }
        }
    }
}