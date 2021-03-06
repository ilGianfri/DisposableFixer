using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue137_PropertyOfOtherType : IssueSpec
    {

        private const string Code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class ClassWithPublicSetableProperty : IDisposable
    {

        public IDisposable Property { get; set; }

        public void Dispose()
        {
            Property?.Dispose();
        }
    }

    class MyClass
    {
        public MyClass()
        {
            using (var instance = new ClassWithPublicSetableProperty())
            {
                instance.Property = new MemoryStream();//should generate a warning about not disposed property of other object
            }
        }
    }
}
";

        [Test]
        public void Then_there_should_one_diagnostic_with_correct_message()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Descriptor.Should().Be(NotDisposed.Assignment.FromObjectCreation.ToProperty.OfAnotherTypeDescriptor);
        }
    }
}