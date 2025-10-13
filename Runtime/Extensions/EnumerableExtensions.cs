using System;
using System.Collections.Generic;

namespace PJH.Utility.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// IEnumerable의 각 요소에 접근합니다.
        /// </summary>
        /// <typeparam name="T">IEnumerable의 요소 타입입니다.</typeparam>
        /// <param name="sequence">순회할 IEnumerable입니다.</param>
        /// <param name="action">>각 요소를 인자로 받아 실행할 메서드 또는 람다식입니다.</param>
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var item in sequence)
            {
                action(item);
            }
        }


        /// <summary>
        /// IEnumerable에서 임의의 요소 하나를 반환합니다.
        /// </summary>
        /// <typeparam name="T">IEnumerable의 요소 타입입니다.</typeparam>
        /// <param name="sequence">무작위로 요소를 선택할 IEnumerable입니다.</param>
        /// <returns>IEnumerable에서 선택된 임의의 요소를 반환합니다.</returns>
        public static T Random<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            if (sequence is IList<T> list)
            {
                if (list.Count == 0)
                    throw new InvalidOperationException("Cannot get a random element from an empty collection.");
                return list[UnityEngine.Random.Range(0, list.Count)];
            }

            // 입력이 IList<T>가 아닌 경우(스트림이나 지연 평가(lazy) 시퀀스일 때는) 리저버 샘플링(reservoir sampling) 기법을 사용합니다.
            using var enumerator = sequence.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Cannot get a random element from an empty collection.");

            T result = enumerator.Current;
            int count = 1;
            while (enumerator.MoveNext())
            {
                if (UnityEngine.Random.Range(0, ++count) == 0)
                {
                    result = enumerator.Current;
                }
            }

            return result;
        }
    }
}