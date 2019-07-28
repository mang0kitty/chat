using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol.Tests
{
    [TestClass]
    public class ParserTests
    {
        public Parser Parser { get; } = new Parser();

        [DataTestMethod]
        [DataRow(".MSG", new[] { "Aideen", "Hello there!" }, ".MSG Aideen \"Hello there!\"")]
        [DataRow(".PING", new string[0], ".PING")]
        [DataRow(".pong", new string[0], ".pong")]
        [DataRow(".msg", new[] { "Benji Panelli", "Test" }, ".msg \"Benji Panelli\" Test")]
        [DataRow(".msg", new[] { "Aideen", "You said \"Hi there\" though!" }, ".msg Aideen \"You said \\\"Hi there\\\" though!\"")]
        [DataRow(".msg", new[] { "Ben", "The path is \\\\test\\data" }, ".msg Ben \"The path is \\\\\\\\test\\\\data\"")]
        [DataRow(".msg", new[] { "Aideen", "Line 1\nLine 2"}, ".msg Aideen \"Line 1\\nLine 2\"")]
        public void TestGenerate(string type, string[] parts, string expected)
        {
            Assert.AreEqual(expected, Parser.Generate(type, parts));
        }

        [DataTestMethod]
        [DataRow(".MSG Aideen \"Hello there!\"", ".MSG", new[] { "Aideen", "Hello there!" })]
        [DataRow(".PING", ".PING", new string[0])]
        [DataRow(".pong", ".pong", new string[0])]
        [DataRow(".msg \"Benji Panelli\" Test", ".msg", new[] { "Benji Panelli", "Test" })]
        [DataRow(".msg Aideen \"You said \\\"Hi there\\\" though!\"", ".msg", new[] { "Aideen", "You said \"Hi there\" though!" })]
        [DataRow(".msg Ben \"The path is \\\\\\\\test\\\\data\"", ".msg", new[] { "Ben", "The path is \\\\test\\data" })]
        [DataRow(".msg Aideen \"Line 1\\nLine 2\"", ".msg", new[] { "Aideen", "Line 1\nLine 2"})]
        public void TestParse(string payload, string expectedType, string[] expectedParts) {
            var parsed = Parser.Parse(payload);
            Assert.AreEqual(expectedType, parsed.Type);
            CollectionAssert.AreEqual(expectedParts, parsed.Parts);
        }
    }
}
