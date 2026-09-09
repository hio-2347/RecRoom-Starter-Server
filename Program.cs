namespace RecRoom
{
    public class Program
    {
        /* 
         * Console.WriteLine(JsonSerializer.Serialize(ctx.Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString()), new JsonSerializerOptions { WriteIndented = true }));
         * above to print form in json
        */

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var webApplication = builder.Build();


            app.Init();

            Signatures.Init();

            app.Run("http:localhost:2059");
        }
    }
}
