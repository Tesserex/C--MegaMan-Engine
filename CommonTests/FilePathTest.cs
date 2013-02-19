using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MegaMan.Common;

namespace MegaMan.Common.Tests
{
    [TestClass]
    public class FilePathTests
    {
        [TestMethod]
        public void ShouldGetRelativeWithTrailingSlashes()
        {
            var filepathWithTrailingSlashes = FilePath.FromAbsolute(@"C:\foo\bar\baz\", @"C:\foo\");

            var expected = @"bar\baz\";

            Assert.AreEqual(expected, filepathWithTrailingSlashes.Relative);
        }

        [TestMethod]
        public void ShouldGetRelativeWithoutTrailingSlashes()
        {
            var filepathWithoutTrailingSlashes = FilePath.FromAbsolute(@"C:\foo\bar\baz", @"C:\foo");

            var expected = @"bar\baz";

            Assert.AreEqual(expected, filepathWithoutTrailingSlashes.Relative);
        }

        [TestMethod]
        public void ShouldGetAbsoluteWithTrailingSlashes()
        {
            var filepathWithTrailingSlashes = FilePath.FromRelative(@"bar\baz\", @"C:\foo\");

            var expected = @"C:\foo\bar\baz\";

            Assert.AreEqual(expected, filepathWithTrailingSlashes.Absolute);
        }

        [TestMethod]
        public void ShouldGetAbsoluteWithoutTrailingSlashes()
        {
            var filepathWithoutTrailingSlashes = FilePath.FromRelative(@"bar\baz", @"C:\foo");

            var expected = @"C:\foo\bar\baz";

            Assert.AreEqual(expected, filepathWithoutTrailingSlashes.Absolute);
        }
    }
}
