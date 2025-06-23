using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map
{
    public class MapGenerator : MonoBehaviour
    {
        [Header("Position")]
        [SerializeField] private int _xDist = 30;
        [SerializeField] private int _yDist = 25;
        [SerializeField] private int _placementRandomness = 5;
        
        [Header("Map Setting")]
        [SerializeField] private int _floors = 16;
        [SerializeField] private int _mapWidth = 7;
        [SerializeField] private int _paths = 6;
        
        [Header("Weights")]
        [SerializeField] private float _BattleRoomWeight = 10.0f;
        [SerializeField] private float _ShopRoomWeight = 2.5f;
        [SerializeField] private float _RestRoomWeight = 4.0f;
        //[SerializeField] private float _EventRoomWeight = 2.0f;
        
        private List<float> _randomRoomTypeWeights = new()
        {
            0.0f, //(Room.Type.NotAssgined
            0.0f, //Room.Type.Battle, 
            0.0f, //Room.Type.Shop,
            0.0f, //Room.Type.Rest, 
            0.0f, //Room.Type.Event, 
            0.0f, //Room.Type.Boss, 
        };

        private float _randomRoomTypeTotalWeight = 0.0f;

        private List<List<Room>> _mapData;


        private void Awake() => GenerateMap();

        private void Start()
        {
            PrintMap();
        }


        //맵 생성
        public List<List<Room>> GenerateMap()
        {
            //1. Grid 생성
            _mapData = GenerateInitialGrid();
            //2. start point 랜덤으로 지정
            List<int> startingPoints = GetRandomStartingPoints();
            
            //3. Path 생성
            for (int j = 0; j < startingPoints.Count; j++)
            {
                int currentJ = j;
                for (int i = 0; i < _floors - 1; i++)
                {
                    currentJ = SetUpConnection(i, currentJ);
                }
            }
            
            //4. Room type 지정
            SetUpBossRoom();
            SetUpRandomRoomWeights();
            SetUpRoomType();
            
            
            return _mapData;
        }
        
        
        //Grid 생성
        private List<List<Room>> GenerateInitialGrid()
        {
            List<List<Room>> grid = new(_floors);

            for (int i = 0; i < _floors; i++)
            {
                List<Room> floorRooms = new List<Room>(_mapWidth);
                for (int j = 0; j < _mapWidth; j++)
                {
                    Vector2 offset = new Vector2(Random.Range(0, 1), Random.Range(0, 1)) * _placementRandomness;

                    Vector2 position = new Vector2(j * _xDist, i * _yDist) + offset;
                    
                    Room currentRoom = new Room(i,j,position);
                    
                    //boss room 일 경우 position.y 값을 고정시킨다. (마지막 floor)
                    if (i == _mapWidth - 1)
                        currentRoom.position.y = (i + 1) * _yDist;
                    floorRooms.Add(currentRoom);
                }
                grid.Add(floorRooms);
            }
            return grid;
        }

        
        //시작 포인트 랜덤으로 지정하기
        //최소 2개 이상의 unique 포인트를 지정해야한다.
        private List<int> GetRandomStartingPoints()
        {
            List<int> yCoordinates = new();
            int uniquePoints = 0;

            while (uniquePoints < 2)
            {
                uniquePoints = 0;
                yCoordinates.Clear();

                for (int i = 0; i < _paths; i++)
                {
                    int startingPoint = Random.Range(0, _mapWidth);
                    if (!yCoordinates.Contains(startingPoint))
                        uniquePoints += 1;
                    yCoordinates.Add(startingPoint);
                } 
            }

            return yCoordinates;
        }


        private int SetUpConnection(int i, int j)
        {
            Room nextRoom = null;
            Room currentRoom = _mapData[i][j];
            
            //교차 가능성 확인, next room이 null이 아닐 때까지 다음 방 후보 찾기 
            while (nextRoom == null || IsCrossingPath(i, j, nextRoom))
            {
                int randomJ = Mathf.Clamp(Random.Range(j - 1, j + 2), 0, _mapWidth - 1);
                nextRoom = _mapData[i + 1][randomJ];
            }
            currentRoom.nextRooms.Add(nextRoom);

            return nextRoom.column;
        }

        private bool IsCrossingPath(int i, int j, Room room)
        {
            Room leftNeighbor = null;
            Room rightNeighbor = null ;
            
            //j = 0 이면, left neighbor 없음
            if (j > 0)
                leftNeighbor = _mapData[i][j - 1];
            //j = mapWidth -1 이면, right neighbor 없음
            if (j < _mapWidth - 1)
                rightNeighbor = _mapData[i][j + 1];
            
            //cross 여부 판단
            //1) 오른쪽 이웃이 왼쪽으로 진행하는 path가 있으면 현재 room에서 오른쪽 방향으로 진행 불가
            if (rightNeighbor != null && room.column > j)
            {
                foreach (var nextRoom in rightNeighbor.nextRooms)
                {
                    if (nextRoom.column < room.column)
                        return true;
                }
            }
            //2) 왼쪽 이웃이 오른쪽으로 진행하는 path가 있으면 현재 room에서 왼쪽 방향으로 진행 불가
            if (leftNeighbor != null && room.column < j)
            {
                foreach (Room nextRoom in leftNeighbor.nextRooms)
                {
                    if (nextRoom.column > room.column)
                        return true;
                }
            }

            return false;
        }

        
        //- 보스룸 이전 floor와 보스 룸 connection 설정
        //- room type 설정
        private void SetUpBossRoom()
        {
            int middle = Mathf.FloorToInt(_mapWidth * 0.5F);
            Room bossRoom = _mapData[_floors - 1][middle];

            for (int j = 0; j < _mapWidth; j++)
            {
                Room currentRoom = _mapData[_floors - 2][j];
                if (currentRoom.nextRooms != null)
                {
                    currentRoom.nextRooms.Add(bossRoom);
                }
            }

            bossRoom.type = Room.Type.Boss;
        }

        private void SetUpRandomRoomWeights()
        {
            _randomRoomTypeWeights[(int)Room.Type.Battle] = _BattleRoomWeight;
            _randomRoomTypeWeights[(int)Room.Type.Rest] = _BattleRoomWeight + _RestRoomWeight;
            _randomRoomTypeWeights[(int)Room.Type.Shop] = _BattleRoomWeight + _RestRoomWeight + _ShopRoomWeight;
            //_randomRoomTypeWeights[Room.Type.Event] = _BattleRoomWeight + _RestRoomWeight + _ShopRoomWeight + _EventRoomWeight;
            _randomRoomTypeTotalWeight = _randomRoomTypeWeights[(int)Room.Type.Shop];
            
        }


        private void SetUpRoomType()
        {
            //규칙에 맞게 설정
            //1. 첫 번째 방은 무조건 battle room
            foreach (Room room in _mapData[0])
            {
                if (room.nextRooms.Count > 0)
                    room.type = Room.Type.Battle;
            }
            
            //2. 커스텀 규칙
            //예) 9th floor(floors / 2)는 항상 shop
            foreach (Room room in _mapData[_floors/2])
            {
                if (room.nextRooms.Count > 0)
                    room.type = Room.Type.Shop;
            }
            //예) 4th floor는 항상 Event 발생
            foreach (Room room in _mapData[3])
            {
                if (room.nextRooms.Count > 0)
                    room.type = Room.Type.Event;
            }
            
            //예) 보스 룸 이전 마지막 floor는 항상 rest room
            foreach (Room room in _mapData[_floors-2])
            {
                if (room.nextRooms.Count > 0)
                    room.type = Room.Type.Rest;
            }
            
            //3. 나머지 랜덤
            foreach (List<Room> currentFloor in _mapData)
            {
                foreach (Room room in currentFloor)
                {
                    //사용하는 방인데 타입이 정해지지 않은 경우
                    if (room.nextRooms.Count > 0 && room.type == Room.Type.NotAssgined)
                        SetRoomRandomly(room);
                }
            }
        }


        private void SetRoomRandomly(Room roomToSet)
        {
            //룰 기반으로 한 플래그 설정
            //예) 4층 이내에 rest room이 있으면 안된다, 연속으로 rest room 배치 불가능, 연속으로 shop room 배치 불가능, 보스룸 이전에 rest room을 강제로 배치해서 floors-3는 rest room이면 안됨

            bool restBelow4 = true;
            bool consecutiveRest = true;
            bool consecutiveShop = true;
            //bool consecutiveEvent = true;
            bool restOn13 = true;

            Room.Type typeCandidate = Room.Type.NotAssgined;
            while (restBelow4 || consecutiveRest || consecutiveShop || restOn13)
            {
                typeCandidate = GetRandomRoomTypeByWeight();
                
                //--------------flag 체크-------------
                bool isRestRoom = typeCandidate == Room.Type.Rest;
                bool hasRestRoomParent = HasParentOfType(roomToSet, Room.Type.Rest);
                bool isShop = typeCandidate == Room.Type.Shop;
                //bool isEvent = typeCandidate
                bool haseShopRoomParent = HasParentOfType(roomToSet, Room.Type.Shop);
                
                //rule을 어기지 않으면 아래 플래그들은 모두 false로 설정됨
                restBelow4 = isRestRoom && roomToSet.row < 3;
                consecutiveRest = isRestRoom && hasRestRoomParent;
                consecutiveShop = isShop && haseShopRoomParent;
                restOn13 = isRestRoom && roomToSet.row == 12;

            }
            
            //rule을 어기지 않는 type candidate 획득
            roomToSet.type = typeCandidate;

        }

        private bool HasParentOfType(Room room, Room.Type roomType)
        {
            List<Room> parent = new();
            //left parent
            if (room.column > 0 && room.row > 0)
            {
                Room parentCandidate = _mapData[room.row - 1][room.column - 1];
                if(parentCandidate.nextRooms.Contains(room))
                    parent.Add(parentCandidate);
            }
            //below parent
            if (room.column > 0)
            {
                Room parentCandidate = _mapData[room.row - 1][room.column];
                if(parentCandidate.nextRooms.Contains(room))
                    parent.Add(parentCandidate);
            }
            //right parent
            if (room.column < _mapWidth-1 && room.row > 0)
            {
                Room parentCandidate = _mapData[room.row - 1][room.column + 1];
                if(parentCandidate.nextRooms.Contains(room))
                    parent.Add(parentCandidate);
            }
            
            foreach (Room pRoom in parent)
            {
                if (pRoom.type == roomType) 
                    return true;
            }

            return false;

        }

        private Room.Type GetRandomRoomTypeByWeight()
        {
            float roll = Random.Range(0f, _randomRoomTypeTotalWeight);

            for( int i = 0; i<_randomRoomTypeWeights.Count; i++)
            {
                float typeWeight = _randomRoomTypeWeights[i];
                if (typeWeight > roll)
                    return (Room.Type)i;
            }
            
            return Room.Type.Battle;
        }


        private void PrintMap()
        {
            List<string> printStr = new List<string>();
            for (int i = 0; i < _floors; i++)
            {
                printStr.Add($"floor {i} : ");
                for (int j = 0; j < _mapData[i].Count; j++)
                {
                    printStr.Add(_mapData[i][j].ToString());
                    if(j!=_mapData.Count-1)
                        printStr.Add(", ");
                    
                }
                printStr.Add("\n");
            }

            string result = string.Join("", printStr);
            Debug.Log(result);
        }

    }
}