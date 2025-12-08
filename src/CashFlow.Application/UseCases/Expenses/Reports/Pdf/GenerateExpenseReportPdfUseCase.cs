
using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Colors;
using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Fonts;
using CashFlow.Domain;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using System.Reflection;

namespace CashFlow.Application.UseCases.Expenses.Reports.Pdf;

public class GenerateExpenseReportPdfUseCase : IGenerateExpenseReportPdfUseCase
{
    private const string CURRENCY_SYMBOL = "R$";
    private const int HEIGHT_ROW_EXPENSE_TABLE = 25;
    private readonly IExpensesReadOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;
    public GenerateExpenseReportPdfUseCase(IExpensesReadOnlyRepository repository, ILoggedUser loggedUser)
    {
        _repository = repository;
        _loggedUser = loggedUser;

        // Qual resolver utilizar para recuperar as fontes

        GlobalFontSettings.FontResolver = new ExpensesReportFontResolver();
    }
    public async Task<byte[]> Execute(DateOnly month)
    {
        var loggedUser = await _loggedUser.Get();

        var expenses = await _repository.FilterByMonth(loggedUser, month);
        if (expenses.Count == 0)
        {
            return [];
        }

        var document = CreateDocument(loggedUser.Name, month);
        var page = CreatePage(document);

        CreateHeaderWithProfilePhotoAndName(loggedUser.Name, page);

        var totalExpenses = expenses.Sum(expense => expense.Amount);

        CreateTotalSpentSection(page, month, totalExpenses);

        foreach (var expense in expenses)
        {
            var table = CreateExpenseTable(page);

            var row = table.AddRow();
            row.Height = HEIGHT_ROW_EXPENSE_TABLE;

            AddExpenseTitle(cell: row.Cells[0], expenseTitle: expense.Title);

            // Coluna de Index 3 pois fizemos o Merge
            // Mesmo com o Merge das colunas, o index continua sendo o mesmo
            AddHeaderForAmount(row.Cells[3]);

            row = table.AddRow();
            row.Height = HEIGHT_ROW_EXPENSE_TABLE;

            row.Cells[0].AddParagraph(expense.Date.ToString("D"));
            SetStyleBaseForExpenseInformation(row.Cells[0]);
            row.Cells[0].Format.LeftIndent = 20;

            row.Cells[1].AddParagraph(expense.Date.ToString("t"));
            SetStyleBaseForExpenseInformation(row.Cells[1]);

            row.Cells[2].AddParagraph(expense.PaymentType.GetEnumDescription());
            SetStyleBaseForExpenseInformation(row.Cells[2]);

            AddAmountForExpense(cell: row.Cells[3], expenseAmount: expense.Amount);

            if (string.IsNullOrWhiteSpace(expense.Description) == false)
            {

                var descriptionRow = table.AddRow();
                descriptionRow.Height = HEIGHT_ROW_EXPENSE_TABLE;

                descriptionRow.Cells[0].AddParagraph(expense.Description);
                descriptionRow.Cells[0].Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 10, Color = ColorsHelper.BLACK };

                // Adiciona cor de fundo para a linha
                descriptionRow.Cells[0].Shading.Color = ColorsHelper.GREEN_LIGHT;
                descriptionRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;

                //Mescla com as duas colunas do lado 
                descriptionRow.Cells[0].MergeRight = 2;

                // Coloca 20px de identação a esquerda
                descriptionRow.Cells[0].Format.LeftIndent = 20;

                /* 
                 * Somente faço o merge com a linha de baixo quando houver 
                 * descrição da despesa para que o valor 
                 * fique alinhado ao meio verticalmente 
                */
                row.Cells[3].MergeDown = 1;
            }

            // Para dar espaçamento entre as tabelas
            AddWhiteSpace(table);
        }

