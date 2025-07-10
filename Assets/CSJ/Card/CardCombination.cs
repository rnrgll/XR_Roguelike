using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

public static class CardCombination
{
    public static CardCombinationEnum CalCombination(List<MinorArcana> cards, out List<int> cardNums)
    {
        // 조합을 판별하는 딕셔너리 - 조합을 키로 bool을 값으로 가진다.
        Dictionary<CardCombinationEnum, bool> IsComb = new();
        // 조합에 사용되는 숫자들을 보관하는 리스트
        // 5장까지 받으므로 5로 미리 생성을 하였다.
        // 조커는 0, A는 1로 일단 취급한다.
        cardNums = new List<int>(5);
        // 문양을 저장하는 배열
        int[] SuitNum = new int[4];
        // 숫자들을 저장하는 배열
        int[] numbers = new int[13];
        // 문양을 대체하는 와일드카드의 개수를 받는다.
        int wildCount = 0;
        // 숫자를 대체하는 조커의 개수를 받는다
        int JokerCount = 0;
        // 더이상 조합이 바뀔 여지가 없을 경우 true 
        bool IsEnd = false;


        //TODO : 플레이어 스탯과의 연계?
        int FlushRequireCount = 5;
        int StraightRequireCount = 5;
        #region 조커 값이 0이므로 미사용
        // 조커 파이브에 저장할 숫자 (미사용)
        // int JokerFiveNum = 14;
        // 조커를 사용한 트리플인지 반환 (미사용)
        // bool JokerUsedTriple = false;
        #endregion

        // 딕셔너리에 각 조합 이름 별로 키값을 등록; 초기값은 false
        foreach (CardCombinationEnum comb in Enum.GetValues(typeof(CardCombinationEnum)))
            IsComb[comb] = false;


        // 조합 판별 전 전처리
        // 카드를 일일히 쓰지않고 카드들의 문양과 숫자로 분해
        #region card의 숫자와 문양을 개별 저장
        foreach (MinorArcana card in cards)
        {
            // 카드의 문양이나 인챈트가 wild일 경우 
            // 문양을 대체할 수 있는 wildCount를 1 증가
            if (card.Enchant.enchantInfo == CardEnchant.Wild ||
            card.CardSuit == MinorSuit.wildCard)
            {
                wildCount++;
            }
            // 만약 카드의 문양이 Special일 경우 그 카드는 패스
            else if (card.CardSuit == MinorSuit.Special ||
            card.CardSuit == MinorSuit.statusEffect) continue;
            // 아닐경우 (문양이 카드일 경우) 해당 문양에 1을 더해준다.
            else
                SuitNum[(int)card.CardSuit]++;

            // 카드의 숫자가 14일 경우 = 조커일 경우
            // 조커의 숫자를 1개 늘려준다.
            if (card.CardNum == 14)
            {
                JokerCount++;
                continue;
            }
            // 카드의 숫자가 14 이상일 경우 = 특수 카드일 경우 
            // 보통 위에서 Special로 걸렀지만 혹시 한번 다시 체크
            else if (card.CardNum > 14) continue;
            // 아닐 경우 = 일반 카드일 경우
            // 해당 숫자에 1을 더해준다.
            numbers[card.CardNum - 1]++;
        }
        #endregion

        // 문양 별로 순회하며 해당 문양이 플러시 요구치를 넘겼을 경우
        // 플러시를 체크하고 다음 과정을 진행한다.
        // TODO : 추후 문양에 따른 추가효과가 생길 경우 해당 배열 리턴? 
        #region flush 체크
        foreach (int _StNum in SuitNum)
        {
            // 문양의 숫자 + wild 카드의 숫자가 요구치보다 클 경우
            // 플러시로 체크한다 (초기값 = 5)
            if (_StNum + wildCount >= FlushRequireCount)
            {
                IsComb[CardCombinationEnum.Flush] = true;
                break;
            }
        }
        #endregion

        // 파이브 조커인지 확인한다.
        #region 파이브조커 확인
        CheckFiveJoker(JokerCount, cardNums, IsComb, ref IsEnd);
        #endregion

        // 스트레이트인지 확인한다.
        #region 스트레이트 확인
        CheckStraight(JokerCount, StraightRequireCount, cardNums, numbers, IsComb, ref IsEnd);
        #endregion


        int i = 1;
        #region 숫자 족보 확인
        // numbers 배열을 순회
        foreach (int _num in numbers)
        {
            // IsEnd가 true라면 바로 탈출
            if (IsEnd) break;
            #region 페어 확인
            // _num이 2라면 페어 이므로 페어에 대한 체크
            if (_num == 2)
            {
                // 원페어가 false라면 OnePair를 체크한다.
                if (!IsComb[CardCombinationEnum.OnePair])
                {
                    CheckOnePair(JokerCount, i, cardNums, IsComb, ref IsEnd);
                }
                // 원페어가 true라면 투페어 쪽을 체크한다.
                else
                {
                    CheckTwoPair(JokerCount, i, cardNums, IsComb, ref IsEnd);
                }
            }
            #endregion

            #region 트리플 확인
            // 해당 숫자가 3개일 경우 = 트리플
            else if (_num == 3)
            {
                CheckTriple(JokerCount, i, cardNums, IsComb, ref IsEnd);
            }
            #endregion

            #region 포카드 확인
            // 해당 숫자가 4개인 경우 = 포카드
            else if (_num == 4)
            {
                CheckFourCard(JokerCount, i, cardNums, IsComb, ref IsEnd);
            }
            #endregion

            // 다음 숫자 배열에 대한 검사를 진행한다.
            i++;
        }
        #endregion

        #region return region
        // 조합이 높은 순부터 체크한다.
        if (IsComb[CardCombinationEnum.FiveJoker])
            return CardCombinationEnum.FiveJoker;

        int numCount = 0;
        foreach (int _num in numbers)
        {
            if (_num >= 1)
            {
                numCount++;
                break;
            }
        }
        if (numCount == 0)
        {
            switch (JokerCount)
            {
                case 1:
                    return CardCombinationEnum.HighCard;
                case 2:
                    return CardCombinationEnum.OnePair;
                case 3:
                    return CardCombinationEnum.Triple;
                case 4:
                    return CardCombinationEnum.FourCard;
            }
        }

        if (IsComb[CardCombinationEnum.FiveCard])
            return CardCombinationEnum.FiveCard;

        if (JokerCount >= 4)
        {
            int X1 = 0;
            foreach (int _num in numbers)
            {
                X1++;
                if (_num == 0) continue;
                cardNums.Clear();
                InsertCardNum(1, X1, cardNums);
            }
            return CardCombinationEnum.FiveCard;
        }

        if (IsComb[CardCombinationEnum.Straight] && IsComb[CardCombinationEnum.Flush])
            return CardCombinationEnum.StraightFlush;

        if (IsComb[CardCombinationEnum.FourCard])
            return CardCombinationEnum.FourCard;
        if (JokerCount >= 3)
        {
            int x2 = 0;
            foreach (int _num in numbers)
            {
                x2++;
                if (_num == 0) continue;
                cardNums.Clear();
                InsertCardNum(1, x2, cardNums);
            }
            return CardCombinationEnum.FourCard;
        }

        if (IsComb[CardCombinationEnum.FullHouse])
            return CardCombinationEnum.FullHouse;

        // 플러시만 존재한다면 CardNum에 더해주어야 한다.
        if (IsComb[CardCombinationEnum.Flush])
        {
            int j = 0;
            cardNums.Clear();
            foreach (int _num in numbers)
            {
                j++;
                if (_num == 0) continue;
                InsertCardNum(_num, j, cardNums);
                if (cardNums.Count >= 5) break;
            }
            return CardCombinationEnum.Flush;
        }

        if (IsComb[CardCombinationEnum.Straight])
            return CardCombinationEnum.Straight;

        if (IsComb[CardCombinationEnum.Triple])
            return CardCombinationEnum.Triple;
        if (JokerCount >= 2)
        {
            int x3 = 0;
            foreach (int _num in numbers)
            {
                x3++;
                if (_num == 0) continue;
                cardNums.Clear();
                InsertCardNum(1, x3, cardNums);
            }
            return CardCombinationEnum.Triple;
        }

        if (IsComb[CardCombinationEnum.TwoPair])
            return CardCombinationEnum.TwoPair;

        if (IsComb[CardCombinationEnum.OnePair])
            return CardCombinationEnum.OnePair;
        if (JokerCount >= 1)
        {
            int x4 = 0;
            foreach (int _num in numbers)
            {
                x4++;
                if (_num == 0) continue;
                cardNums.Clear();
                InsertCardNum(1, x4, cardNums);
            }
            return CardCombinationEnum.OnePair;
        }

        // 여기까지 족보가 없다면 하이카드 체크

        cardNums.Clear();
        int x = 0;
        foreach (int _num in numbers)
        {
            x++;
            if (_num == 0) continue;
            cardNums.Clear();
            InsertCardNum(1, x, cardNums);
        }
        return CardCombinationEnum.HighCard;
        #endregion
    }

