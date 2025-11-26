using ClosedXML.Excel;
using SellerMetrics.Application.TaxReporting.DTOs;
using SellerMetrics.Application.TaxReporting.Queries;
using System.Text;

namespace SellerMetrics.Application.TaxReporting.Commands;

/// <summary>
/// Command to export tax report to CSV or Excel format.
/// </summary>
public record ExportTaxReportCommand(
    int Year,
    ExportFormat Format = ExportFormat.Excel,
    string Currency = "USD");

/// <summary>
/// Export format options.
/// </summary>
public enum ExportFormat
{
    Csv,
    Excel
}

/// <summary>
/// Result of export containing file data.
/// </summary>
public class ExportTaxReportResult
{
    public byte[] FileContents { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}

/// <summary>
/// Handler for ExportTaxReportCommand.
/// </summary>
public class ExportTaxReportCommandHandler
{
    private readonly GetAnnualSummaryQueryHandler _annualSummaryHandler;

    public ExportTaxReportCommandHandler(GetAnnualSummaryQueryHandler annualSummaryHandler)
    {
        _annualSummaryHandler = annualSummaryHandler;
    }

    public async Task<ExportTaxReportResult> HandleAsync(
        ExportTaxReportCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get the annual summary data
        var query = new GetAnnualSummaryQuery(command.Year, command.Currency);
        var summary = await _annualSummaryHandler.HandleAsync(query, cancellationToken);

        return command.Format switch
        {
            ExportFormat.Csv => ExportToCsv(summary, command.Year),
            ExportFormat.Excel => ExportToExcel(summary, command.Year),
            _ => throw new ArgumentOutOfRangeException(nameof(command.Format))
        };
    }

    private static ExportTaxReportResult ExportToCsv(AnnualSummaryDto summary, int year)
    {
        var sb = new StringBuilder();

        // Header
        sb.AppendLine($"Tax Report - {year}");
        sb.AppendLine();

        // Summary Section
        sb.AppendLine("ANNUAL SUMMARY");
        sb.AppendLine($"Total Gross Revenue,{summary.TotalGrossRevenueFormatted}");
        sb.AppendLine($"Total Fees,{summary.TotalFeesFormatted}");
        sb.AppendLine($"Total Net Revenue,{summary.TotalNetRevenueFormatted}");
        sb.AppendLine($"eBay Revenue,{summary.EbayRevenueFormatted}");
        sb.AppendLine($"Service Revenue,{summary.ServiceRevenueFormatted}");
        sb.AppendLine($"Total Expenses,{summary.TotalExpensesFormatted}");
        sb.AppendLine($"Mileage Deduction,{summary.TotalMileageDeductionFormatted}");
        sb.AppendLine($"Net Profit,{summary.NetProfitFormatted}");
        sb.AppendLine();

        // Quarterly Breakdown
        sb.AppendLine("QUARTERLY BREAKDOWN");
        sb.AppendLine("Quarter,eBay Revenue,Service Revenue,Total Revenue,Total Expenses,Mileage Deduction,Net Profit");
        foreach (var q in summary.QuarterlyBreakdown)
        {
            sb.AppendLine($"{q.QuarterDisplay},{q.EbayRevenueFormatted},{q.ServiceRevenueFormatted},{q.TotalRevenueFormatted},{q.TotalExpensesFormatted},{q.MileageDeductionFormatted},{q.NetProfitFormatted}");
        }
        sb.AppendLine();

        // Schedule C Section
        if (summary.ScheduleC != null)
        {
            sb.AppendLine("SCHEDULE C PREVIEW");
            sb.AppendLine();
            sb.AppendLine("PART I - INCOME");
            sb.AppendLine($"Line 1 - Gross receipts or sales,{summary.ScheduleC.Line1Formatted}");
            sb.AppendLine($"Line 3 - Gross receipts minus returns,{summary.ScheduleC.Line3Formatted}");
            sb.AppendLine($"Line 4 - Cost of goods sold,{summary.ScheduleC.Line4Formatted}");
            sb.AppendLine($"Line 5 - Gross profit,{summary.ScheduleC.Line5Formatted}");
            sb.AppendLine($"Line 7 - Gross income,{summary.ScheduleC.Line7Formatted}");
            sb.AppendLine();

            sb.AppendLine("PART II - EXPENSES");
            sb.AppendLine("Line,Description,Amount");
            foreach (var line in summary.ScheduleC.ExpenseLines)
            {
                sb.AppendLine($"{line.LineLabel},{line.Description},{line.AmountFormatted}");
            }
            sb.AppendLine($"Line 28,Total expenses,{summary.ScheduleC.Line28Formatted}");
            sb.AppendLine();

            sb.AppendLine("NET PROFIT");
            sb.AppendLine($"Line 29 - Tentative profit,{summary.ScheduleC.Line29Formatted}");
            sb.AppendLine($"Line 31 - Net profit or loss,{summary.ScheduleC.Line31Formatted}");
            sb.AppendLine();
        }

        // Estimated Tax Payments
        sb.AppendLine("ESTIMATED TAX PAYMENTS");
        sb.AppendLine("Quarter,Due Date,Estimated Amount,Amount Paid,Status,Confirmation");
        foreach (var payment in summary.EstimatedTaxPayments)
        {
            var status = payment.IsPaid ? "Paid" : (payment.IsOverdue ? "OVERDUE" : "Pending");
            sb.AppendLine($"{payment.QuarterDisplay},{payment.DueDateFormatted},{payment.EstimatedAmountFormatted},{payment.AmountPaidFormatted},{status},{payment.ConfirmationNumber ?? ""}");
        }

        return new ExportTaxReportResult
        {
            FileContents = Encoding.UTF8.GetBytes(sb.ToString()),
            FileName = $"TaxReport_{year}.csv",
            ContentType = "text/csv"
        };
    }

