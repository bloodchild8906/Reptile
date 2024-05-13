// StringExtensionsTests.cs

using System.Globalization;
using System.Text;
using FluentAssertions;
using Reptile.SharedKernel.Extensions.String;

// Required for HttpUtility

namespace Reptile.SharedKernel.Tests.Extensions.String;

[TestFixture]
public class StringExtensionsTests
{
    [Test]
    [Description("Ensures that strings are correctly surrounded by double quotes.")]
    [Category("Text Handling")]
    public void SurroundWithDoubleQuotes_ShouldEncloseStringInDoubleQuotes()
    {
        const string input = "test";
        const string expected = "\"test\"";
        var result = input.SurroundWithDoubleQuotes();
        result.Should().Be(expected);
    }

    [Test]
    [Description("Ensures that strings are correctly surrounded by specified characters.")]
    [Category("Text Handling")]
    public void SurroundWith_ShouldEncloseStringInSpecifiedCharacters()
    {
        const string input = "test";
        const string ends = "**";
        const string expected = "**test**";
        var result = input.SurroundWith(ends);
        result.Should().Be(expected);
    }

    [Test]
    [Description("Validates that a sentence is correctly split into words, excluding punctuation.")]
    [Category("Text Handling")]
    public void SplitSentenceIntoWords_ShouldSplitIntoWordsExcludingPunctuation()
    {
        const string sentence = "Hello, world! This is a test.";
        var expected = new[] { "Hello", "world", "This", "is", "a", "test" };
        var result = sentence.SplitSentenceIntoWords().ToArray();
        result.Should().Equal(expected);
    }

    [Test]
    [Description("Ensures that HTML special characters are correctly escaped.")]
    [Category("Text Handling")]
    public void Escape_ShouldConvertToHtmlEntities()
    {
        const string input = "Tom & Jerry";
        const string expected = "Tom &amp; Jerry";
        var result = input.Escape();
        result.Should().Be(expected);
    }

    [Test]
    [Description("Ensures that HTML entities are correctly unescaped to their original characters.")]
    [Category("Text Handling")]
    public void Unescape_ShouldConvertHtmlEntitiesBackToCharacters()
    {
        const string input = "Tom &amp; Jerry";
        const string expected = "Tom & Jerry";
        var result = input.Unescape();
        result.Should().Be(expected);
    }

    [Test]
    [Description("Ensures strings are correctly encoded to Base64.")]
    [Category("Text Handling")]
    public void Base64Encode_ShouldReturnBase64EncodedString()
    {
        const string input = "Hello World";
        var expected = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        var result = input.Base64Encode();
        result.Should().Be(expected);
    }

    [Test]
    [Description("Ensures strings are correctly decoded from Base64.")]
    [Category("Text Handling")]
    public void Base64Decode_ShouldReturnDecodedString()
    {
        var input = Convert.ToBase64String(Encoding.UTF8.GetBytes("Hello World"));
        const string expected = "Hello World";
        var result = input.Base64Decode();
        result.Should().Be(expected);
    }

    [Test]
    [Description("Ensures that strings are correctly transformed to camel case.")]
    [Category("Text Handling")]
    public void ToCamelCase_ShouldConvertStringToCamelCase()
    {
        const string input = "Hello World";
        const string expected = "hello World";
        var result = input.ToCamelCase();
        result.Should().Be(expected);
    }

    [Test]
    [Description("Ensures that strings are correctly transformed to title case.")]
    [Category("Text Handling")]
    public void ToTitleCase_ShouldConvertStringToTitleCase()
    {
        const string input = "hello world";
        const string expected = "Hello World";
        var result = input.ToTitleCase();
        result.Should().Be(expected);
    }

    [Test]
    [Description("Ensures that strings are cleaned according to a specified regex.")]
    [Category("Text Handling")]
    public void Clean_ShouldRemoveSpecifiedCharacters()
    {
        const string input = "hello@world!";
        // ReSharper disable once StringLiteralTypo
        const string expected = "helloworld";
        var result = input.Clean();
        result.Should().Be(expected);
    }

    [Test]
    [Description("Ensures that strings contain only alphanumeric characters.")]
    [Category("Text Handling")]
    public void ToAlphaNumericOnly_ShouldContainOnlyAlphaNumericCharacters()
    {
        const string input = "Hello, World! 123";
        const string expected = "HelloWorld123";
        var result = input.ToAlphaNumericOnly();
        result.Should().Be(expected);
    }

    [Test]
    [Description("Ensures that strings contain only alphabetic characters.")]
    [Category("Text Handling")]
    public void ToAlphaOnly_ShouldContainOnlyAlphabeticCharacters()
    {
        const string input = "123 Hello 456";
        const string expected = "Hello";
        var result = input.ToAlphaOnly();
        result.Should().Be(expected);
    }

    [Test]
    [Description("Ensures that strings contain only numeric characters.")]
    [Category("Text Handling")]
    public void ToNumericOnly_ShouldContainOnlyNumericCharacters()
    {
        const string input = "123 Hello 456";
        const string expected = "123456";
        var result = input.ToNumericOnly();
        result.Should().Be(expected);
    }

    [Test]
    [Description("Verifies that the IsValidRegex method correctly identifies valid regex patterns.")]
    [Category("Text Handling")]
    public void IsValidRegex_ShouldReturnTrueForValidRegex()
    {
        const string validPattern = "^[a-zA-Z0-9]*$";
        var result = validPattern.IsValidRegex();
        result.Should().BeTrue("because the pattern is a valid regex expression.");
    }

    [Test]
    [Description("Verifies that the IsValidRegex method correctly identifies invalid regex patterns.")]
    [Category("Text Handling")]
    public void IsValidRegex_ShouldReturnFalseForInvalidRegex()
    {
        const string invalidPattern = "[a-zA-Z0-9";
        var result = invalidPattern.IsValidRegex();
        result.Should().BeFalse("because the pattern is missing a closing bracket.");
    }

    [Test]
    [Description("Ensures that strings are transformed to title case with cultural awareness.")]
    [Category("Text Handling")]
    public void ToTitleCase_ShouldRespectCulture()
    {
        const string input = "hello world";
        const string cultureInfoName = "tr-TR";
        var cultureInfo = new CultureInfo(cultureInfoName);
        const string expected = "Hello World";
        var result = input.ToTitleCase(cultureInfo);
        result.Should().Be(expected);
    }

    [Test]
    [Description("Ensures that complex string manipulations like adding spaces to a sentence work correctly.")]
    [Category("Text Handling")]
    public void AddSpacesToSentence_ShouldAddSpacesCorrectly()
    {
        const string input = "HelloWorld";
        const string expected = "Hello World";
        var result = input.AddSpacesToSentence();
        result.Should().Be(expected, "because spaces should be inserted between CamelCase words.");
    }
}