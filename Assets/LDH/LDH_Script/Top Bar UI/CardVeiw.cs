using CardEnum;
using DesignPattern;
using InGameShop;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TopBarUI
{
    public class CardVeiw : PooledObject
    {
        [SerializeField] private Image cardImg;
        [SerializeField] private Image enchantImg;
        [SerializeField] private TMP_Text effectText;
        [SerializeField] private string cardName;
        
        public void SetData(MinorArcana minor)
        {
            //마이너 아르카나 이미지
            string cNum = minor.CardNum.ToString();
            string cName = $"MinorArcana/{minor.CardSuit}/{minor.CardSuit}_{cNum}";
            var sprite = Resources.Load<Sprite>(cName);
            
            Debug.Log(cName);
            
            cardName = $"{cName} {minor.CardName}";
            cardImg.sprite = sprite;
            
            //인챈트 이미지
            string enchantEffect = "";
            if (minor.Enchant.enchantInfo == CardEnchant.none)
            {
                enchantImg.enabled = false;
            }
            else
            {
                Debug.Log($"인챈트 효과 있음 : {minor.Enchant.enchantInfo}");
                enchantImg.enabled = true;
                //todo:인챈트에 따른 이미지 적용하기
                enchantImg.sprite = (Manager.Data.GameItemDB.EnchantDB[minor.Enchant.enchantInfo] as EnchantItem)
                    .enchantSprite;
                enchantEffect = minor.Enchant.enchantInfo.ToString();
            }
            
            // 효과 텍스트
            string cardEffect = $"데미지 +{minor.CardNum}";
            effectText.text = $"{cardEffect}{(string.IsNullOrEmpty(enchantEffect) ? string.Empty : $"\n{enchantEffect}")}";

        }

        public void SetData(MajorArcanaSO major)
        {
            cardName = major.cardName;
            
            //메이저 아르카나 이미지
            cardImg.sprite = major.sprite;
            
            //인챈트 이미지
            enchantImg.enabled = false;
            
            //todo : 메이저 카드 효과부분 고치기
            effectText.text = major.cardName;
        }

        public void PrintCardName()
        {
            Debug.Log(cardName);
        }
        
    }
}