    private static ExportTaxReportResult ExportToExcel(AnnualSummaryDto summary, int year)
    {
        using var workbook = new XLWorkbook();

        // Summary Sheet
        CreateSummarySheet(workbook, summary, year);

        // Quarterly Detail Sheet
        CreateQuarterlySheet(workbook, summary);

        // Schedule C Sheet
        if (summary.ScheduleC != null)
        {
            CreateScheduleCSheet(workbook, summary.ScheduleC);
        }

        // Estimated Tax Payments Sheet
        CreateTaxPaymentsSheet(workbook, summary);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return new ExportTaxReportResult
        {
            FileContents = stream.ToArray(),
            FileName = $"TaxReport_{year}.xlsx",
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };
    }

    private static void CreateSummarySheet(IXLWorkbook workbook, AnnualSummaryDto summary, int year)
    {
        var ws = workbook.Worksheets.Add("Summary");

        // Title
        ws.Cell("A1").Value = $"Tax Report Summary - {year}";
        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Font.FontSize = 16;
        ws.Range("A1:B1").Merge();

        // Annual Totals
        var row = 3;
        ws.Cell(row, 1).Value = "ANNUAL TOTALS";
        ws.Cell(row, 1).Style.Font.Bold = true;
        row++;

        AddSummaryRow(ws, ref row, "Total Gross Revenue", summary.TotalGrossRevenue);
        AddSummaryRow(ws, ref row, "Total Fees", summary.TotalFees);
        AddSummaryRow(ws, ref row, "Total Net Revenue", summary.TotalNetRevenue);
        row++;
        AddSummaryRow(ws, ref row, "eBay Revenue", summary.EbayRevenue);
        AddSummaryRow(ws, ref row, "Service Revenue", summary.ServiceRevenue);
        row++;
        AddSummaryRow(ws, ref row, "Total Expenses", summary.TotalExpenses);
        AddSummaryRow(ws, ref row, "Mileage Deduction", summary.TotalMileageDeduction);
        row++;
        ws.Cell(row, 1).Value = "NET PROFIT";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 2).Value = summary.NetProfit;
        ws.Cell(row, 2).Style.NumberFormat.Format = "$#,##0.00";
        ws.Cell(row, 2).Style.Font.Bold = true;
        if (summary.NetProfit >= 0)
            ws.Cell(row, 2).Style.Font.FontColor = XLColor.DarkGreen;
        else
            ws.Cell(row, 2).Style.Font.FontColor = XLColor.DarkRed;

