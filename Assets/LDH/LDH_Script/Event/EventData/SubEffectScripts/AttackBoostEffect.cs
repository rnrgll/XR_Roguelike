namespace Event
{
    public class AttackBoostEffect : SubEffect
    {
        public AttackBoostEffect(int value, int? durationTurns) : base(SubEffectType.AttackBoost, value, durationTurns) { }

        public override void ApplyEffect()
        {
            base.ApplyEffect();
            //todo: player attack bost 적용
            var player = TurnManager.Instance.GetPlayerController();
            player.ApplyFlatAttackBuff(Value, DurationTurns??1);
        }
    }
}