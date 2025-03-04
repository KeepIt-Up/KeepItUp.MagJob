package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.controller.impl;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.GetShiftResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.repository.api.ShiftRepository;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.controller.api.ShiftEditRequestController;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.GetShiftEditRequestResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.GetShiftEditRequestsResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.PatchShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.PostShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.entity.ShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.function.RequestToShiftEditRequestFunction;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.function.ShiftEditRequestToResponseFunction;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.function.ShiftEditRequestsToResponseFunction;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.function.UpdateShiftEditRequestFunction;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.service.api.ShiftEditRequestService;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.service.impl.ShiftEditRequestDefaultService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigInteger;
import java.util.Optional;

@RestController
@Log
public class ShiftEditRequestDefaultConroller implements ShiftEditRequestController {
    private final ShiftEditRequestDefaultService service;
    private final ShiftEditRequestToResponseFunction shiftEditRequestToResponse;
    private final ShiftEditRequestsToResponseFunction shiftEditRequestsToResponse;
    private final RequestToShiftEditRequestFunction requestToShiftEditRequest;
    private final UpdateShiftEditRequestFunction updateShiftEditRequestWithRequest;
    private final ShiftRepository shiftRepository;

    @Autowired
    public ShiftEditRequestDefaultConroller(
            ShiftEditRequestDefaultService service,
            ShiftEditRequestToResponseFunction shiftEditRequestToResponse,
            ShiftEditRequestsToResponseFunction shiftEditRequestsToResponse,
            RequestToShiftEditRequestFunction requestToShiftEditRequest,
            UpdateShiftEditRequestFunction updateShiftEditRequestWithRequest,
            ShiftRepository shiftRepository
    ) {
        this.service = service;
        this.shiftEditRequestToResponse = shiftEditRequestToResponse;
        this.shiftEditRequestsToResponse = shiftEditRequestsToResponse;
        this.requestToShiftEditRequest = requestToShiftEditRequest;
        this.updateShiftEditRequestWithRequest = updateShiftEditRequestWithRequest;
        this.shiftRepository = shiftRepository;
    }

    @Override
    public void deleteShiftEditRequest(BigInteger id) {
        service.delete(id);
    }

    @Override
    public GetShiftEditRequestResponse getShiftEditRequest(BigInteger id) {
        Optional<ShiftEditRequest> shiftEditRequest = service.findById(id);

        if(shiftEditRequest.isPresent()) {
            return shiftEditRequestToResponse.apply(shiftEditRequest.get());
        } else {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Shift edit request not found");
        }
    }
    @Override
    public GetShiftEditRequestsResponse getEditRequests(BigInteger shiftId, int page, int size) {
        PageRequest pageRequest = PageRequest.of(page, size);
        Integer count = service.findByShiftId(shiftId, pageRequest).getNumberOfElements();

        return shiftEditRequestsToResponse.apply(service.findByShiftId(shiftId, pageRequest), count);
    }

    @Override
    public GetShiftEditRequestResponse updateShiftEditRequest(BigInteger id, PatchShiftEditRequest request) {
        ShiftEditRequest shiftEditRequest = service.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Shift edit request not found"));

        ShiftEditRequest updated = service.update(shiftEditRequest, request);
        return shiftEditRequestToResponse.apply(updated);
    }

    @Override
    public GetShiftEditRequestResponse createShiftEditRequest(PostShiftEditRequest request) {
        ShiftEditRequest shiftEditRequest = requestToShiftEditRequest.apply(request);
        Optional<Shift> shiftOpt = shiftRepository.findById(request.getShiftId());
        if(!shiftOpt.isPresent()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Shift not found");
        }
        Shift shift = shiftOpt.get();
        shiftEditRequest.setShift(shift);

        return shiftEditRequestToResponse.apply(service.save(shiftEditRequest));
    }

}
