package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.controller.impl;

import com.keepitup.calendar.api.Calendar.API.Graphic.dto.GetGraphicResponse;
import com.keepitup.calendar.api.Calendar.API.Graphic.dto.PostGraphicRequest;
import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import com.keepitup.calendar.api.Calendar.API.Graphic.function.GraphicToResponseFunction;
import com.keepitup.calendar.api.Calendar.API.Graphic.service.api.GraphicService;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.controller.api.AvailabilityTemplateController;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.*;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.function.*;
import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwt;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.service.api.AvailabilityTemplateService;
import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import com.keepitup.calendar.api.Calendar.API.timeentry.service.api.TimeEntryService;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.service.api.TimeEntryTemplateService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Sort;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigInteger;
import java.sql.Time;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.*;

@RestController
@Log
public class AvailabilityTemplateDefaultController implements AvailabilityTemplateController {
    private final AvailabilityTemplateService service;
    private final TimeEntryTemplateService timeEntryTemplateService;
    private final TimeEntryService timeEntryService;
    private final GraphicService graphicService;
    private final AvailabilityTemplatesToResponseFunction availabilityTemplatesToResponse;
    private final AvailabilityTemplateToResponseFunction availabilityTemplateToResponse;
    private final RequestToAvailabilityTemplateFunction requestToAvailabilityTemplate;
    private final UpdateAvailabilityTemplateWithRequestFunction updateAvailabilityTemplateWithRequest;
    private final PostCreateAndPopulateGraphic postCreateAndPopulateGraphic;
    private final PostCreateAndPopulateGraphicToResponseFunction postCreateAndPopulateGraphicToResponseFunction;
    private final GraphicToResponseFunction graphicToResponseFunction;

    @Autowired
    public AvailabilityTemplateDefaultController(
            AvailabilityTemplateService service,
            TimeEntryTemplateService timeEntryTemplateService,
            AvailabilityTemplatesToResponseFunction availabilityTemplatesToResponse,
            AvailabilityTemplateToResponseFunction availabilityTemplateToResponse,
            RequestToAvailabilityTemplateFunction requestToAvailabilityTemplate,
            UpdateAvailabilityTemplateWithRequestFunction updateAvailabilityTemplateWithRequest,
            PostCreateAndPopulateGraphic postCreateAndPopulateGraphic,
            PostCreateAndPopulateGraphicToResponseFunction postCreateAndPopulateGraphicToResponseFunction,
            TimeEntryService timeEntryService,
            GraphicService graphicService,
            GraphicToResponseFunction graphicToResponseFunction
    ) {
        this.service = service;
        this.timeEntryService = timeEntryService;
        this.timeEntryTemplateService = timeEntryTemplateService;
        this.availabilityTemplatesToResponse = availabilityTemplatesToResponse;
        this.availabilityTemplateToResponse = availabilityTemplateToResponse;
        this.requestToAvailabilityTemplate = requestToAvailabilityTemplate;
        this.updateAvailabilityTemplateWithRequest = updateAvailabilityTemplateWithRequest;
        this.postCreateAndPopulateGraphic = postCreateAndPopulateGraphic;
        this.postCreateAndPopulateGraphicToResponseFunction = postCreateAndPopulateGraphicToResponseFunction;
        this.graphicService = graphicService;
        this.graphicToResponseFunction = graphicToResponseFunction;
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
        UUID id = UUID.randomUUID();
        postAvailabilityTemplateRequest.setId(id);
        service.create(requestToAvailabilityTemplate.apply(postAvailabilityTemplateRequest));
        return service.find(id)
                .map(availabilityTemplateToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.CONFLICT));
    }

    @Override
    public GetAvailabilityTemplateResponse getAvailabilityTemplate(UUID id) {
        return service.find(id)
                .map(availabilityTemplateToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public void deleteAvailabilityTemplate(UUID id) {
        Optional<AvailabilityTemplate> availabilityTemplateTemplate = service.find(id);

        if (availabilityTemplateTemplate.isEmpty()) {
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
    public GetGraphicResponse createAndPopulateGraphic(UUID availabilityTemplateId, LocalDate startDate, PostGraphicRequest graphicPost) {
        List<TimeEntryTemplate> timeEntryTemplates = timeEntryTemplateService
                .findAllTimeEntryTemplatesByAvailabilityTemplateId(availabilityTemplateId)
                .orElse(Collections.emptyList());

        List<TimeEntry> timeEntries = new ArrayList<>();
        if (!timeEntryTemplates.isEmpty()) {
            for (TimeEntryTemplate template : timeEntryTemplates) {
                LocalDateTime startDateTime = startDate
                        .plusDays(template.getStartDayOffset())
                        .atTime(template.getStartTime());

                LocalDateTime endDateTime = startDate
                        .plusDays(template.getEndDayOffset())
                        .atTime(template.getEndTime());

                TimeEntry timeEntry = TimeEntry.builder()
                        .startDateTime(startDateTime)
                        .endDateTime(endDateTime)
                        .build();

                timeEntryService.create(timeEntry);
                timeEntries.add(timeEntry);
            }
        }
        Graphic newGraphic = new Graphic();
        Graphic graphic = postCreateAndPopulateGraphicToResponseFunction.apply(newGraphic, graphicPost);
        graphicService.create(graphic);
        return graphicToResponseFunction.apply(
                graphicService.find(graphic.getId())
                        .orElseThrow(() -> new RuntimeException("Graphic not found with ID: " + graphic.getId()))
        );
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
