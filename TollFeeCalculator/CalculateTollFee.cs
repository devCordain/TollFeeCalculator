using System;
using System.Collections.Generic;
using System.Linq;

namespace TollFeeCalculator
{
    public class CalculateTollFee
    {  
        private static readonly List<TollFeeInterval> _tollFeeIntervals = new List<TollFeeInterval> {
                new TollFeeInterval( new TimeSpan (6, 0, 0), new TimeSpan(6, 29, 0), 8),
                new TollFeeInterval( new TimeSpan (6, 30, 0), new TimeSpan(6, 59, 0), 13),
                new TollFeeInterval( new TimeSpan (7, 0, 0), new TimeSpan(7, 59, 0), 18),
                new TollFeeInterval( new TimeSpan (8, 0, 0), new TimeSpan(8, 29, 0), 13),
                new TollFeeInterval( new TimeSpan (8, 30, 0), new TimeSpan(14, 59, 0), 8),
                new TollFeeInterval( new TimeSpan (15, 0, 0), new TimeSpan(15, 29, 0), 13),
                new TollFeeInterval( new TimeSpan (15, 30, 0), new TimeSpan(16, 59, 0), 18),
                new TollFeeInterval( new TimeSpan (17, 0, 0), new TimeSpan(17, 59, 0), 13),
                new TollFeeInterval( new TimeSpan (18, 0, 0), new TimeSpan(18, 29, 0), 8)
        };
        private const int MAX_DAILY_FEE = 60;

        static void Main()
        {
            PrintTotalFee(Environment.CurrentDirectory + "../../../../testData.txt");
        }

        public static void PrintTotalFee(String filePath) {
            var passageData = System.IO.File.ReadAllText(filePath).Split(", ");
            List<DateTime> passages = new List<DateTime>();
            foreach (var passage in passageData) {
                if (DateTime.TryParse(passage, out DateTime parsedPassage)) {
                    passages.Add(parsedPassage);
                }
                else {
                    throw new ArgumentException("Could not convert passage to datetime");
                }
            }
            Console.Write("The total fee for the inputfile is " + CalculateTotalFee(passages));
        }

        public static int CalculateTotalFee(IEnumerable<DateTime> passages) {
            int totalFee = 0;
            var passagesSortedAndDividedByDate = DivideByDate(OrderAscendingByDate(passages.ToList()));
            foreach (var dailyPassages in passagesSortedAndDividedByDate) {
                var dailyTotalFee = CalculateDailyFee(dailyPassages);
                if (dailyTotalFee > MAX_DAILY_FEE ) {
                    totalFee += MAX_DAILY_FEE;
                }
                else {
                    totalFee += dailyTotalFee;
                }
            }
            return totalFee;
        }

        private static IEnumerable<DateTime[]> DivideByDate(IEnumerable<DateTime> passages) {
            return passages.ToList()
                .GroupBy(x => x.Date)
                .Select(y => y.ToArray())
                .ToList();
        } // TODO: beräkna kostnaden för en given inputfil innehållande datum och tider som en bil åker genom vägtullarna under en dag.
        // Ska vi förvänta oss att vi får enbart 1 dag i filen? Testfilen innehåller fler dagar, så vi har byggt funktionalitet för att hantera det. 
        // Ska vi ha flera dagar eller ska vi kasta exception? Ska vi skriva ut separerat per dag om det innnehåller flera dagar? Eller en totalsumma för samtliga dagar? Eller båda?

        private static IEnumerable<DateTime> OrderAscendingByDate(IEnumerable<DateTime> passagesList) {
            return passagesList.OrderBy(x => x.Date).ToList();
        }

        private static int CalculateDailyFee(IEnumerable<DateTime> dailyPassages) {
            int totalDailyFee = 0;
            int totalHourlyFee = 0;
            var referencePassage = dailyPassages.First(); 
            foreach (var passage in dailyPassages)
            {
                if (IsLessThanAnHourApart(referencePassage, passage)) {
                    totalHourlyFee = Math.Max(GetTollFee(referencePassage), GetTollFee(passage));
                }
                else {
                    totalDailyFee += totalHourlyFee;
                    totalHourlyFee = GetTollFee(passage);
                    referencePassage = passage;
                }
            }
            totalDailyFee += totalHourlyFee;
            return totalDailyFee;
        }

        private static bool IsLessThanAnHourApart(DateTime referencePassage, DateTime passage) { 
            return (passage - referencePassage).TotalMinutes < 60;
        }

        public static int GetTollFee(DateTime passage) {
            if (IsPassageOnAFreeDayOrMonth(passage)) 
                return 0;
            var timeOfDay = passage.TimeOfDay;
            foreach (var tollFee in _tollFeeIntervals) {
                var startOfInterval = tollFee.StartInterval;
                var endOfInterval = tollFee.EndInterval;
                var fee = tollFee.Fee;
                if (timeOfDay >= startOfInterval && timeOfDay <= endOfInterval) 
                    return fee;
            }
            return 0;
        }

        private static bool IsPassageOnAFreeDayOrMonth(DateTime passage) {
            return
                passage.Month == 7 ||
                passage.DayOfWeek == DayOfWeek.Saturday ||
                passage.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
