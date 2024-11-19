using Microsoft.Extensions.Logging;
using WFO.Order.ApplicationService.CartModule.Abstracts;
using WFO.Order.ApplicationService.Common;
using WFO.Order.ApplicationService.OrderManagementModule.Abstracts;
using WFO.Order.Domain;
using WFO.Order.Dtos.OrderManagementModule;
using WFO.Order.Dtos.OrderManagementModule.Report;
using WFO.Order.Infrastructure;
using WFO.Shared.ApplicationService.Product;
using WFO.Shared.Dtos.Common;

namespace WFO.Order.ApplicationService.OrderManagementModule.Implements
{
    public class OrderService : OrderServiceBase, IOrderService
    {
        private readonly ICartService _cartService;
        private readonly IProductInforService _productInforService;

        public OrderService(
            ILogger<OrderService> logger,
            OrderDbContext dbContext,
            ICartService cartService,
            IProductInforService productInforService
        )
            : base(logger, dbContext)
        {
            _cartService = cartService;
            _productInforService = productInforService;
        }

        public List<OrdOrder> GetAll()
        {
            return _dbContext
                .Orders.Select(o => new OrdOrder
                {
                    Id = o.Id,
                    CanceledDate = o.CanceledDate,
                    CreatedDate = o.CreatedDate,
                    CustomerId = o.CustomerId,
                    ShippedDate = o.ShippedDate,
                    Status = o.Status,
                    UpdatedDate = o.UpdatedDate,
                })
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }

        public PageResultDto<ListOrdersOfCustomerDto> MyOrder(int CustomerId)
        {
            var result = new PageResultDto<ListOrdersOfCustomerDto>();

            var query =
                from o in _dbContext.Orders
                join od in _dbContext.OrderDetails on o.Id equals od.OrderId into odDetails
                from od in odDetails.DefaultIfEmpty()
                where o.CustomerId == CustomerId
                group new { o, od } by new
                {
                    o.Id,
                    o.ShippedDate,
                    o.CanceledDate,
                    o.CustomerId,
                    o.Status,
                } into g
                select new ListOrdersOfCustomerDto
                {
                    Id = g.Key.Id,
                    CompletedAt = g.Key.ShippedDate,
                    CanceledAt = g.Key.CanceledDate,
                    TotalPrice = g.Sum(x => x.od != null ? x.od.UnitPrice * x.od.Quantity : 0),
                    ProductCount = g.Count(x => x.od != null),
                    Status = g.Key.Status,
                };

            var totalItems = query.Count();

            result.Items = query;
            result.TotalItem = totalItems;

            return result;
        }

        public OrderDetailDto GetDetailOrder(int OrderId)
        {
            var existOrder = _dbContext.Orders.FirstOrDefault(o => o.Id == OrderId);
            if (existOrder != null)
            {
                var query = _dbContext
                    .Orders.Where(o => o.Id == OrderId)
                    .Join(
                        _dbContext.OrderDetails,
                        ord => ord.Id,
                        od => od.OrderId,
                        (ord, od) =>
                            new OrderDetailItemDto
                            {
                                ProductId = od.ProductId,
                                ProductName = _productInforService.GetProduct(od.ProductId).Name,
                                ProductImage = _productInforService.GetProduct(od.ProductId).Image,
                                ProductSize = _productInforService.GetProduct(od.ProductId).Size,
                                Quantity = od.Quantity,
                                UnitPrice = od.UnitPrice,
                            }
                    )
                    .ToList();

                var result = new OrderDetailDto
                {
                    Id = existOrder.Id,
                    Status = existOrder.Status,
                    Details = query,
                };

                return result;
            }
            else
            {
                throw new Exception("Không tìm thấy đơn hàng");
            }
        }

        public void CreateOrder(CreateOrderDto input, int CustomerId)
        {
            foreach (var CartId in input.CartIds)
            {
                var haveInYourCart = _cartService.HasItemInCart(CartId, CustomerId);
                if (haveInYourCart)
                {
                    continue;
                }
                else
                {
                    throw new Exception(
                        $"Không tồn tại item có Id \"{CartId}\" trong giỏ hàng của bạn."
                    );
                }
            }

            var newOrder = new OrdOrder { CustomerId = CustomerId, CreatedDate = DateTime.Now };

            _dbContext.Orders.Add(newOrder);
            _dbContext.SaveChanges();

            foreach (var CartId in input.CartIds)
            {
                var cartItem = _cartService.GetCartItem(CartId);
                if (cartItem != null)
                {
                    _dbContext.OrderDetails.Add(
                        new OrdOrderDetail
                        {
                            OrderId = newOrder.Id,
                            ProductId = cartItem.ProductId,
                            Note = cartItem.Note,
                            Quantity = cartItem.Quantity,
                            UnitPrice = cartItem.ProductPrice,
                        }
                    );

                    _cartService.RemoveFromCart(CartId, CustomerId);
                    _dbContext.SaveChanges();
                }
            }
        }

