package com.keepitup.workevidence.api.WorkEvidence.API.shift.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.math.BigInteger;
import java.time.LocalDateTime;
import java.util.List;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "DTO for starting a shift")
public class PostStartShiftRequest {

    @Schema(description = "Start time of the shift", example = "2025-01-16T08:00:00")
    private LocalDateTime startTime;

    @Schema(description = "Additional details about the shift", example = "Morning shift at warehouse")
    private String description;
}