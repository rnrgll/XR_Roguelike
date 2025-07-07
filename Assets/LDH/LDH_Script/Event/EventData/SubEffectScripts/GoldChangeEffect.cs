using Managers;

namespace Event
{
    public class GoldGainEffect : SubEffect
    {
        public GoldGainEffect(int value) : base(SubEffectType.ResourceGain, value) { }

        public override void ApplyEffect()
        {
            base.ApplyEffect();
            Manager.GameState.AddGold(Value);
        }
    }
}