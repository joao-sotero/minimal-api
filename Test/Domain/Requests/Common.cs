namespace Test.Domain.Requests
{
    public static class Common
    {
        public static string? jwt;
        private static readonly object _lock = new object();

        public static async Task<string> GetJwtAsync()
        {
            if (jwt == null)
            {
                lock (_lock)
                {
                    // Double-check locking
                    if (jwt == null)
                    {
                        // Assume TestarLogin() is an async method
                        jwt = new AdministradorRequestTest().TestarLogin().GetAwaiter().GetResult();
                    }
                }
            }
            return jwt;
        }
    }
}
