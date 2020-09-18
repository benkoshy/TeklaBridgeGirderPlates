using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;


namespace ContourPlateBridge.Tests
{
    [TestFixture]
    class PrefixMakerTest
    {
        PrefixMaker p1;
        PrefixMaker p2;


        [SetUp]
        protected void SetUp()
        {
            p1 = new PrefixMaker("B80-P42-BER-07");
            p2 = new PrefixMaker("B80-P42-BER-77");
        }

        [Test]
        public void GetAssembly()
        {
            Assert.AreEqual(p1.GetAssembly(), "B80-P42-BER-0");
            Assert.AreEqual(p2.GetAssembly(), "B80-P42-BER-");
        }

        [Test]
        public void GetPrefix()
        {
            Assert.AreEqual(p1.GetPrefix(), 7);
            Assert.AreEqual(p2.GetPrefix(), 77);
        }

        [Test]
        public void IsMatch()
        {
            PrefixMaker invalid = new PrefixMaker("B80-P42-BER-7");
            PrefixMaker invalid2 = new PrefixMaker("B80-P42-BER-7 ");
            Assert.IsFalse(invalid.IsMatch());
            Assert.IsFalse(invalid2.IsMatch());
        }

        [Test]
        public void IsMatchValid()
        {
            Assert.IsTrue(p1.IsMatch());
            Assert.IsTrue(p2.IsMatch());
        }

        
    }
}
