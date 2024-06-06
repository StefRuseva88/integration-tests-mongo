# Integration-Testing-MongoDb-MoviesLibraryAPI

### This is a console-based application built using the .NET Core Framework that manages a collection of movies.

## Project Description

**MoviesLibraryAPI** is a console-based application developed to manage a collection of movies. It allows users to perform various operations, such as creating, reading, updating, and deleting movies from a database in MongoDB Compass. This application is designed to showcase essential software development concepts and practices.

## Key Features

- **Create**: Add new movies to the database.
- **Read**: Retrieve details of movies from the database.
- **Update**: Modify existing movie information.
- **Delete**: Remove movies from the database.

## Technologies Used

- **.NET Core**: The primary framework used for building the console application.
- **MongoDB Compass**: The GUI for MongoDB used for managing the database.
- **MongoDB.Driver**: The .NET driver for MongoDB for database interactions.
- **xUnit**: A unit-testing framework for .NET applications.

## Project Structure

- **MoviesLibraryAPI**: The main project folder containing the console application.
    - **MoviesLibraryAPI.csproj**: Project file for the console application.
    - **Program.cs**: The main entry point of the application.
    - **Movie.cs**: The model representing a movie.
    - **MovieService.cs**: Contains business logic for managing movies.
    - **AppConfig.json**: Configuration file for database connection settings.

- **MoviesLibraryAPI.Tests**: The test project folder containing unit and integration tests.
    - **MoviesLibraryAPI.Tests.csproj**: Project file for the test project.
    - **MovieServiceTests.cs**: Contains tests for the `MovieService` class.
    - **TestUtilities.cs**: Utility methods for setting up and tearing down test environments.

## Getting Started

### Prerequisites

- .NET Core SDK 3.1 or higher
- MongoDB Compass (local or remote)
- Visual Studio 2019 or later / Visual Studio Code

### Installation

1. **Clone the repository:**

    ```bash
    git clone https://github.com/yourusername/MoviesLibraryAPI.git
    ```

2. **Navigate to the project directory:**

    ```bash
    cd MoviesLibraryAPI
    ```

3. **Restore the dependencies:**

    ```bash
    dotnet restore
    ```

4. **Update the database connection string** in `AppConfig.json` to point to your MongoDB instance.

### Running the Application

To run the application, use the following command:

```bash
dotnet run --project MoviesLibraryAPI