        ws.Columns().AdjustToContents();
    }

    private static void AddSummaryRow(IXLWorksheet ws, ref int row, string label, decimal value)
    {
        ws.Cell(row, 1).Value = label;
        ws.Cell(row, 2).Value = value;
        ws.Cell(row, 2).Style.NumberFormat.Format = "$#,##0.00";
        row++;
    }

    private static void CreateQuarterlySheet(IXLWorkbook workbook, AnnualSummaryDto summary)
    {
        var ws = workbook.Worksheets.Add("Quarterly");

        // Headers
        var headers = new[] { "Quarter", "eBay Revenue", "Service Revenue", "Total Revenue", "Expenses", "Mileage", "Net Profit" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // Data
        var row = 2;
        foreach (var q in summary.QuarterlyBreakdown)
        {
            ws.Cell(row, 1).Value = q.QuarterDisplay;
            ws.Cell(row, 2).Value = q.EbayRevenue;
            ws.Cell(row, 3).Value = q.ServiceRevenue;
            ws.Cell(row, 4).Value = q.TotalRevenue;
            ws.Cell(row, 5).Value = q.TotalExpenses;
            ws.Cell(row, 6).Value = q.MileageDeduction;
            ws.Cell(row, 7).Value = q.NetProfit;

            // Format currency columns
            for (int col = 2; col <= 7; col++)
            {
                ws.Cell(row, col).Style.NumberFormat.Format = "$#,##0.00";
            }

            // Color net profit
            if (q.NetProfit >= 0)
                ws.Cell(row, 7).Style.Font.FontColor = XLColor.DarkGreen;
            else
                ws.Cell(row, 7).Style.Font.FontColor = XLColor.DarkRed;

            row++;
        }

        // Totals row
        ws.Cell(row, 1).Value = "TOTAL";
        ws.Cell(row, 1).Style.Font.Bold = true;
        for (int col = 2; col <= 7; col++)
        {
            ws.Cell(row, col).FormulaA1 = $"=SUM({ws.Cell(2, col).Address}:{ws.Cell(row - 1, col).Address})";
            ws.Cell(row, col).Style.NumberFormat.Format = "$#,##0.00";
            ws.Cell(row, col).Style.Font.Bold = true;
        }

        ws.Columns().AdjustToContents();
    }

    private static void CreateScheduleCSheet(IXLWorkbook workbook, ScheduleCSummaryDto scheduleC)
    {
        var ws = workbook.Worksheets.Add("Schedule C");

        // Title
        ws.Cell("A1").Value = $"Schedule C Preview - {scheduleC.Year}";
        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Font.FontSize = 14;

        var row = 3;

        // Part I - Income
        ws.Cell(row, 1).Value = "PART I - INCOME";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
        ws.Range(row, 1, row, 3).Merge();
        row++;

        AddScheduleCLine(ws, ref row, "Line 1", "Gross receipts or sales", scheduleC.Line1_GrossReceipts);
        AddScheduleCLine(ws, ref row, "Line 3", "Gross receipts minus returns", scheduleC.Line3_NetReceipts);
        AddScheduleCLine(ws, ref row, "Line 4", "Cost of goods sold", scheduleC.Line4_CostOfGoodsSold);
        AddScheduleCLine(ws, ref row, "Line 5", "Gross profit", scheduleC.Line5_GrossProfit);
        AddScheduleCLine(ws, ref row, "Line 7", "Gross income", scheduleC.Line7_GrossIncome);

        row++;

        // Part II - Expenses
        ws.Cell(row, 1).Value = "PART II - EXPENSES";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
        ws.Range(row, 1, row, 3).Merge();
        row++;

        foreach (var line in scheduleC.ExpenseLines)
        {
            AddScheduleCLine(ws, ref row, line.LineLabel, line.Description, line.Amount);
        }

        row++;
        AddScheduleCLine(ws, ref row, "Line 28", "Total expenses", scheduleC.Line28_TotalExpenses);
        ws.Cell(row - 1, 1).Style.Font.Bold = true;
        ws.Cell(row - 1, 3).Style.Font.Bold = true;

        row++;

        // Net Profit
        ws.Cell(row, 1).Value = "NET PROFIT";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
        ws.Range(row, 1, row, 3).Merge();
        row++;

        AddScheduleCLine(ws, ref row, "Line 29", "Tentative profit", scheduleC.Line29_TentativeProfit);
        AddScheduleCLine(ws, ref row, "Line 31", "Net profit or loss", scheduleC.Line31_NetProfit);
        ws.Cell(row - 1, 1).Style.Font.Bold = true;
        ws.Cell(row - 1, 3).Style.Font.Bold = true;
        if (scheduleC.Line31_NetProfit >= 0)
            ws.Cell(row - 1, 3).Style.Font.FontColor = XLColor.DarkGreen;
        else
            ws.Cell(row - 1, 3).Style.Font.FontColor = XLColor.DarkRed;

        ws.Columns().AdjustToContents();
    }

    private static void AddScheduleCLine(IXLWorksheet ws, ref int row, string lineLabel, string description, decimal amount)
    {
        ws.Cell(row, 1).Value = lineLabel;
        ws.Cell(row, 2).Value = description;
        ws.Cell(row, 3).Value = amount;
        ws.Cell(row, 3).Style.NumberFormat.Format = "$#,##0.00";
        row++;
    }

    private static void CreateTaxPaymentsSheet(IXLWorkbook workbook, AnnualSummaryDto summary)
    {
        var ws = workbook.Worksheets.Add("Tax Payments");

        // Headers
        var headers = new[] { "Quarter", "Due Date", "Estimated", "Paid", "Remaining", "Status", "Confirmation #", "Payment Method" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // Data
        var row = 2;
        foreach (var payment in summary.EstimatedTaxPayments)
        {
            ws.Cell(row, 1).Value = payment.QuarterDisplay;
            ws.Cell(row, 2).Value = payment.DueDate;
            ws.Cell(row, 2).Style.NumberFormat.Format = "MM/dd/yyyy";
            ws.Cell(row, 3).Value = payment.EstimatedAmount;
            ws.Cell(row, 3).Style.NumberFormat.Format = "$#,##0.00";
            ws.Cell(row, 4).Value = payment.AmountPaid;
            ws.Cell(row, 4).Style.NumberFormat.Format = "$#,##0.00";
            ws.Cell(row, 5).Value = payment.RemainingAmount;
            ws.Cell(row, 5).Style.NumberFormat.Format = "$#,##0.00";

            var status = payment.IsPaid ? "Paid" : (payment.IsOverdue ? "OVERDUE" : "Pending");
            ws.Cell(row, 6).Value = status;
            if (payment.IsPaid)
                ws.Cell(row, 6).Style.Font.FontColor = XLColor.DarkGreen;
            else if (payment.IsOverdue)
            {
                ws.Cell(row, 6).Style.Font.FontColor = XLColor.Red;
                ws.Cell(row, 6).Style.Font.Bold = true;
            }

            ws.Cell(row, 7).Value = payment.ConfirmationNumber ?? "";
            ws.Cell(row, 8).Value = payment.PaymentMethod ?? "";

            row++;
        }

        // Totals
        ws.Cell(row, 1).Value = "TOTAL";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 3).FormulaA1 = $"=SUM(C2:C{row - 1})";
        ws.Cell(row, 3).Style.NumberFormat.Format = "$#,##0.00";
        ws.Cell(row, 3).Style.Font.Bold = true;
        ws.Cell(row, 4).FormulaA1 = $"=SUM(D2:D{row - 1})";
        ws.Cell(row, 4).Style.NumberFormat.Format = "$#,##0.00";
        ws.Cell(row, 4).Style.Font.Bold = true;
        ws.Cell(row, 5).FormulaA1 = $"=SUM(E2:E{row - 1})";
        ws.Cell(row, 5).Style.NumberFormat.Format = "$#,##0.00";
        ws.Cell(row, 5).Style.Font.Bold = true;

        ws.Columns().AdjustToContents();
    }
}
