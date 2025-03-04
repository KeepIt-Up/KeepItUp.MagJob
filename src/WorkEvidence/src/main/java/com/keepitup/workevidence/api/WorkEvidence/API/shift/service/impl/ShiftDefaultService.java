package com.keepitup.workevidence.api.WorkEvidence.API.shift.service.impl;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.repository.api.ShiftRepository;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.service.api.ShiftService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigInteger;
import java.time.LocalDateTime;
import java.util.Optional;

import static org.springframework.http.HttpStatus.*;

@Service
@Log
public class ShiftDefaultService implements ShiftService {

    private final ShiftRepository shiftRepository;

    @Autowired
    public ShiftDefaultService(ShiftRepository shiftRepository) {
        this.shiftRepository = shiftRepository;
    }

    @Override
    @Transactional
    public Optional<Shift> startShift(Shift shift) {

        shift.setStartTime(LocalDateTime.now());
        shift.setEndTime(LocalDateTime.now().plusHours(8));

        return Optional.of(shiftRepository.save(shift));
    }

    @Override
    @Transactional
    public Optional<Shift> endShift(BigInteger shiftId, Shift shift) {

        Shift existingShift = shiftRepository.findById(shiftId)
                .orElseThrow(() -> new ResponseStatusException(NOT_FOUND, "Shift not found"));


        if (existingShift.getEndTime().isBefore(LocalDateTime.now())) {
            throw new ResponseStatusException(CONFLICT, "The shift has already ended.");
        }

        existingShift.setEndTime(LocalDateTime.now());
        return Optional.of(shiftRepository.save(existingShift));
    }

    @Override
    @Transactional
    public void deleteShift(BigInteger shiftId) {
        Shift existingShift = shiftRepository.findById(shiftId)
                .orElseThrow(() -> new ResponseStatusException(NOT_FOUND, "Shift not found"));

        shiftRepository.delete(existingShift);
    }

    @Override
    @Transactional
    public Optional<Shift> findById(BigInteger shiftId) {
        return shiftRepository.findById(shiftId);
    }


}
