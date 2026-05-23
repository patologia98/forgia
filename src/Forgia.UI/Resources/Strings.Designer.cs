using System.Resources;

namespace Forgia.UI.Resources;

internal static class Strings
{
    private static readonly ResourceManager Rm =
        new("Forgia.UI.Resources.Strings", typeof(Strings).Assembly);

    public static string AppTitle => Rm.GetString(nameof(AppTitle))!;

    public static string SetupTitle => Rm.GetString(nameof(SetupTitle))!;
    public static string SetupSubtitle => Rm.GetString(nameof(SetupSubtitle))!;
    public static string SetupSectionPrinter => Rm.GetString(nameof(SetupSectionPrinter))!;
    public static string SetupSectionSpool => Rm.GetString(nameof(SetupSectionSpool))!;
    public static string SetupLabelName => Rm.GetString(nameof(SetupLabelName))!;
    public static string SetupLabelPowerW => Rm.GetString(nameof(SetupLabelPowerW))!;
    public static string SetupLabelPurchaseCost => Rm.GetString(nameof(SetupLabelPurchaseCost))!;
    public static string SetupLabelUsefulLifeH => Rm.GetString(nameof(SetupLabelUsefulLifeH))!;
    public static string SetupLabelMaintenancePerH => Rm.GetString(nameof(SetupLabelMaintenancePerH))!;
    public static string SetupLabelMaterial => Rm.GetString(nameof(SetupLabelMaterial))!;
    public static string SetupLabelCostPerKg => Rm.GetString(nameof(SetupLabelCostPerKg))!;
    public static string SetupLabelCurrentStock => Rm.GetString(nameof(SetupLabelCurrentStock))!;
    public static string SetupButtonSave => Rm.GetString(nameof(SetupButtonSave))!;

    public static string QuoteTitle => Rm.GetString(nameof(QuoteTitle))!;
    public static string QuoteSectionFile => Rm.GetString(nameof(QuoteSectionFile))!;
    public static string QuoteBrowseButton => Rm.GetString(nameof(QuoteBrowseButton))!;
    public static string QuoteDropHint => Rm.GetString(nameof(QuoteDropHint))!;
    public static string QuoteSectionSettings => Rm.GetString(nameof(QuoteSectionSettings))!;
    public static string QuoteLabelPrinter => Rm.GetString(nameof(QuoteLabelPrinter))!;
    public static string QuoteLabelSpool => Rm.GetString(nameof(QuoteLabelSpool))!;
    public static string QuoteLabelElectricity => Rm.GetString(nameof(QuoteLabelElectricity))!;
    public static string QuoteLabelWaste => Rm.GetString(nameof(QuoteLabelWaste))!;
    public static string QuoteSectionQuote => Rm.GetString(nameof(QuoteSectionQuote))!;
    public static string QuoteLabelMargin => Rm.GetString(nameof(QuoteLabelMargin))!;
    public static string QuoteLabelVat => Rm.GetString(nameof(QuoteLabelVat))!;
    public static string QuoteLabelCustomer => Rm.GetString(nameof(QuoteLabelCustomer))!;
    public static string QuoteSectionBreakdown => Rm.GetString(nameof(QuoteSectionBreakdown))!;
    public static string QuoteLabelMaterial => Rm.GetString(nameof(QuoteLabelMaterial))!;
    public static string QuoteLabelElectricityCost => Rm.GetString(nameof(QuoteLabelElectricityCost))!;
    public static string QuoteLabelAmortization => Rm.GetString(nameof(QuoteLabelAmortization))!;
    public static string QuoteLabelMaintenance => Rm.GetString(nameof(QuoteLabelMaintenance))!;
    public static string QuoteLabelWasteOverhead => Rm.GetString(nameof(QuoteLabelWasteOverhead))!;
    public static string QuoteLabelPlateCost => Rm.GetString(nameof(QuoteLabelPlateCost))!;
    public static string QuoteLabelLabor => Rm.GetString(nameof(QuoteLabelLabor))!;
    public static string QuoteLabelDirect => Rm.GetString(nameof(QuoteLabelDirect))!;
    public static string QuoteLabelMarginAmount => Rm.GetString(nameof(QuoteLabelMarginAmount))!;
    public static string QuoteLabelVatAmount => Rm.GetString(nameof(QuoteLabelVatAmount))!;
    public static string QuoteLabelTotal => Rm.GetString(nameof(QuoteLabelTotal))!;
    public static string QuoteExportButton => Rm.GetString(nameof(QuoteExportButton))!;
    public static string QuoteNewQuoteButton => Rm.GetString(nameof(QuoteNewQuoteButton))!;

    public static string ErrorParseFile => Rm.GetString(nameof(ErrorParseFile))!;
    public static string ErrorExportFailed => Rm.GetString(nameof(ErrorExportFailed))!;
}
