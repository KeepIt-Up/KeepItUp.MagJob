package com.keepitup.calendar.api.Calendar.API.timeentry.controller.api;

import com.keepitup.calendar.api.Calendar.API.configuration.PageConfig;
import com.keepitup.calendar.api.Calendar.API.timeentry.dto.GetTimeEntryResponse;
import com.keepitup.calendar.api.Calendar.API.timeentry.dto.GetTimeEntrysResponse;
import com.keepitup.calendar.api.Calendar.API.timeentry.dto.PatchTimeEntryRequest;
import com.keepitup.calendar.api.Calendar.API.timeentry.dto.PostTimeEntryRequest;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@Tag(name="TimeEntrys Controller")
public interface TimeEntryController {
    PageConfig pageConfig = new PageConfig();

    @Operation(summary = "Get all Time Entrys")
    @PostMapping("api/gettimeentrys")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetTimeEntrysResponse getTimeEntrys(
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
    @GetMapping("api/timeentrys/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetTimeEntryResponse getTimeEntry(
            @Parameter(
                    name = "id",
                    description = "TimeEntrys id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Create TimeEntrys")
    @PostMapping("api/timeentrys")
    @ResponseStatus(HttpStatus.CREATED)
    @ResponseBody
    GetTimeEntryResponse createTimeEntrys(
            @Parameter(
                    name = "PostTimeEntrysRequest",
                    description = "PostTimeEntrysRequest DTO",
                    schema = @Schema(implementation = PostTimeEntryRequest.class),
                    required = true
            )
            @RequestBody
            PostTimeEntryRequest postTimeEntryRequest
    );

    @Operation(summary = "Update TimeEntrys")
    @PatchMapping("api/timeentrys/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetTimeEntryResponse updateTimeEntry(
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
                    schema = @Schema(implementation = PatchTimeEntryRequest.class),
                    required = true
            )
            @RequestBody
            PatchTimeEntryRequest patchTimeEntryRequest
    );

    @Operation(summary = "Delete TimeEntrys")
    @DeleteMapping("/api/timeentrys/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    void deleteTimeEntry(
            @Parameter(
                    name = "id",
                    description = "TimeEntrys id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Get TimeEntrys by User")
    @PostMapping("api/timeentrys/{userId}")
    @ResponseStatus(HttpStatus.OK)
    GetTimeEntrysResponse getTimeEntrysByUser(
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