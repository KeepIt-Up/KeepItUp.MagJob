package com.keepitup.workevidence.api.WorkEvidence.API.shift.controller.api;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.*;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.math.BigInteger;

@Tag(name = "Shift Controller")
public interface ShiftController {

    @Operation(summary = "Start a new shift")
    @PostMapping("/api/shifts/start")
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
    @PutMapping("/api/shifts/end/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetEndShiftResponse endShift(
            @Parameter(
                    name = "id",
                    description = "Shift ID",
                    required = true
            )
            @PathVariable("id")
            BigInteger shiftId,
            @Parameter(
                    name = "EndShiftRequest",
                    description = "DTO for ending a shift",
                    schema = @Schema(implementation = PatchEndShiftRequest.class),
                    required = true
            )
            @RequestBody PatchEndShiftRequest endShiftRequest
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
            BigInteger shiftId
    );

    @Operation(summary = "Get a shift by ID")
    @GetMapping("/api/shifts/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetShiftResponse getShift(
            @Parameter(
                    name = "id",
                    description = "Shift ID",
                    required = true
            )
            @PathVariable("id")
            BigInteger shiftId
    );


}
