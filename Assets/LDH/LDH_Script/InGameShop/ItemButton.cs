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
        
        private Button _itemButton;
        private Transform _originTransform;

        private void Awake() => Init();

        private void Init()
        {
            _itemButton = GetComponent<Button>();
            _originTransform = _itemButton.transform;
        }


        public void OnItemClicked()
        {
            //target의 월드 기준 위치 가져오기
            Vector3 targetWorldPos = _targetTransform.position;
            Quaternion targetWorldRot = _targetTransform.rotation;
            Vector3 targetWorldScale = _originTransform.lossyScale * 1.5f;
            
            //현재 오브젝트의 부모 기준 위치로 변환
            Vector3 localTargetPos = _itemButton.transform.parent.InverseTransformPoint(targetWorldPos);
            
            //위치 이동, 회전, 스케일 변화 적용
            Sequence seq = DOTween.Sequence();
            //seq.Join(_itemButton.doan)
            
        }
        
    }
}

