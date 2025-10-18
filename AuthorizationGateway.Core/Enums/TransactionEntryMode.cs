namespace AuthorizationGateway.Core.Enums
{
    /// <summary>
    ///  Defines the method used to enter cardholder data for a transaction.
    /// </summary>
    public enum TransactionEntryMode
    {
        Unknown = 0,
        Contact = 5,
        Contactless = 7,
        ManualPanEntry = 90,
        ECommerce = 91
    }
}
