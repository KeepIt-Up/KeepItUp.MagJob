package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.repository.api;

import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.entity.ShiftEditRequest;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;

import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;

public interface ShiftEditRequestRepository extends JpaRepository<ShiftEditRequest, BigInteger> {
    Page<ShiftEditRequest> findByShiftId(BigInteger shiftId, Pageable pageable);
}
