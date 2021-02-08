using System;

namespace EmailBomber
{

    class Program
    {
        static void Main(string[] args)
        {
            Bomber bomber = BomberFactory.FullBomber("kavkazkoe@yandex.ru");
            bomber.Bomb().Wait();

        }
    }
}
