using UnityEngine;
using System.Collections.Generic;

namespace NoManual.Utils
{
    public class ErrorCode
    {
        public enum ErrorCodeEnum
        {
            None = -1,
            GetItem = 0,
            InventorySlotType = 1,
            DropItem = 2,
            NullComponent = 3,
            GetTextData = 4,
            GetCIP = 5,
            GetANO = 6,
            ANORatingType = 7,
            GetJumpScare = 8,
            GetANOIdleObject = 9,
            GetANOStickerObject = 10,
            ANOReplaceType = 11,
            ANOReplacePercent = 12,
            CombineData = 13,
            GetTimeLineAsset = 14,
            GetAudioClip = 15,
            GetJsonFile = 16,
            GetTaskInfo = 17,
            GetCSVFile = 18,
            GetSaveFile = 19,
            GetSaveFileSlot = 20,
            FailDeleteSaveFile = 21,
            FullFramingPos = 22,
        }

        private static Dictionary<ErrorCodeEnum, string> _ERROR_CODE = new Dictionary<ErrorCodeEnum, string>()
        {
            { ErrorCodeEnum.GetItem, "인벤토리에서 아이템 정보 얻기를 실패했습니다."},
            { ErrorCodeEnum.InventorySlotType, "존재하지 않는 인벤토리 공간입니다."},
            { ErrorCodeEnum.DropItem, "버릴 수 없는 아이템 입니다."},
            { ErrorCodeEnum.NullComponent, "존재하지 않는 컴포넌트 입니다."},
            { ErrorCodeEnum.GetTextData, "존재하지 않는 텍스트 데이터 입니다."},
            { ErrorCodeEnum.GetCIP, "존재하지 CIP UI 입니다."},
            { ErrorCodeEnum.GetANO , "ANO 정보 얻기를 실패했습니다."},
            { ErrorCodeEnum.ANORatingType , "ANO 위험도 판별에 실패했습니다."},
            { ErrorCodeEnum.GetJumpScare , "JumpScare 정보 얻기를 실패했습니다."},
            { ErrorCodeEnum.GetANOIdleObject , "ANO Idle Object 정보 얻기를 실패했습니다."},
            { ErrorCodeEnum.GetANOStickerObject, "스티커를 부착할 ANO Object를 찾기 못했습니다."},
            { ErrorCodeEnum.ANOReplaceType, "ANO 선별 기준을 측정할 수 없습니다."},
            { ErrorCodeEnum.ANOReplacePercent, "ANO 선별 총 확률이 100% 넘었습니다."},
            { ErrorCodeEnum.CombineData, "아이템 조합 데이터가 null 또는 CIP 슬롯이 선택되지 않은 상태입니다."},
            { ErrorCodeEnum.GetTimeLineAsset, "컷신 타임라인 정보를 찾을 수 없습니다."},
            { ErrorCodeEnum.GetAudioClip, "오디오 클립을 찾을 수 없습니다."},
            { ErrorCodeEnum.GetJsonFile, "Json 파일 읽기에 실패했습니다."},
            { ErrorCodeEnum.GetTaskInfo, "근무목록 리스트를 읽을 수 없습니다."},
            { ErrorCodeEnum.GetCSVFile, "CSV 파일 읽기에 실패했습니다." },
            { ErrorCodeEnum.GetSaveFile, "세이브 파일 읽기에 실패했습니다."},
            { ErrorCodeEnum.GetSaveFileSlot, "저장할 세이브 파일 슬롯이 없습니다."},
            { ErrorCodeEnum.FailDeleteSaveFile, "세이브 파일을 지우기 실패했습니다."},
            { ErrorCodeEnum.FullFramingPos, "배치할 파밍 위치 공간이 부족합니다."},
            
        };

        public static void SendError(ErrorCodeEnum errorCodeEnum)
        {
            if (_ERROR_CODE.ContainsKey(errorCodeEnum))
            {
                Debug.LogError(_ERROR_CODE[errorCodeEnum]);
                return;
            }

            Debug.LogError($"{errorCodeEnum} 확인할 수 없는 오류입니다.");
        }
        
        public static void SendError(string errorName, ErrorCodeEnum errorCodeEnum)
        {
            if (_ERROR_CODE.ContainsKey(errorCodeEnum))
            {
                Debug.LogError(errorName + _ERROR_CODE[errorCodeEnum]);
                return;
            }

            Debug.LogError($"{errorCodeEnum} 확인할 수 없는 오류입니다.");
        }
    }
}