using System;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    public class PaymentMethod
    {
        private string _alias;
        private string _cardNumber;
        private string _securityNumber;
        private string _cardHolderName;
        private DateTime _expiration;

        private int _cardTypeId;
        public CardType CardType { get; private set; }


        protected PaymentMethod() { }

        public PaymentMethod(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
        {
            //TODO: Custom exception
            _cardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new Exception(nameof(cardNumber));
            _securityNumber = !string.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new Exception(nameof(securityNumber));
            _cardHolderName = !string.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new Exception(nameof(cardHolderName));

            if (expiration < DateTime.UtcNow)
            {
                throw new Exception(nameof(expiration));
            }

            _alias = alias;
            _expiration = expiration;
            _cardTypeId = cardTypeId;
        }

        public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
        {
            return _cardTypeId == cardTypeId
                   && _cardNumber == cardNumber
                   && _expiration == expiration;
        }
    }
}