using Com.Synopsys.Integration.Nuget.Dotnet3.DependencyResolution.Nuget;
using NUnit.Framework;

namespace NugetDotnet5InspectorTests
{
    public class Tests
    {
        private static NuGet.LibraryModel.LibraryDependencyTarget ignored =
            NuGet.LibraryModel.LibraryDependencyTarget.None;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MinumumVersionInclusive()
        {
            GivenVersionAssertParsed("Example",
                new NuGet.Versioning.VersionRange(new NuGet.Versioning.NuGetVersion(1, 0, 0, 0), true), "[1.0.0, )");
        }

        [Test]
        public void MinumumVersionExclusive()
        {
            GivenVersionAssertParsed("Example",
                new NuGet.Versioning.VersionRange(new NuGet.Versioning.NuGetVersion(1, 0, 0, 0), false), "(1.0.0, )");
        }

        [Test]
        public void MaximumVersionInclusive()
        {
            GivenVersionAssertParsed("Example",
                new NuGet.Versioning.VersionRange(null, true, new NuGet.Versioning.NuGetVersion(1, 0, 0, 0), true),
                "(, 1.0.0]");
        }

        [Test]
        public void MaximumVersionExclusive()
        {
            GivenVersionAssertParsed("Example",
                new NuGet.Versioning.VersionRange(null, true, new NuGet.Versioning.NuGetVersion(1, 0, 0, 0), false),
                "(, 1.0.0)");
        }

        public void GivenVersionAssertParsed(string name, NuGet.Versioning.VersionRange versionRange, string expected)
        {
            var library = new NuGet.LibraryModel.LibraryRange(name, versionRange, ignored);
            var raw = library.ToLockFileDependencyGroupString();

            var package = new NugetLockFileResolver(null).ParseProjectFileDependencyGroup(raw);
            Assert.AreEqual(name, package.GetName());
            Assert.AreEqual(expected, package.GetVersionRange().ToNormalizedString());
        }
    }
}