    /// <summary>
    /// 파이브 조커인지 판별한다.
    /// 조커가 5장일경우 조합에 true 아닐경우 false를 마킹한다.
    /// </summary>
    private static void CheckFiveJoker(int Joker, List<int> cardNum, Dictionary<CardCombinationEnum, bool> IsComb, ref bool IsEnd)
    {
        InsertCardNum(5, 0, cardNum);
        if (Joker == 5)
        {
            IsComb[CardCombinationEnum.FiveJoker] = true;
            IsEnd = true;
        }
        else IsComb[CardCombinationEnum.FiveJoker] = false;

    }

    /// <summary>
    /// 스트레이트인지 확인한다.
    /// 확장 가능성을 고려 스트레이트에 필요한 숫자까지 고려하여 설계
    /// 기존 계산 방식에 문제가 있어 슬라이딩 윈도우 방식으로 변경
    /// </summary>
    private static void CheckStraight(int Joker, int straightLen, List<int> cardNum, int[] numbers, Dictionary<CardCombinationEnum, bool> IsComb, ref bool IsEnd)
    {
        // 카드의 숫자 크기
        int n = numbers.Length;
        // 크기가 n+1인 배열 생성
        int[] AHigh = new int[n + 1];
        // numbers를 복사하여 배열에 삽입
        Array.Copy(numbers, AHigh, n);
        // AHigh의 n+1 항에 A를 삽입
        AHigh[n] = numbers[0];

        // 첫 시작은 뒤에서부터 시작
        // AHigh의 길이에서 스트레이트에 필요한 길이 (기본 5) 만큼 빼고 계산
        int maxStart = AHigh.Length - straightLen;
        for (int start = maxStart; start >= 0; start--)
        {
            // 스트레이트가 비어있는 만큼 계산
            int missing = 0;
            List<int> missingNum = new List<int>();
            for (int offset = 0; offset < straightLen; offset++)
            {
                // AHigh의 값이 0인 경우
                // 비어있으므로 missing을 +
                if (AHigh[start + offset] == 0)
                {
                    missing++;
                    missingNum.Add(start + offset);
                }
                // missing이 Joker보다 커질 경우
                // 브레이크
                if (missing > Joker)
                    break;
            }
            // 순회를 마친 결과 조커보다 missing이 작다면 true
            if (missing <= Joker)
            {
                IsComb[CardCombinationEnum.Straight] = true;
                IsEnd = true;
                cardNum.Clear();
                // cardNum에 반환 값을 넣기 위해 순회 
                for (int offset = 0; offset < straightLen; offset++)
                {
                    int idx = start + offset;
                    if (missingNum.Contains(idx)) continue;
                    // index가 n이라면 (13이라면) 
                    // Ace이므로 1을 rank에 삽입
                    // 이외에는 index+1을 삽입 (배열은 0부터 시작하므로)
                    int rank = (idx == n) ? 1 : (idx + 1);
                    cardNum.Add(rank);
                }
                return;
            }
            // 다시 start 값을 1낮춰서 순회 시작
            #region 수정전
            //             int StartStraightNum = 0;
            //             int UsedJoker = 0;
            //             List<int> JokerIdx = new();
            //             int i = 1;
            //             foreach (int _num in numbers)
            //             {
            //                 if (_num > 1)
            //                 {
            //                     break;
            //                     //TODO: 순서 조정으로 페어 이상이 존재할 경우 CheckStraight로 들어오지 않도록 조정
            //                 }
            // 
            //                 // 해당 숫자가 1개일 경우
            //                 if (_num == 1)
            //                 {
            //                     // 만약 스트레이트의 시작일 경우 스트레이트의 시작 넘버를 기록한다.
            //                     if (StraightNum == 0) StartStraightNum = i;
            //                     // 연속되는 숫자를 나타내는 straightNum을 +해준다.
            //                     StraightNum++;
            //                     // 만일 straightNum이 현재 스트레이트의 수치를 만족하면 스트레이트로 판별하고 다음으로 이동
            //                     // 만약 10,J,Q,K,A가 있을 경우에도 Straight 반환
            //                     if (StraightNum >= straightCrit || (i == 13 && StraightNum == straightCrit - 1 && numbers[0] == 1))
            //                     {
            // 
            //                         IsComb[CardCombinationEnum.Straight] = true;
            // 
            //                         if (i == 13 && StraightNum == straightCrit - 1 && numbers[0] == 1)
            //                         {
            //                             for (int sNum = StartStraightNum; sNum < StartStraightNum + straightCrit - 1; sNum++)
            //                             {
            //                                 if (!JokerIdx.Contains(sNum))
            //                                 {
            //                                     cardNum.Add(sNum);
            //                                 }
            //                             }
            //                             cardNum.Add(1);
            //                         }
            //                         else
            //                         {
            //                             for (int sNum = StartStraightNum; sNum < StartStraightNum + straightCrit; sNum++)
            //                             {
            //                                 if (!JokerIdx.Contains(sNum))
            //                                 {
            //                                     if (!JokerIdx.Contains(sNum))
            //                                     {
            //                                         cardNum.Add(sNum);
            //                                     }
            //                                 }
            //                             }
            //                         }
            //                         break;
            //                     }
            //                 }
            //                 // 스트레이트에 사용할 수 있는 조커가 있는 경우
            //                 else if (Joker > UsedJoker)
            //                 {
            //                     UsedJoker += 1;
            //                     StraightNum++;
            //                     JokerIdx.Add(i);
            //                     if (StraightNum >= straightCrit)
            //                     {
            //                         IsComb[CardCombinationEnum.Straight] = true;
            //                         IsEnd = true;
            //                         break;
            //                     }
            //                 }
            //                 else
            //                 {
            //                     // 1개가 아닐경우 스트레이트에 관한 항목을 초기화한다.
            //                     StartStraightNum = 0;
            //                     StraightNum = 0;
            //                     UsedJoker = 0;
            //                     IsComb[CardCombinationEnum.Straight] = false;
            //                     break;
            //                 }
            //                 i++;
            //             }
            #endregion
        }
        IsComb[CardCombinationEnum.Straight] = false;
    }


