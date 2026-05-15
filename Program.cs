using System;
using System.Collections.Generic;
using System.Linq;


class Program
{
    static void Main(string[] args)
    {
        string dbFilePath = "movies_db.bin";
        DatabaseManager? dbManager;

        try
        {
            dbManager = new DatabaseManager(dbFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Критическая ошибка" +
                $" инициализации БД: {ex.Message}");
            return;
        }

        while (true)
        {
            Console.WriteLine("=== БАЗА ДАННЫХ ФИЛЬМОВ ===");
            Console.WriteLine("1. Просмотр всей базы данных");
            Console.WriteLine("2. Добавить фильм");
            Console.WriteLine("3. Удалить фильм");
            Console.WriteLine("4. Запрос: " +
                "Фильмы конкретного режиссера");
            Console.WriteLine("5. Запрос: " +
                "Фильмы новее указанного года");
            Console.WriteLine("6. Запрос: Самый старый фильм в базе");
            Console.WriteLine("7. Запрос: " +
                "Средняя оценка фильмов студии");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");

            string? choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        PrintMovies(dbManager.GetAllMovies());
                        break;
                    case "2":
                        AddMovieUI(dbManager);
                        break;
                    case "3":
                        DeleteMovieUI(dbManager);
                        break;
                    case "4":
                        Console.Write("Введите имя режиссера: ");
                        PrintMovies(dbManager.GetMoviesByDirector(
                            Console.ReadLine()));
                        break;
                    case "5":
                        int year = ReadInt("Введите год: ");
                        PrintMovies(dbManager.GetMoviesAfterYear(
                            year));
                        break;
                    case "6":
                        var oldest = dbManager.GetOldestMovie();
                        Console.WriteLine(oldest != null ? 
                            $"\nСамый старый фильм:\n{oldest}" : 
                            "\nБаза данных пуста.");
                        break;
                    case "7":
                        Console.Write("Введите название компании (студии): ");
                        var avg = dbManager.GetAverageRatingByCompany(
                            Console.ReadLine());
                        Console.WriteLine(avg.HasValue ? 
                            $"\nСредняя оценка: {avg.Value:F2}" : 
                            "\nФильмы этой компании не найдены.");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный ввод. Попробуйте снова.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ОШИБКА]: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }

    private static void AddMovieUI(DatabaseManager dbManager)
    {
        Console.WriteLine("\n--- Добавление фильма ---");
        Console.Write("Название: ");
        string? title = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Название не может быть пустым.");

        int year = ReadInt("Год выпуска: ");
        double rating = ReadDouble("Средняя оценка (0-10): ");

        Console.Write("Режиссер: ");
        string? director = Console.ReadLine();

        Console.Write("Компания: ");
        string? company = Console.ReadLine();

        dbManager.AddMovie(new Movie(title, year, rating,
            director, company));
        Console.WriteLine("Фильм успешно добавлен!");
    }

    private static void DeleteMovieUI(DatabaseManager dbManager)
    {
        var movies = dbManager.GetAllMovies().ToList();

        if (!movies.Any())
        {
            Console.WriteLine("\nБаза данных пуста. Удалять нечего.");
            return;
        }

        Console.WriteLine("\n--- Список фильмов для удаления ---");
        for (int i = 0; i < movies.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {movies[i]}");
        }

        int choice = ReadInt("\nВведите номер фильма для удаления: ");

        if (choice < 1 || choice > movies.Count)
        {
            Console.WriteLine("Неверный номер. Укажите число из списка.");
            return;
        }

        if (dbManager.DeleteMovieByIndex(choice - 1))
        {
            Console.WriteLine("Фильм успешно удален.");
        }
        else
        {
            Console.WriteLine("Ошибка при удалении.");
        }
    }

    private static void PrintMovies(IEnumerable<Movie> movies)
    {
        Console.WriteLine("\n--- Результат ---");
        if (!movies.Any())
        {
            Console.WriteLine("Список пуст.");
            return;
        }
        int num = 1;
        foreach (var movie in movies)
        {
            Console.WriteLine($"{num}. " + movie.ToString());
            num++;
        }
    }

    private static int ReadInt(string prompt)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int result))
        {
            return result;
        }
        throw new ArgumentException("Ожидалось целое число.");
    }

    private static double ReadDouble(string prompt)
    {
        Console.Write(prompt);
        if (double.TryParse(Console.ReadLine()?.Replace('.', ','),
            out double result))
        {
            return result;
        }
        throw new ArgumentException(
            "Ожидалось число с плавающей точкой.");
    }
}
