using MongoDB.Driver;
using MoviesLibraryAPI.Controllers;
using MoviesLibraryAPI.Controllers.Contracts;
using MoviesLibraryAPI.Data.Models;
using MoviesLibraryAPI.Services;
using MoviesLibraryAPI.Services.Contracts;
using System.ComponentModel.DataAnnotations;

namespace MoviesLibraryAPI.XUnitTests
{
    public class XUnitIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly MoviesLibraryXUnitTestDbContext _dbContext;
        private readonly IMoviesLibraryController _controller;
        private readonly IMoviesRepository _repository;

        public XUnitIntegrationTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _repository = new MoviesRepository(_dbContext.Movies);
            _controller = new MoviesLibraryController(_repository);

            InitializeDatabaseAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeDatabaseAsync()
        {
            await _dbContext.ClearDatabaseAsync();
        }

        [Fact]
        public async Task AddMovieAsync_WhenValidMovieProvided_ShouldAddToDatabase()
        {
            // Arrange
            var movie = new Movie
            {
                Title = "Test Movie",
                Director = "Test Director",
                YearReleased = 2022,
                Genre = "Action",
                Duration = 120,
                Rating = 7.5
            };

            // Act
            await _controller.AddAsync(movie);

            // Assert
            var resultMovie = await _dbContext.Movies.Find(m => m.Title == "Test Movie").FirstOrDefaultAsync();
            Xunit.Assert.NotNull(resultMovie);
            Xunit.Assert.Equal("Test Movie", resultMovie.Title);
            Xunit.Assert.Equal("Test Director", resultMovie.Director);
            Xunit.Assert.Equal(2022, resultMovie.YearReleased);
            Xunit.Assert.Equal("Action", resultMovie.Genre);
            Xunit.Assert.Equal(120, resultMovie.Duration);
            Xunit.Assert.Equal(7.5, resultMovie.Rating);
        }

        [Fact]
        public async Task AddMovieAsync_WhenInvalidMovieProvided_ShouldThrowValidationException()
        {
            // Arrange
            var invalidMovie = new Movie
            {
                YearReleased = 2022,
                Genre = "Action",
                Duration = 120,
                Rating = 7.5
            };

            // Act and Assert
            var exception = await Xunit.Assert.ThrowsAsync<ValidationException>(() => _controller.AddAsync(invalidMovie));
            Xunit.Assert.Equal("Movie is not valid.", exception.Message);
        }

        [Fact]
        public async Task DeleteAsync_WhenValidTitleProvided_ShouldDeleteMovie()
        {
            // Arrange
            var movie = new Movie
            {
                Title = "Test Movie",
                Director = "Test Director",
                YearReleased = 2022,
                Genre = "Action",
                Duration = 86,
                Rating = 7.5
            };
            await _controller.AddAsync(movie);

            // Act            

            await _controller.DeleteAsync(movie.Title); 

            // Assert
            // The movie should no longer exist in the database
            var result = await _dbContext.Movies.Find(m => m.Title == movie.Title).FirstOrDefaultAsync();
            Xunit.Assert.Null(result);
        }


        [Xunit.Theory]
        [Xunit.InlineData(null)]
        [Xunit.InlineData("")]
        public async Task DeleteAsync_WhenTitleIsNull_ShouldThrowArgumentException(string invalidname)
        {
            // Act and Assert
            Xunit.Assert.ThrowsAsync<ArgumentException>(() => _controller.DeleteAsync(invalidname));
        }

        [Fact]
        public async Task DeleteAsync_WhenTitleIsEmpty_ShouldThrowArgumentException()
        {
            Xunit.Assert.ThrowsAsync<ArgumentException>(() => _controller.DeleteAsync(""));
        }

        [Fact]
        public async Task DeleteAsync_WhenTitleDoesNotExist_ShouldThrowInvalidOperationException()
        {
            Xunit.Assert.ThrowsAsync<ArgumentException>(() => _controller.DeleteAsync("Invalid Name"));
        }

