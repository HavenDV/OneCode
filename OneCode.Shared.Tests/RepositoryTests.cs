using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneCode.Shared.Settings;
using OneCode.Shared.Tests.Utilities;

namespace OneCode.Shared.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public void GetMethodsTest()
        {
            var text = ResourcesUtilities.ReadFileAsString("Repository.cs");
            var code = new Code(text);
            var methods = code.Classes
                .SelectMany(i => i.Methods)
                .ToList();

            foreach (var method in methods)
            {
                Console.WriteLine(method.Name);
            }

            Assert.AreEqual(2, methods.Count);

            Assert.AreEqual(Version.Parse("1.1.1.1"), methods[0].Version);
            Assert.AreEqual(Version.Parse("1.0.0.0"), methods[1].Version);

            CollectionAssert.AreEqual(new List<string>{ "GetMethods(string path)", "GetMethods(string path)" }, methods[0].Dependencies);
            CollectionAssert.AreEqual(new List<string>(), methods[1].Dependencies);

            Assert.AreEqual(false, methods[0].IsStatic);
            Assert.AreEqual(true, methods[1].IsStatic);

            Assert.AreEqual(false, methods[0].IsExtension);
            Assert.AreEqual(true, methods[1].IsExtension);
        }

        [TestMethod]
        public void RepositoryLoadTest()
        {
            var repository = new Repository(new RepositorySettings
            {
                Folder = "../../../../OneCode.Shared",
            });

            foreach (var file in repository.Files)
            {
                Console.WriteLine($"RelativePath: {file.RelativePath}");

                foreach (var @class in file.Code?.Classes ?? new List<Class>())
                {
                    Console.WriteLine($"Class: {@class.Name}");

                    foreach (var method in @class.Methods)
                    {
                        Console.WriteLine($"- {method.Name}");
                    }
                }
            }

            Assert.IsTrue(repository.Files.Any());
        }

        [TestMethod]
        public void CodeLoadTest()
        {
            var repository = new Repository(new RepositorySettings
            {
                Folder = "../../../../../CSharpUtilities",
            });
            var file = repository.Files.First();

            Console.WriteLine($"FullPath: {file.FullPath}");
            Console.WriteLine($"TargetFramework: {file.TargetFramework}");
            Console.WriteLine($"RelativePath: {file.RelativePath}");
            Console.WriteLine($"RelativePathWithoutTargetFramework: {file.RelativePathWithoutTargetFramework}");
            Console.WriteLine($"RelativeFolder: {file.RelativeFolder}");
            Console.WriteLine($"RelativeFolderWithoutTargetFramework: {file.RelativeFolderWithoutTargetFramework}");
            Console.WriteLine($"AdditionalNamespace: {file.AdditionalNamespace}");

            foreach (var method in file.Code?.Classes?.SelectMany(i => i.Methods) ?? new List<Method>())
            {
                Console.WriteLine($"- {method.Name}");
            }
        }

        [TestMethod]
        public void CodeSaveTest()
        {
            var text = ResourcesUtilities.ReadFileAsString("Repository.cs");
            var code = new Code(text);

            Assert.AreEqual(2, code.Classes[0].Methods.Count);
            Assert.AreEqual("OneCode.Shared", code.NamespaceName);

            code.NamespaceName = "TEST";
            code.Classes[0].Methods.RemoveAt(0);

            var newCodeText = code.Save();
            Console.WriteLine(newCodeText); 
            
            code = new Code(newCodeText);

            Assert.AreEqual(1, code.Classes[0].Methods.Count);
            Assert.AreEqual("TEST", code.NamespaceName);
        }
    }
}
