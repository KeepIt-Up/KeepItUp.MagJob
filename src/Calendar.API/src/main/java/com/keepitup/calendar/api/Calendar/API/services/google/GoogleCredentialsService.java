package com.keepitup.calendar.api.Calendar.API.services.google;

import com.google.api.services.calendar.CalendarScopes;
import com.google.auth.http.HttpCredentialsAdapter;
import com.google.auth.oauth2.GoogleCredentials;
import com.google.auth.oauth2.ServiceAccountCredentials;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.io.InputStream;
import java.util.Collections;

@Service
public class GoogleCredentialsService {
    private String credentialsPath = "/credentials.json";
    private String impersonatedUserEmail = "magjob@magjob.online";

    public HttpCredentialsAdapter getCredentials() throws IOException {
        GoogleCredentials credentials;

        try (InputStream serviceAccountStream = getClass().getResourceAsStream(credentialsPath)) {
            if (serviceAccountStream == null) {
                throw new IOException("Resource not found: " + credentialsPath);
            }
            
            credentials = ServiceAccountCredentials.fromStream(serviceAccountStream)
                    .createScoped(Collections.singletonList(CalendarScopes.CALENDAR));

            credentials = credentials.createDelegated(impersonatedUserEmail);
            System.out.println("Impersonating user: " + impersonatedUserEmail);
        } catch (IOException e) {
            System.err.println("Error loading service account credentials: " + e.getMessage());
            throw e;
        }
        return new HttpCredentialsAdapter(credentials);
    }
}
