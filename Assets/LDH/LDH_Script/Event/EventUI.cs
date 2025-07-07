using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private Button rewardConfirmButton;
        

        private void OnDestroy()
        {
            foreach (Button eventOption in eventOptions)
            {
                eventOption.onClick.RemoveAllListeners();
            }
        }

        public void UpdateUI(GameEvent gameEvent)
        {
            if (gameEvent.EventName.Contains('('))
            {
                int index = gameEvent.EventName.IndexOf('(');
                string line1 = gameEvent.EventName.Substring(0, index);         // 괄호 이전
                string line2 = gameEvent.EventName.Substring(index);            // 괄호 포함 이후
                eventTitle.text  = line1 + "\n" + line2;
            }
            
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

                EventMainReward mainReward = Manager.Data.EventDB.GetMainRewardById(mainRewardId);
                EventRewardEffect rewardEffect = Manager.Data.EventDB.GetRewardEffectByMainRewardId(mainRewardId);
                
                // List<SubEffect> subEffects = Manager.Data.EventDB.GetSubEffectsByMainRewardId(mainRewardId);
                // Debug.Log(subEffects==null);
                eventOptions[i].onClick.AddListener(() =>
                {
                    rewardConfirmButton.onClick.AddListener(() =>
                    {
                        rewardEffect.ApplyEffects(() =>
                        {
                            resultPanel.SetActive(false);
                            Manager.Map.ShowMap();
                        });
                       
                    });
                    rewardText.text = string.Join("\n", mainReward.ResultText.Split(',').Select(s => s.Trim()));
                    resultPanel.SetActive(true);
                });
            }
        }
    }
}