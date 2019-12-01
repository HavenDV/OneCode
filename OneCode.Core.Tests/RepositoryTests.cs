using System;
using System.Collections.Generic;
using System.IO;
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
            var code = Code.Load(text);

            foreach (var method in code.Methods)
            {
                Console.WriteLine(method.Name);
            }

            Assert.AreEqual(2, code.Methods.Count);

            Assert.AreEqual(Version.Parse("1.1.1.1"), code.Methods[0].Version);
            Assert.AreEqual(Version.Parse("1.0.0.0"), code.Methods[1].Version);

            CollectionAssert.AreEqual(new List<string>{ "GetMethods(string path)", "GetMethods(string path)" }, code.Methods[0].Dependencies);
            CollectionAssert.AreEqual(new List<string>(), code.Methods[1].Dependencies);
        }

        [TestMethod]
        public void RepositoryLoadTest()
        {
            var repository = Repository.Load("../../../../OneCode.Core");

            foreach (var file in repository.Files)
            {
                Console.WriteLine($"RelativePath: {file.RelativePath}");

                foreach (var method in file.Code.Methods)
                {
                    Console.WriteLine($"- {method.Name}");
                }
            }

            Assert.IsTrue(repository.Files.Any());
        }

        [TestMethod]
        public void CodeLoadTest()
        {
            var path = Path.GetFullPath(Directory
                .EnumerateFiles("../../../../../CSharpUtilities", "*.cs", SearchOption.AllDirectories)
                .FirstOrDefault());
            var file = CodeFile.Load(path, Path.GetFullPath("../../../../../CSharpUtilities"));

            Console.WriteLine($"FullPath: {file.FullPath}");
            Console.WriteLine($"TargetFramework: {file.TargetFramework}");
            Console.WriteLine($"RelativePath: {file.RelativePath}");
            Console.WriteLine($"RelativePathWithoutTargetFramework: {file.RelativePathWithoutTargetFramework}");
            Console.WriteLine($"RelativeFolder: {file.RelativeFolder}");
            Console.WriteLine($"RelativeFolderWithoutTargetFramework: {file.RelativeFolderWithoutTargetFramework}");
            Console.WriteLine($"AdditionalNamespace: {file.AdditionalNamespace}");

            foreach (var method in file.Code.Methods)
            {
                Console.WriteLine($"- {method.Name}");
            }
        }

        [TestMethod]
        public void CodeSaveTest()
        {
            var text = ResourcesUtilities.ReadFile("Repository.cs");
            var code = Code.Load(text);

            Assert.AreEqual(2, code.Methods.Count);
            Assert.AreEqual("OneCode.Core", code.NamespaceName);

            code.NamespaceName = "TEST";
            code.Methods.RemoveAt(0);

            var newCodeText = code.Save();
            Console.WriteLine(newCodeText); 
            
            code = Code.Load(newCodeText);

            Assert.AreEqual(1, code.Methods.Count);
            Assert.AreEqual("TEST", code.NamespaceName);
        }
    }
}
