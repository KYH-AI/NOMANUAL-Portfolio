using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.Utils
{
    public class ObjectPooling
    {
        #region 풀 초기화

        public static List<T> InitializeListPool<T>(int poolSize) where T : new()
        {
            List<T> newPool = new List<T>(poolSize);
            return newPool;
        }
        
        public static T[] InitializeArrayPool<T>(int poolSize) where T : new()
        {
            T[] newPool = new T[poolSize];
            return newPool;
        }
        
        public static Queue<T> InitializeQueuePool<T>(int poolSize) where T : new()
        {
            Queue<T> newPool = new Queue<T>(poolSize);
            return newPool;
        }

        #endregion

        #region 객체 생성

        public static T CreatePrefab<T>(T prefab) where T : Component
        {
            GameObject item = GameObject.Instantiate(prefab.gameObject);
            T itemComponent = item.GetComponent<T>();
            return itemComponent;
        } 
        
        public static T CreateInstance<T>(T item) where T : new()
        {
            T newItem = new T();
            return newItem;
        } 

        #endregion

        #region 풀 저장

        public static void AddToListPool<T>(List<T> poolList, T item)
        {
            poolList.Add(item);
        }
        
        public static void AddToArrayPool<T>(T[] poolArray, T item, int index)
        {
            if (index >= 0 && index < poolArray.Length)
            {
                poolArray[index] = item;
            }
        }
        
        public static void AddToQueuePool<T>(Queue<T> poolQueue, T item)
        {
            poolQueue.Enqueue(item);
        }

        #endregion
        
        #region 풀 가져오기

        public static T GetFromListPool<T>(List<T> poolList)
        {
            if (poolList.Count > 0)
            {
                T item = poolList[poolList.Count - 1];
                poolList.RemoveAt(poolList.Count - 1);
                return item;
            }
            return default(T);
        }
        
        public static T GetFromArrayPool<T>(T[] poolArray, int index)
        {
            if (poolArray.Length > 0)
            {
                foreach (T getItem in poolArray)
                {
                 //   if (!getItem.Equals(default(T)))
                    if(!EqualityComparer<T>.Default.Equals(getItem, default(T)))
                    {
                        return getItem;
                    }   
                }
                
                /*
                T item = poolArray[index];
                // 가져온 아이템 공간은 null로 설정
                poolArray[index] = default(T);
                return item;
                */
            }
            return default(T);
        }
        
        public static T GetFromQueuePool<T>(Queue<T> poolQueue)
        {
            if (poolQueue.Count > 0)
            {
                return poolQueue.Dequeue();
            }
            return default(T);
        }
        
        
        #endregion

        #region 풀 반환하기

        public static void ReturnToListPool<T>(List<T> poolList, T returnItem)
        {
            poolList.Add(returnItem);
        }
        
        public static void ReturnToArrayPool<T>(ref T[] poolArray, T returnItem)
        {
            // 배열이 가득 찬 경우
            if (IsArrayFull(poolArray))
            {
                // 현재 배열 크기의 두 배로 새로운 배열 생성
                int newSize = poolArray.Length * 2;
                T[] newArray = new T[newSize];
                // 기존 배열의 내용을 새로운 배열로 복사
                Array.Copy(poolArray, newArray, poolArray.Length);
                // 새로운 아이템 추가
                newArray[poolArray.Length] = returnItem;
                // 새로운 배열을 기존 배열로 대체
                poolArray = newArray;
            }
            else
            {
                for (int i = 0; i < poolArray.Length; i++)
                {
                    if (EqualityComparer<T>.Default.Equals(poolArray[i], default(T)))
                    {
                        poolArray[i] = returnItem;
                        return; // 아이템이 배열에 추가되면 메서드 종료
                    }
                }
            }
        }
        
        /// <summary>
        /// 배열이 가득 찼는지 확인하는 메서드
        /// </summary>
        private static bool IsArrayFull<T>(T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(array[i], default(T)))
                {
                    return false; // 배열에 비어있는 요소가 있으면 가득 차지 않음
                }
            }
            return true; // 배열의 모든 요소가 채워져 있으면 가득 참
        }
        
        public static void ReturnToQueuePool<T>(Queue<T> poolQueue, T returnItem)
        {
            poolQueue.Enqueue(returnItem);
        }

        #endregion


    }
}
