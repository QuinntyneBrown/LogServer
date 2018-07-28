using System;

namespace IntegrationTests
{
    public class LogScenarioBase: ScenarioBase
    {
        public static class Get
        {
            public static string Logs = "api/logs";

            public static string LogById(Guid id)
            {
                return $"api/logs/{id}";
            }
        }

        public static class Post
        {
            public static string Logs = "api/logs";
        }
    }
}
