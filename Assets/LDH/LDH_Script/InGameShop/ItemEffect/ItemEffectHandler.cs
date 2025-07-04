using InGameShop;
using UnityEngine;

namespace Item
{
    public class ItemEffectHandler
    {
        public ItemEffectHandler(PlayerController player)
        {
            _player = player;
        }

        private PlayerController _player;
        
        public void ApplyEffect(InventoryItem item)
        {
            //todo:고치기
            ItemEffect effect = item.effectGroups[1].effects[0];
            //if (!TryChance(effect.chance)) return;
            switch (effect.effectType)
            {
                case EffectType.Heal:
                    ApplyHeal(effect);
                    break;

                case EffectType.HPReduce:
                    //RunHPReduce(effect);
                    break;

                case EffectType.BuffAttack:
                    //RunBuffAttack(effect);
                    break;

                case EffectType.DrawCard:
                    //RunDrawCard(effect);
                    break;

                case EffectType.Invincible:
                    //RunInvincible(effect);
                    break;

                case EffectType.GainJoker:
                    //RunGainJoker(effect);
                    break;

                default:
                    Debug.LogWarning($"[ItemEffectRunner] 알 수 없는 효과 타입: {effect.effectType}");
                    break;
            }
        }

        private void ApplyHeal(ItemEffect effect)
        {
            if (effect.duration <= 1)
            {
                if (effect.value!=0)
                    _player.ChangeHp(effect.value);
                else
                    _player.ChangeHpByPercent(effect.percentValue);
            }
            else
            if (effect.value!=0)
                _player.AddHealBuff(effect.value, effect.duration);
            else
                _player.AddHealBuff(effect.percentValue, effect.duration);
        }

        // private void RunHPReduce(Effect effect)
        // {
        //     _playerStatus.ReduceHP(ParseValue(effect.value));
        // }
        //
        // private void RunBuffAttack(Effect effect)
        // {
        //     _buffManager.ApplyAttackBuff(ParseValue(effect.value), effect.duration);
        // }
        //
        // private void RunDrawCard(Effect effect)
        // {
        //     _cardManager.DrawCard(ParseValue(effect.value));
        // }
        //
        // private void RunGainJoker(Effect effect)
        // {
        //     _jokerDeck.TryAddJoker(effect.value);
        // }
        //
        // private void RunInvincible(Effect effect)
        // {
        //     _buffManager.ApplyInvincible(effect.duration);
        // }
        //
        
        private bool TryChance(float chance)
        {
            return UnityEngine.Random.value <= chance;
        }

    }
}