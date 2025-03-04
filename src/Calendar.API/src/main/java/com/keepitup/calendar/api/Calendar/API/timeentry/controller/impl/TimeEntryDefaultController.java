package com.keepitup.calendar.api.Calendar.API.timeentry.controller.impl;

import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwt;
import com.keepitup.calendar.api.Calendar.API.timeentry.controller.api.TimeEntryController;
import com.keepitup.calendar.api.Calendar.API.timeentry.dto.GetTimeEntryResponse;
import com.keepitup.calendar.api.Calendar.API.timeentry.dto.GetTimeEntrysResponse;
import com.keepitup.calendar.api.Calendar.API.timeentry.dto.PatchTimeEntryRequest;
import com.keepitup.calendar.api.Calendar.API.timeentry.dto.PostTimeEntryRequest;
import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import com.keepitup.calendar.api.Calendar.API.timeentry.function.RequestToTimeEntryFunction;
import com.keepitup.calendar.api.Calendar.API.timeentry.function.TimeEntryToResponseFunction;
import com.keepitup.calendar.api.Calendar.API.timeentry.function.TimeEntrysToResponseFunction;
import com.keepitup.calendar.api.Calendar.API.timeentry.function.UpdateTimeEntryWithRequestFunction;
import com.keepitup.calendar.api.Calendar.API.timeentry.service.api.TimeEntryService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Sort;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

import java.util.Optional;
import java.util.UUID;

@RestController
@Log
public class TimeEntryDefaultController implements TimeEntryController {
    private final TimeEntryService service;
    private final TimeEntrysToResponseFunction timeEntrysToResponse;
    private final TimeEntryToResponseFunction timeEntryToResponse;
    private final RequestToTimeEntryFunction requestToTimeEntry;
    private final UpdateTimeEntryWithRequestFunction updateTimeEntryWithRequest;

    @Autowired
    public TimeEntryDefaultController(
            TimeEntryService service,
            TimeEntrysToResponseFunction timeEntrysToResponse,
            TimeEntryToResponseFunction timeEntryToResponse,
            RequestToTimeEntryFunction requestToTimeEntry,
            UpdateTimeEntryWithRequestFunction updateTimeEntryWithRequest
    ) {
        this.service = service;
        this.timeEntrysToResponse = timeEntrysToResponse;
        this.timeEntryToResponse = timeEntryToResponse;
        this.requestToTimeEntry = requestToTimeEntry;
        this.updateTimeEntryWithRequest = updateTimeEntryWithRequest;
    }

    @Override
    public GetTimeEntrysResponse getTimeEntrys(int page, int size, boolean ascending, String sortField) {
        Sort sort = ascending ? Sort.by(sortField).ascending() : Sort.by(sortField).descending();
        PageRequest pageRequest = PageRequest.of(page, size, sort);
        Integer count = service.findAll().size();
        return timeEntrysToResponse.apply(service.findAll(pageRequest), count);
    }


    @Override
    public GetTimeEntryResponse createTimeEntrys(PostTimeEntryRequest postTimeEntryRequest) {
        UUID id = UUID.randomUUID();
        postTimeEntryRequest.setId(id);
        service.create(requestToTimeEntry.apply(postTimeEntryRequest));
        return service.find(id)
                .map(timeEntryToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.CONFLICT));
    }

    @Override
    public GetTimeEntryResponse getTimeEntry(UUID id) {
        return service.find(id)
                .map(timeEntryToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public void deleteTimeEntry(UUID id) {
        Optional<TimeEntry> timeEntryTemplate = service.find(id);

        if (timeEntryTemplate.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }
        service.delete(id);
    }

    @Override
    public GetTimeEntrysResponse getTimeEntrysByUser(int page, int size, UUID userId) {
        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
        UUID loggedInUserId = UUID.fromString(jwt.getExternalId());

        if (!loggedInUserId.equals(userId)) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Optional<Page<TimeEntry>> countOptional = service.findAllTimeEntrysByUser(userId, pageRequest);
        Integer count = countOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND)).getNumberOfElements();

        Optional<Page<TimeEntry>> timeEntrysOptional = service.findAllTimeEntrysByUser(userId, pageRequest);

        Page<TimeEntry> timeEntrys = timeEntrysOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        return timeEntrysToResponse.apply(timeEntrys, count);
    }

    @Override
    public GetTimeEntryResponse updateTimeEntry(UUID id, PatchTimeEntryRequest patchTimeEntryRequest) {
        Optional<TimeEntry> timeEntry = service.find(id);

        if (timeEntry.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }

        service.update(updateTimeEntryWithRequest.apply(timeEntry.get(), patchTimeEntryRequest));
        return getTimeEntry(id);
    }
}
