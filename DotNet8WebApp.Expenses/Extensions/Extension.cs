using Newtonsoft.Json;

namespace DotNet8WebApp.Expenses.Extensions
{
    public static class Extension
    {
        public static T ToObject<T>(this string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr)!;
        }
    }
}
