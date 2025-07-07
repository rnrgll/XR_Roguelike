using System;

namespace Event
{
    public class AttackBoostEffect : SubEffect
    {
        public AttackBoostEffect(int value, int? durationTurns) : base(SubEffectType.AttackBoost, value, durationTurns) { }

        public override void ApplyEffect(Action onComplete)
        {
            base.ApplyEffect(null);
            //todo: player attack bost 적용 테스트 필요 
            var player = TurnManager.Instance.GetPlayerController();
            player.AddAttackBuff(Value, DurationTurns ?? 1);

            onComplete?.Invoke();
        }
    }
}