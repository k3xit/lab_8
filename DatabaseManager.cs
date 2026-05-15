using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class DatabaseManager
{
    private List<Movie> _movies;
    private readonly string _filePath;

    public DatabaseManager(string filePath)
    {
        _filePath = filePath;
        _movies = new List<Movie>();
        LoadFromFile();
    }

    public void LoadFromFile()
    {
        _movies.Clear();
        if (!File.Exists(_filePath)) return;

        try
        {
            using (BinaryReader reader =
                new BinaryReader(File.Open(_filePath, FileMode.Open)))
            {
                while (reader.BaseStream.Position
                    != reader.BaseStream.Length)
                {
                    string title = reader.ReadString();
                    int year = reader.ReadInt32();
                    double rating = reader.ReadDouble();
                    string director = reader.ReadString();
                    string company = reader.ReadString();
                    _movies.Add(new Movie(title, year, rating,
                        director, company));
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при чтении файла БД:" +
                $" {ex.Message}");
        }
    }

    private void SaveToFile()
    {
        try
        {
            using (BinaryWriter writer =
                new BinaryWriter(File.Open(_filePath, FileMode.Create)))
            {
                foreach (var m in _movies)
                {
                    writer.Write(m.Title);
                    writer.Write(m.ReleaseYear);
                    writer.Write(m.Rating);
                    writer.Write(m.Director);
                    writer.Write(m.Company);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при записи в файл БД:" +
                $" {ex.Message}");
        }
    }

    public void AddMovie(Movie movie)
    {
        if (_movies.Any(m => m.Title.Equals(movie.Title,
            StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("Фильм с таким" +
                " названием уже существует в базе!");
        }

        _movies.Add(movie);
        SaveToFile();
    }

    public bool DeleteMovieByIndex(int index)
    {
        if (index < 0 || index >= _movies.Count)
        {
            return false;
        }
        _movies.RemoveAt(index);
        SaveToFile();
        return true;
    }

    public IEnumerable<Movie> GetAllMovies()
    {
        return _movies.AsEnumerable();
    }

    public IEnumerable<Movie> GetMoviesByDirector(string? director)
    {
        return _movies
            .Where(m => m.Director.Equals(director,
            StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(m => m.Rating);
    }

    public IEnumerable<Movie> GetMoviesAfterYear(int year)
    {
        return _movies
            .Where(m => m.ReleaseYear > year)
            .OrderBy(m => m.ReleaseYear);
    }

    public Movie GetOldestMovie()
    {
        return _movies.OrderBy(m => m.ReleaseYear).FirstOrDefault();
    }

    public double? GetAverageRatingByCompany(string? company)
    {
        var companyMovies = _movies.Where(m => m.Company.Equals(company,
            StringComparison.OrdinalIgnoreCase));
        if (!companyMovies.Any())
        {
            return null;
        }
        return companyMovies.Average(m => m.Rating);
    }
}
