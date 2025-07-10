using CustomUtility.IO;
using DesignPattern;
using Event;
using InGameShop;
using System;
using UnityEngine;
using Utils;

namespace Managers
{
    public class DataManager : Singleton<DataManager>
    {

        public ItemDataBase GameItemDB;
        public EventDataBase EventDB;
        
        private void Awake()
        {
            SingletonInit();
        }

        private void Start()
        {
            GameItemDB = new ItemDataBase();
            EventDB = new EventDataBase();
            
            //csv file load
            
            //이벤트 로드
            StartCoroutine(CSVDownloader.Start(
                CSVLink.CsvLinkDict["Event"],
                EventDB.LoadEventData
            ));
            StartCoroutine(CSVDownloader.Start(
                CSVLink.CsvLinkDict["EventMainReward"],
                EventDB.LoadMainRewardData
            ));
            StartCoroutine(CSVDownloader.Start(
                CSVLink.CsvLinkDict["EventRewardEffect"],
                EventDB.LoadRewardEffectData
            ));
        }
    }
}