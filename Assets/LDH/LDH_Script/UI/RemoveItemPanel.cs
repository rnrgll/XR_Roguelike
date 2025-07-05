using InGameShop;
using Item;
using Managers;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RemoveItemPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _infoText;
        
        [SerializeField] private List<Button> slots;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color normalColor;
        
        private int requiredRemoveCount = 1;
        private List<int> selectedIndices = new();

        private Action _onItemRemoved; //아이템 remove 후에 콜백으로 실행할 액션 (아이템 추가 등등...)
        private void OnEnable()
        {
            selectedIndices.Clear();
            confirmButton.interactable = false;
            
            
            for(int i =0; i < Manager.GameState.MaxItemInventorySize; i++)
            {
                int slotIndex = i;
                if (Manager.GameState.CurrentItemCount!=3) return;
                
                UpdateSlot(i, Manager.GameState.ItemInventory[slotIndex]);
                slots[slotIndex].onClick.RemoveAllListeners();
                slots[slotIndex].onClick.AddListener(() => ToggleSelect(slotIndex));
                SetSlotVisual(slotIndex, false);
                SetSlotVisual(slotIndex, false);
                
                // slots[slotIndex].onClick.AddListener(() => OnSlotClicked(slotIndex));
            }
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmRemove);
        }

        private void OnDisable()
        {
            foreach (var slot in slots)
            {
                slot.onClick.RemoveAllListeners();
            }
        }

        
        //초기화 및 설정
        public void InitPanel(int requiredCount, Action onItemRemoved)
        {
            requiredRemoveCount = requiredCount;
            _onItemRemoved = onItemRemoved;
        }


        //슬롯 ui 업데이트
        private void UpdateSlot(int slotIndex, string itemId)
        {
            _infoText.text = $"삭제할 아이템을 {requiredRemoveCount}개 선택하세요.";
            
            GameItem item = Manager.Data.GameItemDB.GetItemById(itemId);
            slots[slotIndex].transform.GetChild(0).GetComponent<Image>().sprite = item.sprite;
        }
        
        
        private void ToggleSelect(int slotIndex)
        {
            if (selectedIndices.Contains(slotIndex))
            {
                selectedIndices.Remove(slotIndex);
                SetSlotVisual(slotIndex, false);
            }
            else
            {
                if (selectedIndices.Count < requiredRemoveCount)
                {
                    selectedIndices.Add(slotIndex);
                    SetSlotVisual(slotIndex, true);
                }
            }

            confirmButton.interactable = (selectedIndices.Count == requiredRemoveCount);
        }
        
        //슬롯 색깔
        private void SetSlotVisual(int slotIndex, bool selected)
        {
            var image = slots[slotIndex].GetComponent<Image>();
            image.color = selected ? selectedColor : normalColor;
        }
        
        private void ConfirmRemove()
        {
            foreach (var index in selectedIndices)
            {
                Manager.GameState.RemoveItem(Manager.GameState.ItemInventory[index]);
            }

            Manager.UI.SetUIActive(GlobalUI.ItemRemove, false);
            _onItemRemoved?.Invoke();
            _onItemRemoved = null;
        }
        
        // private void OnSlotClicked(int slotIndex)
        // {
        //     Debug.Log($"slot index = {slotIndex}, ");
        //     Debug.Log($"itemid = {Manager.GameState.ItemInventory[slotIndex]}");
        //     Manager.GameState.RemoveItem(Manager.GameState.ItemInventory[slotIndex]);
        //     Manager.UI.SetUIActive(GlobalUI.ItemRemove, false);
        //     
        //     _onItemRemoved?.Invoke();
        //     _onItemRemoved = null; // 초기화
        // }
        
        
        
    }
}