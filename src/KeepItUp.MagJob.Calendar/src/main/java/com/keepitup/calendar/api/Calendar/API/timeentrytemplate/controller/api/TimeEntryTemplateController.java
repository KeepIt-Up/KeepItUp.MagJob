package com.keepitup.calendar.api.Calendar.API.timeentrytemplate.controller.api;

import com.keepitup.calendar.api.Calendar.API.configuration.PageConfig;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto.*;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@Tag(name="TimeEntrys Controller")
public interface TimeEntryTemplateController {
    PageConfig pageConfig = new PageConfig();

    @Operation(summary = "Get all Time Entry")
    @PostMapping("api/gettimeentrytemplates")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetTimeEntryTemplatesResponse getTimeEntryTemplates(
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
    @GetMapping("api/timeentrytemplates/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetTimeEntryTemplateResponse getTimeEntryTemplate(
            @Parameter(
                    name = "id",
                    description = "TimeEntrys id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Create TimeEntrys")
    @PostMapping("api/timeentrytemplates")
    @ResponseStatus(HttpStatus.CREATED)
    @ResponseBody
    GetTimeEntryTemplateResponse createTimeEntryTemplates(
            @Parameter(
                    name = "PostTimeEntryTemplatesRequest",
                    description = "PostTimeEntrysRequest DTO",
                    schema = @Schema(implementation = PostTimeEntryTemplateRequest.class),
                    required = true
            )
            @RequestBody
            PostTimeEntryTemplateRequest postTimeEntryTemplateRequest
    );

    @Operation(summary = "Update TimeEntrys")
    @PatchMapping("api/timeentrytemplates/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetTimeEntryTemplateResponse updateTimeEntryTemplate(
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
                    schema = @Schema(implementation = PatchTimeEntryTemplateRequest.class),
                    required = true
            )
            @RequestBody
            PatchTimeEntryTemplateRequest patchTimeEntryTemplateRequest
    );

    @Operation(summary = "Delete TimeEntrys")
    @DeleteMapping("/api/timeentrytemplates/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    void deleteTimeEntryTemplate(
            @Parameter(
                    name = "id",
                    description = "TimeEntrys id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Get TimeEntryTemplates by User")
    @PostMapping("api/timeentrytemplates/{userId}")
    @ResponseStatus(HttpStatus.OK)
    GetTimeEntryTemplatesResponse getTimeEntryTemplatesByUser(
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