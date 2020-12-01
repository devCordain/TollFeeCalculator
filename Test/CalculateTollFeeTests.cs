using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TollFeeCalculator;

namespace Test
{
    [TestClass]
    public class CalulateTollFeeTests
    {

        //Programmets mål är att beräkna kostnaden för en given inputfil innehållande datum och tider
        //som en bil åker genom vägtullarna under en dag.Programmet ska utifrån detta skriva ut i
        //terminalen hur mycket den totala kostnaden är.Varje passage genom en betalstation kostar
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


        [DataTestMethod]
        [DataRow("2020 - 11 - 28 10:13")]
        [DataRow("2020 - 11 - 29 10:13")] 
        [DataRow("2020 - 07 - 28 10:13")]
        public void Should_return_True_if_DayOfTheWeek_is_saturday_or_sunday_or_during_july(string tollBoothPassageTime) {
            var input = DateTime.Parse(tollBoothPassageTime);
            Assert.IsTrue(CalculateTollFee.IsPassageOnAFreeDayOrMonth(input));
        }

        [TestMethod]
        public void Should_return_False_if_DayOfTheWeek_is_Not_saturday_or_sunday_or_during_july() {
            var input = new DateTime(2020, 11, 30, 10, 13, 00);
            Assert.IsFalse(CalculateTollFee.IsPassageOnAFreeDayOrMonth(input));
        }

        [DataTestMethod]
        [DataRow("2020 - 11 - 30 06:00", 8)]
        [DataRow("2020 - 11 - 30 06:29", 8)]
        [DataRow("2020 - 11 - 30 06:30", 13)]
        [DataRow("2020 - 11 - 30 06:59", 13)]
        [DataRow("2020 - 11 - 30 07:00", 18)]
        [DataRow("2020 - 11 - 30 07:59", 18)]
        [DataRow("2020 - 11 - 30 08:00", 13)]
        [DataRow("2020 - 11 - 30 08:29", 13)]
        [DataRow("2020 - 11 - 30 08:30", 8)]
        [DataRow("2020 - 11 - 30 14:59", 8)]
        [DataRow("2020 - 11 - 30 15:00", 13)]
        [DataRow("2020 - 11 - 30 15:29", 13)]
        [DataRow("2020 - 11 - 30 15:30", 18)]
        [DataRow("2020 - 11 - 30 16:59", 18)]
        [DataRow("2020 - 11 - 30 17:00", 13)]
        [DataRow("2020 - 11 - 30 17:59", 13)]
        [DataRow("2020 - 11 - 30 18:00", 8)]
        [DataRow("2020 - 11 - 30 18:29", 8)]
        [DataRow("2020 - 11 - 30 18:30", 0)]
        [DataRow("2020 - 11 - 30 05:59", 0)]
        public void Should_return_correct_tollFee_for_each_passageTime(string tollBoothPassageTime, int tollFee) {
            var input = DateTime.Parse(tollBoothPassageTime);
            Assert.AreEqual(tollFee, CalculateTollFee.TollFeePass(input));
        }

        [TestMethod]
        public void Should_return_max_60_per_day() {
            var input = new DateTime[24];
            for (int i = 0; i < 24; i++)
            {
                input[i] = new DateTime(2020, 11, 30, i, 30, 00);
            }
            Assert.AreEqual(60, CalculateTollFee.TotalFeeCost(input));
        }

        [TestMethod]
        public void Should_return_correct_daily_fee_with_multiple_passages_per_hour() {
            var input = new DateTime[] {
                new DateTime(2020, 11, 30, 00, 15, 00),
                new DateTime(2020, 11, 30, 06, 15, 00),
                new DateTime(2020, 11, 30, 06, 45, 00),
                new DateTime(2020, 11, 30, 06, 55, 00),
                new DateTime(2020, 11, 30, 11, 55, 00),
                new DateTime(2020, 11, 30, 14, 55, 00),
                new DateTime(2020, 11, 30, 15, 15, 00),
                new DateTime(2020, 11, 30, 15, 35, 00)
            };
            Assert.AreEqual(39, CalculateTollFee.TotalFeeCost(input));
        }

        [TestMethod]
        public void Should_return_max_60_per_day_with_multiple_days() {
            var input = new DateTime[24];
            for (int i = 0; i < 24; i++)
            {
                input[i] = new DateTime(2020, 11, 30, i, 30, 00);
            }
            Assert.AreEqual(60, CalculateTollFee.TotalFeeCost(input));
        }

    }
}
