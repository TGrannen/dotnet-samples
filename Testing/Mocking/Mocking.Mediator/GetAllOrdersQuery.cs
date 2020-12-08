using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mocking.Mediator
{
    public class GetAllOrdersQuery : IRequest<List<OrderResponse>>
    {
    }

    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderResponse>>
    {
        private readonly ILogger<GetAllOrdersQueryHandler> _logger;

        public GetAllOrdersQueryHandler(ILogger<GetAllOrdersQueryHandler> logger)
        {
            _logger = logger;
        }

        public Task<List<OrderResponse>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var rng = new Random();
            var list = Enumerable.Range(1, 5).Select(index => new OrderResponse
            {
                Number = rng.Next(-20, 55)
            }).ToList();

            _logger.LogInformation("Got list of {Number} Entries", list.Count);

            return Task.FromResult(list);
        }
    }
}