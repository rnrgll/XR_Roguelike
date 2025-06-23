using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{

    public class Room
    {
        public enum Type
        {
            NotAssgined,
            Battle, // 전투
            Shop, // 상점
            Rest, // 휴식
            Event, // 이벤트
            Boss, // 보스
        }

        public Type type;
        public int row, column;
        public Vector2 position;
        public List<Room> nextRooms;
        public bool selected;


        //기본 생성자
        public Room() { }

        public Room(int row, int column, Vector2 position)
        {
            //this.type = type;
            this.row = row;
            this.column = column;
            this.position = position;
            nextRooms = new();
            selected = false;
        }


        //debug----------
        public override string ToString()
        {
            return $"{column} ({type.ToString()[0]})";
        }
    }
}