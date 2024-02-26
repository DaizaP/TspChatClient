namespace TspChatClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            AbstractClient client = new Client();
            await client.Run();
            while (true)
            {
                Console.ReadKey();
                break;
            }
        }
    }
}
