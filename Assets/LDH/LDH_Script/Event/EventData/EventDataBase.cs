using CustomUtility.IO;
using Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Event
{
    public class EventDataBase
    {
        //DB
        private List<GameEvent> EventDB;
        private Dictionary<int, EventMainReward> MainRewardDB;
        private Dictionary<int, EventRewardEffect> RewardEffectDB;
        
        //DB별 요소 개수
        public int EventsCount => EventDB.Count-1; //0번 인덱스는 비워둠
        public int MainRewardsCount => MainRewardDB.Count;
        public int RewardEffectsCount => RewardEffectDB.Count;
        
        
        public string eventSpirteFolder = "EventSprites"; //todo : 수정 필요

        #region HardCoding 값

        private int EventTable_OptionColumn = 'E' - 'A'; //이벤트 테이블의 option 데이터 시작 열 인덱스
        private List<int> usableEventList = new() {2, 4, 5, 8, 11, 23, 25, 30, 33, 34 };  //현재 기획에서 지정한 사용 가능한 이벤트. 여기 있는 이벤트 내에서만 랜덤으로 이벤트를 선별.
        
        
        #endregion

        //이벤트 관련
        public bool isEventLoadEnd = false;
        public bool isMainRewardLoadEnd = false;
        public bool isRewardEffectLoadEnd = false;
        public bool IsReady => isEventLoadEnd && isMainRewardLoadEnd && isRewardEffectLoadEnd;
        public Action OnDataLoadEnd;
        
        
        
        
        public EventDataBase()
        {
            EventDB = new();
            MainRewardDB = new();
            RewardEffectDB = new();
            
            EventDB.Add(new GameEvent()); //0번은 빈 이벤트를 넣어서 비워두기. event id와 index를 맞추기 위함
        }
        
        
        //데이터 로드가 모두 완료되면 이벤트 호출
        public void CheckAllLoaded()
        {
            if(IsReady)
                OnDataLoadEnd?.Invoke();
        }


        #region DB 데이터 로드
        
        public void LoadEventData(CsvTable table)
        {
            int rowCnt = table.Table.GetLength(0);
            int columnCnt = table.Table.GetLength(1);
            
            for (int r = 1; r < rowCnt; r++)
            {
                //옵션 리스트 생성
                List<Option> options = new();
                for (int i = 0; i < 3; i++)
                {
                    int index = EventTable_OptionColumn + i * 2;
                    options.Add(new Option(
                        int.Parse(table.Table[r, index]),
                        table.Table[r, index + 1]
                    ));
                }
                
                string fileName = table.Table[r, 1]; //image1.png
                string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
                
                //게임 이벤트 클래스 생성 및 파싱
                GameEvent gameEvent = new(
                    int.Parse(table.Table[r, 0]),
                    table.Table[r, 2].Trim(),
                    table.Table[r, columnCnt - 1],
                    options,
                    Resources.Load<Sprite>(Path.Combine(eventSpirteFolder, fileNameNoExt))
                );
                
                

                EventDB.Add(gameEvent);
            }
            
            Debug.Log(EventDB.Count + "개 이벤트 데이터를 로드했습니다.");
            isEventLoadEnd = true;
            CheckAllLoaded();

        }

        public void LoadMainRewardData(CsvTable table)
        {
            int rowCnt = table.Table.GetLength(0);
            int columnCnt = table.Table.GetLength(1);
            
            for (int r = 1; r < rowCnt; r++)
            {
                int key = int.Parse(table.Table[r, 0]);
                //EventMainReward(메인 보상 테이블 파싱)
                EventMainReward mainReward = new(
                    key,
                    table.Table[r,1],
                    (EffectType)(int.Parse(table.Table[r,2].Substring(0,2))),
                    int.Parse(table.Table[r, 3]),
                    table.Table[r, 4]
                );
                MainRewardDB.Add(key, mainReward);
            }
            Debug.Log(MainRewardDB.Count + "개 메인 보상 데이터를 로드했습니다.");
            isMainRewardLoadEnd = true;
            CheckAllLoaded();
        }
        
        public void LoadRewardEffectData(CsvTable table)
        {
            int rowCnt = table.Table.GetLength(0);
            int columnCnt = table.Table.GetLength(1);
            
            for (int r = 1; r < rowCnt; r++)
            {
                int key = int.Parse(table.Table[r, 0]);
                //EventRewardEffect(보상 효과 테이블 파싱)
                EventRewardEffect rewardEffect = new();
                
                //id
                rewardEffect.RewardEffectID = key;
                
                //subEffects
                List<SubEffect> subEffects = new();
                for (int c = 1; c < 10; c += 3)
                {
                    string rawData = table.Table[r, c];

                    int type = (int)(SubEffectType.NoEffect);
                    bool hasType = !string.IsNullOrEmpty(rawData) 
                                   && rawData.Length >= 2 
                                   &&
                        int.TryParse(rawData.Substring(0, 2), out type);
                    bool hasValue = int.TryParse(table.Table[r, c+1], out int value);
                    bool hasDurtaion = int.TryParse(table.Table[r, c+2], out int duration);
                    if (hasType)
                    {
                        SubEffect subEffect = (SubEffectType)type switch
                        {
                            SubEffectType.NoEffect => new NoEffect(),
                            SubEffectType.AttackBoost => new AttackBoostEffect(value, duration),
                            SubEffectType.ResourceGain => new GoldGainEffect(value),
                            SubEffectType.ResourceLoss => new GoldLossEffect(value),
                            SubEffectType.ObtainItem => new ObtainItemEffect(value,int.Parse(table.Table[r,columnCnt-2])==1),
                            SubEffectType.ObtainEnhancedCard => new ObtainCardEffect(value),
                        };
                            
                        subEffects.Add(subEffect);
                    }
                    else
                    {
                        break;
                    }
                }
                rewardEffect.SubEffectList = subEffects;
                    

                //패널티 비용
                rewardEffect.PenaltyCost =
                    int.TryParse(table.Table[r, columnCnt - 4], out int penalty) ? penalty : null;
                    
                //패널티 대체 비용
                rewardEffect.SubstituteCost = int.TryParse(table.Table[r, columnCnt - 3], out int subCost) ? subCost : null;
                
                
                RewardEffectDB.Add(key, rewardEffect);
                
            }
            //4000번 아무 효과 없음 추가
            EventRewardEffect noEffect = new EventRewardEffect();
            noEffect.RewardEffectID = 4000;
            List<SubEffect> effects = new();
            effects.Add(new NoEffect());
            noEffect.SubEffectList = effects;
            RewardEffectDB.Add(4000, noEffect);
            
            
            Debug.Log(RewardEffectDB.Count + "개 보상 효과 데이터를 로드했습니다.");
            isRewardEffectLoadEnd = true;
            CheckAllLoaded();
        }
        
        #endregion


        #region DB API
        /// <summary>
        /// 가능한 모든 이벤트들의 이름을 출력. 기획에서 지정한 이벤트들이 제대로 매칭되는지 확인용(디버깅용)
        /// </summary>
        public void PrintUsableEventsName()
        {
            foreach (int id in usableEventList)
            {
                Debug.Log(EventDB[id].EventName);
            }
        }

        /// <summary>
        /// 기획에서 지정한 사용 가능한 이벤트 중에서 랜덤으로 하나를 선택해서 반환
        /// </summary>
        /// <returns></returns>
        public GameEvent GetRandomEvent()
        {
            int roll = Manager.randomManager.RandInt(0, usableEventList.Count);
            int eventID = usableEventList[roll];
            return EventDB[eventID];
        }

        
        /// <summary>
        /// Game Event의 아이디로 EventDB에서 데이터를 조회해서 반환한다.(EventDB의 인덱스와 아이템 아이디 동일)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameEvent GetGameEventById(int id )
        {
            return EventDB.FirstOrDefault(e => e.EventID == id);
        }

        public EventMainReward GetMainRewardById(int id)
        {
            return MainRewardDB.TryGetValue(id, out EventMainReward mainReward) ? mainReward : null;
        }

        public int GetRewardEffectId(int mainRewardID)
        {
            return MainRewardDB.TryGetValue(mainRewardID, out EventMainReward mainReward) ? mainReward.RewardEffectID : -1;
        }

        public EventRewardEffect GetRewardEffectById(int id)
        {
            return RewardEffectDB.TryGetValue(id, out EventRewardEffect rewardEffect) ? rewardEffect : null;
        }

        public List<SubEffect> GetSubEffectsByRewardId(int rewardEffectId)
        {
            return RewardEffectDB.TryGetValue(rewardEffectId, out EventRewardEffect rewardEffect) ? rewardEffect.SubEffectList : null;
        }

        public EventRewardEffect GetRewardEffectByMainRewardId(int mainRewardId)
        {
            return GetRewardEffectById(GetRewardEffectId(mainRewardId));
        }
        
        #endregion

    }

}
