package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.service.impl;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.repository.api.ShiftRepository;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.PatchShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.entity.ShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.repository.api.ShiftEditRequestRepository;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.service.api.ShiftEditRequestService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;

import static org.springframework.http.HttpStatus.NOT_FOUND;
@Service
@Log
public class ShiftEditRequestDefaultService implements ShiftEditRequestService {
    private final ShiftEditRequestRepository shiftEditRequestRepository;
    private final ShiftRepository shiftRepository;

    @Autowired
    public ShiftEditRequestDefaultService(ShiftEditRequestRepository shiftEditRequestRepository,ShiftRepository shiftRepository) {
        this.shiftEditRequestRepository = shiftEditRequestRepository;
        this.shiftRepository = shiftRepository;
    }

    @Override
    @Transactional
    public void delete(BigInteger id) {
        ShiftEditRequest existing = shiftEditRequestRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(NOT_FOUND, "Shift edit not found"));
        shiftEditRequestRepository.delete(existing);
    }

    @Override
    @Transactional
    public ShiftEditRequest save(ShiftEditRequest shiftEditRequest) {
        return shiftEditRequestRepository.save(shiftEditRequest);
    }

    @Override
    @Transactional
    public Optional<ShiftEditRequest> findById(BigInteger id) {
        return shiftEditRequestRepository.findById(id);
    }

    @Override
    @Transactional
    public Page<ShiftEditRequest> findByShiftId(BigInteger shiftId, Pageable pageable) {
        return shiftEditRequestRepository.findByShiftId(shiftId, pageable);
    }

    @Override
    @Transactional
    public ShiftEditRequest update(ShiftEditRequest shiftEditRequest, PatchShiftEditRequest request) {
        // Update the shift edit request with the new data
        shiftEditRequest.setStatus(request.getStatus());
        shiftEditRequest.setStartTime(request.getStartTime());
        shiftEditRequest.setEndTime(request.getEndTime());

        // If the status is accepted, update the original shift
        if ("accepted".equalsIgnoreCase(request.getStatus())) {
            Shift originalShift = shiftEditRequest.getShift();
            originalShift.setStartTime(shiftEditRequest.getStartTime());
            originalShift.setEndTime(shiftEditRequest.getEndTime());
            shiftRepository.save(originalShift);
        }

        return shiftEditRequestRepository.save(shiftEditRequest);
    }
}
