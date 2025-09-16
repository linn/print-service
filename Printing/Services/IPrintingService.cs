namespace Linn.PrintService.Printing.Services
{
    using System.Threading.Tasks;

    using Linn.PrintService.Resources.RequestResources;

    public interface IPrintingService
    {
        Task<PrintResult> Print(PrintJobRequestResource request);
    }
}