using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using MyStore.Business.Data.Entities;
using MyStore.Business.Orders;

namespace MyStore.Api.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderConfirmation _orderConfirmation;
        private readonly IOrderProcessor _orderProcessor;
        private readonly IPrintingService _printingService;

        public OrdersController(IOrderConfirmation orderConfirmation, 
            IOrderProcessor orderProcessor,
            IPrintingService printingService)
        {
            _orderConfirmation = orderConfirmation ?? throw new ArgumentNullException(nameof(orderConfirmation));
            _orderProcessor = orderProcessor ?? throw new ArgumentNullException(nameof(orderProcessor));
            _printingService = printingService ?? throw new ArgumentNullException(nameof(printingService));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order model)
        {
            var order = await _orderProcessor.Process(model);

            BackgroundJob.Enqueue(() => _printingService.Print(order));
            BackgroundJob.Enqueue(() => _orderConfirmation.Confirm(order));
            return Created(Request.Path + $"/{order.Id}", order);
        }
    }
}