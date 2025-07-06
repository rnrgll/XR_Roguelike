using System;
using TMPro;
using UnityEngine;

namespace InGameShop
{
    public class CrystalBallButton : MonoBehaviour
    {
        [Header("Reroll Price")] 
        [SerializeField] private int _rerollPrice;

        [Header("UI")] 
        [SerializeField] private TMP_Text _rerollPriceText;
        [SerializeField] private GameObject _alarmPopUp;
        
        private ButtonCondition _condition;
        
        
        private void Awake() => Init();
        private void Start()
        {
            GameStateManager.Instance.OnGoldChanged.AddListener(SetButton);
            SetButton(GameStateManager.Instance.Gold);
        }

        private void OnEnable()
        {
            //GameStateManager.Instance.OnGetGold.AddListener(SetButton);
        }

        private void Init()
        {
            _condition = GetComponent<ButtonCondition>();
            _rerollPriceText.text = _rerollPrice.ToString();
        }
        
        
        public void SetButton(int currentGold)
        {
            if (currentGold < _rerollPrice)
            {
                _condition.SetButtonState(ButtonActiveState.Deactive, () => _alarmPopUp.SetActive(true) );
            }
            else
            {
                _condition.SetButtonState(ButtonActiveState.Active, ()=> Reroll());
            }
            
        }

        private void Reroll()
        {
            GameStateManager.Instance.AddGold(-_rerollPrice);
            
            ShopManager.Instance.Reroll();
            
        }


    }
}