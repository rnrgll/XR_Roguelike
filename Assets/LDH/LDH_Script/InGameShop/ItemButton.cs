using DG.Tweening;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameShop
{
    public class ItemButton : MonoBehaviour
    {
        [Header("Item Data")] 
         public int slotIndex;
        
        
        [Header("PopUp UI")]
        [SerializeField] private ItemPopUpPanel _popUpPanel;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Button _returnButton;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private Ease _easeType = Ease.OutQuint;
        
        [Header("Item UI")] 
        [SerializeField] private GameObject _priceLabel;
        private TMP_Text _priceText;
        private Image _itemImage;
        
        
        //버튼 이동 관련(Pop up)
        private Button _itemButton;
        private Canvas _canvas;
        private RectTransform _buttonRec;
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
            _canvas = GetComponent<Canvas>();
            _buttonRec = _itemButton.GetComponent<RectTransform>();
            _originWorldPos = _itemButton.transform.position;
            _originWorldScale = _itemButton.transform.lossyScale;
            _originWorldRot = _itemButton.transform.rotation;
            
            //ui 관련 참조 설정
            _priceText = _priceLabel.GetComponentInChildren<TMP_Text>();
            _itemImage = _itemButton.GetComponent<Image>();

        }


        #region Item Data 연동

        public void OnItemUpdated(string itemID)
        {
            var itemData = Manager.Data.ItemDB.GetItemById(itemID);
            //이미지 설정
            SetImage(itemData.image);
            
            //price 설정
            _priceText.text = itemData.price.ToString();

        }

        #endregion

        #region UI Update

        private void SetImage(Sprite itemImage)
        {
            _itemImage.sprite = itemImage;
        }
        

        #endregion

        #region Pop Up 관련 버튼 이동
        public void MoveToPopUp()
        {
            //canvas sorting order 변경
            _canvas.sortingOrder = (int)SortOrder.PopUp + 1;
            
            //버튼 비활성화
            _itemButton.enabled = false;
            
            //pirce tag 비활성화
            _priceLabel.SetActive(false);
            
            //target의 월드 기준 위치 가져오기
            Vector3 targetWorldPos = _targetTransform.position;
            Quaternion targetWorldRot = _targetTransform.rotation;
            Vector3 targetWorldScale = _originWorldScale * 1.5f;
            

            //위치 이동, 회전, 스케일 변화 적용
            Sequence seq = DOTween.Sequence();
            seq.Join(_buttonRec.DOMove(targetWorldPos, _duration)).SetEase(_easeType)
                .Join(_buttonRec.DORotateQuaternion(targetWorldRot, _duration)).SetEase(_easeType)
                .Join(_buttonRec.DOScale(targetWorldScale, _duration)).SetEase(_easeType);

            seq.OnComplete(() =>
            {
                //pop up panel 활성화
                _popUpPanel.Show(slotIndex);

                //Return Button onclick 이벤트 구독처리
                _returnButton.onClick.AddListener(ReturnToOrigin);
            });

        }

        public void ReturnToOrigin()
        {
            // canvas sorting order 원래대로
            _canvas.sortingOrder = (int)SortOrder.Item;
              
            // 현재 오브젝트의 부모 기준 위치로 변환
            // Vector3 localTargetPos = _itemButton.transform.parent.InverseTransformPoint(_originWorldPos);

            //팝업 패널 비활성화
            _popUpPanel.Hide();
            
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
                //price label 활성화
                _priceLabel.SetActive(true);
                
            });

        }
        #endregion
    }
}

