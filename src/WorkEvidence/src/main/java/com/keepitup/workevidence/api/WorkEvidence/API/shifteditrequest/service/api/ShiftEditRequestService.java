package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.service.api;

import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.PatchShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.entity.ShiftEditRequest;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;

public interface ShiftEditRequestService {
    Optional<ShiftEditRequest> findById(BigInteger id);
    ShiftEditRequest save(ShiftEditRequest shiftEditRequest);
    void delete(BigInteger id);
    Page<ShiftEditRequest> findByShiftId(BigInteger shiftId, Pageable pageable);

    ShiftEditRequest update(ShiftEditRequest shiftEditRequest, PatchShiftEditRequest request);
}
