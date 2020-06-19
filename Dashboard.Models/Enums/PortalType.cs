using System.ComponentModel;

namespace Dashboard.Models.Enums
{
    public enum PortalType
    {
        WebApp,

        [Description("Service-REST")]
        ServiceREST,

        [Description("Service-SOAP")]
        ServiceSOAP
    }
}
