package com.keepitup.workevidence.api.WorkEvidence.API.shift.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.math.BigInteger;
import java.time.LocalDateTime;
import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "DTO for ending a shift response")
public class GetEndShiftResponse {

    @Schema(description = "Unique identifier of the shift", example = "123e4567-e89b-12d3-a456-426614174000")
    private BigInteger id;

    @Schema(description = "End time of the shift", example = "2025-01-16T16:00:00")
    private LocalDateTime endTime;

    @Schema(description = "Optional notes for the shift", example = "Shift ended earlier due to maintenance.")
    private String notes;
}