package com.keepitup.workevidence.api.WorkEvidence.API.shift.service.impl;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.repository.api.ShiftRepository;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.ArgumentCaptor;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.web.server.ResponseStatusException;

import java.time.LocalDateTime;
import java.time.ZoneId;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class ShiftServiceTest {

    @Mock
    private ShiftRepository shiftRepository;

    @InjectMocks
    private ShiftDefaultService service;

    @Test
    void startShift_withoutTimes_setsDefaultsAndSaves() {
        UUID memberId = UUID.randomUUID();
        Shift input = new Shift();
        input.setMemberId(memberId);

        when(shiftRepository.save(any(Shift.class))).thenAnswer(invocation -> {
            Shift s = invocation.getArgument(0);
            s.setId(UUID.randomUUID());
            return s;
        });

        Optional<Shift> result = service.startShift(input);
        assertTrue(result.isPresent());
        Shift saved = result.get();

        assertTrue(saved.isStatus());
        assertNotNull(saved.getStartTime());
        assertNotNull(saved.getEndTime());
        assertEquals(memberId, saved.getMemberId());
        assertTrue(saved.getEndTime().isAfter(saved.getStartTime()));
    }

    @Test
    void startShift_withProvidedTimes_keepsProvided() {
        UUID memberId = UUID.randomUUID();
        LocalDateTime start = LocalDateTime.now().minusHours(2);
        LocalDateTime end = LocalDateTime.now().plusHours(6);

        Shift input = new Shift();
        input.setMemberId(memberId);
        input.setStartTime(start);
        input.setEndTime(end);

        when(shiftRepository.save(any(Shift.class))).thenAnswer(invocation -> invocation.getArgument(0));

        Optional<Shift> result = service.startShift(input);
        assertTrue(result.isPresent());
        Shift saved = result.get();

        assertEquals(start, saved.getStartTime());
        assertEquals(end, saved.getEndTime());
        assertEquals(memberId, saved.getMemberId());
        assertTrue(saved.isStatus());
    }

    @Test
    void endShift_success_setsEndTimeAndStatusFalse() {
        UUID id = UUID.randomUUID();
        LocalDateTime start = LocalDateTime.now().minusHours(3);
        LocalDateTime originalEnd = LocalDateTime.now().plusHours(2);

        Shift existing = new Shift();
        existing.setId(id);
        existing.setStartTime(start);
        existing.setEndTime(originalEnd);
        existing.setStatus(true);

        when(shiftRepository.findById(id)).thenReturn(Optional.of(existing));
        when(shiftRepository.save(any(Shift.class))).thenAnswer(invocation -> invocation.getArgument(0));

        Optional<Shift> result = service.endShift(id);
        assertTrue(result.isPresent());
        Shift saved = result.get();

        assertFalse(saved.isStatus());
        LocalDateTime nowWarsaw = LocalDateTime.now(ZoneId.of("Europe/Warsaw"));
        // endTime should be around now (no exact match), but after start
        assertTrue(saved.getEndTime().isAfter(start));
        assertTrue(saved.getEndTime().isBefore(nowWarsaw.plusSeconds(5)));
    }

    @Test
    void endShift_notFound_throws() {
        UUID id = UUID.randomUUID();
        when(shiftRepository.findById(id)).thenReturn(Optional.empty());
        assertThrows(ResponseStatusException.class, () -> service.endShift(id));
    }

    @Test
    void endShift_alreadyEnded_throwsConflict() {
        UUID id = UUID.randomUUID();
        Shift existing = new Shift();
        existing.setId(id);
        existing.setStartTime(LocalDateTime.now().minusDays(1));
        existing.setEndTime(LocalDateTime.now().minusHours(1)); // already ended

        when(shiftRepository.findById(id)).thenReturn(Optional.of(existing));
        assertThrows(ResponseStatusException.class, () -> service.endShift(id));
    }

    @Test
    void deleteShift_existing_deletes() {
        UUID id = UUID.randomUUID();
        Shift existing = new Shift();
        existing.setId(id);

        when(shiftRepository.findById(id)).thenReturn(Optional.of(existing));
        doNothing().when(shiftRepository).delete(existing);

        service.deleteShift(id);
        verify(shiftRepository, times(1)).delete(existing);
    }

    @Test
    void deleteShift_notFound_throws() {
        UUID id = UUID.randomUUID();
        when(shiftRepository.findById(id)).thenReturn(Optional.empty());
        assertThrows(ResponseStatusException.class, () -> service.deleteShift(id));
    }

    @Test
    void findById_returnsOptional() {
        UUID id = UUID.randomUUID();
        Shift s = new Shift();
        s.setId(id);
        when(shiftRepository.findById(id)).thenReturn(Optional.of(s));

        Optional<Shift> res = service.findById(id);
        assertTrue(res.isPresent());
        assertEquals(id, res.get().getId());
    }

    @Test
    void getActiveShifts_returnsActive() {
        UUID memberId = UUID.randomUUID();
        Shift s = new Shift();
        s.setMemberId(memberId);
        s.setStatus(true);
        when(shiftRepository.findByMemberIdAndStatusTrue(memberId)).thenReturn(Optional.of(s));

        Optional<Shift> res = service.getActiveShifts(memberId);
        assertTrue(res.isPresent());
        assertTrue(res.get().isStatus());
        assertEquals(memberId, res.get().getMemberId());
    }

    @Test
    void getAllShifts_returnsList() {
        UUID memberId = UUID.randomUUID();
        Shift s1 = new Shift();
        s1.setMemberId(memberId);
        Shift s2 = new Shift();
        s2.setMemberId(memberId);

        when(shiftRepository.findAllByMemberId(memberId)).thenReturn(Optional.of(List.of(s1, s2)));

        Optional<List<Shift>> res = service.getAllShifts(memberId);
        assertTrue(res.isPresent());
        assertEquals(2, res.get().size());
    }
}