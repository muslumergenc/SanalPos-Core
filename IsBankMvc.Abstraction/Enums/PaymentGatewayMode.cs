namespace IsBankMvc.Abstraction.Enums
{
    public enum PaymentGatewayMode
    {
        UnKnown = 0,
        IFrame = 1,
        RedirectHTML = 2,
        RedirectURL = 3,
        Form = 4
    }

    public enum PaymentFilterMode
    {
        UnKnown = 0,
        Pending = 1,
        Completed = 2
    }
}
