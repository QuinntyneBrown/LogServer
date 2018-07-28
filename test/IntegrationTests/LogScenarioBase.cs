namespace IntegrationTests
{
    public class LogScenarioBase: ScenarioBase
    {
        public static class Get
        {
            public static string Logs = "api/logs";

            public static string LogById(int id)
            {
                return $"api/logs/{id}";
            }
        }

        public static class Post
        {
            public static string Logs = "api/logs";
        }

        public static class Delete
        {
            public static string Log(int id)
            {
                return $"api/logs/{id}";
            }
        }
    }
}
