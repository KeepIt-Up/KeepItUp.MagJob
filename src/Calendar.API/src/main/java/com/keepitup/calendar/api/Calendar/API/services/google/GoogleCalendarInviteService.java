package com.keepitup.calendar.api.Calendar.API.services.google;

import com.google.api.client.googleapis.javanet.GoogleNetHttpTransport;
import com.google.api.client.json.gson.GsonFactory;
import com.google.api.services.calendar.Calendar;
import com.google.api.services.calendar.model.Event;
import com.google.api.services.calendar.model.EventAttendee;
import com.google.api.services.calendar.model.EventDateTime;
import com.google.auth.http.HttpCredentialsAdapter;
import com.keepitup.calendar.api.Calendar.API.services.google.GoogleCredentialsService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.security.GeneralSecurityException;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.util.Arrays;
import java.util.Date;
import java.util.List;

@Service
public class GoogleCalendarInviteService {

    private static final String APPLICATION_NAME = "MagJob Calendar";
    
    @Autowired
    private GoogleCredentialsService credentialsService;

    public void sendCalendarInvite(String recipientEmail, String eventTitle, String description, 
                                 LocalDateTime startTime, LocalDateTime endTime)
            throws IOException, GeneralSecurityException {
        
        Calendar calendarService = getCalendarService();
        
        Event event = new Event()
                .setSummary(eventTitle)
                .setDescription(description);

        String timeZone = ZoneId.systemDefault().getId();
        EventDateTime start = new EventDateTime()
                .setDateTime(new com.google.api.client.util.DateTime(
                    Date.from(startTime.atZone(ZoneId.of(timeZone)).toInstant())))
                .setTimeZone(timeZone);
        event.setStart(start);

        EventDateTime end = new EventDateTime()
                .setDateTime(new com.google.api.client.util.DateTime(
                    Date.from(endTime.atZone(ZoneId.of(timeZone)).toInstant())))
                .setTimeZone(timeZone);
        event.setEnd(end);

        EventAttendee attendee = new EventAttendee()
                .setEmail(recipientEmail);
        event.setAttendees(Arrays.asList(attendee));
        event.setGuestsCanSeeOtherGuests(false);

        String calendarId = "c_1ac8b9279de61bc3a9f7947258ab98939c2961f4bf545530e72101eef328cc12@group.calendar.google.com";
        Event createdEvent = calendarService.events()
                .insert(calendarId, event)
                .setSendNotifications(true)
                .execute();

    }

    private Calendar getCalendarService() throws IOException, GeneralSecurityException {
        HttpCredentialsAdapter credentials = credentialsService.getCredentials();
        
        return new Calendar.Builder(
                GoogleNetHttpTransport.newTrustedTransport(),
                GsonFactory.getDefaultInstance(),
                credentials)
                .setApplicationName(APPLICATION_NAME)
                .build();
    }
}