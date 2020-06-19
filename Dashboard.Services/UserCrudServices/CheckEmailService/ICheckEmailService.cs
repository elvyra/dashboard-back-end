namespace Dashboard.Services.UserCrudServices
{
    /// <summary>
    /// Checks if email is already in use
    /// </summary>
    public interface ICheckEmailService
    {
        /// <summary>
        /// If email is in use (already taken)
        /// </summary>
        /// <param name="email">Email to check</param>
        /// <returns>bool</returns>
        bool IsAlreadyTaken(string email);
    }
}