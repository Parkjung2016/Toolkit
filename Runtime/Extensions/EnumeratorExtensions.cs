using System.Collections.Generic;

namespace PJH.Utility.Extensions
{
    public static class EnumeratorExtensions
    {
        /// <summary>
        /// IEnumerator를 IEnumerable로 변환합니다.
        /// </summary>
        /// <param name="e">IEnumerator 인스턴스입니다.</param>
        /// <returns>입력된 IEnumerator와 동일한 요소들을 가진 IEnumerable을 반환합니다.</returns>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> e)
        {
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }
    }
}