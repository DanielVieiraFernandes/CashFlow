﻿using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using ClosedXML.Excel;

namespace CashFlow.Application.UseCases.Expenses.Reports.Excel;
public class GenerateExpensesReportUseCase : IGenerateExpensesReportExcelUseCase
{
    private const string CURRENCY_SYMBOL = "R$";
    private readonly IExpensesReadOnlyRepository _repository;
    public GenerateExpensesReportUseCase(IExpensesReadOnlyRepository repository)
    {
        _repository = repository;
    }
    public async Task<byte[]> Execute(DateTime month)
    {
        var monthInDateOnly = new DateOnly(year: month.Year, month: month.Month, day: month.Day);

        List<Expense> expenses = await _repository.FilterByMonth(monthInDateOnly);

        if (expenses.Count == 0)
        {
            return [];
        }

        using XLWorkbook workbook = new(); // o using chama a função dispose para liberar recursos do programa

        workbook.Author = "Daniel Vieira";
        workbook.Style.Font.FontSize = 12;
        workbook.Style.Font.FontName = "Roboto";

        string fileName = month.ToString("Y");

        var worksheet = workbook.Worksheets.Add(fileName);

        InsertHeader(worksheet);

        var raw = 2;
        foreach (var expense in expenses)
        {
            worksheet.Cell($"A{raw}").Value = expense.Title;
            worksheet.Cell($"B{raw}").Value = expense.Date;
            worksheet.Cell($"C{raw}").Value = ConvertPaymentType(expense.PaymentType);
            worksheet.Cell($"D{raw}").Value = expense.Amount;
            worksheet.Cell($"D{raw}").Style.NumberFormat.Format = $"- {CURRENCY_SYMBOL} #,##0.00"; // formando isso como uma moeda com duas casas decimais
            worksheet.Cell($"E{raw}").Value = expense.Description;

            raw++;
        }

        worksheet.Columns().AdjustToContents(); // Ajusta o tamanho das colunas de acordo o conteúdo

        // Stream é uma camada entre um arquivo e uma aplicação

        var file = new MemoryStream();

        workbook.SaveAs(file);

        return file.ToArray();
    }

    private string ConvertPaymentType(PaymentType payment)
    {
        return payment switch
        {
            PaymentType.Cash => "Dinheiro",
            PaymentType.CreditCard => "Cartão de Crédito",
            PaymentType.DebitCard => "Cartão de Débito",
            PaymentType.EletronicTransfer => "Transferência Bancária",
            _ => throw new NotImplementedException()
        };
    }

    private void InsertHeader(IXLWorksheet worksheet)
    {
        worksheet.Cell("A1").Value = ResourceReportGenerationMessages.TITLE;
        worksheet.Cell("B1").Value = ResourceReportGenerationMessages.DATE;
        worksheet.Cell("C1").Value = ResourceReportGenerationMessages.PAYMENT_TYPE;
        worksheet.Cell("D1").Value = ResourceReportGenerationMessages.AMOUNT;
        worksheet.Cell("E1").Value = ResourceReportGenerationMessages.DESCRIPTION;

        worksheet.Cells("A1:E1").Style.Font.Bold = true;

        worksheet.Cells("A1:E1").Style.Fill.BackgroundColor = XLColor.FromHtml("#F5C2B6");

        worksheet.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("B1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("C1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("D1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
    }
}
