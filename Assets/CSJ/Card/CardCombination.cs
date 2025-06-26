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
        bool IsFlush = false;
        bool IsStraight = false;
        bool IsFullHouse = false;
        bool IsOnePair = false;
        bool IsTwoPair = false;
        bool IsTriple = false;
        bool IsFourCard = false;
        bool IsFiveCard = false;
        bool IsFiveJoker = false;
        bool JokerUsedTriple = false;
        cardNums = new List<int>(5);

        //TODO : 플레이어 스탯과의 연계?
        int nowFlushNum = 5;
        int nowStraightNum = 5;

        #region card의 숫자와 문양을 개별 저장
        foreach (MinorArcana card in cards)
        {
            if (card.Enchant.enchantInfo == CardEnchant.Wild) wildCount++;
            else SuitNum[(int)card.CardSuit]++;

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
                IsFlush = true;
                break;
            }
        }
        #endregion

        #region 파이브조커 확인
        CheckFiveJoker(JokerNum, JokerFiveNum, cardNums, out IsFiveJoker);
        #endregion

        #region 스트레이트 확인
        CheckStraight(JokerNum, nowStraightNum, cardNums, numbers, out IsStraight);
        #endregion

        int i = 1;
        // card들의 숫자를 확인
        #region 숫자 족보 확인
        foreach (int _num in numbers)
        {
            #region 페어 확인
            // 해당 숫자가 2개일 경우 = 페어
            if (_num == 2)
            {
                // 원페어가 성립되지 않았던 경우 
                if (!IsOnePair)
                {
                    // 만약 조커가 없을 경우
                    if (JokerNum < 1)
                    {
                        // 해당 페어 항목에 채워넣고 일단 OnePair를 true로 만든다.
                        IsOnePair = true;
                        for (int re = 0; re < 2; re++)
                        {
                            cardNums.Add(i);
                        }
                        // 트리플을 이미 판별한 경우 풀하우스 이므로 풀하우스로 체크한 후 나간다.
                        if (IsTriple)
                        {
                            // 조커를 사용한 트리플일 경우
                            // 현재의 숫자가 이전 페어의 숫자보다 크므로
                            // 현재 페어의 숫자로 조커를 대체해 준다.
                            if (JokerUsedTriple)
                            {
                                cardNums[2] = i;
                            }
                            // 풀하우스를 판별한다.
                            IsFullHouse = true;
                            break;
                        }
                    }

                    else if (JokerNum == 1)
                    {
                        //트리플을 체크한다.
                        IsTriple = true;
                        JokerUsedTriple = true;
                        for (int re = 0; re < 3; re++)
                        {
                            cardNums.Add(i);
                        }
                    }
                    else if (JokerNum == 2)
                    {
                        //포카드를 체크하고 탈출한다.
                        IsFourCard = true;
                        for (int re = 0; re < 4; re++)
                        {
                            cardNums.Add(i);
                        }
                        break;
                    }
                    else if (JokerNum == 3)
                    {
                        IsFiveCard = true;
                        for (int re = 0; re < 5; re++)
                        {
                            cardNums.Add(i);
                        }
                        break;
                    }
                }
                // 첫번째 페어 항목이 차있을 경우
                else
                {
                    // 투페어의 숫자를 기록하고 투페어로 판별한 후 탈출한다
                    // ※ 조커가 0개이고 투페어 일 경우 다른 경우의 수가 없기 때문
                    if (JokerNum < 1)
                    {
                        IsTwoPair = true;
                        for (int re = 0; re < 2; re++)
                        {
                            cardNums.Add(i);
                        }
                        break;
                    }
                    else
                    {
                        //트리플을 체크한다.
                        IsTriple = true;
                        JokerUsedTriple = true;
                        for (int re = 0; re < 3; re++)
                        {
                            cardNums.Add(i);
                        }
                        IsFullHouse = true;
                    }

                }
            }
            #endregion

            #region 트리플 확인
            // 해당 숫자가 3개일 경우 = 트리플
            else if (_num == 3)
            {
                // 조커가 없을 경우
                if (JokerNum < 1)
                {
                    // 트리플을 true로 하고 해당 숫자를 기록한다.
                    IsTriple = true;
                    for (int re = 0; re < 3; re++)
                    {
                        cardNums.Add(i);
                    }
                    // OnePair가 이미 판별된 경우 풀하우스이므로 이를 체크하고 탈출한다.
                    if (IsOnePair)
                    {
                        IsFullHouse = true;
                        break;
                    }
                }
                else
                {
                    // 조커가 1개라면
                    if (JokerNum == 1)
                    {
                        // 포카드를 체크한다.
                        IsFourCard = true;
                        for (int re = 0; re < 4; re++)
                        {
                            cardNums.Add(i);
                        }
                        break;
                    }
                    // 조커가 2개라면 
                    else if (JokerNum == 2)
                    {
                        //파이브 카드를 체크한다.
                        IsFiveCard = true;
                        for (int re = 0; re < 5; re++)
                        {
                            cardNums.Add(i);
                        }
                        break;
                    }
                }
            }
            #endregion

            #region 포카드 확인
            // 해당 숫자가 4개인 경우 = 포카드
            else if (_num == 4)
            {
                //조커가 있을 경우 파이브 카드를 체크한다
                if (JokerNum == 1)
                {
                    IsFiveCard = true;
                    for (int re = 0; re < 5; re++)
                    {
                        cardNums.Add(i);
                    }
                    break;
                }
                //포카드를 체크하고 탈출한다.
                IsFourCard = true;
                for (int re = 0; re < 4; re++)
                {
                    cardNums.Add(i);
                }
                break;
            }
            #endregion
            // 다음 숫자 배열에 대한 검사를 진행한다.
            i++;
        }
        #endregion

        #region return region
        if (IsFiveJoker) return CardCombinationEnum.FiveJoker;
        if (IsFiveCard) return CardCombinationEnum.FiveCard;
        if (IsStraight && IsFlush) return CardCombinationEnum.StraightFlush;
        if (IsFourCard) return CardCombinationEnum.FourCard;
        if (IsFullHouse) return CardCombinationEnum.FullHouse;
        if (IsFlush)
        {
            int j = 0;
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
        if (IsStraight) return CardCombinationEnum.Straight;
        if (IsTriple) return CardCombinationEnum.Triple;
        if (IsTwoPair) return CardCombinationEnum.TwoPair;
        if (IsOnePair) return CardCombinationEnum.OnePair;

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
                cardNums.Add(i);
            }
            return CardCombinationEnum.Triple;
        }
        else if (JokerNum == 3)
        {
            for (int re = 0; re < 3; re++)
            {
                cardNums.Add(i);
            }
            return CardCombinationEnum.FourCard;
        }
        else if (JokerNum == 4)
        {
            for (int re = 0; re < 4; re++)
            {
                cardNums.Add(i);
            }
            return CardCombinationEnum.FiveCard;
        }
        else return CardCombinationEnum.HighCard;
        #endregion
    }

    private static bool CheckFiveJoker(int Joker, int JokerFiveNum, List<int> cardNum, out bool IsFiveJoker)
    {
        if (Joker == 5)
        {
            IsFiveJoker = true;
            int s = JokerFiveNum;
            // TODO : 현재 조커의 값을 14로 설정
            cardNum.AddRange(new List<int> { s, s, s, s, s });
        }
        else IsFiveJoker = false;

        return IsFiveJoker;
    }

    // TODO: 현재 straightCrit값만 넘으면 작은거부터 기준값만큼의 값을 반환해서 만약 더커도 작은 배열로 반환한다.
    // 이부분을 수정해야함
    private static bool CheckStraight(int Joker, int straightCrit, List<int> cardNum, int[] numbers, out bool IsStraight)
    {
        int StraightNum = 0;
        int StartStraightNum = 0;
        int UsedJoker = 0;
        int i = 1;
        IsStraight = false;
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

                    IsStraight = true;

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
                    IsStraight = true;
                    break;
                }
            }
            else
            {
                // 1개가 아닐경우 스트레이트에 관한 항목을 초기화한다.
                StartStraightNum = 0;
                StraightNum = 0;
                UsedJoker = 0;
                IsStraight = false;
                break;
            }
        }

        return IsStraight;


    }
}