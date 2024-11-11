namespace IsBankMvc.Abstraction.Enums
{
    public enum ThirdPartyProvider
    {
        UnKnown = 0,
        IsBank = 1,
        DenizBank = 2,
        PayZee = 3,
        HalkBank = 4,
        GarantiBank = 5,
        AkBank = 6,
        ZiraatBank = 7,
        FinansBank = 8,
        YapiKrediBank = 9,
        KuveytTurkBank = 10,
        VakifBank = 11,
    }

    public enum Currency
    {
        UnKnown = 0,
        EUR = 1,
        USD = 2,
        TL = 3,
        GBP = 4,
        JPY = 5,
        RUB = 6,
        CAD = 7,
    }

    public enum PaymentSection
    {
        UnKnown = 0,
        Clinic = 1,
        Travel = 2,
        Health = 3
    }
}