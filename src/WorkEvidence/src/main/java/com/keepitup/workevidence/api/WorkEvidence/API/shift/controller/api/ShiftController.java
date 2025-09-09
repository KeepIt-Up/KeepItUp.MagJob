package com.keepitup.workevidence.api.WorkEvidence.API.shift.controller.api;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.*;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.math.BigInteger;
import java.util.UUID;

@Tag(name = "Shift Controller")
public interface ShiftController {

    @Operation(summary = "Start a new shift")
    @PostMapping("shifts/start")
    @ResponseStatus(HttpStatus.CREATED)
    @ResponseBody
    GetShiftResponse startShift(
            @Parameter(
                    name = "StartShiftRequest",
                    description = "DTO for starting a shift",
                    schema = @Schema(implementation = PostStartShiftRequest.class),
                    required = true
            )
            @RequestBody PostStartShiftRequest startShiftRequest
    );

    @Operation(summary = "End an existing shift")
    @PutMapping("/shifts/end/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetEndShiftResponse endShift(
            @Parameter(
                    name = "id",
                    description = "Shift ID",
                    required = true
            )
            @PathVariable("id")
            UUID shiftId
    );

    @Operation(summary = "Delete a shift")
    @DeleteMapping("/api/shifts/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    void deleteShift(
            @Parameter(
                    name = "id",
                    description = "Shift ID",
                    required = true
            )
            @PathVariable("id")
            UUID shiftId
    );

    @Operation(summary = "Get a shift by ID")
    @GetMapping("shifts/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetShiftResponse getShift(
            @Parameter(
                    name = "id",
                    description = "Shift ID",
                    required = true
            )
            @PathVariable("id")
            UUID shiftId
    );

    @Operation(summary = "Get all active shifts from a user")
    @GetMapping("shifts/active/{memberId}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetShiftResponse getActiveShifts(
            @Parameter(
                    name = "memberId",
                    description = "Member ID",
                    required = true
            )
            @PathVariable("memberId")
            UUID memberId
    );

    @Operation(summary = "Get all shifts from a user")
    @GetMapping("shifts/all/{memberId}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetShiftsResponse getAllShifts(
            @Parameter(
                    name = "memberId",
                    description = "Member ID",
                    required = true
            )
            @PathVariable("memberId")
            UUID memberId
    );

}
