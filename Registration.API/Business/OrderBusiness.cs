using System.Net;
using AutoMapper;
using Registration.API.Common;

namespace Registration.API.Business
{
    public class OrderBusiness(RegContext context, IMapper mapper) : GenericBusiness<Order>(context, mapper)
    {
        public async Task<ActionReport> UpdateRequest(int orderId, string? authority, string? requestReportContent, int? code)
        {
            try
            {
                var order = await GetById(orderId, true);
                if (order is null)
                    return ActionReport.Error(HttpStatusCode.NotFound);
                order.RequestContent = requestReportContent;
                order.RequestStatus = code;
                order.Authority = authority;
                order.RequestDate = DateTime.Now;
                return await SaveChanges();
            }
            catch (Exception e)
            {
                return ActionReport.Error(HttpStatusCode.InternalServerError, e.Message, e);
            }
        }

        public Task<Order?> GetByAuthority(string authority)
            => FirstOrDefaultAsync(e => e.Authority == authority, true);
    }
}
