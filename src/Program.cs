using System;
using System.Collections.Generic;
using System.Threading;
using RoyalQuiz.Core;
using RoyalQuiz.Logic;

namespace RoyalQuiz
{
    public class Program
    {
        public static Random Random = new Random();

        public static readonly List<Quiz> ActiveQuizzes = new List<Quiz>();

        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        private static void Main()
        {
            Console.Title = "RoyalQuiz";

            Questions.Initialize();
            DiscordManager.Initialize();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}