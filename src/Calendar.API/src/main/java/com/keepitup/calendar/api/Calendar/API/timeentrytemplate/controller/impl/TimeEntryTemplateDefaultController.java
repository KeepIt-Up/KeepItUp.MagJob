package com.keepitup.calendar.api.Calendar.API.timeentrytemplate.controller.impl;

import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwt;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.controller.api.TimeEntryTemplateController;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto.GetTimeEntryTemplateResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto.GetTimeEntryTemplatesResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto.PatchTimeEntryTemplateRequest;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto.PostTimeEntryTemplateRequest;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.function.*;
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
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.function.UpdateTimeEntryTemplateWithRequestFunction;
import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;

@RestController
@Log
public class TimeEntryTemplateDefaultController implements TimeEntryTemplateController {
    private final TimeEntryTemplateService service;
    private final TimeEntryTemplatesToResponseFunction timeEntrysToResponse;
    private final TimeEntryTemplateToResponseFunction timeEntryToResponse;
    private final RequestToTimeEntryTemplateFunction requestToTimeEntry;
    private final UpdateTimeEntryTemplateWithRequestFunction updateTimeEntryWithRequest;

    @Autowired
    public TimeEntryTemplateDefaultController(
            TimeEntryTemplateService service,
            TimeEntryTemplatesToResponseFunction timeEntrysToResponse,
            TimeEntryTemplateToResponseFunction timeEntryToResponse,
            RequestToTimeEntryTemplateFunction requestToTimeEntry,
            UpdateTimeEntryTemplateWithRequestFunction updateTimeEntryWithRequest
    ) {
        this.service = service;
        this.timeEntrysToResponse = timeEntrysToResponse;
        this.timeEntryToResponse = timeEntryToResponse;
        this.requestToTimeEntry = requestToTimeEntry;
        this.updateTimeEntryWithRequest = updateTimeEntryWithRequest;
    }

    @Override
    public GetTimeEntryTemplatesResponse getTimeEntryTemplates(int page, int size, boolean ascending, String sortField) {
        Sort sort = ascending ? Sort.by(sortField).ascending() : Sort.by(sortField).descending();
        PageRequest pageRequest = PageRequest.of(page, size, sort);
        Integer count = service.findAll().size();
        return timeEntrysToResponse.apply(service.findAll(pageRequest), count);
    }


    @Override
    public GetTimeEntryTemplateResponse createTimeEntryTemplates(PostTimeEntryTemplateRequest postTimeEntryTemplateRequest) {
        UUID id = UUID.randomUUID();
        postTimeEntryTemplateRequest.setId(id);
        service.create(requestToTimeEntry.apply(postTimeEntryTemplateRequest));
        return service.find(id)
                .map(timeEntryToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.CONFLICT));
    }

    @Override
    public GetTimeEntryTemplateResponse getTimeEntryTemplate(UUID id) {
        return service.find(id)
                .map(timeEntryToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public void deleteTimeEntryTemplate(UUID id) {
        Optional<TimeEntryTemplate> timeEntryTemplate = service.find(id);

        if (timeEntryTemplate.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }
        service.delete(id);
    }

    @Override
    public GetTimeEntryTemplatesResponse getTimeEntryTemplatesByUser(int page, int size, UUID userId) {
        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
        UUID loggedInUserId = UUID.fromString(jwt.getExternalId());

        if (!loggedInUserId.equals(userId)) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Optional<Page<TimeEntryTemplate>> countOptional = service.findAllTimeEntryTemplatesByUser(userId, pageRequest);
        Integer count = countOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND)).getNumberOfElements();

        Optional<Page<TimeEntryTemplate>> timeEntrysOptional = service.findAllTimeEntryTemplatesByUser(userId, pageRequest);

        Page<TimeEntryTemplate> timeEntrys = timeEntrysOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        return timeEntrysToResponse.apply(timeEntrys, count);
    }

    @Override
    public GetTimeEntryTemplateResponse updateTimeEntryTemplate(UUID id, PatchTimeEntryTemplateRequest patchTimeEntryTemplateRequest) {
        Optional<TimeEntryTemplate> timeEntry = service.find(id);

        if (timeEntry.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }

        service.update(updateTimeEntryWithRequest.apply(timeEntry.get(), patchTimeEntryTemplateRequest));
        return getTimeEntryTemplate(id);
    }
}
