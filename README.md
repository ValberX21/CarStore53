# CarStore53

## Overview
The **Import Manager System** is a web application designed to manage the importation of cars from Europe to Brazil. This system provides tools for users to register and manage data related to car brands, cars, ships, and lots containing information about cars and their respective ships. It features secure authentication, report generation in PDF format, and efficient database interactions.

## Features
- **User Management**:
  - Secure authentication using JWT tokens.
- **Data Management**:
  - Register car brands and cars.
  - Register ship data.
  - Manage lots containing cars and ship information.
- **Reports**:
  - Generate and download reports in PDF format.
- **Database Interaction**:
  - Use of stored procedures for optimized database operations.
  - Retrieval of data using `SqlDataReader`.

## Technology Stack
- **Frontend**: ASP.NET MVC Razor Pages
- **Backend**: C#
- **Database Access**: ADO.NET with raw SQL queries and stored procedures
- **Authentication**: JWT (JSON Web Tokens)
- **PDF Generation**: iText library

## Installation
1. **Prerequisites**:
   - .NET SDK installed.
   - SQL Server installed and configured.
   - Visual Studio or Visual Studio Code for development.

2. **Clone the Repository**:
   ```bash
   git clone <repository_url>
   cd ImportManagerSystem
   ```

3. **Database Setup**:
   - Create a database in SQL Server.
   - Execute the provided SQL scripts in the `/DatabaseScripts` folder to create tables and stored procedures.

4. **Configure the Application**:
   - Open `appsettings.json` and configure the database connection string under `ConnectionStrings`.
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;"
   }
   ```

5. **Run the Application**:
   - Open the project in your IDE.
   - Build and run the application.
   - Navigate to `http://localhost:<port>` in your browser.

## Usage
1. **Authentication**:
   - Login to the system using your credentials to obtain a JWT token.
   - Use the token to access secured endpoints.

2. **Register Data**:
   - Navigate to the appropriate sections for managing car brands, cars, ships, and lots.
   - Fill in the forms and submit data.

3. **Generate Reports**:
   - Access the "Reports" section.
   - Select the desired data to generate and download a PDF report.

## Folder Structure
- `/Controllers`: Contains the controllers handling API endpoints.
- `/Views`: Razor Pages for the frontend.
- `/Models`: Contains the application’s data models.

## Development Notes
- **Database Interaction**:
  - Data is retrieved using `SqlDataReader` with queries written as strings.
  - Stored procedures are used for operations like inserts, updates, and deletes.
- **PDF Generation**:
  - The iText library is used to create and format PDF reports.
- **Authentication**:
  - JWT tokens are implemented to secure API endpoints.

## Future Improvements
- Add unit tests for key features.
- Implement logging for better monitoring and debugging.

## Contributing
Contributions are welcome! Please follow the steps below:
1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Commit your changes and create a pull request.


