using System;
using System.Collections.Generic;
using System.Linq;

namespace TollFeeCalculator.Core
{
    public class Calculator
    {  
        private static readonly List<TollFeeInterval> _tollFeeIntervals = new List<TollFeeInterval> {
                new TollFeeInterval( 
                    new TimeSpan(6, 0, 0), 
                    new TimeSpan(6, 29, 0), 
                    8),
                new TollFeeInterval( 
                    new TimeSpan(6, 30, 0), 
                    new TimeSpan(6, 59, 0), 
                    13),
                new TollFeeInterval( 
                    new TimeSpan(7, 0, 0), 
                    new TimeSpan(7, 59, 0), 
                    18),
                new TollFeeInterval( 
                    new TimeSpan(8, 0, 0), 
                    new TimeSpan(8, 29, 0), 
                    13),
                new TollFeeInterval( 
                    new TimeSpan(8, 30, 0), 
                    new TimeSpan(14, 59, 0), 
                    8),
                new TollFeeInterval( 
                    new TimeSpan(15, 0, 0), 
                    new TimeSpan(15, 29, 0), 
                    13),
                new TollFeeInterval( 
                    new TimeSpan(15, 30, 0), 
                    new TimeSpan(16, 59, 0), 
                    18),
                new TollFeeInterval( 
                    new TimeSpan(17, 0, 0), 
                    new TimeSpan(17, 59, 0), 
                    13),
                new TollFeeInterval( 
                    new TimeSpan (18, 0, 0), 
                    new TimeSpan(18, 29, 0), 
                    8)
        };
        private const int MAX_DAILY_FEE = 60;

        static void Main() {
            Run(Environment.CurrentDirectory + "../../../../testData.txt");
        }

        public static void Run(string filePath) {
            var passages = GetPassagesFromFile(filePath);
            passages = ValidatePassages(passages);
            passages = OrderAscendingByTime(passages);
            PrintTotalFee(CalculateTotalFee(passages));
        }

        private static void PrintTotalFee(double fee) {
            Console.Write("The total fee for the inputfile is " + fee);
        }

        private static IEnumerable<DateTime> GetPassagesFromFile(string filePath)
        {
            var passageData = System.IO.File.ReadAllText(filePath).Split(", ");
            List<DateTime> passages = new List<DateTime>();
            foreach (var passage in passageData) {
                if (DateTime.TryParse(passage, out var parsedPassage)) {
                    passages.Add(parsedPassage);
                }
                else {
                    throw new FormatException("Could not convert passage to datetime");
                }
            }
            return passages;
        }

        private static IEnumerable<DateTime> ValidatePassages(IEnumerable<DateTime> passages) {
            var firstDate = passages.First().Date;
            if (passages.Any(passage => firstDate != passage.Date)) {
                throw new ArgumentException("Passages on multiple days not supported");
            }
            return passages;
        }

        private static IEnumerable<DateTime> OrderAscendingByTime(IEnumerable<DateTime> passagesList) {
            return passagesList.OrderBy(x => x.TimeOfDay);
        }

        public static int CalculateTotalFee(IEnumerable<DateTime> passages) {
            var totalFee = 0;
            var totalHourlyFee = 0;
            var referencePassage = passages.First();
            foreach (var passage in passages) {
                if (IsLessThanAnHourApart(referencePassage, passage)) {
                    totalHourlyFee = Math.Max(GetTollFee(referencePassage), GetTollFee(passage));
                }
                else {
                    totalFee += totalHourlyFee;
                    totalHourlyFee = GetTollFee(passage);
                    referencePassage = passage;
                }
            }
            totalFee += totalHourlyFee;
            return Math.Min(totalFee, MAX_DAILY_FEE);
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
