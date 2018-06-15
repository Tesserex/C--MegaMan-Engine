using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MegaMan.Common.Tests
{
    [TestClass]
    public class FilePathTests
    {
        [TestMethod, TestCategory("FilePath")]
        public void ShouldGetRelativeWithTrailingSlashes()
        {
            var filepathWithTrailingSlashes = FilePath.FromAbsolute(@"C:\foo\bar\baz\", @"C:\foo\");

            var expected = @"bar\baz\";

            Assert.AreEqual(expected, filepathWithTrailingSlashes.Relative);
        }

        [TestMethod, TestCategory("FilePath")]
        public void ShouldGetRelativeWithoutTrailingSlashes()
        {
            var filepathWithoutTrailingSlashes = FilePath.FromAbsolute(@"C:\foo\bar\baz", @"C:\foo");

            var expected = @"bar\baz";

            Assert.AreEqual(expected, filepathWithoutTrailingSlashes.Relative);
        }

        [TestMethod, TestCategory("FilePath")]
        public void ShouldGetAbsoluteWithTrailingSlashes()
        {
            var filepathWithTrailingSlashes = FilePath.FromRelative(@"bar\baz\", @"C:\foo\");

            var expected = @"C:\foo\bar\baz\";

            Assert.AreEqual(expected, filepathWithTrailingSlashes.Absolute);
        }

        [TestMethod, TestCategory("FilePath")]
        public void ShouldGetAbsoluteWithoutTrailingSlashes()
        {
            var filepathWithoutTrailingSlashes = FilePath.FromRelative(@"bar\baz", @"C:\foo");

            var expected = @"C:\foo\bar\baz";

            Assert.AreEqual(expected, filepathWithoutTrailingSlashes.Absolute);
        }

        [TestMethod, TestCategory("FilePath")]
        public void ShouldFindRelativeAncestorPath()
        {
            var filepathWithoutTrailingSlashes = FilePath.FromAbsolute(@"C:\foo\bar.txt", @"C:\foo\baz\buzz");

            var expected = @"..\..\bar.txt";

            Assert.AreEqual(expected, filepathWithoutTrailingSlashes.Relative);
        }

        [TestMethod, TestCategory("FilePath"), ExpectedException(typeof(ArgumentException))]
        public void ExceptionWhenAbsoluteNull()
        {
            var filepath = FilePath.FromAbsolute(null, @"C:\foo");

            Assert.IsNull(filepath.Relative);
        }

        [TestMethod, TestCategory("FilePath"), ExpectedException(typeof(ArgumentException))]
        public void ExceptionNullWhenBasePathNull()
        {
            var filepath = FilePath.FromAbsolute(@"C:\foo\bar.txt", null);

            Assert.IsNull(filepath.Relative);
        }

        [TestMethod, TestCategory("FilePath")]
        public void AbsolutePathShouldNotContainBackDots()
        {
            var filepath = FilePath.FromRelative(@"..\..\bar\baz", @"C:\foo\stuff");

            var expected = @"C:\bar\baz";

            Assert.AreEqual(expected, filepath.Absolute);
        }

        [TestMethod, TestCategory("FilePath")]
        public void RelativeShouldAllowExtensionsInBasePath()
        {
            var filepath = FilePath.FromRelative(@"bar\baz", @"C:\foo\stuff.zip");

            var expected = @"C:\foo\stuff.zip\bar\baz";

            Assert.AreEqual(expected, filepath.Absolute);
        }

        [TestMethod, TestCategory("FilePath")]
        public void AbsoluteShouldAllowExtensionsInBasePath()
        {
            var filepath = FilePath.FromAbsolute(@"C:\foo\stuff.zip\bar\baz", @"C:\foo\stuff.zip");

            var expected = @"bar\baz";

            Assert.AreEqual(expected, filepath.Relative);
        }
    }
}
