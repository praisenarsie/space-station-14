using System.IO;
using NUnit.Framework;
using SS14.Client.Graphics;
using YamlDotNet.RepresentationModel;

namespace SS14.UnitTesting.Client.Graphics
{
    [TestFixture]
    public class TextureLoadParametersTest
    {
        [Test]
        public void TestLoadEmptyYaml()
        {
            // Test whether it defaults for empty YAML.
            var yaml = new YamlMappingNode();
            var loaded = TextureLoadParameters.FromYaml(yaml);
            var defaultParams = TextureLoadParameters.Default;
            Assert.That(loaded.SampleParameters.Filter, Is.EqualTo(defaultParams.SampleParameters.Filter));
            Assert.That(loaded.SampleParameters.WrapMode, Is.EqualTo(defaultParams.SampleParameters.WrapMode));
        }

        [Test]
        public void TestLoadEmptySamplingYaml()
        {
            // Test whether it defaults for empty YAML.
            var yaml = _getMapping("sample: {}\n");
            var loaded = TextureLoadParameters.FromYaml(yaml);
            var defaultParams = TextureLoadParameters.Default;
            Assert.That(loaded.SampleParameters.Filter, Is.EqualTo(defaultParams.SampleParameters.Filter));
            Assert.That(loaded.SampleParameters.WrapMode, Is.EqualTo(defaultParams.SampleParameters.WrapMode));
        }

        [Test]
        public void TestLoadYamlOne()
        {
            var yaml = _getMapping(TestDataOne);
            var loaded = TextureLoadParameters.FromYaml(yaml);
            Assert.That(loaded.SampleParameters.Filter, Is.EqualTo(true));
            Assert.That(loaded.SampleParameters.WrapMode, Is.EqualTo(TextureWrapMode.Repeat));
        }

        [Test]
        public void TestLoadYamlTwo()
        {
            var yaml = _getMapping(TestDataTwo);
            var loaded = TextureLoadParameters.FromYaml(yaml);
            Assert.That(loaded.SampleParameters.Filter, Is.EqualTo(false));
            Assert.That(loaded.SampleParameters.WrapMode, Is.EqualTo(TextureWrapMode.MirroredRepeat));
        }

        [Test]
        public void TestLoadYamlThree()
        {
            var yaml = _getMapping(TestDataThree);
            var loaded = TextureLoadParameters.FromYaml(yaml);
            Assert.That(loaded.SampleParameters.WrapMode, Is.EqualTo(TextureWrapMode.None));
        }

        private YamlMappingNode _getMapping(string data)
        {
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(data));
            return (YamlMappingNode) yamlStream.Documents[0].RootNode;
        }

        private const string TestDataOne = @"
sample:
  filter: true
  wrap: repeat
";

        private const string TestDataTwo = @"
sample:
  filter: false
  wrap: mirrored_repeat
";

        private const string TestDataThree = @"
sample:
  wrap: none
";
    }
}
