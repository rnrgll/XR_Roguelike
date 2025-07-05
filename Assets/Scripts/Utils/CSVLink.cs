using System.Collections.Generic;

namespace Utils
{
    public static class CSVLink
    {
        private static Dictionary<string, string> csvLinckDic = new()
        {
            ["Event"] = "https://docs.google.com/spreadsheets/d/1V2n99Lr8J_F5ck9xHI7qfK4NyOnkpOEi/gviz/tq?tqx=out:csv&sheet=이벤트테이블&range=A1:K38",
            ["EventMainReward"] = "https://docs.google.com/spreadsheets/d/1noXpW5wIHXMxbs-BJjmM7okO8mzrIOuu/gviz/tq?tqx=out:csv&sheet=메인보상테이블&tq=SELECT+A,B,C,D,E",
            ["EventRewardEffect"] = "https://docs.google.com/spreadsheets/d/1noXpW5wIHXMxbs-BJjmM7okO8mzrIOuu/gviz/tq?tqx=out:csv&sheet=보상효과테이블&tq=SELECT+A,B,C,D,E,F,G,H,I,J,K,L,M,N",
        };

        public static IReadOnlyDictionary<string, string> CsvLinkDict => csvLinckDic;
    }
}