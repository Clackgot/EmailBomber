

using NLog;

namespace EmailBomber
{
    
    class Program
    {
        static void Main(string[] args)
        {
            BomberLogger.GetLogger().Info("Запуск логгера");
            string email = "kavkazkoe123@yandex.ru";
            LogManager.Configuration.Variables["email"] = email;

            Bomber bomber = BomberFactory.FullBomber(email);//С помощью фабрики создаём новый экземпляр бомбера
            bomber.Bomb().Wait();//Начинаем бомбить и ждём завершения задачи
        }
    }
}
