using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
                //테스트용
                int effectId = options[i].Id;
                Debug.Log(effectId);
            }
        }




    }
}