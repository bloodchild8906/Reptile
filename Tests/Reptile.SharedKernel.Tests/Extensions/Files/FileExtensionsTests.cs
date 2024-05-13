using FluentAssertions;
using Reptile.SharedKernel.Extensions.Files;

namespace Reptile.SharedKernel.Tests.Extensions.Files;

[TestFixture]
public class FileExtensionsTests
{
    /* Implement this function */
    [TestCase(".txt", true, Description = "Check a simple valid file extension.")]
    [TestCase(".jpg", true, Description = "Check a valid image file extension.")]
    [TestCase("jpg", false, Description = "Check an extension without the dot.")]
    [TestCase(".j*pg", false, Description = "Check an extension with invalid characters.")]
    [Category("Validation")]
    public void IsFileExtensionValid_ReturnsExpectedResult(string extension, bool expected)
    {
        var result = extension.IsFileExtensionValid();
    
        result.Should().Be(expected);
    }

    [TestCase("image.jpg", false, "image.js", Description = "Convert a JPG file extension to a JS extension.")]
    [TestCase("document.docx", false, "document.js", Description = "Convert a DOCX file extension to a JS extension.")]
    [TestCase("archive.tar.gz", true, "archive.tar.gz.js", Description = "Handle files with double extensions.")]
    [TestCase("script.js", false, "script.js", Description = "Ensure that an existing JS file remains unchanged.")]
    [TestCase("image.jpg", true, "image.jpg.js", Description = "Force appending.js even when already valid.")]
    [Category("Transformation")]
    public void ToJavaScriptExtension_ReturnsExpectedResult(string originalFilename, bool append, string expected)
    {
        var result = originalFilename.ToJavaScriptExtension(append);
        result.Should().Be(expected);
    }
}