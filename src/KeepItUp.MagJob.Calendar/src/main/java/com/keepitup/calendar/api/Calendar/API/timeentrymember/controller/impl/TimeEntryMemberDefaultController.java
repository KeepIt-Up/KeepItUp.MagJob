package com.keepitup.calendar.api.Calendar.API.timeentrymember.controller.impl;

import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwt;
import com.keepitup.calendar.api.Calendar.API.services.google.GoogleCalendarInviteService;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.controller.api.TimeEntryMemberController;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.GetTimeEntryMemberResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.GetTimeEntryMembersResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.PatchTimeEntryMemberRequest;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.PostTimeEntryMemberRequest;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.PostTimeEntryMembersBulkRequest;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.function.RequestToTimeEntryMemberFunction;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.function.TimeEntryMemberToResponseFunction;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.function.TimeEntryMembersToResponseFunction;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.function.UpdateTimeEntryMemberWithRequestFunction;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.service.api.TimeEntryMemberService;
import com.keepitup.calendar.api.Calendar.API.user.service.UserServiceClient;
import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import com.keepitup.calendar.api.Calendar.API.timeentry.service.api.TimeEntryService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageImpl;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Sort;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

import java.io.Console;
import java.util.List;
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
    private final GoogleCalendarInviteService googleCalendarInviteService;
    private final UserServiceClient memberServiceClient;
    private final TimeEntryService timeEntryService;

    @Autowired
    public TimeEntryMemberDefaultController(
            TimeEntryMemberService service,
            TimeEntryMembersToResponseFunction timeEntrysToResponse,
            TimeEntryMemberToResponseFunction timeEntryToResponse,
            RequestToTimeEntryMemberFunction requestToTimeEntry,
            UpdateTimeEntryMemberWithRequestFunction updateTimeEntryWithRequest,
            GoogleCalendarInviteService googleCalendarInviteService,
            UserServiceClient memberServiceClient,
            TimeEntryService timeEntryService
    ) {
        this.service = service;
        this.timeEntrysToResponse = timeEntrysToResponse;
        this.timeEntryToResponse = timeEntryToResponse;
        this.requestToTimeEntry = requestToTimeEntry;
        this.updateTimeEntryWithRequest = updateTimeEntryWithRequest;
        this.googleCalendarInviteService = googleCalendarInviteService;
        this.memberServiceClient = memberServiceClient;
        this.timeEntryService = timeEntryService;
    }

    @Override
    public GetTimeEntryMembersResponse getTimeEntryMembers(int page, int size, boolean ascending, String sortField) {
        Sort sort = ascending ? Sort.by(sortField).ascending() : Sort.by(sortField).descending();
        PageRequest pageRequest = PageRequest.of(page, size, sort);
        Integer count = service.findAll().size();
        return timeEntrysToResponse.apply(service.findAll(pageRequest), count);
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
    public GetTimeEntryMembersResponse getUpcomingConfirmedTimeEntryMembers(int minutesBefore, int page, int size) {
        PageRequest pageRequest = PageRequest.of(page, size);
        Page<TimeEntryMember> members = service.findUpcomingConfirmedMembers(minutesBefore, pageRequest);
        return timeEntrysToResponse.apply(members, (int) members.getTotalElements());
    }

    @Override
    public GetTimeEntryMembersResponse getTimeEntryMembersByUser(int page, int size, UUID userId) {
        // var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
        // UUID loggedInUserId = UUID.fromString(jwt.getExternalId());

        // if (!loggedInUserId.equals(userId)) {
        //     throw new ResponseStatusException(HttpStatus.FORBIDDEN);
        // }

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
        
        // Fetch the updated member
        TimeEntryMember updatedMember = service.find(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
        
        // Send email notification after update
        if (updatedMember.getTimeEntry() != null && updatedMember.getId() != null) {
            try {
                if ("Confirmed".equals(updatedMember.getStatus())) {
                    UUID timeEntryId = updatedMember.getTimeEntry().getId();
                    TimeEntry fullTimeEntry = timeEntryService.find(timeEntryId)
                            .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
                    
                    String userEmail = getUserEmail(updatedMember.getId());
                    sendEmailNotification(updatedMember, fullTimeEntry, userEmail);
                }
            } catch (Exception e) {
                log.warning("Failed to send notification for member: " + updatedMember.getMemberId() + ", error: " + e.getMessage());
            }
        }
        
        return timeEntryToResponse.apply(updatedMember);
    }

    @Override
    public GetTimeEntryMembersResponse createTimeEntryMembersBulk(PostTimeEntryMembersBulkRequest postTimeEntryMembersBulkRequest) {
        List<TimeEntryMember> createdMembers = service.createBulk(postTimeEntryMembersBulkRequest);
        
        // Send email notifications for bulk creation
        createdMembers.forEach(member -> {
            if (member.getTimeEntry() != null && member.getMemberId() != null) {
                try {
                    if ("Confirmed".equals(member.getStatus())) {
                        UUID timeEntryId = member.getTimeEntry().getId();
                        TimeEntry timeEntry = timeEntryService.find(timeEntryId)
                                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
                        
                        String userEmail = getUserEmail(member.getMemberId());
                        sendEmailNotification(member, timeEntry, userEmail);
                    }
                } catch (Exception e) {
                    log.warning("Failed to send notification for member: " + member.getMemberId() + ", error: " + e.getMessage());
                }
            }
        });
        
        return timeEntrysToResponse.apply(new PageImpl<>(createdMembers), createdMembers.size());
    }

    /**
     * Fetches user email from Identity service based on Member
     */
    private String getUserEmail(UUID id) {
        return memberServiceClient.getUserEmail(id);
    }

    @Override
    public GetTimeEntryMembersResponse getTimeEntryMembersByGraphic(int page, int size, UUID graphicId) {
        PageRequest pageRequest = PageRequest.of(page, size);
        
        Page<TimeEntryMember> timeEntryMembers = service.findByGraphicId(graphicId, pageRequest);
        
        Integer count = (int) timeEntryMembers.getTotalElements();
        
        return timeEntrysToResponse.apply(timeEntryMembers, count);
    }

    private void sendEmailNotification(TimeEntryMember timeEntryMember, TimeEntry timeEntry, String memberEmail) {
        try {
            String eventTitle = "Work";
            String description = String.format(
                "You have been added to a time entry.\n\n" +
                "Event: %s\n" +
                "Start Time: %s\n" +
                "End Time: %s\n" +
                "Status: %s",
                eventTitle,
                timeEntry.getStartDateTime() != null ? timeEntry.getStartDateTime() : "N/A",
                timeEntry.getEndDateTime() != null ? timeEntry.getEndDateTime() : "N/A",
                timeEntryMember.getStatus() != null ? timeEntryMember.getStatus() : "N/A"
            );

            googleCalendarInviteService.sendCalendarInvite(
                memberEmail,
                eventTitle,
                description,
                timeEntry.getStartDateTime(),
                timeEntry.getEndDateTime()
            );

            log.info("Calendar invite sent successfully to: " + memberEmail + " for time entry: " + timeEntry.getId());

        } catch (Exception e) {
            log.severe("Failed to send calendar invite: " + e.getMessage());
        }
    }
}
