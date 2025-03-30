package com.keepitup.calendar.api.Calendar.API.Graphic.controller.impl;

import com.keepitup.calendar.api.Calendar.API.Graphic.controller.api.GraphicController;
import com.keepitup.calendar.api.Calendar.API.Graphic.dto.*;
import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import com.keepitup.calendar.api.Calendar.API.Graphic.function.*;
import com.keepitup.calendar.api.Calendar.API.Graphic.service.api.GraphicService;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.service.api.AvailabilityTemplateService;
import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwt;
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

import java.time.LocalDateTime;
import java.util.*;

@RestController
@Log
public class GraphicDefaultController implements GraphicController {
    private final GraphicService service;
    private final GraphicsToResponseFunction GraphicsToResponse;
    private final GraphicToResponseFunction graphicToResponseFunction;
    private final RequestToGraphicFunction requestToGraphic;
    private final UpdateGraphicWithRequestFunction updateGraphicWithRequest;
    private final TimeEntryTemplateService timeEntryTemplateService;
    private final TimeEntryService timeEntryService;
    private final PostCreateAndPopulateGraphicToResponseFunction postCreateAndPopulateGraphicToResponseFunction;
    private final AvailabilityTemplateService availabilityTemplateService;

    @Autowired
    public GraphicDefaultController(
            GraphicService service,
            GraphicsToResponseFunction GraphicsToResponse,
            GraphicToResponseFunction graphicToResponseFunction,
            RequestToGraphicFunction requestToGraphic,
            UpdateGraphicWithRequestFunction updateGraphicWithRequest,
            TimeEntryTemplateService timeEntryTemplateService,
            TimeEntryService timeEntryService,
            PostCreateAndPopulateGraphicToResponseFunction postCreateAndPopulateGraphicToResponseFunction,
            AvailabilityTemplateService availabilityTemplateService
    ) {
        this.service = service;
        this.GraphicsToResponse = GraphicsToResponse;
        this.graphicToResponseFunction = graphicToResponseFunction;
        this.requestToGraphic = requestToGraphic;
        this.updateGraphicWithRequest = updateGraphicWithRequest;
        this.timeEntryTemplateService = timeEntryTemplateService;
        this.timeEntryService = timeEntryService;
        this.postCreateAndPopulateGraphicToResponseFunction = postCreateAndPopulateGraphicToResponseFunction;
        this.availabilityTemplateService = availabilityTemplateService;
    }

    @Override
    public GetGraphicsResponse getGraphics(int page, int size, boolean ascending, String sortField) {
        Sort sort = ascending ? Sort.by(sortField).ascending() : Sort.by(sortField).descending();
        PageRequest pageRequest = PageRequest.of(page, size, sort);
        Integer count = service.findAll().size();
        return GraphicsToResponse.apply(service.findAll(pageRequest), count);
    }


    @Override
    public GetGraphicResponse createGraphics(PostGraphicRequest postGraphicRequest) {
        UUID id = UUID.randomUUID();
        service.create(requestToGraphic.apply(postGraphicRequest));
        return service.find(id)
                .map(graphicToResponseFunction)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.CONFLICT));
    }

    @Override
    public GetGraphicResponse getGraphic(UUID id) {
        return service.find(id)
                .map(graphicToResponseFunction)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public void deleteGraphic(UUID id) {
        Optional<Graphic> availabilityTemplateTemplate = service.find(id);

        if (availabilityTemplateTemplate.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }
        service.delete(id);
    }

    @Override
    public GetGraphicsResponse getGraphicsByUser(int page, int size, UUID userId) {
        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
        UUID loggedInUserId = UUID.fromString(jwt.getExternalId());

        if (!loggedInUserId.equals(userId)) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Optional<Page<Graphic>> countOptional = service.findAllGraphicsByUser(userId, pageRequest);
        Integer count = countOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND)).getNumberOfElements();

        Optional<Page<Graphic>> GraphicsOptional = service.findAllGraphicsByUser(userId, pageRequest);

        Page<Graphic> Graphics = GraphicsOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        return GraphicsToResponse.apply(Graphics, count);
    }

    @Override
    public GetGraphicResponse updateGraphic(UUID id, PatchGraphicRequest patchGraphicRequest) {
        Optional<Graphic> availabilityTemplate = service.find(id);

        if (availabilityTemplate.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }

        service.update(updateGraphicWithRequest.apply(availabilityTemplate.get(), patchGraphicRequest));
        return getGraphic(id);
    }

    @Override
    public GetGraphicResponse createAndPopulateGraphic(PostCreateAndPopulateGraphic graphicPost) {
        AvailabilityTemplate avt = availabilityTemplateService.find(graphicPost.getAvailabilityTemplateId()).
          orElseThrow(() -> new RuntimeException("Availability Template doesn't exist")
          );
//        avt.setTimeEntryTemplates(null);
        System.out.println(avt);
        List<TimeEntryTemplate> timeEntryTemplates = timeEntryTemplateService
            .findAllTimeEntryTemplatesByAvailabilityTemplate(avt)
            .orElse(Collections.emptyList());
        System.out.println("TETA: " + timeEntryTemplates);
        List<TimeEntry> timeEntries = new ArrayList<>();
        if (!timeEntryTemplates.isEmpty()) {
            for (TimeEntryTemplate template : timeEntryTemplates) {
                LocalDateTime startDateTime = graphicPost.getStartDate()
                  .plusDays(template.getStartDayOffset())
                  .atTime(template.getStartTime());

                LocalDateTime endDateTime =  graphicPost.getStartDate()
                  .plusDays(template.getEndDayOffset())
                  .atTime(template.getEndTime());

                TimeEntry timeEntry = TimeEntry.builder()
                  .startDateTime(startDateTime)
                  .endDateTime(endDateTime)
                  .build();

                System.out.println("TE:" +timeEntry.getStartDateTime());
//                timeEntryService.create(timeEntry);
                timeEntries.add(timeEntry);
            }
        }

        Graphic graphic = postCreateAndPopulateGraphicToResponseFunction.apply(graphicPost);
        graphic.setTimeEntries(timeEntries);
        Graphic createdGraphic = service.create(graphic);
        System.out.println(timeEntries);
        return graphicToResponseFunction.apply(
            service.find(createdGraphic.getId())
               .orElseThrow(() -> new RuntimeException("Graphic not found with ID: " + createdGraphic.getId()))
        );
    }
}
