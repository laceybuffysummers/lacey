﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoMapper;
using Lacey.Medusa.Common.Email.Services.Email;
using Lacey.Medusa.Youtube.Services.Transfer.Services;
using Lacey.Medusa.Youtube.Transfer.Run.Configuration;
using Lacey.Medusa.Youtube.Transfer.Run.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lacey.Medusa.Youtube.Transfer.Run
{
    class Program
    {
        private static ILogger<Program> logger;

        static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            var config = configuration.GetSection("App").Get<AppConfiguration>();
            var connectionString = configuration.GetConnectionString("Default");

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddAutoMapper()
                .AddLogging(logBuilder => 
                    logBuilder
                        .AddLog4Net()
                        .SetMinimumLevel(LogLevel.Trace))
                .AddAppServices(config, connectionString)
                .BuildServiceProvider();

            //configure console logging
            logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogTrace("Welcome to the YouTube tool!");

            Console.WriteLine("0 - Update Last Videos");
            Console.WriteLine("1 - Update Last Thumbnails");
            Console.WriteLine("2 - Channel Info");
            Console.WriteLine("3 - Videos");
            Console.WriteLine("4 - Thumbnails");
            Console.WriteLine("5 - Playlists");
            Console.WriteLine("6 - Sections");
            Console.WriteLine("7 - Subscriptions");
            Console.WriteLine("8 - Instagram");

            var answer = Console.ReadLine();

            var transferService = serviceProvider.GetService<ITransferService>();

            var sb = new StringBuilder();
            for (var i = 0; i < config.SourceChannels.Length; i++)
            {
                var sourceChannelId = config.SourceChannels[i].ChannelId;
                var destChannelId = config.DestChannels[i].ChannelId;

                var sourceInstagram = config.SourceChannels[i].Instagram;
                var destInstagram = config.DestChannels[i].Instagram;

                var replacements = new Dictionary<string, string>
                {
                    [sourceInstagram] = destInstagram
                };

                logger.LogTrace($"[{sourceChannelId}] => [{destChannelId}]...");

                try
                {
                    if (answer == "0")
                    {
                        transferService.TransferVideosLast(
                            sourceChannelId, 
                            destChannelId,
                            replacements).Wait();
                    }
                    else if (answer == "1")
                    {
                        transferService.SetThumbnailsLast(sourceChannelId, destChannelId).Wait();
                    }
                    else if (answer == "2")
                    {
                        transferService.TransferMetadata(sourceChannelId, destChannelId).Wait();
                    }
                    else if (answer == "3")
                    {
                        transferService.TransferVideos(sourceChannelId, destChannelId).Wait();
                    }
                    else if (answer == "4")
                    {
                        transferService.SetThumbnails(sourceChannelId, destChannelId).Wait();
                    }
                    else if (answer == "5")
                    {
                        transferService.TransferPlaylists(sourceChannelId, destChannelId).Wait();
                    }
                    else if (answer == "6")
                    {
                        transferService.TransferSections(sourceChannelId, destChannelId).Wait();
                    }
                    else if (answer == "7")
                    {
                        transferService.TransferSubscriptions(sourceChannelId, destChannelId).Wait();
                    }
                    else if (answer == "8")
                    {
                        transferService.UpdateInstagram(destChannelId, sourceInstagram, destInstagram).Wait();
                    }
                }
                catch (Exception exc)
                {
                    logger.LogError(exc.Message);
                }

                logger.LogTrace($"[{sourceChannelId}] => [{destChannelId}] completed.{Environment.NewLine}");
                sb.AppendLine($"[https://www.youtube.com/channel/{sourceChannelId}] => [https://www.youtube.com/channel/{destChannelId}]");
            }

            if (config.Email.IsSendEmails)
            {
                var emailService = serviceProvider.GetService<IEmailProvider>();
                var currentFolder = Directory.GetCurrentDirectory();
                emailService.Send(
                    config.Email.From,
                    config.Email.To,
                    config.Email.Subject,
                    sb.ToString(),
                    true,
                    new[]
                    {
                        Path.Combine(currentFolder, config.Logs.LogFile)
                    });
            }

            serviceProvider.Dispose();
        }
    }
}
