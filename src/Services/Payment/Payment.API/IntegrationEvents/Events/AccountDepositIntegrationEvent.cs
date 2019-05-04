namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    public class AccountDepositIntegrationEvent
    {
        public string BuyerName { get; }
        public decimal DepositAmount { get; }
        public decimal PreviousAmount { get; set; }

        public AccountDepositIntegrationEvent(string buyerName, decimal depositAmount, decimal previousAmount)
        {
            this.BuyerName = buyerName;
            this.DepositAmount = depositAmount;
            this.PreviousAmount = previousAmount;
        }
    }
}