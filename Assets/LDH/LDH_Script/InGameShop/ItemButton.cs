using CustomUtility.UI;
using DG.Tweening;
using Item;
using Managers;
using System.Collections;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InGameShop
{
    public class ItemButton : MonoBehaviour
    {
        [Header("Item Data")] 
        public int slotIndex;
        private GameItem currentItem;
        
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

        //버튼 애니메이션 관련
        private ButtonState _currentState;
        private Tween _floatTween;
        private EventSystem _eventSystem;
        private Vector2 _initialAnchorPos; 
        private Coroutine _floatingCoroutine;

        
        
        private void Awake() => Init();

        private void OnEnable()
        {
            _itemButton.onClick.AddListener(MoveToPopUp);
            _currentState = ButtonState.Idle;
            StartFloating();

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

            _initialAnchorPos = _buttonRec.anchoredPosition;
            
            
            //ui 관련 참조 설정
            _priceText = _priceLabel.GetComponentInChildren<TMP_Text>();
            _itemImage = _itemButton.GetComponent<Image>();

            
            //이벤트 시스템 캐싱
            _eventSystem = EventSystem.current;
            if (_eventSystem == null)
                Debug.LogError("EventSystem이 씬에 없습니다!");
        }


        #region Item Data 연동

        public void OnItemUpdated(GameItem item)
        {
            currentItem = item;
            if (item is InventoryItem inventoryItem)
            {
                
                if(_itemButton.transform.childCount!=0)
                    foreach (Transform child in _itemButton.transform)
                    {
                        Destroy(child.gameObject);
                    }
            }
            else if(item is EnchantItem enchantItem)
            {
                Image enchantImage = new GameObject("EnchantImage", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
                enchantImage.transform.SetParent(_itemButton.transform, false); // false → 로컬 기준 유지
                UILayout.Stretch(enchantImage.GetComponent<RectTransform>());

                enchantImage.sprite = enchantItem.enchantSprite;
                enchantImage.color = EnchantFrameColor.GetColor(enchantItem.enchantType);
                enchantImage.preserveAspect = true;

            }
            
            //이미지 설정
            SetImage(item.sprite);
            //price 설정
            _priceText.text = item.price.ToString();

            
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
            if (_currentState != ButtonState.Idle) return;
            
            //버튼 상태 변경, 애니메이션 중지
            _currentState = ButtonState.PoppedUp;
            StopFloating();
            
            //클릭 이벤트 다 막기
            _eventSystem.enabled = false;
            
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
                
                //클릭이벤트 다시 활성화
                _eventSystem.enabled = true;
            });

        }

        public void ReturnToOrigin()
        {
            _eventSystem.enabled = false;
            
            // canvas sorting order 원래대로
            _canvas.sortingOrder = (int)SortOrder.Item;
              
            // 현재 오브젝트의 부모 기준 위치로 변환
            // Vector3 localTargetPos = _itemButton.transform.parent.InverseTransformPoint(_originWorldPos);

            //팝업 패널 비활성화
            _popUpPanel.Hide();
            
            //복귀 애니메이션
            Sequence seq = DOTween.Sequence();
            seq.Join(_buttonRec.DOMove(_originWorldPos, 0.1f)).SetEase(_easeType)
                .Join(_buttonRec.DORotateQuaternion(_originWorldRot, 0.1f)).SetEase(_easeType)
                .Join(_buttonRec.DOScale(_originWorldScale, 0.1f)).SetEase(_easeType);
            
            seq.OnComplete(() =>
            {
                //이벤트 해제
                _returnButton.onClick.RemoveListener(ReturnToOrigin);
                //버튼 활성화
                _itemButton.enabled = true;
                //price label 활성화
                _priceLabel.SetActive(true);
                
                _eventSystem.enabled = true;
                
                //버튼 상태 변경
                _currentState = ButtonState.Idle;
                //애니메이션 시작
                StartFloating();
                
            });

        }
        #endregion


        #region Button 애니메이션

        // private void StartFloating()
        // {
        //     float startDelay = UnityEngine.Random.Range(0f, 1f); // 랜덤 딜레이
        //     
        //     _floatTween = _buttonRec
        //         .DOAnchorPosY(_buttonRec.anchoredPosition.y + 10f, 1f)
        //         .SetEase(Ease.InOutSine)
        //         .SetLoops(-1, LoopType.Yoyo)
        //         .SetDelay(startDelay);
        // }
        //
        // private void StopFloating()
        // {
        //     
        //     _floatTween?.Complete();
        //     _floatTween?.Kill();
        //     _floatTween = null;    
        //     _buttonRec.anchoredPosition = _initialAnchorPos;
        //
        // }
        
        private void StartFloating()
        {
            
            if (!gameObject.activeInHierarchy)
                return;
            
            _floatingCoroutine = StartCoroutine(DelayedStartFloating());
        }
        

        private IEnumerator DelayedStartFloating()
        {
            float delay = UnityEngine.Random.Range(0f, 1f);
            yield return new WaitForSeconds(delay);

            _floatTween = _buttonRec
                .DOAnchorPosY(_initialAnchorPos.y + 10f, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        
        private void StopFloating()
        {
            if (_floatingCoroutine != null)
            {
                StopCoroutine(_floatingCoroutine);
                _floatingCoroutine = null;
            }

            _floatTween?.Kill(true);
            _floatTween = null;

            _buttonRec.anchoredPosition = _initialAnchorPos;
        }

        #endregion
    }
}

