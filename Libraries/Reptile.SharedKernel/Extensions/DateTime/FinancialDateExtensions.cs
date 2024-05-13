namespace Reptile.SharedKernel.Extensions.DateTime;

public static class FinancialDateExtensions
{
    public static string FormatFinancialYear(this System.DateTime date)
    {
        var year1 = date.Year - (date.Month <= 6 ? 1 : 0);
        var year2 = date.Year + (date.Month <= 6 ? 0 : 1);
        return $"{year1}/{year2}";
    }

	public static System.DateTime GetFinancialYearStartDate(this System.DateTime date) => new System.DateTime(date.Year - (date.Month <= 6 ? 1 : 0), 7, 1);

	public static System.DateTime GetFinancialYearEndDate(this System.DateTime date) => new System.DateTime(date.Year + (date.Month <= 6 ? 0 : 1), 6, 30);
}