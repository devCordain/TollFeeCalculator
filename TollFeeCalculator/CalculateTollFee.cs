using System;
using System.Collections.Generic;
using System.Linq;

namespace TollFeeCalculator
{
    public class CalculateTollFee
    {
        //Programmets mål är att beräkna kostnaden för en given inputfil innehållande datum och tider
        //som en bil åker genom vägtullarna under en dag.Programmet ska utifrån detta skriva ut i
        //terminalen hur mycket den totala kostnaden är. Varje passage genom en betalstation kostar
        //0, 8, 13 eller 18 kronor beroende på tidpunkt. Det maximala beloppet per dag är 60 kronor.

        //Tider Belopp
        //06:00–06:29 - 8 kr
        //06:30–06:59 - 13 kr
        //07:00–07:59 - 18 kr
        //08:00–08:29 - 13 kr
        //08:30–14:59 - 8 kr
        //15:00–15:29 - 13 kr
        //15:30–16:59 - 18 kr
        //17:00–17:59 - 13 kr
        //18:00–18:29 - 8 kr
        //18:30–05:59 - 0 kr

        //Vägtull tas ut för fordon som passerar en betalstation måndag till fredag mellan 06.00 och
        //18.29. Tull tas inte ut lördagar och söndagar eller under juli månad.En bil som passerar flera
        //betalstationer inom 60 minuter beskattas bara en gång.Det belopp som då ska betalas är
        //det högsta beloppet av de passagerna.

       static void Main()
        {
            run(Environment.CurrentDirectory + "../../../../testData.txt");
        }

        public static void run(String inputFile) {
            string indata = System.IO.File.ReadAllText(inputFile);
            String[] dateStrings = indata.Split(", ");
            DateTime[] dates = new DateTime[dateStrings.Length-1]; //TODO: wrong size
            for(int i = 0; i < dates.Length; i++) {
                dates[i] = DateTime.Parse(dateStrings[i]);
            }
            Console.Write("The total fee for the inputfile is" + TotalFeeCost(dates)); //TODO: add space
        }

        public static int TotalFeeCost(DateTime[] passages) {
            int totalFee = 0;
            var passagesList = passages.ToList();
            var passagesSortedAndDividedByDate = DivideIntoListByDate(OrderAscendingByDate(passagesList));
            foreach (var dailyPassages in passagesSortedAndDividedByDate) {
                totalFee += GetTotalFeeForDate(dailyPassages);
            }
            return totalFee;
        }

        private static List<DateTime> OrderAscendingByDate(List<DateTime> passagesList) {
            return passagesList.OrderBy(x => x.Date).ToList();
        }

        private static List<DateTime[]> DivideIntoListByDate(List<DateTime> passages) {
            return passages.ToList()
                .GroupBy(x => x.Date)
                .Select(y => y.ToArray())
                .ToList();
        }

        public static int GetTotalFeeForDate(DateTime[] dailyPassages) {
            int dailyFee = 0;
            int hourlyFee = 0;
            DateTime referencePassage = dailyPassages.First(); 
            foreach (var passage in dailyPassages)
            {
                if (IsLessThanAnHourApart(referencePassage, passage)) {
                    hourlyFee = Math.Max(TollFeePass(referencePassage), TollFeePass(passage));
                }
                else {
                    dailyFee += hourlyFee;
                    hourlyFee = TollFeePass(passage);
                    referencePassage = passage;
                }
            }
            dailyFee += hourlyFee;
            return Math.Min(dailyFee, 60);
        }

        private static bool IsLessThanAnHourApart(DateTime referencePassage, DateTime passage) { 
            return (passage - referencePassage).TotalMinutes < 60;
        }

        public static int TollFeePass(DateTime d) {
            if (free(d)) return 0;
            int hour = d.Hour;
            int minute = d.Minute;
            if (hour == 6 && minute >= 0 && minute <= 29) return 8;
            else if (hour == 6 && minute >= 30 && minute <= 59) return 13;
            else if (hour == 7 && minute >= 0 && minute <= 59) return 18;
            else if (hour == 8 && minute >= 0 && minute <= 29) return 13;
            else if (hour >= 8 && hour <= 14 && minute >= 30 && minute <= 59) return 8;
            else if (hour == 15 && minute >= 0 && minute <= 29) return 13;
            else if (hour == 15 && minute >= 0 || hour == 16 && minute <= 59) return 18;
            else if (hour == 17 && minute >= 0 && minute <= 59) return 13;
            else if (hour == 18 && minute >= 0 && minute <= 29) return 8;
            else return 0;
        }

        public static bool free(DateTime day) {
        return (int)day.DayOfWeek == 6 || (int)day.DayOfWeek == 0 || day.Month == 7;
        }
    }
}
