using ContentCrawler.Model;
using HtmlAgilityPack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting url");
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                   
                    ArticleDbContext db = new ArticleDbContext();
                    //var existArt = db.Articles.Where(a => a.Link.Equals(message));
                    string existArt = null;
                    if (existArt == null)
                    {
                        Console.WriteLine("Creating article");
                        try
                        {
                            var doc = new HtmlWeb().Load(message);

                            var listTitle = doc.DocumentNode.SelectNodes("//h1[contains(@class, 'title-detail')]");
                            var title = "";
                            if (listTitle == null)
                            {
                                return;
                            }
                            Console.WriteLine(listTitle.First().InnerText);

                            title = listTitle.First().InnerText;

                            var images = doc.DocumentNode.Descendants("img")
                                                              .Select(a => a.GetAttributeValue("data-src", null))
                                                              .Where(u => !String.IsNullOrEmpty(u));
                            var img = "";
                            if (images == null)
                            {
                                return;
                            }
                            img = images.First();


                            var listContent = doc.DocumentNode.SelectNodes("//p[contains(@class, 'Normal')]");
                            if(listContent == null)
                            {
                                return;
                            }
                            Console.WriteLine(listContent.First().InnerText);
                            StringBuilder content = new StringBuilder();
                            foreach (var item in listContent)
                            {

                                content.AppendLine(item.InnerText);

                            }
                            Console.WriteLine(content);



                            // Create demo: Create a article instance and save it to the database
                            Article article = new Article
                            {

                                Link = message,
                                Title = title,
                                Images = img,
                                Content = content.ToString()
                            };
                            db.Articles.Add(article);
                            db.SaveChanges();
                            Console.WriteLine("\nCreated article: ");

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    }
                    else
                    {
                        Console.WriteLine(message + "\nExisted");

                    }




                };
                channel.BasicConsume(queue: "hello",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }









        }

    }
}