        public void ConfirmOrder(int OrderId)
        {
            var existOrder = _dbContext.Orders.FirstOrDefault(o =>
                o.Id == OrderId && o.Status != 10
            );
            if (existOrder != null)
            {
                switch (existOrder.Status)
                {
                    case 0:
                        existOrder.Status = 1;
                        existOrder.UpdatedDate = DateTime.Now;

                        _dbContext.Orders.Update(existOrder);
                        _dbContext.SaveChanges();
                        break;
                    case 1:
                    case 2:
                    case 3:
                        throw new Exception("Đơn hàng đã được xác nhận trước đó");
                    case 5:
                        throw new Exception("Đơn hàng đã hoàn thành");
                    case 10:
                        throw new Exception("Đơn hàng đã bị hủy");
                }
            }
            else
            {
                throw new Exception($"Đơn hàng có Id \"{OrderId}\" không tồn tại hoặc đã bị hủy");
            }
        }

        public void TransferToCarrier(int OrderId)
        {
            var existOrder = _dbContext.Orders.FirstOrDefault(o => o.Id == OrderId);
            if (existOrder != null)
            {
                switch (existOrder.Status)
                {
                    case 0:
                        throw new Exception("Đơn hàng chưa được xác nhận");
                    case 1:
                    case 2:
                        existOrder.Status = 2;
                        existOrder.UpdatedDate = DateTime.Now;

                        _dbContext.Orders.Update(existOrder);
                        _dbContext.SaveChanges();
                        break;
                    case 3:
                        throw new Exception(
                            "Khách hàng đã nhận đơn này nên không thể chuyển sang trạng thái 'Đang giao'"
                        );
                    case 5:
                        throw new Exception("Đơn hàng đã hoàn thành");
                    case 10:
                        throw new Exception("Đơn hàng đã bị hủy");
                }
            }
            else
            {
                throw new Exception($"Đơn hàng có Id \"{OrderId}\" không tồn tại");
            }
        }

        public void CustomerConfirmReceive(int OrderId, int CustomerId)
        {
            var existOrder = _dbContext.Orders.FirstOrDefault(o =>
                o.Id == OrderId && o.CustomerId == CustomerId
            );
            if (existOrder != null)
            {
                switch (existOrder.Status)
                {
                    case 0:
                        throw new Exception("Đơn hàng chưa được xác nhận");
                    case 1:
                        throw new Exception(
                            "Bạn không thể xác nhận nhận đơn trong khi đơn chưa chuyển sang 'Đang giao'"
                        );
                    case 2:
                        existOrder.Status = 3;
                        existOrder.ShippedDate = DateTime.Now;

                        _dbContext.Orders.Update(existOrder);
                        _dbContext.SaveChanges();
                        break;
                    case 3:
                        throw new Exception("Bạn đã xác nhận nhận đơn hàng này rồi");
                    case 5:
                        throw new Exception("Đơn hàng đã hoàn thành");
                    case 10:
                        throw new Exception("Đơn hàng đã bị hủy");
                }
            }
            else
            {
                throw new Exception(
                    $"Đơn hàng có Id \"{OrderId}\" không tồn tại hoặc bạn không có quyền"
                );
            }
        }

        public void SucceededOrder(int OrderId)
        {
            var existOrder = _dbContext.Orders.FirstOrDefault(o => o.Id == OrderId);
            if (existOrder != null)
            {
                switch (existOrder.Status)
                {
                    case 0:
                        throw new Exception("Đơn hàng chưa được xác nhận");
                    case 1:
                    case 2:
                        throw new Exception("Khách hàng chưa xác nhận nhận đơn");
                    case 3:
                        existOrder.Status = 5;

                        _dbContext.Orders.Update(existOrder);
                        _dbContext.SaveChanges();
                        break;
                    case 5:
                        throw new Exception("Đơn hàng đã hoàn thành");
                    case 10:
                        throw new Exception("Đơn hàng đã bị hủy");
                }
            }
            else
            {
                throw new Exception($"Đơn hàng có Id \"{OrderId}\" không tồn tại");
            }
        }