        return RenderDocument(document);
    }

    private Document CreateDocument(string author, DateOnly month)
    {
        var document = new Document();
        document.Info.Title = $"{ResourceReportGenerationMessages.EXPENSES_FOR} {month:Y}";
        document.Info.Author = author;

        var style = document.Styles["Normal"];

        style!.Font.Name = FontHelper.RALEWAY_REGULAR;

        return document;
    }

    private Section CreatePage(Document document)
    {
        var section = document.AddSection();
        section.PageSetup = document.DefaultPageSetup.Clone();

        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.LeftMargin = 40;
        section.PageSetup.RightMargin = 40;
        section.PageSetup.TopMargin = 80;
        section.PageSetup.BottomMargin = 80;


        return section;
    }

    private byte[] RenderDocument(Document document)
    {
        var renderer = new PdfDocumentRenderer { Document = document };

        // Renderiza o documento e carrega todas as configurações e conteúdo
        renderer.RenderDocument();

        // Salva o arquivo na memória da máquina
        using var file = new MemoryStream();
        renderer.PdfDocument.Save(file);

        // Devolve o arquivo em bytes
        return file.ToArray();
    }

    private void CreateHeaderWithProfilePhotoAndName(string name, Section page)
    {
        var table = page.AddTable();

        // Primeiro devemos adicionar as colunas que precisamos
        table.AddColumn();
        table.AddColumn("300");

        // Depois adicionamos a linha
        // Salvamos em uma variável pois queremos manipular a linha
        var row = table.AddRow();

        var assembly = Assembly.GetExecutingAssembly();
        var directoryName = Path.GetDirectoryName(assembly.Location);

        row.Cells[0].AddImage(Path.Combine(directoryName!, "Logo", "GOJO.png"));

        row.Cells[1].AddParagraph($"Olá,{name}");
        row.Cells[1].Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 16 };
        row.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
    }

    private Table CreateExpenseTable(Section page)
    {
        var table = page.AddTable();

        table.AddColumn("195").Format.Alignment = ParagraphAlignment.Left;
        table.AddColumn("80").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("120").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("120").Format.Alignment = ParagraphAlignment.Right;

        return table;
    }

    private void CreateTotalSpentSection(Section page, DateOnly month, decimal totalExpenses)
    {
        var paragraph = page.AddParagraph();

        // Passamos como string para ser em pixel
        paragraph.Format.SpaceBefore = "40";
        paragraph.Format.SpaceAfter = "40";

        var title = string.Format(ResourceReportGenerationMessages.TOTAL_SPENT_IN, month.ToString("Y"));
        paragraph.AddFormattedText(title, new Font { Name = FontHelper.RALEWAY_REGULAR, Size = 15 });

        paragraph.AddLineBreak();

        paragraph.AddFormattedText($"{totalExpenses} {CURRENCY_SYMBOL}",
            new Font { Name = FontHelper.WORKSANS_BLACK, Size = 50 });
    }

    private void AddExpenseTitle(Cell cell, string expenseTitle)
    {
        cell.AddParagraph(expenseTitle);
        cell.Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 14, Color = ColorsHelper.BLACK };

        // Adiciona cor de fundo para a linha
        cell.Shading.Color = ColorsHelper.RED_LIGHT;
        cell.VerticalAlignment = VerticalAlignment.Center;

        //Mescla com as duas colunas do lado 
        cell.MergeRight = 2;

        // Coloca 20px de identação a esquerda
        cell.Format.LeftIndent = 20;
    }

    private void AddHeaderForAmount(Cell cell)
    {
        cell.AddParagraph(ResourceReportGenerationMessages.AMOUNT);
        cell.Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 14, Color = ColorsHelper.WHITE };
        cell.Shading.Color = ColorsHelper.RED_DARK;
        cell.VerticalAlignment = VerticalAlignment.Center;
    }

    private void SetStyleBaseForExpenseInformation(Cell cell)
    {
        cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 12, Color = ColorsHelper.BLACK };
        cell.Shading.Color = ColorsHelper.GREEN_DARK;
        cell.VerticalAlignment = VerticalAlignment.Center;
    }

    private void AddAmountForExpense(Cell cell, decimal expenseAmount)
    {
        cell.AddParagraph($"-{expenseAmount} {CURRENCY_SYMBOL}");
        cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 14, Color = ColorsHelper.BLACK };
        cell.Shading.Color = ColorsHelper.WHITE;
        cell.VerticalAlignment = VerticalAlignment.Center;
    }

    private void AddWhiteSpace(Table table)
    {
        var row = table.AddRow();
        row.Height = 30;
        row.Borders.Visible = false;
    }
}