    private static void CheckOnePair(int Joker, int _cardNum, List<int> cardNums, Dictionary<CardCombinationEnum, bool> IsComb, ref bool IsEnd)
    {
        // 페어이므로 (조커의 값은 0이므로)
        // CardNum 배열에 일단 해당 숫자를 2개 삽입
        InsertCardNum(2, _cardNum, cardNums);

        // 트리플을 이미 판별한 경우 풀하우스 이므로 풀하우스로 체크한 후 나간다.
        if (IsComb[CardCombinationEnum.Triple])
        {
            // 풀하우스를 판별한다.
            IsComb[CardCombinationEnum.FullHouse] = true;
            IsEnd = true;
            return;

            #region 미사용
            /*// 조커를 사용한 트리플일 경우
            // 현재의 숫자가 이전 페어의 숫자보다 크므로
            // 현재 페어의 숫자로 조커를 대체해 준다.
            if (JokerUsedTriple)
            {
                cardNums[2] = _cardNum;
            }*/
            #endregion
        }
        // 만약 조커가 없을 경우
        if (Joker < 1)
        {
            // 해당 페어 항목에 채워넣고 일단 OnePair를 true로 만든다.
            IsComb[CardCombinationEnum.OnePair] = true;
        }
        // 조커가 1개라면
        else if (Joker == 1)
        {
            //트리플을 체크한다.
            IsComb[CardCombinationEnum.Triple] = true;
        }
        // 조커가 2개라면
        else if (Joker == 2)
        {
            // 포카드를 체크하고 4장을 사용했으므로
            // IsEnd를 체크하고 탈출한다.
            IsComb[CardCombinationEnum.FourCard] = true;
            IsEnd = true;
        }
        // 조커가 3개라면
        else if (Joker == 3)
        {
            // 파이브 카드를 체크하고 IsEnd를 체크한다.
            IsComb[CardCombinationEnum.FiveCard] = true;
            IsEnd = true;
        }
    }


