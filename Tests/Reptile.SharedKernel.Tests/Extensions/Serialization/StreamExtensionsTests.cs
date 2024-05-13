using System.Text;
using FluentAssertions;
using Reptile.SharedKernel.Extensions.Serialization;

namespace Reptile.SharedKernel.Tests.Extensions.Serialization;

[TestFixture]
public class StreamExtensionsTests
{
    [Test(Description = "Convert a stream to a byte array and ensure it matches the original content.")]
    [Category("StreamConversion")]
    public void ToByteArray_ConvertsStreamToByteArray()
    {
        // Arrange
        const string data = "Hello, stream!";
        byte[] expectedArray = Encoding.UTF8.GetBytes(data);
        using var stream = new MemoryStream(expectedArray);

        // Act
        var resultArray = stream.ToByteArray();

        // Assert
        resultArray.Should().Equal(expectedArray);
    }

    [Test(Description = "Convert a stream to a string and verify the output.")]
    [Category("StreamConversion")]
    public void ConvertToString_ConvertsStreamToString()
    {
        const string data = "Hello, stream!";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var result = stream.ConvertToString();
        result.Should().Be(data);
    }

    [Test(Description = "Write a stream to a file and ensure the file contains the correct data.")]
    [Category("StreamToFile")]
    public void ConvertToFile_WritesStreamToFile()
    {
        var tempPath = Path.GetTempFileName();
        const string data = "Test data for file";
        try
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            stream.ConvertToFile(tempPath);

            var fileContents = File.ReadAllText(tempPath);
            fileContents.Should().Be(data);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}