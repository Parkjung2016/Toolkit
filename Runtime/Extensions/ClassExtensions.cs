using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PJH.Utility.Extensions
{
    public static class ClassExtensions
    {
        /// <summary>
        /// 객체의 깊은 복사본을 생성합니다.
        /// 이 메서드는 객체가 직렬화 가능하고 ISerializable 인터페이스를 구현하지 않는 경우에만 작동합니다.
        /// </summary>
        /// <typeparam name="T">복사할 객체의 타입입니다. 이 타입은 클래스여야 합니다.</typeparam>
        /// <param name="obj">복사할 객체입니다.</param>
        /// <returns>복사된 객체의 깊은 복사본입니다. 직렬화가 불가능한 경우 null을 반환합니다.</returns>
        [System.Obsolete("이 메서드는 성능에 영향을 미칠 수 있으므로, 빈번한 호출이 필요한 경우에는 다른 복사 방법을 고려하는 것이 좋습니다.")]
        public static T DeepCopy<T>(this T obj) where T : class
        {
            if (typeof(T).IsSerializable == false
                || typeof(ISerializable).IsAssignableFrom(typeof(T)))
            {
                return null;
            }

            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}