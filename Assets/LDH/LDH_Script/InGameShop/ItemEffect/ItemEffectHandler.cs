using CardEnum;
using InGameShop;
using Managers;
using System.Collections.Generic;
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
        private CardController _card => _player.GetCardController();
        private List<MinorArcana> previousHand = new();
        
        
        public void ApplyEffect(ItemEffect effect)
        {
            _player ??= Manager.turnManager.GetPlayerController();
            //todo:고치기
            if (effect.effectType == EffectType.None)
            {

                Debug.Log($"[아이템] 효과 적용 : {effect.effectType.ToString()}");
                return;
            }

            Debug.Log(
                $"[아이템] 효과 적용 : {effect.effectType.ToString()}, value : {effect.value}, percent value : {effect.percentValue}, turn : {effect.duration}");

            switch (effect.effectType)
            {

                case EffectType.Heal:
                    ApplyHeal(effect);
                    break;

                case EffectType.HPReduce:
                    ApplyHPReduce(effect);
                    break;

                case EffectType.BuffAttack:
                    ApplyBuffAttack(effect);
                    break;

                case EffectType.DrawCard:
                    ApplyDrawCard(effect);
                    break;
                case EffectType.DiscardHand:
                    previousHand = new List<MinorArcana>(_card.Hand.GetCardList());
                    Debug.Log(previousHand.Count);
                    _player.OnTurnEnd += DiscardHand;
                    break;
                case EffectType.Invincible:
                    ApplyInvincible(effect);
                    break;

                case EffectType.GainJoker:
                    ApplyTryGainJoker(effect);
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
                if (effect.value != 0)
                    _player.ChangeHp(effect.value);
                else
                    _player.ChangeHpByPercent(effect.percentValue);
            }
            else if (effect.value != 0)
            {
                _player.ChangeHp(effect.value);
                _player.AddHealBuff(effect.value, effect.duration - 1);
            }

            else
            {
                _player.ChangeHpByPercent(effect.percentValue);
                _player.AddHealBuff(effect.percentValue, effect.duration - 1);
            }

        }

        private void ApplyHPReduce(ItemEffect effect)
        {
            if (effect.duration <= 1)
            {
                if (effect.value != 0)
                    _player.ChangeHp(-effect.value);
                else
                    _player.ChangeHpByPercent(-effect.percentValue);
            }
            else if (effect.value != 0)
                _player.AddHealBuff(-effect.value, effect.duration);
            else
                _player.AddHealBuff(-effect.percentValue, effect.duration);
        }

        private void ApplyBuffAttack(ItemEffect effect)
        {
            _player.AddAttackBuff(effect.value, effect.duration);
        }

        private void ApplyDrawCard(ItemEffect effect)
        {
            _card.Draw(effect.value);
        }

        private void ApplyTryGainJoker(ItemEffect effect)
        {

            //조커 카드 보유 여부 확인
            //battle deck에서 카드 문양이 와일드이고, 카드 넘버가 14 인 카드가 있는지 확인
            //있으면 인덱스 반환, 없으면 -1 반환
            int jockerIndex = _card.BattleDeck.GetCardList()
                .FindIndex(minor => minor.CardNum == 14);

            Debug.Log($"jockerIndex in battle deck : {jockerIndex}");
            //조커 카드 미보유시 아이템 효과 적용 실패
            if (jockerIndex == -1)
            {
                Debug.Log("[아이템] 보유하고 있는 조커가 없습니다. 아이템 효과 적용 실패");
                return;
            }

            //조커 카드 보유시 battle deck에서 해당 카드를 빼서
            //핸드에 넣어주어야 함
            MinorArcana jockerCard = _card.BattleDeck.GetCard(jockerIndex);
            _card.BattleDeck.Remove(jockerCard);
            _card.Draw(jockerCard);

        }

        private void ApplyInvincible(ItemEffect effect)
        {
            Debug.Log("[아이템] 무적 상태를 1턴 동안 적용합니다.");
            _player.SetInvincible();
        }

        private void DiscardHand()
        {
            Debug.Log("[아이템 효과] 남은 핸드를 전부 버립니다.");
            Debug.Log(_card.Hand.GetCardList().Count);
            List<MinorArcana> currentHand = new(_card.Hand.GetCardList());
            List<MinorArcana> discardList = new();
            foreach (var card in previousHand)
            {
                if(currentHand.Contains(card))
                    discardList.Add(card);
            }
            Debug.Log(discardList.Count);
            _card.Discard(discardList); //손패에 있는 카드 전부 버리기
            
            previousHand.Clear();
            _player.OnTurnEnd -= DiscardHand; //등록한거 삭제
        }

    }

}