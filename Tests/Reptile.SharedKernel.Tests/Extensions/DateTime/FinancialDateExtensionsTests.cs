using FluentAssertions;
using Reptile.SharedKernel.Extensions.DateTime;

namespace Reptile.SharedKernel.Tests.Extensions.DateTime;

[TestFixture]
public class FinancialDateExtensionsTests
{
    [Test(Description = "Ensure that the financial year is correctly formatted.")]
    [Category("FinancialDates")]
    public void FormatFinancialYear_CorrectlyFormats()
    {
        var date = new System.DateTime(2023, 7, 1);
        var result = date.FormatFinancialYear();

        result.Should().Be("2023/2024");
    }

    [Test(Description = "Verify that the start date of the financial year is calculated correctly.")]
    [Category("FinancialDates")]
    public void GetFinancialYearStartDate_ReturnsCorrectDate()
    {
        var date = new System.DateTime(2023, 8, 15);
        var result = date.GetFinancialYearStartDate();

        result.Should().Be(new System.DateTime(2023, 7, 1));
    }

    [Test(Description = "Verify that the end date of the financial year is calculated correctly.")]
    [Category("FinancialDates")]
    public void GetFinancialYearEndDate_ReturnsCorrectDate()
    {
        var date = new System.DateTime(2023, 2, 15);
        var result = date.GetFinancialYearEndDate();

        result.Should().Be(new System.DateTime(2023, 6, 30));
    }
}