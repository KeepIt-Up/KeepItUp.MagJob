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
@Schema(description = "DTO for patching the end of a shift with ID")
public class PatchEndShiftRequest {

    @Schema(description = "Shift ID", example = "123e4567-e89b-12d3-a456-426614174000")
    private BigInteger shiftId;

    @Schema(description = "End time of the shift", example = "2025-01-16T16:00:00")
    private LocalDateTime endTime;

    @Schema(description = "Additional notes or summary for the shift", example = "Completed inventory check")
    private String notes;
}