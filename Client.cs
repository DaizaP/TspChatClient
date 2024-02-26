using System.Net.Sockets;

namespace TspChatClient
{
    internal class Client : AbstractClient
    {
        public override async Task Run()
        {
            string host = "127.0.0.1";
            int port = 55555;
            using TcpClient client = new TcpClient();
            Console.Write("Введите свое имя: ");
            UserName = Console.ReadLine();
            Console.WriteLine($"Добро пожаловать, {UserName}");
            StreamReader? Reader = null;
            StreamWriter? Writer = null;

            try
            {
                client.Connect(host, port); //подключение клиента
                Reader = new StreamReader(client.GetStream());
                Writer = new StreamWriter(client.GetStream());
                if (Writer is null || Reader is null) return;
                // запускаем новый поток для получения данных
                Task.Run(() => ReceiveMessageAsync(Reader, Writer));
                // запускаем ввод сообщений
                await SendMessageAsync(Writer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Writer?.Close();
            Reader?.Close();
            Console.WriteLine("Работа завершена");

        }

        protected override void Print(string message)
        {
            if (OperatingSystem.IsWindows())    // если ОС Windows
            {
                var position = Console.GetCursorPosition(); // получаем текущую позицию курсора
                int left = position.Left;   // смещение в символах относительно левого края
                int top = position.Top;     // смещение в строках относительно верха
                                            // копируем ранее введенные символы в строке на следующую строку
                Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
                // устанавливаем курсор в начало текущей строки
                Console.SetCursorPosition(0, top);
                // в текущей строке выводит полученное сообщение
                Console.WriteLine(message);
                // переносим курсор на следующую строку
                // и пользователь продолжает ввод уже на следующей строке
                Console.SetCursorPosition(left, top + 1);
            }
            else Console.WriteLine(message);
        }

        protected override async Task ReceiveMessageAsync(StreamReader reader, StreamWriter writer)
        {
            while (true)
            {
                try
                {
                    // считываем ответ в виде строки
                    string? message = await reader.ReadLineAsync();
                    // если пустой ответ, ничего не выводим на консоль
                    if (string.IsNullOrEmpty(message))
                    {
                        continue;
                    }
                    if (message.Contains(Code.succesCode))
                    {
                        Console.WriteLine(" | OK");
                        continue;
                    }
                    if (message.Contains(Code.shutdownServerCode))
                    {
                        Console.WriteLine(" | Сервер был выключен");
                        break;
                    }
                    else
                    {
                        Print(message);//вывод сообщения
                    }
                    // Отправляем подтверждение отправки
                    await writer.WriteLineAsync($"{DateTime.Now}. Code:{Code.succesCode} The message received by user {UserName}"); //передача данных
                    await writer.FlushAsync();
                }
                catch
                {
                    Print(" | Получение сообщений остановлено");
                    break;
                }
            }
        }

        protected override async Task SendMessageAsync(StreamWriter writer)
        {
            await writer.WriteLineAsync(UserName);
            await writer.FlushAsync();
            Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");
            while (true)
            {
                try
                {
                    string? message = Console.ReadLine();
                    if (message == "Exit")
                    {
                        Console.WriteLine("Вы вышли из чата");
                        await writer.WriteLineAsync(message);
                        await writer.FlushAsync();
                        break;
                    }
                    await writer.WriteLineAsync(message);
                    await writer.FlushAsync();
                }
                catch (Exception ex)
                {
                    Print(" | Failed");
                    break;
                }
                finally
                {
                    await writer.FlushAsync();
                }
            }
        }
    }
}
