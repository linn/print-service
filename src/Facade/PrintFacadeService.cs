namespace Linn.PrintService.Facade
{
    using System.Threading.Tasks;

    using Linn.Common.Facade;
    using Linn.PrintService.Printing;
    using Linn.PrintService.Resources;

    public class PrintFacadeService : IPrintFacadeService
    {
        private readonly IIppPrintingService printingService;

        public PrintFacadeService(IIppPrintingService printingService)
        {
            this.printingService = printingService;
        }

        public async Task<IResult<PrintResultResource>> PrintAsync(string printerUri, string jobName, byte[] data)
        {
            try
            {
                var result = await this.printingService.Print(printerUri, jobName, data);
                return new SuccessResult<PrintResultResource>(new PrintResultResource
                {
                    Success = result.Success,
                    HttpStatus = result.HttpStatus,
                    ResponsePreview = result.ResponsePreview,
                    State = result.State
                });
            }
            catch (IppPrintingException e)
            {
                return new BadRequestResult<PrintResultResource>(e.Message);
            }
        }

        public async Task<IResult<PrintResultResource>> GetDetailedStatusAsync(string printerUri)
        {
            try
            {
                var result = await this.printingService.GetDetailedStatus(printerUri);
                return new SuccessResult<PrintResultResource>(new PrintResultResource
                {
                    Success = result.Success,
                    HttpStatus = result.HttpStatus,
                    ResponsePreview = result.ResponsePreview,
                    State = result.State
                });
            }
            catch (IppPrintingException e)
            {
                return new BadRequestResult<PrintResultResource>(e.Message);
            }
        }
    }
}
