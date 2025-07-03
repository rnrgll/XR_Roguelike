using Managers;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Event
{
    public class EventUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text eventTitle;
        [SerializeField] private TMP_Text eventText;
        [SerializeField] private Image eventImage;
        [SerializeField] private List<Button> eventOptions;


        private void OnDestroy()
        {
            foreach (Button eventOption in eventOptions)
            {
                eventOption.onClick.RemoveAllListeners();
            }
        }

        public void UpdateUI(GameEvent gameEvent)
        {
            eventTitle.text = gameEvent.EventName;
            eventText.text = gameEvent.EventText;
            eventImage.sprite = gameEvent.EventImage;
            SetOptions(gameEvent.Options);
        }

        public void SetOptions(List<Option> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                //option button text 설정
                eventOptions[i].GetComponentInChildren<TMP_Text>().text = options[i].Text;
                
                //option button onclick event 연결
                //버튼에 저장된 main reward id로 subeffect 리스트를 가져온다.
                int mainRewardId = options[i].MainRewardId;
                List<SubEffect> subEffects = Manager.Data.EventDB.GetSubEffectsByMainRewardId(mainRewardId);
                Debug.Log(subEffects==null);
                eventOptions[i].onClick.AddListener(() =>
                {
                    foreach (SubEffect subEffect in subEffects)
                    {
                        Debug.Log(subEffect.SubEffectType.ToString());
                        subEffect.ApplyEffect();
                    }
                });
            }
        }




    }
}