using System;


public class Movie
{
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    public double Rating { get; set; }
    public string Director { get; set; }
    public string Company { get; set; }

    public Movie(string title, int releaseYear,
        double rating, string director, string company)
    {
        Title = title;
        ReleaseYear = releaseYear;
        Rating = rating;
        Director = director;
        Company = company;
    }

    public override string ToString()
    {
        return $"Фильм: «{Title}» ({ReleaseYear} г.) | Режиссер: " +
            $"{Director} | Студия: {Company} | Оценка: {Rating:F1}";
    }
}
