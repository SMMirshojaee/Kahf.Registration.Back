using System.Net;
using AutoMapper;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

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

        public async Task<ActionReport<OrderDto>> InsertInstallment(int userId, InstallmentDto newInstallment)
        {
            Order? newOrder = Mapper.Map<Order>(newInstallment);
            newOrder.UserId = userId;
            newOrder.VerifyDate = newOrder.RequestDate = newInstallment.Date;
            newOrder.RequestStatus = newOrder.VerifyStatus = 100;
            ActionReport report = await Add(newOrder);
            if (report.Successful)
            {
                return ActionReport<OrderDto>.Success(Mapper.Map<OrderDto>(newOrder));
            }

            return ActionReport<OrderDto>.Error(report);
        }
    }
}
