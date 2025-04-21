namespace AuthSystem.Domain.Common.Enums
{
    /// <summary>
    /// Tipos de usuario en el sistema
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// Usuario interno (empleado, administrador)
        /// </summary>
        Internal = 1,

        /// <summary>
        /// Usuario externo (cliente, proveedor)
        /// </summary>
        External = 2
    }
}
