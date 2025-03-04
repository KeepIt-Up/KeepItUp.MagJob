package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.controller.api;

import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.GetShiftEditRequestResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.GetShiftEditRequestsResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.PatchShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.PostShiftEditRequest;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.math.BigInteger;

@Tag(name = "ShiftEditRequest Controller")
public interface ShiftEditRequestController {



    @Operation(summary = "Get a shift edit request")
    @GetMapping("/api/shift-edit-requests/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetShiftEditRequestResponse getShiftEditRequest(
            @Parameter(
                    name = "id",
                    description = "Shift edit request ID",
                    required = true
            )
            @PathVariable("id")
            BigInteger shiftEditRequestId
    );

    @Operation(summary = "Get all shift edit requests")
    @GetMapping("/api/shift-edit-requests/shift/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetShiftEditRequestsResponse getEditRequests(
            @Parameter(
                    name = "id",
                    description = "Shift ID",
                    required = true
            )
            @PathVariable("id")
            BigInteger shiftId,

            @RequestParam(defaultValue = "0")
            int page,
            @RequestParam(defaultValue = "10")
            int size
    );

    @Operation(summary = "Create a shift edit request")
    @PostMapping("/api/shift-edit-requests")
    @ResponseStatus(HttpStatus.CREATED)
    @ResponseBody
    GetShiftEditRequestResponse createShiftEditRequest(
        @Parameter(
                name = "CreateShiftEditRequest",
                description = "DTO for creating a shift edit request",
                required = true
        )
        @RequestBody PostShiftEditRequest createShiftEditRequest
    );

    @Operation(summary = "Delete a shift edit request")
    @DeleteMapping("/api/shift-edit-requests/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    void deleteShiftEditRequest(
        @Parameter(
                name = "id",
                description = "Shift edit request ID",
                required = true
        )
        @PathVariable("id")
        BigInteger shiftEditRequestId
    );

    @Operation(summary = "Uppdate a shift edit request")
    @PatchMapping("/api/shift-edit-requests/{id}")
    @ResponseStatus(HttpStatus.OK)
    GetShiftEditRequestResponse updateShiftEditRequest(
        @Parameter(
                name = "id",
                description = "Shift edit request ID",
                required = true
        )
        @PathVariable("id")
        BigInteger shiftEditRequestId,
        @Parameter(
                name = "UpdateShiftEditRequest",
                description = "DTO for updating a shift edit request",
                required = true
        )
        @RequestBody PatchShiftEditRequest updateShiftEditRequest
    );



}
