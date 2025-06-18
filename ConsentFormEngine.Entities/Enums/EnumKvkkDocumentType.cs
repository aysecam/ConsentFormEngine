using System.ComponentModel;

namespace ConsentFormEngine.Entities.Enums
{
    public enum EnumKvkkDocumentType
    {
        [Description("Aydınlatma Metni")]
        ClarificationText = 1,
        [Description("Açık Rıza Metni")]
        ExplicitConsentText = 2,
    }
}
