namespace Buffs
{
    public class Buff
    {
        public int value;
        public float percentValue;
        public int remainTurn;
        public int turn;

        public Buff(int value, int turn)
        {
            this.value = value;
            this.turn = turn;
            remainTurn = turn;
            percentValue = 0;
        }

        public Buff(float percentValue, int turn)
        {
            this.percentValue = percentValue;
            this.turn = turn;
            remainTurn = turn;
            value = 0;
        }
    }
}