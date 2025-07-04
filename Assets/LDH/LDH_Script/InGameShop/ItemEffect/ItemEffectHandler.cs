using InGameShop;
using Managers;
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
        
        public void ApplyEffect(ItemEffect effect)
        {
            _player ??= Manager.turnManager.GetPlayerController();
            //todo:고치기
            if (effect.effectType == EffectType.None)
            {
               
                Debug.Log($"[아이템] 효과 적용 : {effect.effectType.ToString()}");
                return;
            }
            
            Debug.Log($"[아이템] 효과 적용 : {effect.effectType.ToString()}, value : {effect.value}, percent value : {effect.percentValue}, turn : {effect.duration}");
            
            switch (effect.effectType)
            {
                
                case EffectType.Heal:
                    ApplyHeal(effect);
                    break;

                case EffectType.HPReduce:
                    //RunHPReduce(effect);
                    break;

                case EffectType.BuffAttack:
                    ApplyBuffAttack(effect);
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
        private void ApplyBuffAttack(ItemEffect effect)
        {
            _player.AddAttackBuff(effect.value, effect.duration);
        }
        
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
        

    }
}