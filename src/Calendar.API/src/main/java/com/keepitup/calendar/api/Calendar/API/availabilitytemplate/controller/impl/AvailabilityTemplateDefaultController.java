package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.controller.impl;

import com.keepitup.calendar.api.Calendar.API.Graphic.function.GraphicToResponseFunction;
import com.keepitup.calendar.api.Calendar.API.Graphic.service.api.GraphicService;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.controller.api.AvailabilityTemplateController;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.*;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.function.*;
import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwt;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.service.api.AvailabilityTemplateService;
import com.keepitup.calendar.api.Calendar.API.timeentry.service.api.TimeEntryService;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.service.api.TimeEntryTemplateService;
import com.keepitup.calendar.api.Calendar.API.services.google.GoogleCalendarInviteService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Sort;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

import java.time.LocalDateTime;
import java.util.*;

@RestController
@Log
public class AvailabilityTemplateDefaultController implements AvailabilityTemplateController {
    private final AvailabilityTemplateService service;
    private final TimeEntryTemplateService timeEntryTemplateService;
    private final TimeEntryService timeEntryService;
    private final GraphicService graphicService;
    private final GoogleCalendarInviteService googleCalendarInviteService;
    private final AvailabilityTemplatesToResponseFunction availabilityTemplatesToResponse;
    private final AvailabilityTemplateToResponseFunction availabilityTemplateToResponse;
    private final RequestToAvailabilityTemplateFunction requestToAvailabilityTemplate;
    private final UpdateAvailabilityTemplateWithRequestFunction updateAvailabilityTemplateWithRequest;
    private final GraphicToResponseFunction graphicToResponseFunction;

    @Autowired
    public AvailabilityTemplateDefaultController(
            AvailabilityTemplateService service,
            TimeEntryTemplateService timeEntryTemplateService,
            AvailabilityTemplatesToResponseFunction availabilityTemplatesToResponse,
            AvailabilityTemplateToResponseFunction availabilityTemplateToResponse,
            RequestToAvailabilityTemplateFunction requestToAvailabilityTemplate,
            UpdateAvailabilityTemplateWithRequestFunction updateAvailabilityTemplateWithRequest,
            TimeEntryService timeEntryService,
            GraphicService graphicService,
            GraphicToResponseFunction graphicToResponseFunction,
            GoogleCalendarInviteService googleCalendarInviteService
    ) {
        this.service = service;
        this.timeEntryService = timeEntryService;
        this.timeEntryTemplateService = timeEntryTemplateService;
        this.availabilityTemplatesToResponse = availabilityTemplatesToResponse;
        this.availabilityTemplateToResponse = availabilityTemplateToResponse;
        this.requestToAvailabilityTemplate = requestToAvailabilityTemplate;
        this.updateAvailabilityTemplateWithRequest = updateAvailabilityTemplateWithRequest;
        this.graphicService = graphicService;
        this.graphicToResponseFunction = graphicToResponseFunction;
        this.googleCalendarInviteService = googleCalendarInviteService;
    }

    @Override
    public GetAvailabilityTemplatesResponse getAvailabilityTemplates(int page, int size, boolean ascending, String sortField) {
        Sort sort = ascending ? Sort.by(sortField).ascending() : Sort.by(sortField).descending();
        PageRequest pageRequest = PageRequest.of(page, size, sort);
        Integer count = service.findAll().size();
        return availabilityTemplatesToResponse.apply(service.findAll(pageRequest), count);
    }


