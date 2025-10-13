using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PJH.Utility.Extensions
{
    public static class ReflectionExtensions
    {
        static readonly Dictionary<Type, string> TypeDisplayNames = new Dictionary<Type, string>
        {
            { typeof(int), "int" },
            { typeof(float), "float" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(string), "string" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(uint), "uint" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(char), "char" },
            { typeof(object), "object" }
        };

        static readonly Type[] ValueTupleTypes =
        {
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>)
        };

        static readonly Type[][] PrimitiveTypeCastHierarchy =
        {
            new[] { typeof(byte), typeof(sbyte), typeof(char) },
            new[] { typeof(short), typeof(ushort) },
            new[] { typeof(int), typeof(uint) },
            new[] { typeof(long), typeof(ulong) },
            new[] { typeof(float) },
            new[] { typeof(double) }
        };

        /// <summary>
        /// 지정된 타입의 기본값을 반환합니다.
        /// </summary>
        /// <param name="type">기본값을 가져올 타입입니다.</param>
        /// <returns>값 형식이면 해당 타입의 인스턴스, 참조 형식이면 null을 반환합니다.</returns>
        public static object Default(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
        /// <summary>
        /// 지정된 타입이 제네릭 타입 매개변수 T로부터 할당 가능한지 확인합니다.
        /// </summary>
        /// <typeparam name="T">비교할 타입입니다.</typeparam>
        /// <param name="type">검사할 타입입니다.</param>
        /// <returns>할당 가능하면 true, 아니면 false를 반환합니다.</returns>
        public static bool Is<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        /// <summary>
        /// 해당 타입이 델리게이트인지 확인합니다.
        /// </summary>
        /// <param name="type">검사할 타입입니다.</param>
        /// <returns>델리게이트이면 true, 아니면 false를 반환합니다.</returns>
        public static bool IsDelegate(this Type type) => typeof(Delegate).IsAssignableFrom(type);

        /// <summary>
        /// 해당 타입이 구체적인(강하게 형식화된) 델리게이트인지 확인합니다.
        /// </summary>
        /// <param name="type">검사할 타입입니다.</param>
        /// <returns>
        /// Delegate, MulticastDelegate 같은 추상적인 델리게이트 타입은 false를 반환하고,
        /// Action, Func 또는 사용자가 만든 delegate 같은 구체적인 델리게이트 타입은 true를 반환합니다.
        /// </returns>
        public static bool IsStrongDelegate(this Type type)
        {
            if (!type.IsDelegate()) return false;

            return !type.IsAbstract;
        }

        /// <summary>
        /// 해당 필드가 델리게이트인지 확인합니다.
        /// </summary>
        /// <param name="fieldInfo">검사할 필드입니다.</param>
        /// <returns>델리게이트이면 true, 아니면 false를 반환합니다.</returns>
        public static bool IsDelegate(this FieldInfo fieldInfo) => fieldInfo.FieldType.IsDelegate();

        /// <summary>
        /// 해당 타입이 구체적인(강하게 형식화된) 델리게이트인지 확인합니다.
        /// </summary>
        /// <param name="fieldInfo">검사할 필드입니다.</param>
        /// <returns>
        /// Delegate, MulticastDelegate 같은 추상적인 델리게이트 타입은 false를 반환하고,
        /// Action, Func 또는 사용자가 만든 delegate 같은 구체적인 델리게이트 타입은 true를 반환합니다.
        /// </returns>
        public static bool IsStrongDelegate(this FieldInfo fieldInfo) => fieldInfo.FieldType.IsStrongDelegate();

        /// <summary>
        /// 지정된 타입이 특정 제네릭 타입의 인스턴스인지 확인합니다.
        /// </summary>
        /// <param name="genericType">검사할 타입입니다.</param>
        /// <param name="nonGenericType">비제네릭 타입 정의입니다.</param>
        /// <returns>해당 타입이 제네릭 정의의 인스턴스이면 true, 아니면 false를 반환합니다.</returns>
        public static bool IsGenericTypeOf(this Type genericType, Type nonGenericType)
        {
            if (!genericType.IsGenericType)
            {
                return false;
            }

            return genericType.GetGenericTypeDefinition() == nonGenericType;
        }

        /// <summary>
        /// 주어진 타입이 지정된 기반(base) 타입으로부터 파생된 타입인지 확인합니다.
        /// </summary>
        /// <param name="type">검사할 타입입니다 (this 키워드를 사용하는 확장 메서드 대상).</param>
        /// <param name="baseType">기준이 되는 부모 타입입니다.</param>
        /// <returns>파생된 타입이면 true, 그렇지 않으면 false를 반환합니다.</returns>
        public static bool IsDerivedTypeOf(this Type type, Type baseType) => baseType.IsAssignableFrom(type);

        /// <summary>
        /// 특정 타입이 다른 타입으로 암시적 또는 명시적으로 캐스팅 가능한지 확인합니다.
        /// </summary>
        /// <param name="from">원본 타입입니다.</param>
        /// <param name="to">대상 타입입니다.</param>
        /// <param name="implicitly">암시적 캐스팅만 허용할지 여부입니다.</param>
        /// <returns>캐스팅 가능하면 true를 반환합니다.</returns>
        public static bool IsCastableTo(this Type from, Type to, bool implicitly = false)
            => to.IsAssignableFrom(from) || from.HasCastDefined(to, implicitly);

        /// <summary>
        /// 두 타입 간에 형 변환(cast)이 정의되어 있는지 확인합니다.
        /// </summary>
        /// <param name="from">형 변환의 원본 타입입니다.</param>
        /// <param name="to">형 변환의 대상 타입입니다.</param>
        /// <param name="implicitly">암시적 형 변환만 고려할지 여부입니다.</param>
        /// <returns>형 변환이 정의되어 있으면 true, 아니면 false를 반환합니다.</returns>
        static bool HasCastDefined(this Type from, Type to, bool implicitly)
        {
            if ((!from.IsPrimitive && !from.IsEnum) || (!to.IsPrimitive && !to.IsEnum))
            {
                return IsCastDefined
                       (
                           to,
                           m => m.GetParameters()[0].ParameterType,
                           _ => from,
                           implicitly,
                           false
                       )
                       || IsCastDefined
                       (
                           from,
                           _ => to,
                           m => m.ReturnType, implicitly,
                           true
                       );
            }

            if (!implicitly)
            {
                return from == to || (from != typeof(bool) && to != typeof(bool));
            }

            IEnumerable<Type> lowerTypes = Enumerable.Empty<Type>();
            foreach (Type[] types in PrimitiveTypeCastHierarchy)
            {
                if (types.Any(t => t == to))
                {
                    return lowerTypes.Any(t => t == from);
                }

                lowerTypes = lowerTypes.Concat(types);
            }

            return false; // IntPtr, UIntPtr, Enum, Boolean
        }

        /// <summary>
        /// 두 타입 간에 형 변환(cast)이 정의되어 있는지 확인합니다.
        /// </summary>
        /// <param name="type">형 변환 정의를 검사할 타입입니다.</param>
        /// <param name="baseType">메서드에서 기준 타입(변환 대상 타입)을 가져오는 함수입니다.</param>
        /// <param name="derivedType">메서드에서 파생 타입(변환할 원본 타입)을 가져오는 함수입니다.</param>
        /// <param name="implicitly">암시적 형 변환만 확인할지 여부입니다.</param>
        /// <param name="lookInBase">기본 클래스 계층까지 검색할지 여부입니다.</param>
        /// <returns>형 변환이 정의되어 있으면 true, 그렇지 않으면 false를 반환합니다.</returns>
        static bool IsCastDefined(
            Type type,
            Func<MethodInfo, Type> baseType,
            Func<MethodInfo, Type> derivedType,
            bool implicitly,
            bool lookInBase
        )
        {
            // Set the binding flags to search for public and static methods, and optionally include the base hierarchy.
            var flags = BindingFlags.Public | BindingFlags.Static |
                        (lookInBase ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);

            // Get all methods from the type with the specified binding flags.
            MethodInfo[] methods = type.GetMethods(flags);

            // Check if any method is an implicit or explicit cast operator and if the base type is assignable from the derived type.
            return methods.Where(m => m.Name == "op_Implicit" || (!implicitly && m.Name == "op_Explicit"))
                .Any(m => baseType(m).IsAssignableFrom(derivedType(m)));
        }

        /// <summary>
        /// 런타임에 객체를 지정된 타입으로 캐스팅합니다.
        /// </summary>
        /// <param name="type">캐스팅할 대상 타입입니다.</param>
        /// <param name="data">캐스팅할 객체입니다.</param>
        /// <returns>캐스팅된 객체를 반환합니다.</returns>
        public static object Cast(this Type type, object data)
        {
            if (type.IsInstanceOfType(data))
            {
                return data;
            }

            try
            {
                return Convert.ChangeType(data, type);
            }
            catch (InvalidCastException)
            {
                var srcType = data.GetType();
                var dataParam = Expression.Parameter(srcType, "data");
                Expression body = Expression.Convert(Expression.Convert(dataParam, srcType), type);

                var run = Expression.Lambda(body, dataParam).Compile();
                return run.DynamicInvoke(data);
            }
        }

        /// <summary>
        /// 메서드가 오버라이드된 것인지 확인합니다.
        /// </summary>
        /// <param name="methodInfo">검사할 메서드입니다.</param>
        /// <returns>오버라이드된 메서드이면 true를 반환합니다.</returns>
        public static bool IsOverride(this MethodInfo methodInfo)
            => methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType;

        /// <summary>
        /// 대상에 지정된 특성(Attribute)이 존재하는지 확인합니다.
        /// </summary>
        /// <typeparam name="T">검사할 특성 타입입니다.</typeparam>
        /// <param name="provider">특성을 제공하는 대상입니다.</param>
        /// <param name="searchInherited">상속된 특성도 검사할지 여부입니다.</param>
        /// <returns>특성이 존재하면 true를 반환합니다.</returns>
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider, bool searchInherited = true)
            where T : Attribute
        {
            try
            {
                return provider.IsDefined(typeof(T), searchInherited);
            }
            catch (MissingMethodException)
            {
                return false;
            }
        }

        /// <summary>
        /// 지정된 타입의 사람이 읽기 좋은 표시 이름을 가져옵니다.
        /// </summary>
        /// <param name="type">이름을 가져올 타입입니다.</param>
        /// <param name="includeNamespace">네임스페이스를 포함할지 여부입니다.</param>
        /// <returns>포맷된 표시 이름을 반환합니다</returns>
        public static string GetDisplayName(this Type type, bool includeNamespace = false)
        {
            if (type.IsGenericParameter)
            {
                return type.Name;
            }

            if (type.IsArray)
            {
                int rank = type.GetArrayRank();
                string innerTypeName = GetDisplayName(type.GetElementType(), includeNamespace);
                return $"{innerTypeName}[{new string(',', rank - 1)}]";
            }

            if (TypeDisplayNames.TryGetValue(type, out string baseName1))
            {
                if (!type.IsGenericType || type.IsConstructedGenericType)
                    return baseName1;
                Type[] genericArgs = type.GetGenericArguments();
                return $"{baseName1}<{new string(',', genericArgs.Length - 1)}>";
            }

            if (type.IsGenericTypeOf(typeof(Nullable<>)))
            {
                var innerType = type.GetGenericArguments()[0];
                return $"{innerType.GetDisplayName()}?";
            }

            if (type.IsGenericType)
            {
                var baseType = type.GetGenericTypeDefinition();
                Type[] genericArgs = type.GetGenericArguments();

                if (ValueTupleTypes.Contains(baseType))
                {
                    return GetTupleDisplayName(type, includeNamespace);
                }

                if (type.IsConstructedGenericType)
                {
                    var genericNames = new string[genericArgs.Length];
                    for (var i = 0; i < genericArgs.Length; i++)
                    {
                        genericNames[i] = GetDisplayName(genericArgs[i], includeNamespace);
                    }

                    string baseName = GetDisplayName(baseType, includeNamespace).Split('<')[0];
                    return $"{baseName}<{string.Join(", ", genericNames)}>";
                }

                string typeName = includeNamespace
                    ? type.FullName
                    : type.Name;

                return $"{typeName?.Split('`')[0]}<{new string(',', genericArgs.Length - 1)}>";
            }

            var declaringType = type.DeclaringType;
            if (declaringType == null)
            {
                return includeNamespace
                    ? type.FullName
                    : type.Name;
            }

            string declaringName = GetDisplayName(declaringType, includeNamespace);
            return $"{declaringName}.{type.Name}";
        }

        /// <summary>
        /// 튜플 타입에 대한 사람이 읽기 좋은 표시 이름을 가져옵니다.
        /// </summary>
        /// <param name="type">튜플 타입입니다.</param>
        /// <param name="includeNamespace">네임스페이스 포함 여부입니다.</param>
        /// <returns>튜플에 대한 표시 이름을 반환합니다.</returns>
        static string GetTupleDisplayName(this Type type, bool includeNamespace = false)
        {
            IEnumerable<string> parts = type
                .GetGenericArguments()
                .Select(x => x.GetDisplayName(includeNamespace));

            return $"({string.Join(", ", parts)})";
        }

        /// <summary>
        /// 서로 다른 타입에 정의된 두 메서드가 동일한 시그니처를 가지는지 확인합니다.
        /// </summary>
        /// <param name="a">첫 번째 메서드입니다.</param>
        /// <param name="b">두 번째 메서드입니다.</param>
        /// <returns>두 메서드가 동일한 시그니처를 가지면 <c>true</c>를 반환합니다.</returns>
        public static bool AreMethodsEqual(MethodInfo a, MethodInfo b)
        {
            if (a.Name != b.Name) return false;

            ParameterInfo[] paramsA = a.GetParameters();
            ParameterInfo[] paramsB = b.GetParameters();

            if (paramsA.Length != paramsB.Length) return false;
            for (var i = 0; i < paramsA.Length; i++)
            {
                var pa = paramsA[i];
                var pb = paramsB[i];

                if (pa.Name != pb.Name) return false;
                if (pa.HasDefaultValue != pb.HasDefaultValue) return false;

                var ta = pa.ParameterType;
                var tb = pb.ParameterType;

                if (ta.ContainsGenericParameters || tb.ContainsGenericParameters)
                    continue;
                if (ta != tb) return false;
            }

            if (a.IsGenericMethod != b.IsGenericMethod) return false;

            if (!a.IsGenericMethod || !b.IsGenericMethod) return true;
            {
                Type[] genericA = a.GetGenericArguments();
                Type[] genericB = b.GetGenericArguments();

                if (genericA.Length != genericB.Length) return false;
                for (var i = 0; i < genericA.Length; i++)
                {
                    var ga = genericA[i];
                    var gb = genericB[i];

                    if (ga.Name != gb.Name) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 동일한 시그니처를 가진 해당 메서드를 새 타입에서 찾아서, 해당 메서드를 새 타입에 맞게 재지정(Rebase)합니다.
        /// </summary>
        /// <param name="method">재지정할 원본 메서드입니다.</param>
        /// <param name="newBase">메서드를 재지정할 대상 타입입니다.</param>
        /// <returns>새 타입에서 찾은 대응되는 메서드를 반환합니다.</returns>
        public static MethodInfo RebaseMethod(this MethodInfo method, Type newBase)
        {
            var flags = BindingFlags.Default;

            flags |= method.IsStatic ? BindingFlags.Static : BindingFlags.Instance;

            flags |= method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;

            MethodInfo[] candidates = newBase.GetMethods(flags)
                .Where(x => AreMethodsEqual(x, method))
                .ToArray();

            if (candidates.Length == 0)
            {
                throw new ArgumentException(
                    $"Could not rebase method {method} onto type {newBase} as no matching candidates were found");
            }

            if (candidates.Length > 1)
            {
                throw new ArgumentException(
                    $"Could not rebase method {method} onto type {newBase} as too many matching candidates were found");
            }

            return candidates[0];
        }
    }
}