using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vis;
using System.Linq;
using System.Diagnostics;

namespace VisTests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1()
        {
            string Day2016_input = @"L1, L3, L5, L3, R1, L4, L5, R1, R3, L5, R1, L3, L2, L3, R2, R2, L3, L3, R1, L2, R1, L3, L2, R4, R2, L5, R4, L5, R4, L2, R3, L2, R4, R1, L5, L4, R1, L2, R3, R1, R2, L4, R1, L2, R3, L2, L3, R5, L192, R4, L5, R4, L1, R4, L4, R2, L5, R45, L2, L5, R4, R5, L3, R5, R77, R2, R5, L5, R1, R4, L4, L4, R2, L4, L1, R191, R1, L1, L2, L2, L4, L3, R1, L3, R1, R5, R3, L1, L4, L2, L3, L1, L1, R5, L4, R1, L3, R1, L2, R1, R4, R5, L4, L2, R4, R5, L1, L2, R3, L4, R2, R2, R3, L2, L3, L5, R3, R1, L4, L3, R4, R2, R2, R2, R1, L4, R4, R1, R2, R1, L2, L2, R4, L1, L2, R3, L3, L5, L4, R4, L3, L1, L5, L3, L5, R5, L5, L4, L2, R1, L2, L4, L2, L4, L1, R4, R4, R5, R1, L4, R2, L4, L2, L4, R2, L4, L1, L2, R1, R4, R3, R2, R2, R5, L1, L2";
            Assert.AreEqual(5, Day2016_1(@"R2, L3"));
            Assert.AreEqual(2, Day2016_1(@"R2, R2, R2"));
            Assert.AreEqual(12, Day2016_1(@"R5, L5, R5, R3"));
            Assert.AreEqual(4, Day2016_2(@"R8, R4, R4, R8"));
            Assert.AreEqual(299, Day2016_1(Day2016_input));
            Assert.AreEqual(181, Day2016_2(Day2016_input));
        }
        public int Day2016_1(string input)
        {
            var t = new Turtle(trackvisit: true);
            foreach (var step in input.Split(',').Select(s => s.Trim())) {
                switch (step[0]) {
                    case 'R': t.Right(); break;
                    case 'L': t.Left(); break;
                }
                t.Move(int.Parse(step.Substring(1)));
            }
            return t.Manhattan(new Point(0,0));
        }
        public int Day2016_2(string input)
        {
            var t = new Turtle(trackvisit: true);
            t.Revisit += () =>  false; 
            foreach (var step in input.Split(',').Select(s => s.Trim())) {
                switch (step[0]) {
                    case 'R': t.Right(); break;
                    case 'L': t.Left(); break;
                }
                if (!t.Move(int.Parse(step.Substring(1))))
                    break;
            }
            return t.Manhattan(new Point(0, 0));
        }
    }
}
