using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InGameShop
{
    public class ItemButton : MonoBehaviour
    {
        [Header("PopUp UI")]
        [SerializeField] private GameObject _popUpPanel;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Button _returnButton;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private Ease _easeType = Ease.OutQuint;
        
        
        private Button _itemButton;
        private RectTransform _buttonRec;
        private Transform _originParent;
        private Vector3 _originWorldPos;
        private Vector3 _originWorldScale;
        private Quaternion _originWorldRot;

        private void Awake() => Init();

        private void OnEnable()
        {
            _itemButton.onClick.AddListener(MoveToPopUp);
        }

        private void OnDisable()
        {
            _itemButton.onClick.RemoveListener(MoveToPopUp);
        }


        private void Init()
        {
            _itemButton = GetComponent<Button>();
            _buttonRec = _itemButton.GetComponent<RectTransform>();
            _originParent = _itemButton.transform.parent;
            _originWorldPos = _itemButton.transform.position;
            _originWorldScale = _itemButton.transform.lossyScale;
            _originWorldRot = _itemButton.transform.rotation;
        }


        public void MoveToPopUp()
        {
            //버튼 비활성화
            _itemButton.enabled = false;
            
            //target의 월드 기준 위치 가져오기
            Vector3 targetWorldPos = _targetTransform.position;
            Quaternion targetWorldRot = _targetTransform.rotation;
            Vector3 targetWorldScale = _originWorldScale * 1.5f;
            
            // //현재 오브젝트의 부모 기준 위치로 변환
            // Vector3 localTargetPos = _itemButton.transform.parent.InverseTransformPoint(targetWorldPos);
            
            //위치 이동, 회전, 스케일 변화 적용
            Sequence seq = DOTween.Sequence();
            seq.Join(_buttonRec.DOMove(targetWorldPos, _duration)).SetEase(_easeType)
                .Join(_buttonRec.DORotateQuaternion(targetWorldRot, _duration)).SetEase(_easeType)
                .Join(_buttonRec.DOScale(targetWorldScale, _duration)).SetEase(_easeType);

            seq.OnComplete(() =>
            {
                //pop up panel 활성화
                _popUpPanel.SetActive(true);
                //pop up panel로 계층 옮기기
                _itemButton.transform.SetParent(_popUpPanel.transform, true);
                //Return Button onclick 이벤트 구독처리
                _returnButton.onClick.AddListener(ReturnToOrigin);
            });

        }

        public void ReturnToOrigin()
        {
            // 계층 구조 원래대로 복귀
            _itemButton.transform.SetParent(_originParent,true);
           
              
            // //현재 오브젝트의 부모 기준 위치로 변환
            // Vector3 localTargetPos = _itemButton.transform.parent.InverseTransformPoint(_originWorldPos);

            //팝업 패널 비활성화
            _popUpPanel.SetActive(false);
            
            //복귀 애니메이션
            Sequence seq = DOTween.Sequence();
            seq.Join(_buttonRec.DOMove(_originWorldPos, _duration)).SetEase(_easeType)
                .Join(_buttonRec.DORotateQuaternion(_originWorldRot, _duration)).SetEase(_easeType)
                .Join(_buttonRec.DOScale(_originWorldScale, _duration)).SetEase(_easeType);
            
            seq.OnComplete(() =>
            {
                //이벤트 해제
                _returnButton.onClick.RemoveListener(ReturnToOrigin);
                //버튼 활성화
                _itemButton.enabled = true;
            });

        }
        
    }
}

