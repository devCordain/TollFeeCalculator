using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TollFeeCalculator.Core;

namespace Test
{
    [TestClass]
    public class TollFeeCalculatorTests
    {
        [DataTestMethod]
        [DataRow("2020 - 11 - 28 10:13")]
        [DataRow("2020 - 11 - 29 10:13")] 
        [DataRow("2020 - 07 - 28 10:13")]
        public void Should_return_expected_fee_if_DayOfTheWeek_is_saturday_or_sunday_or_during_july(string tollBoothPassageTime) {
            var input = DateTime.Parse(tollBoothPassageTime);
            Assert.AreEqual(0, Calculator.GetTollFee(input));
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
            Assert.AreEqual(tollFee, Calculator.GetTollFee(input));
        }

        [TestMethod]
        public void Should_return_max_60_per_day() {
            var input = new DateTime[24];
            for (int i = 0; i < 24; i++)
            {
                input[i] = new DateTime(2020, 11, 30, i, 30, 00);
            }
            Assert.AreEqual(60, Calculator.CalculateTotalFee(input));
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
            Assert.AreEqual(39, Calculator.CalculateTotalFee(input));
        }

        [TestMethod]
        public void Should_throw_expected_exception_If_input_data_contains_multiple_dates() {
            var input = (Environment.CurrentDirectory + "../../../../mockMultipleDatesTestData.txt");
            Assert.ThrowsException<ArgumentException>(() => Calculator.Run(input));
        }

        [TestMethod]
        public void Should_print_the_total_cost_for_passages() {
            var input = (Environment.CurrentDirectory + "../../../../mockTestData.txt");
            var sw = new StringWriter();
            Console.SetOut(sw);
            Calculator.Run(input);
            Assert.AreEqual("The total fee for the inputfile is 29", sw.ToString());
        }

        [TestMethod]
        public void Should_throw_expected_exception_If_input_data_cannot_be_parsed() {
            var input = (Environment.CurrentDirectory + "../../../../mockInvalidPassageTestData.txt");
            Assert.ThrowsException<FormatException>(() => Calculator.Run(input));
        }

    }
}
