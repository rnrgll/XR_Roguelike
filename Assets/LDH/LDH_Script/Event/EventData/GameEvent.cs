using System;
using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [Serializable]
    public class Option
    {
        public int Id;
        public string Text;

        public Option(int id, string text)
        {
            Id = id;
            Text = text;
        }
    }
    
    public class GameEvent
    {
        public int EventID;
        public string EventName;
        public string EventText;
        public List<Option> Options;
        public Sprite EventImage;

        public GameEvent() { }
        public GameEvent(int eventID, string eventName, string eventText, List<Option> options, Sprite eventImage)
        {
            EventID = eventID;
            EventName = eventName;
            EventText = eventText;
            Options = options;
            EventImage = eventImage;
        }
    }
}