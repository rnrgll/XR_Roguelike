using InGameShop;
using Item;
using Managers;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UI;
using UnityEngine.EventSystems;

namespace TopBarUI
{
    public class InventorySlot : MonoBehaviour
    {
        public int slotIndex;
        public string itemId;

        public bool IsLock { get; private set; } = true;
        
        
        private Button _slotButton;
        [SerializeField] private Image _itemImage;
        private EventTrigger _trigger;
        
        
        
        private void Awake()
        {
            _slotButton = GetComponent<Button>();
            _trigger = GetComponent<EventTrigger>();
        }
        
        private void Start()
        {
            ItemManager.Instance.EnrollInventorySlot(this);
            
            if (IsLock)
                _slotButton.interactable = !IsLock;
        }
        

        private void OnEnable()
        {
            Manager.GameState.OnItemChanged.AddListener(UpdateSlot);
            _slotButton.onClick.AddListener(UseItem);
            AddEventTrigger(EventTriggerType.PointerEnter, (e) => ActiveItemInfo(true)); //이벤트 트리거 등록
            AddEventTrigger(EventTriggerType.PointerExit, (e) => ActiveItemInfo(false)); //이벤트 트리거 등록
        }

        private void OnDisable()
        {
            Manager.GameState.OnItemChanged.RemoveListener(UpdateSlot);
            _slotButton.onClick.RemoveListener(UseItem);
            _trigger.triggers.Clear(); // 이벤트트리거 초기화
        }

        
        //슬롯 lock 설정
        public void SetLock(bool isLock)
        {
            IsLock = isLock;
            _slotButton.interactable = !isLock;
        }

        
        //아이템 슬롯 ui 및 정보 업데이트
        private void UpdateSlot(string itemId)
        {
            this.itemId = Manager.GameState.ItemInventory[slotIndex];
            if (string.IsNullOrEmpty(this.itemId))
            {
                _itemImage.sprite = null;
            }
            else
            {
                GameItem item = Manager.Data.GameItemDB.GetItemById(this.itemId);
                _itemImage.sprite = item.sprite;
            }
        }
        
        
        //아이템 hover시 상세 정보 활성화
        public void ActiveItemInfo(bool isActive)
        {
            if (isActive)
            {
                if (string.IsNullOrEmpty(itemId)) return;
                SendData();
            }
            
            Manager.UI.SetUIActive(GlobalUI.Item, isActive);
        }
        
        private void SendData()
        {
            if (string.IsNullOrEmpty(itemId)) return;
            Manager.UI.ItemUI.GetComponent<InventoryItemPopUp>().SetData(itemId);
        }
        
        
        
        //아이템 사용
        private void UseItem()
        {
            if (string.IsNullOrEmpty(itemId)) return;
            ItemManager.Instance.UseItem(itemId);
        }
        
        
        
        //트리거 추가
        private void AddEventTrigger(EventTriggerType type, Action<BaseEventData> action)
        {
            if (_trigger == null) return;

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = type
            };
            
            entry.callback.AddListener((data) => action(data));
            _trigger.triggers.Add(entry);
        }
    }
}