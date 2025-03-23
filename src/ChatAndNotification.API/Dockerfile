FROM maven:3.9.6-eclipse-temurin-17-alpine AS build
WORKDIR /app
COPY ./src/ChatAndNotification.API/pom.xml .
COPY ./src/ChatAndNotification.API/src ./src
RUN mvn clean package -DskipTests

FROM eclipse-temurin:17-jre-alpine
WORKDIR /app
COPY --from=build /app/target/*.jar app.jar
EXPOSE 80
ENTRYPOINT ["java", "-jar", "app.jar"]
