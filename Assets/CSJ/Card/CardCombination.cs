using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

public static class CardCombination
{
    public static CardCombinationEnum CalCombination(List<MinorArcana> cards, out List<int> cardNums)
    {
        int[] SuitNum = new int[4];
        int wildCount = 0;
        int[] numbers = new int[13];
        int JokerNum = 0;
        int JokerFiveNum = 14;
        bool IsEnd = false;
        bool JokerUsedTriple = false;
        Dictionary<CardCombinationEnum, bool> IsComb = new Dictionary<CardCombinationEnum, bool>();
        cardNums = new List<int>(5);

        //TODO : 플레이어 스탯과의 연계?
        int nowFlushNum = 5;
        int nowStraightNum = 5;
        foreach (CardCombinationEnum comb in Enum.GetValues(typeof(CardCombinationEnum)))
        {
            IsComb[comb] = false;
        }


        #region card의 숫자와 문양을 개별 저장
        foreach (MinorArcana card in cards)
        {
            if (card.Enchant.enchantInfo == CardEnchant.Wild ||
            card.CardSuit == MinorSuit.wildCard) wildCount++;
            else if (card.CardSuit == MinorSuit.Special) continue;
            else
                SuitNum[(int)card.CardSuit]++;

            if (card.CardNum == 14)
            {
                JokerNum++;
                continue;
            }
            numbers[card.CardNum - 1]++;
        }
        #endregion

        #region flush 체크
        foreach (int _StNum in SuitNum)
        {
            if (_StNum + wildCount >= nowFlushNum)
            {
                IsComb[CardCombinationEnum.Flush] = true;
                break;
            }
        }
        #endregion

        #region 파이브조커 확인
        CheckFiveJoker(JokerNum, JokerFiveNum, cardNums, IsComb, ref IsEnd);
        #endregion

        #region 스트레이트 확인
        CheckStraight(JokerNum, nowStraightNum, cardNums, numbers, IsComb, ref IsEnd);
        #endregion


        int i = 1;
        // card들의 숫자를 확인
        #region 숫자 족보 확인
        foreach (int _num in numbers)
        {
            if (IsEnd) break;
            #region 페어 확인
            if (_num == 2)
            {
                if (!IsComb[CardCombinationEnum.OnePair])
                {
                    CheckOnePair(JokerNum, _num, ref JokerUsedTriple, cardNums, IsComb, ref IsEnd);
                }
                else
                {
                    CheckTwoPair(JokerNum, _num, ref JokerUsedTriple, cardNums, IsComb, ref IsEnd);
                }
            }
            #endregion

            #region 트리플 확인
            // 해당 숫자가 3개일 경우 = 트리플
            else if (_num == 3)
            {
                CheckTriple(JokerNum, _num, cardNums, IsComb, ref IsEnd);
            }
            #endregion

            #region 포카드 확인
            // 해당 숫자가 4개인 경우 = 포카드
            else if (_num == 4)
            {
                CheckFourCard(JokerNum, _num, cardNums, IsComb, ref IsEnd);
            }
            #endregion
            // 다음 숫자 배열에 대한 검사를 진행한다.
            i++;
        }
        #endregion

        #region return region
        if (IsComb[CardCombinationEnum.FiveJoker]) return CardCombinationEnum.FiveJoker;

        if (IsComb[CardCombinationEnum.FiveCard]) return CardCombinationEnum.FiveCard;

        if (IsComb[CardCombinationEnum.Straight] && IsComb[CardCombinationEnum.Flush])
            return CardCombinationEnum.StraightFlush;

        if (IsComb[CardCombinationEnum.FourCard])
            return CardCombinationEnum.FourCard;

        if (IsComb[CardCombinationEnum.FullHouse])
            return CardCombinationEnum.FullHouse;

        if (IsComb[CardCombinationEnum.Flush])
        {
            int j = 0;
            cardNums.Clear();
            foreach (int _num in numbers)
            {
                if (_num == 0) continue;
                for (int x = 0; x < _num; x++)
                {
                    cardNums.Add(j);
                }
                j++;
                if (cardNums.Count >= 5) break;
            }
            return CardCombinationEnum.Flush;
        }
        if (IsComb[CardCombinationEnum.Straight])
            return CardCombinationEnum.Straight;

        if (IsComb[CardCombinationEnum.Triple])
            return CardCombinationEnum.Triple;

        if (IsComb[CardCombinationEnum.TwoPair])
            return CardCombinationEnum.TwoPair;

        if (IsComb[CardCombinationEnum.OnePair])
            return CardCombinationEnum.OnePair;

        int k = 0;
        int Max = 0;
        foreach (int _num in numbers)
        {
            if (_num != 0)
            {
                Max = _num;
            }
            k++;
        }
        cardNums.Add(Max);

        // 마지막으로 조커가 있는지 체크
        if (JokerNum == 1)
        {
            cardNums.Add(Max);
            return CardCombinationEnum.OnePair;
        }
        else if (JokerNum == 2)
        {
            for (int re = 0; re < 2; re++)
            {
                cardNums.Add(Max);
            }
            return CardCombinationEnum.Triple;
        }
        else if (JokerNum == 3)
        {
            for (int re = 0; re < 3; re++)
            {
                cardNums.Add(Max);
            }
            return CardCombinationEnum.FourCard;
        }
        else if (JokerNum == 4)
        {
            for (int re = 0; re < 4; re++)
            {
                cardNums.Add(Max);
            }
            return CardCombinationEnum.FiveCard;
        }
        else return CardCombinationEnum.HighCard;
        #endregion
    }

