using System;
using System.Collections.Generic;

namespace NoManual.Utils
{
    public class NoManualUtilsHelper
    {
        // 열거형 값을 문자열로 변환하는 메서드
        public static string EnumToString<T>(T enumValue)
        {
            return enumValue.ToString();
        }

        // 문자열을 열거형으로 변환하는 메서드
        public static T StringToEnum<T>(string enumString) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), enumString, true);
        }

        // 열거형을 기본 데이터 타입으로 변환하는 메서드
        public static T EnumToGeneric<T>(Enum enumValue) where T : struct, IConvertible
        {
            // T가 열거형이 아니거나 기본 변수형이 아닌 경우 예외 처리
            if (!typeof(T).IsEnum && !typeof(T).IsPrimitive)
            {
                throw new ArgumentException("EnumToGeneric 메서드 오류");
            }

            // 열거형 값을 기본 변수형으로 변환
            T result = (T)Convert.ChangeType(enumValue, typeof(T));
            return result;
        }

        /// <summary>
        /// 중복 찾기
        /// </summary>
        /// <param name="targetValue">비교할 데이터</param>
        /// <param name="comparisonTarget">원본 데이터</param>
        public static bool FindDuplicate<T>(T targetValue, T[] comparisonTargets)
        {
            foreach (T value in comparisonTargets)
            {
                if (value.Equals(targetValue))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 문자열 공백 또는 null 값 확인 (true : null)
        /// </summary>
        /// <param name="targetString">확인할 문자열</param>
        public static bool FindStringEmptyOrNull(string targetString)
        {
           return string.IsNullOrEmpty(targetString);
        }
        
        /// <summary>
        /// 리스트에서 중복 없이 랜덤으로 T 타입의 항목들을 선택해서 반환
        /// </summary>
        /// <typeparam name="T">리스트의 항목 타입</typeparam>
        /// <param name="source">선택할 항목들이 포함된 리스트</param>
        /// <param name="count">선택할 항목의 수</param>
        /// <returns>랜덤으로 선택된 항목들의 리스트</returns>
        public static List<T> SelectRandomItems<T>(List<T> source, int count)
        {
            // 리스트의 복사본 생성 (원본 리스트를 변경하지 않기 위해)
            List<T> tempList = new List<T>(source);

            // 결과를 저장할 리스트
            List<T> selectedItems = new List<T>();

            // 선택할 항목 수가 원본 리스트의 항목 수보다 큰 경우 예외 처리
            if (count > tempList.Count) return null;

            // 랜덤하게 항목 선택
            for (int i = 0; i < count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, tempList.Count);
                selectedItems.Add(tempList[randomIndex]);
                tempList.RemoveAt(randomIndex); // 중복을 방지하기 위해 선택된 항목을 제거
            }

            return selectedItems;
        }

        /// <summary>
        /// 유니티 에디터 Log 출력
        /// </summary>
        /// <param name="logTextColor">Text Color</param>
        /// <param name="log">출력할 Text</param>
        public static void EditorDebugLog(LogTextColor logTextColor, string log)
        {
            #if UNITY_EDITOR
                    string colorCode = logTextColor switch
                    {
                        LogTextColor.cyan => "cyan",
                        LogTextColor.yellow => "yellow",
                        LogTextColor.red => "red",
                        LogTextColor.green => "green",
                        _ => "white" // 기본값 설정
                    };
                    UnityEngine.Debug.Log($"<color={colorCode}>{log}</color>");
            #endif
        }

        public enum LogTextColor
        {
            cyan = 0,
            yellow = 1,
            red = 2,
            green = 3,
        }
    }
}