    @Override
    public GetAvailabilityTemplateResponse createAvailabilityTemplates(PostAvailabilityTemplateRequest postAvailabilityTemplateRequest) {
        AvailabilityTemplate availabilityTemplate = requestToAvailabilityTemplate.apply(postAvailabilityTemplateRequest);
        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
        UUID loggedInUserId = UUID.fromString(jwt.getExternalId());
        availabilityTemplate.setUserId(loggedInUserId);
        AvailabilityTemplate availabilityTemplateCreated = service.create(availabilityTemplate);

        for(TimeEntryTemplate timeEntryTemplate: postAvailabilityTemplateRequest.getTimeEntryTemplates()){
              timeEntryTemplate.setAvailabilityTemplate(availabilityTemplateCreated);
              timeEntryTemplateService.update(timeEntryTemplate);
              
              // Send email notification for each time entry
              sendEmailNotification(timeEntryTemplate, availabilityTemplateCreated);
        }

        return availabilityTemplateToResponse.apply(availabilityTemplateCreated);
    }

    private void sendEmailNotification(TimeEntryTemplate timeEntryTemplate, AvailabilityTemplate availabilityTemplate, ) {
        try {
            // Convert LocalTime to LocalDateTime by combining with current date
            LocalDateTime startTime = timeEntryTemplate.getStartTime().atDate(LocalDateTime.now().toLocalDate());
            LocalDateTime endTime = timeEntryTemplate.getEndTime().atDate(LocalDateTime.now().toLocalDate());
            
            String eventTitle = "New Availability Template Created - " + availabilityTemplate.getName();
            String description = String.format(
                "A new availability template has been created.\n\n" +
                "Template Name: %s\n" +
                "Time Entry: %s - %s\n" +
                "Created by User ID: %s",
                availabilityTemplate.getName() != null ? availabilityTemplate.getName() : "N/A",
                timeEntryTemplate.getStartTime() != null ? timeEntryTemplate.getStartTime() : "N/A",
                timeEntryTemplate.getEndTime() != null ? timeEntryTemplate.getEndTime() : "N/A",
                availabilityTemplate.getUserId() != null ? availabilityTemplate.getUserId() : "N/A"
            );

            // Send calendar invite
            googleCalendarInviteService.sendCalendarInvite(
                "filips750@gmail.com",
                eventTitle, 
                description,
                startTime,
                endTime
            );

            log.info("Test email notification sent successfully for availability template: " + availabilityTemplate.getName());

        } catch (Exception e) {
            log.severe("Failed to send test email notification: " + e.getMessage());
            // Don't throw exception to avoid disrupting the main flow
        }
    }

    @Override
    public GetAvailabilityTemplateResponse getAvailabilityTemplate(UUID id) {
        return service.find(id)
                .map(availabilityTemplateToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public void deleteAvailabilityTemplate(UUID id) {
        Optional<AvailabilityTemplate> availabilityTemplate = service.find(id);

        if (availabilityTemplate.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }
        service.delete(id);
    }

    @Override
    public GetAvailabilityTemplatesResponse getAvailabilityTemplatesByUser(int page, int size, UUID userId) {
        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
        UUID loggedInUserId = UUID.fromString(jwt.getExternalId());

        if (!loggedInUserId.equals(userId)) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Optional<Page<AvailabilityTemplate>> countOptional = service.findAllAvailabilityTemplatesByUser(userId, pageRequest);
        Integer count = countOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND)).getNumberOfElements();

        Optional<Page<AvailabilityTemplate>> availabilityTemplatesOptional = service.findAllAvailabilityTemplatesByUser(userId, pageRequest);

        Page<AvailabilityTemplate> availabilityTemplates = availabilityTemplatesOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        return availabilityTemplatesToResponse.apply(availabilityTemplates, count);
    }

    @Override
    public GetAvailabilityTemplateResponse updateAvailabilityTemplate(UUID id, PatchAvailabilityTemplateRequest patchAvailabilityTemplateRequest) {
        Optional<AvailabilityTemplate> availabilityTemplate = service.find(id);

        if (availabilityTemplate.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }

        service.update(updateAvailabilityTemplateWithRequest.apply(availabilityTemplate.get(), patchAvailabilityTemplateRequest));
        return getAvailabilityTemplate(id);
    }
}
