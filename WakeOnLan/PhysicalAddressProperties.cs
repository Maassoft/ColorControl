namespace System.Net
{
    /// <summary>
    /// Der Administrator der physikalischen Adresse gibt an, ob die Adresse globally unique oder local administrated ist.
    /// </summary>
    public enum PhysicalAddressAdministrator
    {
        /// <summary>
        /// Die Adresse ist global einzigartig (nach der OUI).
        /// </summary>
        Global,

        /// <summary>
        /// Die Adresse ist lokal administriert.
        /// </summary>
        Local
    }

    /// <summary>
    /// Der Typ der physikalischen Adresse gibt an, ob es um eine UNicast oder Multicast-Adresse handelt.
    /// </summary>
    public enum PhysicalAddressType
    {
        /// <summary>
        /// Bezeichnet eine Unicast-Adresse.
        /// </summary>
        Unicast,

        /// <summary>
        /// Bezeichnet eine Multicast-Adresse
        /// </summary>
        Multicast
    }
}
