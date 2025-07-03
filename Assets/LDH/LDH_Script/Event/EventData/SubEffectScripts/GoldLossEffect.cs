using Managers;

namespace Event
{
    public class GoldLossEffect : SubEffect
    {
        public GoldLossEffect(int value) : base(SubEffectType.ResourceGain, value) { }
        public override void ApplyEffect()
        {
            base.ApplyEffect();
            
            Manager.GameState.AddGold(-Value);
        }
    }
}