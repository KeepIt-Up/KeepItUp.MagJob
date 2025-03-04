package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
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
@Schema(description = "DTO for getting a shift edit request")
public class GetShiftEditRequestResponse {

    @Schema(description = "ID of the shift edit request", example = "123e4567-e89b-12d3-a456-426614174000")
    private BigInteger id;

    @Schema(description = "Status of the shift edit request", example = "PENDING")
    private String status;

    @Schema(description = "New start time of the shift", example = "2025-01-16T08:00:00")
    private LocalDateTime startTime;

    @Schema(description = "New end time of the shift", example = "2025-01-16T08:00:00")
    private LocalDateTime endTime;

}