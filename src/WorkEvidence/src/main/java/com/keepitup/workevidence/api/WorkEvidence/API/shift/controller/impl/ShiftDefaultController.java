package com.keepitup.workevidence.api.WorkEvidence.API.shift.controller.impl;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.controller.api.ShiftController;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.GetEndShiftResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.GetShiftResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.PatchEndShiftRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.PostStartShiftRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.function.*;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.service.api.ShiftService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigInteger;
import java.util.Optional;


@RestController
@Log
public class ShiftDefaultController implements ShiftController {
    private final ShiftService service;
    private final ShiftToResponseFunction shiftToResponse;
    private final ShiftsToResponseFunction shiftsToResponse;
    private final RequestToShiftFunction requestToShift;
    private final UpdateShiftWithRequestFunction updateShiftWithRequest;

    @Autowired
    public ShiftDefaultController(
            ShiftService service,
            ShiftToResponseFunction shiftToResponse,
            ShiftsToResponseFunction shiftsToResponse,
            RequestToShiftFunction requestToShift,
            UpdateShiftWithRequestFunction updateShiftWithRequest
    ) {
        this.service = service;
        this.shiftToResponse = shiftToResponse;
        this.shiftsToResponse = shiftsToResponse;
        this.requestToShift = requestToShift;
        this.updateShiftWithRequest = updateShiftWithRequest;
    }

    @Override
    public GetShiftResponse startShift(PostStartShiftRequest postShiftRequest) {
        Shift shift = requestToShift.apply(postShiftRequest);
        service.startShift(shift);
        GetShiftResponse response = new GetShiftResponse();
        response.setId(shift.getId());  // Zwracamy ID zapisanej zmiany
        response.setStartTime(shift.getStartTime());
        response.setEndTime(shift.getEndTime());
        response.setDescription(shift.getDescription());

        return response;
    }

    @Override
    public void deleteShift(BigInteger id) {
        Optional<Shift> shift = service.findById(id);

        if(shift.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Shift not found");
        }
        service.deleteShift(id);
    }

    @Override
    public GetEndShiftResponse endShift(BigInteger id, PatchEndShiftRequest request) {
        Optional<Shift> shift = service.findById(id);

        if (shift.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Shift not found");
        }
        Shift updatedShift = updateShiftWithRequest.apply(shift.get(), request);
        service.endShift(id, updatedShift);
        return null;
    }

    @Override
    public GetShiftResponse getShift(BigInteger id) {
        Optional<Shift> shift = service.findById(id);

        if (shift.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Shift not found");
        }
        return shiftToResponse.apply(shift.get());
    }


}
