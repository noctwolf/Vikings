using System;
using System.ComponentModel.DataAnnotations;
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
            Assert.IsTrue(dict.Any(f => f.Key == Test.A && f.Value == "枚举A"));
            Assert.IsTrue(dict.Any(f => f.Key == Test.Ab && f.Value == "枚举AB"));
            Assert.IsTrue(dict.Any(f => f.Key == Test.Abc && f.Value == "Abc"));
        }
    }

    public enum Test
    {
        [Display(Name = "枚举A")]
        A,
        [Display(Name = "枚举AB")]
        Ab,
        //[System.ComponentModel.Description("CCC")]
        Abc
    }
}