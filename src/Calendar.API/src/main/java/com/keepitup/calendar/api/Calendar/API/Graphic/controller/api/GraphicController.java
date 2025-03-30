package com.keepitup.calendar.api.Calendar.API.Graphic.controller.api;

import com.keepitup.calendar.api.Calendar.API.Graphic.dto.*;
import com.keepitup.calendar.api.Calendar.API.configuration.PageConfig;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.UUID;

@Tag(name="Graphic Controller")
public interface GraphicController {
    PageConfig pageConfig = new PageConfig();

    @Operation(summary = "Get all Graphics")
    @PostMapping("api/getgraphics")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetGraphicsResponse getGraphics(
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

    @Operation(summary = "Get Graphics")
    @GetMapping("api/graphics/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetGraphicResponse getGraphic(
            @Parameter(
                    name = "id",
                    description = "Graphics id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Create Graphics")
    @PostMapping("api/graphics")
    @ResponseStatus(HttpStatus.CREATED)
    @ResponseBody
    GetGraphicResponse createGraphics(
            @Parameter(
                    name = "PostGraphicsRequest",
                    description = "PostGraphicsRequest DTO",
                    schema = @Schema(implementation = PostGraphicRequest.class),
                    required = true
            )
            @RequestBody
            PostGraphicRequest postGraphicRequest
    );

    @Operation(summary = "Update Graphics")
    @PatchMapping("api/graphics/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetGraphicResponse updateGraphic(
            @Parameter(
                    name = "id",
                    description = "Graphics id value",
                    required = true
            )
            @PathVariable("id")
            UUID id,
            @Parameter(
                    name = "PatchGraphicsRequest",
                    description = "PatchGraphicsRequest DTO",
                    schema = @Schema(implementation = PatchGraphicRequest.class),
                    required = true
            )
            @RequestBody
            PatchGraphicRequest patchGraphicRequest
    );


    @Operation(summary = "Delete Graphics")
    @DeleteMapping("/api/graphics/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    void deleteGraphic(
            @Parameter(
                    name = "id",
                    description = "Graphics id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Get Graphics by User")
    @PostMapping("api/graphics/{userId}")
    @ResponseStatus(HttpStatus.OK)
    GetGraphicsResponse getGraphicsByUser(
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
                    description = "Graphics userId value",
                    required = true
            )
            @PathVariable("userId")
            UUID userId
    );

    @Operation(summary = "Create graphic")
    @PostMapping("api/createGraphic")
    @ResponseStatus(HttpStatus.OK)
    GetGraphicResponse createAndPopulateGraphic(
        @Parameter(
            name = "Graphic",
            description = "Graphic containing managerId and Name of the Graphic"
        )
        @RequestBody PostCreateAndPopulateGraphic graphic
    );
}
