namespace EmailBomber
{

    class Program
    {
        static void Main(string[] args)
        {
            Bomber bomber = BomberFactory.FullBomber("kavkazkoe@yandex.ru");//С помощью фабрики создаём новый экземпляр бомбера
            bomber.Bomb().Wait();//Начинаем бомбить и ждём завершения задачи
        }
    }
}
