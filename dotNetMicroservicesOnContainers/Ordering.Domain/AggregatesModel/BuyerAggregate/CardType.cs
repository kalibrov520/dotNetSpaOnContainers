namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    public class CardType
    {
        public static CardType Amex = new CardType(1, "Amex");
        public static CardType Visa = new CardType(2, "Visa");
        public static CardType MasterCard = new CardType(3, "MasterCard");

        public CardType(int id, string name)
        {
        }
    }
}