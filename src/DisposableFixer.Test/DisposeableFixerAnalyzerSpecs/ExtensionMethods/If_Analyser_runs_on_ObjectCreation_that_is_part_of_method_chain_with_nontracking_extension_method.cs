using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ExtensionMethods
{
    [TestFixture]
    internal class If_Analyser_runs_on_ObjectCreation_that_is_part_of_method_chain_with_nontracking_extension_method :
        DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System;
using System.IO;
using System.Collections.Generic;
using Reactive.Bindings.Extensions;
namespace DisFixerTest {
    internal class Usage : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>(); 
        public Usage()
        {
            // two diagnostics newMemoryStream() and return value of AddTo - both are the same instance, but analyzer does'nt know that
            new MemoryStream().AddTo(_disposables);
        }
        public void SomeMethod(){}

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
namespace Reactive.Bindings.Extensions{
    internal static class ExtensionNotInListOfTrackingTypes
    {
        public static T AddTo<T>(this T item, ICollection<T> disposables) where T : IDisposable
        {
            disposables.Add(item);

            return item;
        }
    }
}
";

        [Test]
        public void Then_there_should_be_one_Diagnostics()
        {
            PrintCodeToFix(Code);
            _diagnostics.Length.Should().Be(2);
        }
    }
}