        public void CancelOrder(int OrderId, int CustomerId)
        {
            var havePermisson =
                _dbContext.Orders.Any(o => o.Id == OrderId && o.CustomerId == CustomerId)
                || CustomerId == 3;

            if (!havePermisson)
            {
                throw new Exception("Bạn không có quyền chỉnh sửa đơn hàng này");
            }
            else
            {
                var existOrder = _dbContext.Orders.FirstOrDefault(o =>
                    o.Id == OrderId && o.Status != 10
                );
                if (existOrder != null)
                {
                    switch (existOrder.Status)
                    {
                        case 0:
                        case 1:
                        case 2:
                            existOrder.Status = 10;
                            existOrder.CanceledDate = DateTime.Now;

                            _dbContext.Orders.Update(existOrder);
                            _dbContext.SaveChanges();
                            break;
                        case 3:
                            throw new Exception(
                                "Khách hàng đã xác nhận nhận đơn nên không thể hủy"
                            );
                        case 5:
                            throw new Exception("Đơn hàng đã hoàn thành");
                        case 10:
                            throw new Exception("Đơn hàng đã bị hủy");
                    }
                }
                else
                {
                    throw new Exception(
                        $"Đơn hàng có Id \"{OrderId}\" không tồn tại hoặc đã bị hủy"
                    );
                }
            }
        }

        public List<ResultRangePickerDto> ReportByRange(RangePickerDto input)
        {
            /*
             SQL query

            select A.CompletedAt, sum(A.TotalPrice) as TotalAmount
            from (
                select o.Id, cast(o.ShippedDate as date) as CompletedAt, sum(od.Quantity * od.UnitPrice) as TotalPrice
                from ord.OrdOrder as o join ord.OrdOrderDetail as od on o.Id = od.OrderId
                group by o.Id, o.ShippedDate
            ) as A
            group by A.CompletedAt
            having A.CompletedAt >= '2024-09-21' and A.CompletedAt <= '2024-11-21'
             */


            //var findTonDon = from t in _dbContext.Orders
            //                 join o in _dbContext.OrderDetails on t.Id equals o.OrderId
            //                 where t.ShippedDate != null
            //                 select new
            //                 {
            //                     t.Id,
            //                     t.ShippedDate,
            //                     o.UnitPrice,
            //                     o.Quantity,
            //                 };

            //var group = findTonDon.Where(s => s.ShippedDate.HasValue)
            //            .GroupBy(x => new { x.ShippedDate, s = x.Quantity * x.UnitPrice }).Select(
            //            h => new ResultRangePickerDto
            //            {
            //                Date = h.Key.ShippedDate,
            //                Amount = h.Key.s
            //            }
            //    ).ToList();


            var result = _dbContext
                .Orders.Join(
                    _dbContext.OrderDetails,
                    o => o.Id,
                    od => od.OrderId,
                    (o, od) =>
                        new
                        {
                            o.Id,
                            o.ShippedDate,
                            od.Quantity,
                            od.UnitPrice,
                        }
                )
                .Where(x => x.ShippedDate.HasValue)
                .GroupBy(
                    x => new
                    {
                        x.Id,
                        CompletedAt = DateOnly.FromDateTime(x.ShippedDate.Value),
                        s = x.Quantity * x.UnitPrice,
                    }, // Group by Id and the date part of ShippedDate
                    x => x.Quantity * x.UnitPrice
                ) // Calculate TotalPrice (Quantity * UnitPrice)
                .Select(g => new
                {
                    CompletedAt = g.Key.CompletedAt,
                    TotalPrice = g.Sum(), // Sum the calculated TotalPrice for each group
                })
                .Where(x => x.CompletedAt >= input.startDate && x.CompletedAt <= input.endDate)
                .GroupBy(x => x.CompletedAt)
                .Select(g => new ResultRangePickerDto
                {
                    Date = g.Key,
                    Amount = g.Sum(x => x.TotalPrice), // Sum the TotalPrice to get TotalAmount per CompletedAt
                })
                .ToList();

            return result;
        }
    }
}
