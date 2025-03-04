package com.keepitup.calendar.api.Calendar.API.timeentrymember.controller.api;

import com.keepitup.calendar.api.Calendar.API.configuration.PageConfig;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.GetTimeEntryMemberResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.GetTimeEntryMembersResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.PatchTimeEntryMemberRequest;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.PostTimeEntryMemberRequest;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@Tag(name="TimeEntrys Controller")
public interface TimeEntryMemberController {
    PageConfig pageConfig = new PageConfig();

    @Operation(summary = "Get all Time Entry")
    @PostMapping("api/gettimeentrymember.s")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetTimeEntryMembersResponse getTimeEntryMembers(
            @Parameter(
                    name = "page number",
                    description = "Page number to retrieve"
            )
            @RequestParam(defaultValue = "#{pageConfig.number}")
            int page,
            @Parameter(
                    name = "page size",
                    description = "Number of records per page"
            )
            @RequestParam(defaultValue = "#{pageConfig.size}")
            int size,
            @Parameter(
                    name = "ascending",
                    description = "Is ascending"
            )
            @RequestParam(defaultValue = "true")
            boolean ascending,
            @Parameter(
                    name = "sortField",
                    description = "Field to sort by"
            )
            @RequestParam(defaultValue = "id")
            String sortField
    );

    @Operation(summary = "Get TimeEntrys")
    @GetMapping("api/timeentrymember.s/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetTimeEntryMemberResponse getTimeEntryMember(
            @Parameter(
                    name = "id",
                    description = "TimeEntrys id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Create TimeEntrys")
    @PostMapping("api/timeentrymember.s")
    @ResponseStatus(HttpStatus.CREATED)
    @ResponseBody
    GetTimeEntryMemberResponse createTimeEntryMembers(
            @Parameter(
                    name = "PostTimeEntryMembersRequest",
                    description = "PostTimeEntrysRequest DTO",
                    schema = @Schema(implementation = PostTimeEntryMemberRequest.class),
                    required = true
            )
            @RequestBody
            PostTimeEntryMemberRequest postTimeEntryMemberRequest
    );

    @Operation(summary = "Update TimeEntrys")
    @PatchMapping("api/timeentrymember.s/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetTimeEntryMemberResponse updateTimeEntryMember(
            @Parameter(
                    name = "id",
                    description = "TimeEntrys id value",
                    required = true
            )
            @PathVariable("id")
            UUID id,
            @Parameter(
                    name = "PatchTimeEntrysRequest",
                    description = "PatchTimeEntrysRequest DTO",
                    schema = @Schema(implementation = PatchTimeEntryMemberRequest.class),
                    required = true
            )
            @RequestBody
            PatchTimeEntryMemberRequest patchTimeEntryMemberRequest
    );

    @Operation(summary = "Delete TimeEntrys")
    @DeleteMapping("/api/timeentrymember.s/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    void deleteTimeEntryMember(
            @Parameter(
                    name = "id",
                    description = "TimeEntrys id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Get TimeEntryMembers by User")
    @PostMapping("api/timeentrymember.s/{userId}")
    @ResponseStatus(HttpStatus.OK)
    GetTimeEntryMembersResponse getTimeEntryMembersByUser(
            @Parameter(
                    name = "page number",
                    description = "Page number to retrieve"
            )
            @RequestParam(defaultValue = "#{pageConfig.number}")
            int page,
            @Parameter(
                    name = "page size",
                    description = "Number of records per page"
            )
            @RequestParam(defaultValue = "#{pageConfig.size}")
            int size,
            @Parameter(
                    name = "userId",
                    description = "TimeEntrys userId value",
                    required = true
            )
            @PathVariable("userId")
            UUID userId
    );
}