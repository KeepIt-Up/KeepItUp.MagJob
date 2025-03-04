package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.controller.api;

import com.keepitup.calendar.api.Calendar.API.configuration.PageConfig;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.GetAvailabilityTemplateResponse;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.GetAvailabilityTemplatesResponse;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.PatchAvailabilityTemplateRequest;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.PostAvailabilityTemplateRequest;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@Tag(name="AvailabilityTemplate Controller")
public interface AvailabilityTemplateController {
    PageConfig pageConfig = new PageConfig();

    @Operation(summary = "Get all AvailabilityTemplates")
    @PostMapping("api/getavailabilitytemplates")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetAvailabilityTemplatesResponse getAvailabilityTemplates(
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

    @Operation(summary = "Get AvailabilityTemplates")
    @GetMapping("api/availabilitytemplates/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetAvailabilityTemplateResponse getAvailabilityTemplate(
            @Parameter(
                    name = "id",
                    description = "AvailabilityTemplates id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Create AvailabilityTemplates")
    @PostMapping("api/availabilitytemplates")
    @ResponseStatus(HttpStatus.CREATED)
    @ResponseBody
    GetAvailabilityTemplateResponse createAvailabilityTemplates(
            @Parameter(
                    name = "PostAvailabilityTemplatesRequest",
                    description = "PostAvailabilityTemplatesRequest DTO",
                    schema = @Schema(implementation = PostAvailabilityTemplateRequest.class),
                    required = true
            )
            @RequestBody
            PostAvailabilityTemplateRequest postAvailabilityTemplateRequest
    );

    @Operation(summary = "Update AvailabilityTemplates")
    @PatchMapping("api/availabilitytemplates/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetAvailabilityTemplateResponse updateAvailabilityTemplate(
            @Parameter(
                    name = "id",
                    description = "AvailabilityTemplates id value",
                    required = true
            )
            @PathVariable("id")
            UUID id,
            @Parameter(
                    name = "PatchAvailabilityTemplatesRequest",
                    description = "PatchAvailabilityTemplatesRequest DTO",
                    schema = @Schema(implementation = PatchAvailabilityTemplateRequest.class),
                    required = true
            )
            @RequestBody
            PatchAvailabilityTemplateRequest patchAvailabilityTemplateRequest
    );


    @Operation(summary = "Delete AvailabilityTemplates")
    @DeleteMapping("/api/availabilitytemplates/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    void deleteAvailabilityTemplate(
            @Parameter(
                    name = "id",
                    description = "AvailabilityTemplates id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Get AvailabilityTemplates by User")
    @PostMapping("api/availabilitytemplates/{userId}")
    @ResponseStatus(HttpStatus.OK)
    GetAvailabilityTemplatesResponse getAvailabilityTemplatesByUser(
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
                    description = "AvailabilityTemplates userId value",
                    required = true
            )
            @PathVariable("userId")
            UUID userId
    );
}