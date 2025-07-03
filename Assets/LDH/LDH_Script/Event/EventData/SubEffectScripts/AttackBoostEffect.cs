namespace Event
{
    public class AttackBoostEffect : SubEffect
    {
        public AttackBoostEffect(int value, int? durationTurns) : base(SubEffectType.AttackBoost, value, durationTurns) { }

        public override void ApplyEffect()
        {
            base.ApplyEffect();
        }
    }
}