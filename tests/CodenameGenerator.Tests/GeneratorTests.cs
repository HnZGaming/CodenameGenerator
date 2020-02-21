﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;

namespace CodenameGenerator.Tests
{
    [TestClass]
    public class GeneratorTests
    {
        [TestMethod]
        public void CTorTests()
        {
            var @char = '-';
            var separator = @char.ToString();
            var casing = Casing.PascalCase;
            var generator = new Generator(separator, casing, null, WordBank.Verbs, WordBank.Adjectives);
            Assert.IsTrue(generator.Separator == "-");
            Assert.IsTrue(generator.Casing == casing);
            var name = generator.Generate();
            var parts = name.Split(@char);
            Assert.IsTrue(parts.Length == 2);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(3)]
        [DataRow(int.MaxValue)]
        public void SeededTests(int seed)
        {
            var generator = new Generator(seed: seed);
            generator.SetParts(WordBank.Adjectives, WordBank.Nouns);
            string result = generator.Generate();
            generator = new Generator(seed: seed);
            generator.SetParts(WordBank.Adjectives, WordBank.Nouns);
            Assert.IsTrue(result == generator.Generate());
        }

        [TestMethod]
        public void Defaults()
        {
            var generator = new Generator();
            Assert.IsTrue(generator.Separator == " ");
            Assert.IsTrue(generator.Casing == Casing.LowerCase);
            Assert.IsTrue(generator.EndsWith == "");
        }

        [TestMethod]
        public void Generate()
        {
            var generator = new Generator();
            generator.SetParts(WordBank.Adjectives, WordBank.Nouns);
            string result = generator.Generate();
            Assert.IsNotNull(result);
            Assert.AreNotEqual("", result);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(4)]
        [DataRow(8)]
        public void GenerateMany(int count)
        {
            var generator = new Generator();
            generator.SetParts(WordBank.Adjectives, WordBank.Nouns);
            string[] results = generator.GenerateMany(count);
            Assert.IsNotNull(results);
            Assert.AreEqual(count, results.Length);
            foreach (var result in results)
            {
                Assert.AreNotEqual("", result);
            }
        }

        [TestMethod]
        [DataRow("-")]
        [DataRow("_")]
        [DataRow(".")]
        public void Generate_SetSeparator(string separator)
        {
            var generator = new Generator();
            generator.SetParts(WordBank.Adjectives, WordBank.Nouns);
            generator.Separator = separator;
            string result = generator.Generate();
            Assert.IsTrue(result.Contains(separator.ToString()));
        }

        [TestMethod]
        [DataRow(2, "-.-")]
        [DataRow(4, ".")]
        [DataRow(6, "-")]
        public void GenerateMany_SetSeparator(int count, string separator)
        {
            var generator = new Generator();
            generator.SetParts(WordBank.Adjectives, WordBank.Nouns);
            generator.Separator = separator;
            var results = generator.GenerateMany(count);
            foreach (var result in results)
            {
                Assert.IsTrue(result.Contains(separator.ToString()));
            }
        }

        [TestMethod]
        public void Generate_SetEndsWith()
        {
            var generator = new Generator();
            generator.SetParts(WordBank.Adjectives, WordBank.Nouns);
            var emailSuffix = "@gmail.com";
            generator.EndsWith = emailSuffix;
            var result = generator.Generate();
            Assert.IsTrue(result.EndsWith(emailSuffix));
        }

        [TestMethod]
        public void Generate_SetParts()
        {
            var generator = new Generator();
            generator.SetParts(WordBank.Titles, WordBank.FirstNames, WordBank.LastNames);
            Assert.IsTrue(generator.Parts[0] == WordBank.Titles);
            Assert.IsTrue(generator.Parts[1] == WordBank.FirstNames);
            Assert.IsTrue(generator.Parts[2] == WordBank.LastNames);

            var result = generator.Generate();
            var strings = result.Split(new string[] { generator.Separator }, StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(strings.Length == 3);
            Assert.IsTrue(strings[0] == "aunt" || strings[0] == "uncle");
            Assert.IsTrue(strings[1] == "david" || strings[1] == "roger");
            Assert.IsTrue(strings[2] == "smith" || strings[2] == "jones");
        }

        [TestMethod]
        [DataRow("cat dog")]
        [DataRow("dog cat")]
        [DataRow("cat cat")]
        [DataRow("dog dog")]
        public void GenerateUnique(string reserved)
        {
            var generator = new Generator();
            generator.SetParts(WordBank.Nouns);
            var result = generator.GenerateUnique(new string[] { reserved });
            Assert.AreNotEqual(reserved, result);
        }

        [TestMethod]
        public void CasingEnum()
        {
            var pascalCase = Casing.PascalCase;
            var camelCase = Casing.CamelCase;
            var upperCase = Casing.UpperCase;
            var lowerCase = Casing.LowerCase;
        }

        [TestMethod]
        public void Generate_SetCasing()
        {
            var generator = new Generator();
            generator.Separator = "";
            generator.Casing = (Casing.PascalCase);
            generator.SetParts(WordBank.JobTitles, WordBank.FirstNames);
            var result = generator.Generate();
            var regex = @"^[A-Z][a-z]+([A-Z][a-z]+)+$";
            Assert.IsTrue(Regex.IsMatch(result, regex));

            generator.Casing = (Casing.CamelCase);
            result = generator.Generate();
            regex = @"^([a-z][a-z0-9]*[A-Z][A-Z0-9]*[a-z])[A-Za-z0-9]*";
            Assert.IsTrue(Regex.IsMatch(result, regex));

            generator.Casing = (Casing.UpperCase);
            result = generator.Generate();
            regex = @"[A-Z]";
            Assert.IsTrue(Regex.IsMatch(result, regex));

            generator.Casing = Casing.LowerCase;
            result = generator.Generate();
            regex = @"[a-z]";
            Assert.IsTrue(Regex.IsMatch(result, regex));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void HandleNulls(int test)
        {
            var generator = new Generator();

            switch (test)
            {
                case 0:
                    generator.Parts = null;
                    break;
                case 1:
                    generator.Separator = null;
                    break;
                case 2:
                    generator.EndsWith = null;
                    break;
                case 3:
                    generator.Parts = null;
                    break;
            }
        }
    }
}