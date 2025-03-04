package com.keepitup.workevidence.api.WorkEvidence.API.shift.dto;

import com.keepitup.workevidence.api.WorkEvidence.API.Member.entity.Member;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.GetShiftEditRequestResponse;
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
@Schema(description = "ShiftsResponse DTO")
public class GetShiftsResponse {

    @Getter
    @Setter
    @Builder
    @NoArgsConstructor
    @AllArgsConstructor(access = AccessLevel.PRIVATE)
    @ToString
    @EqualsAndHashCode
    public static class Shift {

        @Schema(description = "Unique identifier of the shift", example = "123e4567-e89b-12d3-a456-426614174000")
        private BigInteger id;

        @Schema(description = "Start time of the shift", example = "2025-01-16T08:00:00")
        private LocalDateTime startTime;

        @Schema(description = "End time of the shift", example = "2025-01-16T16:00:00")
        private LocalDateTime endTime;

        @Schema(description = "Optional description of the shift", example = "Morning shift")
        private String description;

        @Schema(description = "Member entity")
        private Member member;

        @Schema(description = "GetShiftEditRequestResponse DTO")
        private List<GetShiftEditRequestResponse> shiftEditRequests;
    }

    @Singular
    @Schema(description = "List of shifts")
    private List<Shift> shifts;

    @Schema(description = "Total number of shifts")
    private Integer count;
}
