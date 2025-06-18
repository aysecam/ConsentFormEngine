namespace ConsentFormEngine.Core.Services
{
    /// <summary>
    /// Mevcut HTTP isteğini yapan kullanıcının kimliğini döner.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// O anki kullanıcının ID’si. Eğer authenticate değilse null veya "SYSTEM" dönebilir.
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// O anki isteği yapan client’ın IP adresi.
        /// </summary>
        string IpAddress { get; }
    }


}
