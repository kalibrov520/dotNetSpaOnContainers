using System;
using Ordering.Domain.Common;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    //TODO: custom exceptions
    public class OrderItem : Entity
    {
        private readonly string _productName;
        private readonly string _pictureUrl;
        private readonly decimal _unitPrice;
        private decimal _discount;
        private int _units;

        public int ProductId { get; }

        protected OrderItem()
        {
        }

        public OrderItem(int productId, string productName, decimal unitPrice, decimal discount, string PictureUrl,
            int units = 1)
        {
            if (units <= 0) throw new Exception("");

            if (unitPrice * units < discount)
                throw new Exception("");

            ProductId = productId;

            _productName = productName;
            _unitPrice = unitPrice;
            _discount = discount;
            _units = units;
            _pictureUrl = PictureUrl;
        }

        public string GetPictureUri()
        {
            return _pictureUrl;
        }

        public decimal GetCurrentDiscount()
        {
            return _discount;
        }

        public int GetUnits()
        {
            return _units;
        }

        public decimal GetUnitPrice()
        {
            return _unitPrice;
        }

        public string GetOrderItemProductName()
        {
            return _productName;
        }

        public void SetNewDiscount(decimal discount)
        {
            if (discount < 0) throw new Exception("");

            _discount = discount;
        }

        public void AddUnits(int units)
        {
            if (units < 0) throw new Exception("");

            _units += units;
        }
    }
}