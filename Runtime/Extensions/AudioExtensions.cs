using UnityEngine;

namespace PJH.Toolkit.Extensions
{
    public static class AudioExtensions
    {
        /// <summary>
        /// 볼륨 슬라이더 위치를 나타내는 float 값을 로그 스케일의 볼륨 값으로 변환합니다.
        /// 이를 통해 슬라이더를 움직일 때 보다 부드럽고 자연스러운 음량 변화를 제공합니다.
        ///
        /// 수학적으로 다음 단계를 수행합니다:
        /// - 슬라이더 값이 0보다 크거나 같은 최소값 0.0001 이상이 되도록 보정하여, 로그 함수에 0이 전달되는 것을 방지합니다.
        /// - 슬라이더 값을 상용로그(밑이 10인 로그)로 변환합니다.
        /// - 그 결과값에 20을 곱합니다. 오디오 공학에서는 dB 스케일에서 1 단위의 변화가
        ///   사람의 귀로 느껴지는 음량의 약 두 배 혹은 절반의 차이에 해당하기 때문에 20을 곱합니다.
        ///
        /// 이 메서드는 Unity의 Audio Mixer와 함께 사용하는 UI 볼륨 슬라이더 값을 정규화할 때 유용합니다.
        /// </summary>
        public static float ToLogarithmicVolume(this float sliderValue)
        {
            return Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        }

        /// <summary>
        /// [0, 1] 범위의 분수를 받아 이를 로그 스케일([0, 1] 범위)로 변환합니다.
        /// 이는 사람이 소리를 인지하는 방식(사람의 음량 인식은 로그 스케일)에 맞춘 변환입니다.
        ///
        /// 수학적으로 다음 단계를 수행합니다:
        /// - Log10 함수 안에서, 원래 분수에 9를 곱한 값에 1을 더한 후 로그를 취합니다.
        ///   이렇게 하면 분수가 로그 곡선에 맞춰 부드럽게 스케일링되며 [0, 1] 범위에 맞게 조정됩니다.
        /// - 보간된 분수의 상용로그(밑 10)를 계산합니다.
        /// - 결과를 Log10(10)으로 나누어 정규화합니다. 
        ///   이렇게 하면 [0, 1] 범위에 결과가 맞춰지며, 로그 함수 입력이 1에서 10 사이로 변환된 후에도 올바르게 범위가 유지됩니다.
        ///
        /// 이 메서드는 오디오 클립 간 페이드 효과를 보다 자연스럽게 적용할 때 유용합니다.
        /// </summary>
        public static float ToLogarithmicFraction(this float fraction)
        {
            return Mathf.Log10(1 + 9 * fraction) / Mathf.Log10(10);
        }
    }
}