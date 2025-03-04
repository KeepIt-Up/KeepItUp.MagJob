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
@Schema(description = "ShiftResponse DTO")
public class GetShiftResponse {

    @Schema(description = "Unique identifier of the shift", example = "123e4567-e89b-12d3-a456-426614174000")
    private BigInteger id;

    @Schema(description = "Start time of the shift", example = "2025-01-16T08:00:00")
    private LocalDateTime startTime;

    @Schema(description = "End time of the shift", example = "2025-01-16T16:00:00")
    private LocalDateTime endTime;

    @Schema(description = "Additional notes or summary for the shift", example = "Completed inventory check")
    private String description;

    @Schema(description = "Member entity")
    private Member member;

    @Schema(description = "GetShiftEditRequestResponse DTO")
    private List<GetShiftEditRequestResponse> shiftEditRequests;

}
