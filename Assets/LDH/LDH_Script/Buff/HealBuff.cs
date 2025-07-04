namespace Buff
{
    public class HealBuff
    {
        public int value;
        public float percentValue;
        public int remainTurn;
        public int turn;

        public HealBuff(int value, int turn)
        {
            this.value = value;
            this.turn = turn;
            remainTurn = turn;
            percentValue = 0;
        }

        public HealBuff(float percentValue, int turn)
        {
            this.percentValue = percentValue;
            this.turn = turn;
            remainTurn = turn;
            value = 0;
        }
    }
}