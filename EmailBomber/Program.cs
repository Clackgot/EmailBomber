using System;

namespace EmailBomber
{

    class Program
    {
        static void Main(string[] args)
        {
            Bomber bomber = new Bomber("kavkazkoe@yandex.ru");
            bomber.Bomb().Wait();
        }
    }
}