        [Fact]
        public async Task GetAllAsync_WhenNoMoviesExist_ShouldReturnEmptyList()
        {

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            Xunit.Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenMoviesExist_ShouldReturnAllMovies()
        {
            // Arrange
            var movie1 = new Movie
            {
                Title = "Test Movie",
                Director = "Test Director",
                YearReleased = 2022,
                Genre = "Action",
                Duration = 86,
                Rating = 7.5
            };

            var movie2 = new Movie

            {
                Title = "Dune",
                Director = "Denis Villeneuve",
                YearReleased = 2022,
                Genre = "Sci-Fi",
                Duration = 86,
                Rating = 7.5
            };
            await _dbContext.Movies.InsertManyAsync(new []{ movie1, movie2 });

            // Act
            var result = await _controller.GetAllAsync();
            // Assert
            // Ensure that all movies are returned
            Xunit.Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByTitle_WhenTitleExists_ShouldReturnMatchingMovie()
        {
            // Arrange
            var movie1 = new Movie
            {
                Title = "The Matrix",
                Director = "Test Director",
                YearReleased = 2022,
                Genre = "Action",
                Duration = 86,
                Rating = 7.5
            };

            await _controller.AddAsync(movie1);

            var movie2 = new Movie

            {
                Title = "LOTR",
                Director = "Denis Villeneuve",
                YearReleased = 2022,
                Genre = "Sci-Fi",
                Duration = 86,
                Rating = 7.5
            };

            await _dbContext.Movies.InsertManyAsync(new[] { movie1, movie2 });

            // Act
            var result = await _controller.GetByTitle(movie1.Title);
            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(movie1.Director, result.Director);
        }

        [Fact]
        public async Task GetByTitle_WhenTitleDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _controller.GetByTitle("Invalid Title");
            // Assert
            Xunit.Assert.Null(result);
        }


        [Fact]
        public async Task SearchByTitleFragmentAsync_WhenTitleFragmentExists_ShouldReturnMatchingMovies()
        {
            // Arrange
            var firstMovie = new Movie
            {
                Title = "My Fragment",
                Director = "Test Director",
                YearReleased = 2022,
                Genre = "Action",
                Duration = 86,
                Rating = 7.5
            };

            await _controller.AddAsync(firstMovie);

            var secondMovie = new Movie

            {
                Title = "Your Fragment",
                Director = "Denis Villeneuve",
                YearReleased = 2022,
                Genre = "Sci-Fi",
                Duration = 86,
                Rating = 7.5
            };

            await _dbContext.Movies.InsertManyAsync(new[] { firstMovie, secondMovie });

            // Act
            var result = await _controller.SearchByTitleFragmentAsync("Your");

            // Assert // Should return one matching movie
            Xunit.Assert.Equal(1, result.Count());
            var movie = result.First();
            Xunit.Assert.Equal(secondMovie.Title, movie.Title);
        }

        [Fact]
        public async Task SearchByTitleFragmentAsync_WhenNoMatchingTitleFragment_ShouldThrowKeyNotFoundException()
        {
            // Act and Assert
            Xunit.Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.SearchByTitleFragmentAsync("Invalid Fragment"));
        }

        [Fact]
        public async Task UpdateAsync_WhenValidMovieProvided_ShouldUpdateMovie()
        {
            // Arrange
            var movie1 = new Movie
            {
                Title = "First Movie to Update",
                Director = "Test Director",
                YearReleased = 2022,
                Genre = "Action",
                Duration = 86,
                Rating = 7.5
            };

            await _controller.AddAsync(movie1);

            var movie2 = new Movie

            {
                Title = "Second Movie to Update",
                Director = "Denis Villeneuve",
                YearReleased = 2022,
                Genre = "Sci-Fi",
                Duration = 86,
                Rating = 7.5
            };

            await _dbContext.Movies.InsertManyAsync(new[] { movie1, movie2 });

            var movieToUpdate = await _dbContext.Movies.Find(m => m.Title == movie1.Title).FirstOrDefaultAsync();

            movieToUpdate.Title = "Updated Title";
            movieToUpdate.Director = "Updated Director";
            // Act
            await _controller.UpdateAsync(movieToUpdate);
            // Assert
            var updatedMovie = await _dbContext.Movies.Find(m => m.Title == "Updated Title").FirstOrDefaultAsync();
            Xunit.Assert.NotNull(updatedMovie);
            Xunit.Assert.Equal("Updated Title", updatedMovie.Title);
            Xunit.Assert.Equal("Updated Director", updatedMovie.Director);
        }

        [Fact]
        public async Task UpdateAsync_WhenInvalidMovieProvided_ShouldThrowValidationException()
        {
            // Arrange
            // Movie without required fields
            var invalidMovie = new Movie
            {
                YearReleased = 2022,
                Genre = "Action",
                Duration = 120,
                Rating = 7.5
            };
            // Act and Assert
            var exception = await Xunit.Assert.ThrowsAsync<ValidationException>(() => _controller.UpdateAsync(invalidMovie));
            Xunit.Assert.Equal("Movie is not valid.", exception.Message);
        }
    }
}
