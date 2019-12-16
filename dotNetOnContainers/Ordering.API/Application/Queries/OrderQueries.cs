using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Ordering.API.Application.Queries
{
    public class OrderQueries : IOrderQueries
    {
        private string _connectionString;

        public OrderQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }
        
        public async Task<Order> GetOrderAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                var result = await connection.QueryAsync<dynamic>(
                    @"select o.[Id] as OrderNumber,o.OrderDate as Date, o.Description as Description,
                        o.Address_City as City, o.Address_Country as Country, o.Address_State as State, o.Address_Street as Street, o.Address_ZipCode as ZipCode,
                        os.Name as Status, 
                        oi.ProductName as ProductName, oi.Units as units, oi.UnitPrice as UnitPrice, oi.PictureUrl as PictureUrl
                        FROM ordering.Orders o
                        LEFT JOIN ordering.OrderItems oi ON o.Id = oi.OrderId 
                        LEFT JOIN ordering.OrderStatus os on o.OrderStatusId = os.Id
                        WHERE o.Id=@id"
                    , new { id }
                );

                if (result.AsList().Count == 0)
                    throw new KeyNotFoundException();

                return MapOrderItems(result);
            }
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(Guid userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<OrderSummary>(@"SELECT o.[Id] as OrderNumber,o.[OrderDate] as [Date],os.[Name] as [Status], SUM(oi.Units*oi.UnitPrice) as Total
                     FROM [ordering].[Orders] o
                     LEFT JOIN[ordering].[OrderItems] oi ON  o.Id = oi.OrderId 
                     LEFT JOIN[ordering].[OrderStatus] os on o.OrderStatusId = os.Id                     
                     LEFT JOIN[ordering].[Buyers] ob on o.BuyerId = ob.Id
                     WHERE ob.IdentityGuid = @userId
                     GROUP BY o.[Id], o.[OrderDate], os.[Name] 
                     ORDER BY o.[Id]", new { userId });
            }
        }

        public async Task<IEnumerable<CardType>> GetCardTypesAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<CardType>("SELECT * FROM ordering.CardTypes");
            }
        }

        private Order MapOrderItems(dynamic result)
        {
            var order = new Order
            {
                OrderNumber = result[0].ordernumber,
                Date = result[0].date,
                Status = result[0].status,
                Description = result[0].description,
                Street = result[0].street,
                City = result[0].city,
                ZipCode = result[0].zipcode,
                Country = result[0].country,
                OrderItems = new List<OrderItem>(),
                Total = 0
            };

            foreach (dynamic item in result)
            {
                var orderItem = new OrderItem
                {
                    ProductName = item.productname,
                    Units = item.units,
                    UnitPrice = (double)item.unitprice,
                    PictureUrl = item.pictureurl
                };

                order.Total += item.units * item.unitprice;
                order.OrderItems.Add(orderItem);
            }

            return order;
        }
    }
}