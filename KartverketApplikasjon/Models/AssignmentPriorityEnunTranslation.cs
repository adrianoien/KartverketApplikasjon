using KartverketApplikasjon.Models;
// Enum with the sole purpose of translating the other enums
public static class EnumExtensions
{
    public static string ToNorwegian(this CorrectionStatus status)
    {
        return status switch
        {
            CorrectionStatus.Pending => "Venter",
            CorrectionStatus.Approved => "Godkjent",
            CorrectionStatus.Rejected => "Avvist",
            _ => status.ToString()
        };
    }
}