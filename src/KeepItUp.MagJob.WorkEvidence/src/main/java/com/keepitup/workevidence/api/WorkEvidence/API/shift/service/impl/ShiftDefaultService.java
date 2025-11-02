package com.keepitup.workevidence.api.WorkEvidence.API.shift.service.impl;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.repository.api.ShiftRepository;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.service.api.ShiftService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;
import java.time.ZoneId;

import java.time.LocalDateTime;
import java.util.Optional;
import java.util.UUID;

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
        shift.setStatus(true); // Assuming 'true' means the shift is active
        if(shift.getEndTime() != null && shift.getStartTime() != null){
            shift.setStartTime(shift.getStartTime());
            shift.setEndTime(shift.getEndTime());
        }
        else{
            shift.setStartTime(LocalDateTime.now());
            shift.setEndTime(LocalDateTime.now().plusHours(8));
        }
        shift.setMemberId(shift.getMemberId());

        return Optional.of(shiftRepository.save(shift));
    }

    @Override
    @Transactional
    public Optional<Shift> endShift(UUID shiftId) {

        Shift existingShift = shiftRepository.findById(shiftId)
                .orElseThrow(() -> new ResponseStatusException(NOT_FOUND, "Shift not found"));


        if (existingShift.getEndTime().isBefore(LocalDateTime.now())) {
            throw new ResponseStatusException(CONFLICT, "The shift has already ended.");
        }

        if (existingShift.getEndTime().isBefore(existingShift.getStartTime())) {
            throw new ResponseStatusException(CONFLICT, "Wrong shift time");
        }

        existingShift.setEndTime(LocalDateTime.now(ZoneId.of("Europe/Warsaw")));
        existingShift.setStatus(false); // Assuming 'false' means the shift is no longer active
        return Optional.of(shiftRepository.save(existingShift));
    }

    @Override
    @Transactional
    public void deleteShift(UUID shiftId) {
        Shift existingShift = shiftRepository.findById(shiftId)
                .orElseThrow(() -> new ResponseStatusException(NOT_FOUND, "Shift not found"));

        shiftRepository.delete(existingShift);
    }

    @Override
    @Transactional
    public Optional<Shift> findById(UUID shiftId) {
        return shiftRepository.findById(shiftId);
    }

    @Override
    @Transactional
    public Optional<Shift> getActiveShifts(UUID userId) {
        return shiftRepository.findByMemberIdAndStatusTrue(userId);
    }
    @Override
    @Transactional
    public Optional<java.util.List<Shift>> getAllShifts(UUID memberId) {
        return shiftRepository.findAllByMemberId(memberId);
    }

}
