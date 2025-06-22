package com.keepitup.calendar.api.Calendar.API.timeentrymember.controller.impl;

import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwt;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.controller.api.TimeEntryMemberController;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.GetTimeEntryMemberResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.GetTimeEntryMembersResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.PatchTimeEntryMemberRequest;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.PostTimeEntryMemberRequest;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.function.RequestToTimeEntryMemberFunction;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.function.TimeEntryMemberToResponseFunction;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.function.TimeEntryMembersToResponseFunction;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.function.UpdateTimeEntryMemberWithRequestFunction;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.service.api.TimeEntryMemberService;
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
public class TimeEntryMemberDefaultController implements TimeEntryMemberController {
    private final TimeEntryMemberService service;
    private final TimeEntryMembersToResponseFunction timeEntrysToResponse;
    private final TimeEntryMemberToResponseFunction timeEntryToResponse;
    private final RequestToTimeEntryMemberFunction requestToTimeEntry;
    private final UpdateTimeEntryMemberWithRequestFunction updateTimeEntryWithRequest;

    @Autowired
    public TimeEntryMemberDefaultController(
            TimeEntryMemberService service,
            TimeEntryMembersToResponseFunction timeEntrysToResponse,
            TimeEntryMemberToResponseFunction timeEntryToResponse,
            RequestToTimeEntryMemberFunction requestToTimeEntry,
            UpdateTimeEntryMemberWithRequestFunction updateTimeEntryWithRequest
    ) {
        this.service = service;
        this.timeEntrysToResponse = timeEntrysToResponse;
        this.timeEntryToResponse = timeEntryToResponse;
        this.requestToTimeEntry = requestToTimeEntry;
        this.updateTimeEntryWithRequest = updateTimeEntryWithRequest;
    }

    @Override
    public GetTimeEntryMembersResponse getTimeEntryMembers(int page, int size, boolean ascending, String sortField) {
        Sort sort = ascending ? Sort.by(sortField).ascending() : Sort.by(sortField).descending();
        PageRequest pageRequest = PageRequest.of(page, size, sort);
        Integer count = service.findAll().size();
        return timeEntrysToResponse.apply(service.findAll(pageRequest), count);
    }


    @Override
    public GetTimeEntryMemberResponse createTimeEntryMembers(PostTimeEntryMemberRequest postTimeEntryMemberRequest) {
        UUID id = UUID.randomUUID();
        postTimeEntryMemberRequest.setId(id);
        service.create(requestToTimeEntry.apply(postTimeEntryMemberRequest));
        return service.find(id)
                .map(timeEntryToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.CONFLICT));
    }

    @Override
    public GetTimeEntryMemberResponse getTimeEntryMember(UUID id) {
        return service.find(id)
                .map(timeEntryToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public void deleteTimeEntryMember(UUID id) {
        Optional<TimeEntryMember> timeEntryTemplate = service.find(id);

        if (timeEntryTemplate.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }
        service.delete(id);
    }

    @Override
    public GetTimeEntryMembersResponse getTimeEntryMembersByUser(int page, int size, UUID userId) {
        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
        UUID loggedInUserId = UUID.fromString(jwt.getExternalId());

        if (!loggedInUserId.equals(userId)) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Optional<Page<TimeEntryMember>> countOptional = service.findAllTimeEntryMembersByUser(userId, pageRequest);
        Integer count = countOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND)).getNumberOfElements();

        Optional<Page<TimeEntryMember>> timeEntrysOptional = service.findAllTimeEntryMembersByUser(userId, pageRequest);

        Page<TimeEntryMember> timeEntrys = timeEntrysOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        return timeEntrysToResponse.apply(timeEntrys, count);
    }

    @Override
    public GetTimeEntryMemberResponse updateTimeEntryMember(UUID id, PatchTimeEntryMemberRequest patchTimeEntryMemberRequest) {
        Optional<TimeEntryMember> timeEntry = service.find(id);

        if (timeEntry.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }

        service.update(updateTimeEntryWithRequest.apply(timeEntry.get(), patchTimeEntryMemberRequest));
        return getTimeEntryMember(id);
    }
}
