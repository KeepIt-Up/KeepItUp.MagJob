FROM maven:3.9.6-eclipse-temurin-17-alpine AS build
WORKDIR /app
COPY ./src/KeepItUp.MagJob.WorkEvidence/pom.xml .
COPY ./src/KeepItUp.MagJob.WorkEvidence/src ./src
RUN mvn clean package

FROM eclipse-temurin:17-jre-alpine
WORKDIR /app
COPY --from=build /app/target/*.jar app.jar
EXPOSE 80
ENTRYPOINT ["java", "-jar", "app.jar"]
