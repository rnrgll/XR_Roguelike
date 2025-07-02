using CustomUtility.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Event
{
    public class EventDataBase
    {
        public List<GameEvent> EventDB { get; private set; }
        public int TotalEventCount => EventDB.Count-1; //0번 인덱스는 비워둠
        public string eventSpirteFolder = "EventSprites"; //todo : 수정 필요

        #region HardCoding 값...

        private int EventTable_OptionColumn = 'E' - 'A'; //이벤트 테이블의 option 데이터 시작 열 인덱스

        #endregion
        
        public EventDataBase()
        {
            EventDB = new();
            EventDB.Add(new GameEvent()); //0번은 빈 이벤트를 넣어서 비워두기. event id와 index를 맞추기 위함
        }
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
            
            Debug.Log(EventDB.Count.ToString() + "개 이벤트 데이터를 로드했습니다.");

        }

        /// <summary>
        /// Game Event의 아이디로 EventDB에서 데이터를 조회해서 반환한다.(EventDB의 인덱스와 아이템 아이디 동일)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameEvent GetGameEventById(int id )
        {
            return EventDB[id];
        }
        
    }

}