    private static void CheckFiveJoker(int Joker, int JokerFiveNum, List<int> cardNum, Dictionary<CardCombinationEnum, bool> IsComb, ref bool IsEnd)
    {
        if (Joker == 5)
        {
            IsComb[CardCombinationEnum.FiveJoker] = true;
            int s = JokerFiveNum;
            // TODO : 현재 조커의 값을 14로 설정
            cardNum.AddRange(new List<int> { s, s, s, s, s });
            IsEnd = true;
        }
        else IsComb[CardCombinationEnum.FiveJoker] = false;

    }

    // TODO: 현재 straightCrit값만 넘으면 작은거부터 기준값만큼의 값을 반환해서 만약 더커도 작은 배열로 반환한다.
    // 이부분을 수정해야함
    private static void CheckStraight(int Joker, int straightCrit, List<int> cardNum, int[] numbers, Dictionary<CardCombinationEnum, bool> IsComb, ref bool IsEnd)
    {
        int StraightNum = 0;
        int StartStraightNum = 0;
        int UsedJoker = 0;
        int i = 1;
        foreach (int _num in numbers)
        {
            if (_num > 1)
            {
                break;
                //TODO: 순서 조정으로 페어 이상이 존재할 경우 CheckStraight로 들어오지 않도록 조정
            }

            // 해당 숫자가 1개일 경우
            if (_num == 1)
            {
                // 만약 스트레이트의 시작일 경우 스트레이트의 시작 넘버를 기록한다.
                if (StraightNum == 0) StartStraightNum = i;
                // 연속되는 숫자를 나타내는 straightNum을 +해준다.
                StraightNum++;
                // 만일 straightNum이 현재 스트레이트의 수치를 만족하면 스트레이트로 판별하고 다음으로 이동
                // 만약 10,J,Q,K,A가 있을 경우에도 Straight 반환
                if (StraightNum >= straightCrit || (i == 13 && StraightNum == straightCrit - 1 && numbers[0] == 1))
                {

                    IsComb[CardCombinationEnum.Straight] = true;

                    if (i == 13 && StraightNum == straightCrit - 1 && numbers[0] == 1)
                    {
                        for (int sNum = StartStraightNum; sNum < StartStraightNum + straightCrit - 1; sNum++)
                        {
                            cardNum.Add(sNum);
                        }
                        cardNum.Add(1);
                    }
                    else
                    {
                        for (int sNum = StartStraightNum; sNum < StartStraightNum + straightCrit; sNum++)
                        {
                            cardNum.Add(sNum);
                        }
                    }
                    break;
                }
            }
            // 스트레이트에 사용할 수 있는 조커가 있는 경우
            else if (Joker > UsedJoker)
            {
                UsedJoker += 1;
                StraightNum++;
                if (StraightNum >= straightCrit)
                {
                    IsComb[CardCombinationEnum.Straight] = true;
                    IsEnd = true;
                    break;
                }
            }
            else
            {
                // 1개가 아닐경우 스트레이트에 관한 항목을 초기화한다.
                StartStraightNum = 0;
                StraightNum = 0;
                UsedJoker = 0;
                IsComb[CardCombinationEnum.Straight] = false;
                break;
            }
        }
    }


