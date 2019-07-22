using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vikings.Base.Tests
{
    [TestClass]
    public class EnumTests
    {
        [TestMethod]
        public void ToDictionaryTest()
        {
            var dict = Enum.ToDictionary<Test>();
            dict.ToList().ForEach(f => Console.WriteLine($"{f.Key}={f.Value}"));
            Assert.IsTrue(dict.Any(f => f.Key == Test.A && f.Value == "AAA"));
            Assert.IsTrue(dict.Any(f => f.Key == Test.Ab && f.Value == "BBB"));
            Assert.IsTrue(dict.Any(f => f.Key == Test.Abc && f.Value == "Abc"));
        }
    }

    public enum Test
    {
        [System.ComponentModel.Description("AAA")]
        A,
        [System.ComponentModel.Description("BBB")]
        Ab,
        //[System.ComponentModel.Description("CCC")]
        Abc
    }
}