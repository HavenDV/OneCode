using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneCode.Core.Tests.Utilities;

namespace OneCode.Core.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public void GetMethodsTest()
        {
            var text = ResourcesUtilities.ReadFile("Repository.cs");

            var methods = Repository.GetMethods(text);

            foreach (var method in methods)
            {
                Console.WriteLine(method.Name);
            }

            Assert.AreEqual(2, methods.Count);

            Assert.AreEqual(Version.Parse("1.1.1.1"), methods[0].Version);
            Assert.AreEqual(Version.Parse("1.0.0.0"), methods[1].Version);
        }

        [TestMethod]
        public void LoadTest()
        {
            var dictionary = Repository.Load("../../../../OneCode.Core");

            foreach (var (path, methods) in dictionary)
            {
                Console.WriteLine($"Path: {path}");
                foreach (var method in methods)
                {
                    Console.WriteLine($"- {method.Name}");
                }
            }

            Assert.IsTrue(dictionary.Any());
        }
    }
}
