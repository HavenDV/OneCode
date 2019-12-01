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

            foreach (var (name, _) in methods)
            {
                Console.WriteLine(name);
            }

            Assert.AreEqual(2, methods.Count);
        }

        [TestMethod]
        public void LoadTest()
        {
            var dictionary = Repository.Load("../../../../OneCode.Core");

            foreach (var (path, methods) in dictionary)
            {
                Console.WriteLine($"Path: {path}");
                foreach (var (name, _) in methods)
                {
                    Console.WriteLine($"- {name}");
                }
            }

            Assert.IsTrue(dictionary.Any());
        }
    }
}
