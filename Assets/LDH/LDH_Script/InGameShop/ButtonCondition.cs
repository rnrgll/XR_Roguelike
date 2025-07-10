using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace InGameShop
{
    public class ButtonCondition : MonoBehaviour
    {
        private Button _button;
        private Image _image;
        private Color32 _disableColor;
        private Color32 _normalColor;
        private ButtonActiveState activeState;
        
        private void Awake() => Init();
        
        private void Init()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
            
            _disableColor = _button.colors.disabledColor;
            _normalColor = _button.colors.normalColor;
        }

        /// <summary>
        /// 버튼의 상태를 설정하고, 클릭 시 실행할 동작을 지정합니다.
        /// 상태에 따라 버튼의 색상을 변경하고, 전달된 onClick 액션을 버튼 클릭 이벤트로 연결합니다
        /// </summary>
        /// <param name="activeState">버튼의 시각적 상태 (활성/비활성)</param>
        /// <param name="onClick">클릭 시 실행할 외부 동작</param>
        public void SetButtonState(ButtonActiveState activeState, Action onClick)
        {
            // 현재 상태 저장 (필요 시 외부에서 참조하거나 디버깅용으로 활용 가능)
            this.activeState = activeState;

            // 버튼 이미지 색상 변경
            // - Active: 원래 색상(normalColor)
            // - Deactive: 비활성화 색상(disableColor) (단, 실제 비활성화는 아님)
            _image.color = activeState switch
            {
                ButtonActiveState.Active => _normalColor,
                ButtonActiveState.Deactive => _disableColor,
                _ => throw new ArgumentOutOfRangeException()
            };

            // 기존 클릭 이벤트 제거
            _button.onClick.RemoveAllListeners();

            // 새로운 클릭 이벤트 등록
            // 전달된 onClick 액션을 래핑하여 실행 (null 안전 처리 포함)
            _button.onClick.AddListener(() => onClick?.Invoke());
        }

    }
}