    private static void CheckTwoPair(int Joker, int _cardNum, List<int> cardNums, Dictionary<CardCombinationEnum, bool> IsComb, ref bool IsEnd)
    {
        // 페어이므로 (조커의 값은 0이므로)
        // CardNum 배열에 일단 해당 숫자를 2개 삽입
        InsertCardNum(2, _cardNum, cardNums);

        // 투페어의 숫자를 기록하고 투페어로 판별한 후 탈출한다
        // ※ 조커가 0개이고 투페어 일 경우 다른 경우의 수가 없기 때문
        if (Joker < 1)
        {
            IsComb[CardCombinationEnum.TwoPair] = true;
        }
        // Joker가 1개일 경우
        else
        {
            // 트리플과 풀하우스를 체크한다.
            IsComb[CardCombinationEnum.Triple] = true;
            IsComb[CardCombinationEnum.FullHouse] = true;
        }
        // 투페어 체크를 마친 경우 적어도 4장을 사용했으므로 IsEnd를 true로 바꾼다.
        IsEnd = true;
    }

    private static void CheckTriple(int Joker, int _cardNum, List<int> cardNums, Dictionary<CardCombinationEnum, bool> IsComb, ref bool IsEnd)
    {
        // 트리플이므로 (조커의 값은 0이므로)
        // CardNum 배열에 일단 해당 숫자를 3개 삽입
        InsertCardNum(3, _cardNum, cardNums);

        // 조커가 없을 경우
        if (Joker < 1)
        {
            // 트리플을 true로 하고 해당 숫자를 기록한다.
            IsComb[CardCombinationEnum.Triple] = true;

            // OnePair가 이미 판별된 경우 풀하우스이므로 이를 체크하고 탈출한다.
            if (IsComb[CardCombinationEnum.OnePair])
            {
                IsComb[CardCombinationEnum.FullHouse] = true;
                IsEnd = true;
            }
        }
        // 조커가 있을 경우 (4장 이상 사용)
        else
        {
            // 조커가 1개라면
            if (Joker == 1)
            {
                // 포카드를 체크한다.
                IsComb[CardCombinationEnum.FourCard] = true;
            }
            // 조커가 2개라면 
            else if (Joker == 2)
            {
                //파이브 카드를 체크한다.
                IsComb[CardCombinationEnum.FiveCard] = true;
            }
            IsEnd = true;
        }
    }

    private static void CheckFourCard(int Joker, int _cardNum, List<int> cardNums, Dictionary<CardCombinationEnum, bool> IsComb, ref bool IsEnd)
    {
        // 포카드이므로 (조커는 0이므로)
        // CardNum 배열에 일단 해당 숫자를 4개 삽입
        InsertCardNum(4, _cardNum, cardNums);

        //조커가 있을 경우 파이브 카드를 체크한다
        if (Joker == 1)
        {
            IsComb[CardCombinationEnum.FiveCard] = true;
        }
        else
        {
            // 없을 경우 포카드를 체크하고 탈출한다.
            IsComb[CardCombinationEnum.FourCard] = true;
        }
        IsEnd = true;
    }

    // CardNum에 동일한 개수의 숫자를 삽입한다.
    private static void InsertCardNum(int Count, int _cardNum, List<int> cardNums)
    {
        for (int re = 0; re < Count; re++)
        {
            cardNums.Add(_cardNum);
        }
    }
}