    private static void CheckOnePair(int Joker, int _cardNum, ref bool JokerUsedTriple, List<int> cardNums, Dictionary<CardCombinationEnum, bool> IsComb, ref bool IsEnd)
    {
        IsEnd = false;
        // 만약 조커가 없을 경우
        if (Joker < 1)
        {
            // 해당 페어 항목에 채워넣고 일단 OnePair를 true로 만든다.
            IsComb[CardCombinationEnum.OnePair] = true;
            for (int re = 0; re < 2; re++)
            {
                cardNums.Add(_cardNum);
            }

            // 트리플을 이미 판별한 경우 풀하우스 이므로 풀하우스로 체크한 후 나간다.
            if (!IsComb[CardCombinationEnum.Triple])
            {
                // 조커를 사용한 트리플일 경우
                // 현재의 숫자가 이전 페어의 숫자보다 크므로
                // 현재 페어의 숫자로 조커를 대체해 준다.
                if (JokerUsedTriple)
                {
                    cardNums[2] = _cardNum;
                }
                // 풀하우스를 판별한다.
                IsComb[CardCombinationEnum.FullHouse] = true;
                IsEnd = true;
            }
        }

        else if (Joker == 1)
        {
            //트리플을 체크한다.
            IsComb[CardCombinationEnum.Triple] = true;
            JokerUsedTriple = true;
            for (int re = 0; re < 3; re++)
            {
                cardNums.Add(_cardNum);
            }
        }
        else if (Joker == 2)
        {
            //포카드를 체크하고 탈출한다.
            IsComb[CardCombinationEnum.FourCard] = true;
            for (int re = 0; re < 4; re++)
            {
                cardNums.Add(_cardNum);
            }
            IsEnd = true;
        }
        else if (Joker == 3)
        {
            IsComb[CardCombinationEnum.FiveCard] = true;
            for (int re = 0; re < 5; re++)
            {
                cardNums.Add(_cardNum);
            }
            IsEnd = true;
        }
    }


    private static void CheckTwoPair(int Joker, int _cardNum, ref bool JokerUsedTriple, List<int> cardNums, Dictionary<CardCombinationEnum, bool> IsComb, ref bool IsEnd)
    {
        // 투페어의 숫자를 기록하고 투페어로 판별한 후 탈출한다
        // ※ 조커가 0개이고 투페어 일 경우 다른 경우의 수가 없기 때문
        if (Joker < 1)
        {
            IsComb[CardCombinationEnum.TwoPair] = true;
            for (int re = 0; re < 2; re++)
            {
                cardNums.Add(_cardNum);
            }
        }
        else
        {
            //트리플을 체크한다.
            IsComb[CardCombinationEnum.Triple] = true;
            JokerUsedTriple = true;
            for (int re = 0; re < 3; re++)
            {
                cardNums.Add(_cardNum);
            }
            IsComb[CardCombinationEnum.FullHouse] = true;
        }
        IsEnd = true;
    }

    private static void CheckTriple(int Joker, int _cardNum, List<int> cardNums, Dictionary<CardCombinationEnum, bool> IsComb, ref bool IsEnd)
    {
        // 조커가 없을 경우
        if (Joker < 1)
        {
            // 트리플을 true로 하고 해당 숫자를 기록한다.
            IsComb[CardCombinationEnum.Triple] = true;
            for (int re = 0; re < 3; re++)
            {
                cardNums.Add(_cardNum);
            }
            // OnePair가 이미 판별된 경우 풀하우스이므로 이를 체크하고 탈출한다.
            if (IsComb[CardCombinationEnum.OnePair])
            {
                IsComb[CardCombinationEnum.FullHouse] = true;
                IsEnd = true;
            }
        }
        else
        {
            // 조커가 1개라면
            if (Joker == 1)
            {
                // 포카드를 체크한다.
                IsComb[CardCombinationEnum.FourCard] = true;
                for (int re = 0; re < 4; re++)
                {
                    cardNums.Add(_cardNum);
                }
            }
            // 조커가 2개라면 
            else if (Joker == 2)
            {
                //파이브 카드를 체크한다.
                IsComb[CardCombinationEnum.FiveCard] = true;
                for (int re = 0; re < 5; re++)
                {
                    cardNums.Add(_cardNum);
                }
            }
            IsEnd = true;
        }
    }

    private static void CheckFourCard(int Joker, int _cardNum, List<int> cardNums, Dictionary<CardCombinationEnum, bool> IsComb, ref bool IsEnd)
    {
        //조커가 있을 경우 파이브 카드를 체크한다
        if (Joker == 1)
        {
            IsComb[CardCombinationEnum.FiveCard] = true;
            for (int re = 0; re < 5; re++)
            {
                cardNums.Add(_cardNum);
            }
        }
        else
        {
            // 없을 경우 포카드를 체크하고 탈출한다.
            IsComb[CardCombinationEnum.FourCard] = true;
            for (int re = 0; re < 4; re++)
            {
                cardNums.Add(_cardNum);
            }
        }
        IsEnd = true;
    }
}
