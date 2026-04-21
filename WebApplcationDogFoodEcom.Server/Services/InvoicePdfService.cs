using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WebApplcationDogFoodEcom.Server.Models;

namespace WebApplcationDogFoodEcom.Server.Services;

public static class InvoicePdfService
{
    // Color palette matching the billing statement template
    private static readonly string HeaderBg = "#2B4C7E";
    private static readonly string LightBlueBg = "#C5D3E8";
    private static readonly string RowAltBg = "#DCE6F1";
    private static readonly string BorderColor = "#7B9CC2";
    private static readonly string White = "#FFFFFF";
    private static readonly string DarkText = "#1A2B42";

    public static byte[] Generate(Order order)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(DarkText));

                page.Header().Element(c => ComposeHeader(c, order));
                page.Content().Element(c => ComposeContent(c, order));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, Order order)
    {
        container.Column(col =>
        {
            // ── PawMeds branding row ──
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("PawMeds").FontSize(22).Bold().FontColor(HeaderBg);
                    c.Item().Text("Dog Medicine & Vaccine Store").FontSize(9).FontColor(Colors.Grey.Medium);
                });
                row.ConstantItem(150).AlignRight().AlignMiddle()
                    .Text("support@pawmeds.com").FontSize(8).FontColor(Colors.Grey.Medium);
            });

            col.Item().PaddingVertical(8);

            // ── Billing Statement title bar ──
            col.Item().Background(HeaderBg).Padding(10).AlignCenter()
                .Text("Billing Statement").FontSize(18).Bold().FontColor(White);

            // ── Info grid: Bill To / Statement details ──
            col.Item().Border(1).BorderColor(BorderColor).Column(info =>
            {
                // Row 1
                info.Item().Row(row =>
                {
                    Cell(row.RelativeItem(1), "Bill To:", true, LightBlueBg);
                    Cell(row.RelativeItem(2), order.CustomerName);
                    Cell(row.RelativeItem(1), "Statement Date:", true, LightBlueBg);
                    Cell(row.RelativeItem(1), order.OrderDate.ToString("dd-MM-yyyy"));
                });
                // Row 2
                info.Item().Row(row =>
                {
                    Cell(row.RelativeItem(1), "", true, LightBlueBg);
                    Cell(row.RelativeItem(2), order.ShippingAddress);
                    Cell(row.RelativeItem(1), "Statement #:", true, LightBlueBg);
                    Cell(row.RelativeItem(1), $"{order.Id}");
                });
                // Row 3
                info.Item().Row(row =>
                {
                    Cell(row.RelativeItem(1), "", true, LightBlueBg);
                    Cell(row.RelativeItem(2), order.CustomerEmail);
                    Cell(row.RelativeItem(1), "Customer Code:", true, LightBlueBg);
                    Cell(row.RelativeItem(1), $"PM{order.Id:D4}");
                });
            });

            col.Item().PaddingVertical(6);

            // ── Remittance Details / Account Summary ──
            col.Item().Border(1).BorderColor(BorderColor).Column(remit =>
            {
                remit.Item().Row(row =>
                {
                    row.RelativeItem(1).Background(HeaderBg).Padding(6).AlignCenter()
                        .Text("Remittance Details").FontSize(9).Bold().FontColor(White);
                    row.RelativeItem(1).Background(HeaderBg).Padding(6).AlignCenter()
                        .Text("Account Summary").FontSize(9).Bold().FontColor(White);
                });

                var subtotal = order.TotalAmount;
                var tax = subtotal * 0.08m;
                var grandTotal = subtotal + tax;

                remit.Item().Row(row =>
                {
                    row.RelativeItem(1).Border(1).BorderColor(BorderColor).Padding(5)
                        .Text("To ensure proper credit, please reference your Customer ID with payment.").FontSize(8).Italic();
                    row.RelativeItem(1).Column(summCol =>
                    {
                        summCol.Item().Row(r =>
                        {
                            Cell(r.RelativeItem(1), "Balance Due", true, LightBlueBg);
                            Cell(r.RelativeItem(1), $"$ {grandTotal:F2}");
                        });
                        summCol.Item().Row(r =>
                        {
                            Cell(r.RelativeItem(1), "Payment Due Date", true, LightBlueBg);
                            Cell(r.RelativeItem(1), order.OrderDate.AddDays(30).ToString("dd-MM-yyyy"));
                        });
                    });
                });

                remit.Item().Row(row =>
                {
                    Cell(row.RelativeItem(1), "Payment Method:", true, LightBlueBg);
                    Cell(row.RelativeItem(2), "All checks payable to PawMeds Store");
                });
            });

            col.Item().PaddingVertical(4);

            // ── Note ──
            col.Item().Background(LightBlueBg).Border(1).BorderColor(BorderColor).Padding(5).AlignCenter()
                .Text("Note: Please write your Customer ID behind on your check.").FontSize(8).Italic();

            col.Item().PaddingVertical(6);
        });
    }

    private static void ComposeContent(IContainer container, Order order)
    {
        container.Column(col =>
        {
            // ── Section title ──
            col.Item().Background(HeaderBg).Padding(6)
                .Text("Account Activity").FontSize(10).Bold().FontColor(White);

            // ── Table header ──
            col.Item().Background(HeaderBg).Border(1).BorderColor(BorderColor).Row(row =>
            {
                HeaderCell(row.ConstantItem(55), "DATE");
                HeaderCell(row.ConstantItem(75), "INVOICE #");
                HeaderCell(row.RelativeItem(3), "DESCRIPTION");
                HeaderCell(row.RelativeItem(1), "PAYMENT");
                HeaderCell(row.RelativeItem(1), "INVOICE\nAMOUNT");
                HeaderCell(row.RelativeItem(1), "BALANCE");
            });

            // ── Balance Forward row ──
            decimal runningBalance = 0;
            col.Item().Background(LightBlueBg).Border(1).BorderColor(BorderColor).Row(row =>
            {
                DataCell(row.ConstantItem(55), "");
                DataCell(row.ConstantItem(75), "");
                DataCell(row.RelativeItem(3), "Balance Forward");
                DataCell(row.RelativeItem(1), "");
                DataCell(row.RelativeItem(1), "");
                DataCell(row.RelativeItem(1), $"$ {runningBalance:F2}");
            });

            // ── Item rows ──
            for (int i = 0; i < order.Items.Count; i++)
            {
                var item = order.Items[i];
                var bg = i % 2 == 0 ? White : RowAltBg;
                runningBalance += item.Total;
                var invoiceNum = $"INV{order.Id:D3}{(i + 1):D2}";

                col.Item().Background(bg).Border(1).BorderColor(BorderColor).Row(row =>
                {
                    DataCell(row.ConstantItem(55), order.OrderDate.AddDays(i).ToString("dd-MM-yy"));
                    DataCell(row.ConstantItem(75), invoiceNum);
                    DataCell(row.RelativeItem(3), $"{item.ProductName} (x{item.Quantity})");
                    DataCell(row.RelativeItem(1), "");
                    DataCell(row.RelativeItem(1), $"$ {item.Total:F2}");
                    DataCell(row.RelativeItem(1), $"$ {runningBalance:F2}");
                });
            }

            // ── Tax row ──
            var subtotal = order.TotalAmount;
            var tax = subtotal * 0.08m;
            runningBalance += tax;

            col.Item().Background(RowAltBg).Border(1).BorderColor(BorderColor).Row(row =>
            {
                DataCell(row.ConstantItem(55), "");
                DataCell(row.ConstantItem(75), "");
                DataCell(row.RelativeItem(3), "Tax (8%)");
                DataCell(row.RelativeItem(1), "");
                DataCell(row.RelativeItem(1), $"$ {tax:F2}");
                DataCell(row.RelativeItem(1), $"$ {runningBalance:F2}");
            });

            // ── Grand Total row ──
            col.Item().Background(HeaderBg).Border(1).BorderColor(BorderColor).Row(row =>
            {
                row.ConstantItem(55).Padding(6).Text("").FontColor(White);
                row.ConstantItem(75).Padding(6).Text("").FontColor(White);
                row.RelativeItem(3).Padding(6).Text("GRAND TOTAL").Bold().FontSize(10).FontColor(White);
                row.RelativeItem(1).Padding(6).Text("").FontColor(White);
                row.RelativeItem(1).Padding(6).Text("").FontColor(White);
                row.RelativeItem(1).Padding(6).AlignRight().Text($"$ {runningBalance:F2}").Bold().FontSize(10).FontColor(White);
            });

            // ── Thank you note ──
            col.Item().PaddingTop(20).Background(LightBlueBg).Border(1).BorderColor(BorderColor).Padding(14).Column(note =>
            {
                note.Item().Text("Thank you for choosing PawMeds!").Bold().FontSize(11).FontColor(HeaderBg);
                note.Item().PaddingTop(4).Text("Keep your furry friend healthy and happy. If you have any questions about your order, please contact support@pawmeds.com.")
                    .FontSize(9).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(1).LineColor(BorderColor);
            col.Item().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Text("PawMeds © 2025 — Dog Medicine & Vaccine Store").FontSize(8).FontColor(Colors.Grey.Medium);
                row.RelativeItem().AlignRight().Text(t =>
                {
                    t.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                    t.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                    t.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                    t.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        });
    }

    // ── Helper: Info grid cell ──
    private static void Cell(IContainer container, string text, bool bold = false, string? bg = null)
    {
        var c = container.Border(1).BorderColor(BorderColor);
        if (bg is not null) c = c.Background(bg);
        var t = c.Padding(5).Text(text).FontSize(8);
        if (bold) t.Bold();
    }

    // ── Helper: Table header cell ──
    private static void HeaderCell(IContainer container, string text)
    {
        container.Border(1).BorderColor(BorderColor).Padding(6).AlignCenter()
            .Text(text).FontSize(8).Bold().FontColor(White);
    }

    // ── Helper: Table data cell ──
    private static void DataCell(IContainer container, string text)
    {
        container.Border(1).BorderColor(BorderColor).Padding(5).AlignRight()
            .Text(text).FontSize(8);
    }
}
