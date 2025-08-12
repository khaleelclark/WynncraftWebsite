# Spring Boot Player CRUD Application

This project is a Spring Boot application that provides a RESTful API for managing player entities. It includes CRUD (Create, Read, Update, Delete) operations and is integrated with Swagger for API documentation.

## Project Structure

```
springboot-player-crud
├── src
│   ├── main
│   │   ├── java
│   │   │   └── com
│   │   │       └── example
│   │   │           └── playercrud
│   │   │               ├── PlayerCrudApplication.java
│   │   │               ├── controller
│   │   │               │   └── PlayerController.java
│   │   │               ├── model
│   │   │               │   └── Player.java
│   │   │               ├── repository
│   │   │               │   └── PlayerRepository.java
│   │   │               └── service
│   │   │                   └── PlayerService.java
│   │   └── resources
│   │       ├── application.properties
│   │       └── static
│   └── test
│       └── java
│           └── com
│               └── example
│                   └── playercrud
│                       └── PlayerCrudApplicationTests.java
├── pom.xml
└── README.md
```

## Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd springboot-player-crud
   ```

2. **Build the Project**
   Ensure you have Maven installed, then run:
   ```bash
   mvn clean install
   ```

3. **Run the Application**
   You can run the application using:
   ```bash
   mvn spring-boot:run
   ```

4. **Access the API**
   The API will be available at `http://localhost:8080/api/players`.

5. **Swagger Documentation**
   Access the Swagger UI at `http://localhost:8080/swagger-ui/` to view and interact with the API endpoints.

## API Endpoints

- **Create Player**
  - `POST /api/players`
  
- **Get Player by ID**
  - `GET /api/players/{id}`
  
- **Update Player**
  - `PUT /api/players/{id}`
  
- **Delete Player**
  - `DELETE /api/players/{id}`
  
- **Get All Players**
  - `GET /api/players`

## Dependencies

This project uses the following dependencies:
- Spring Boot
- Spring Data JPA
- Swagger for API documentation

## License

This project is licensed under the MIT License. See the LICENSE file for more details.