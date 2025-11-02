package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.service.impl;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.repository.api.ShiftRepository;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.PatchShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.entity.ShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.repository.api.ShiftEditRequestRepository;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageImpl;
import org.springframework.data.domain.PageRequest;
import org.springframework.web.server.ResponseStatusException;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class ShiftEditRequestServiceTest {

    @Mock
    private ShiftEditRequestRepository shiftEditRequestRepository;

    @Mock
    private ShiftRepository shiftRepository;

    @InjectMocks
    private ShiftEditRequestDefaultService service;

    @Test
    void delete_existing_deletes() {
        UUID id = UUID.randomUUID();
        ShiftEditRequest existing = new ShiftEditRequest();
        existing.setId(id);

        when(shiftEditRequestRepository.findById(id)).thenReturn(Optional.of(existing));
        doNothing().when(shiftEditRequestRepository).delete(existing);

        service.delete(id);

        verify(shiftEditRequestRepository, times(1)).delete(existing);
    }

    @Test
    void delete_notFound_throws() {
        UUID id = UUID.randomUUID();
        when(shiftEditRequestRepository.findById(id)).thenReturn(Optional.empty());
        assertThrows(ResponseStatusException.class, () -> service.delete(id));
    }

    @Test
    void save_delegatesToRepository() {
        ShiftEditRequest req = new ShiftEditRequest();
        req.setId(UUID.randomUUID());

        when(shiftEditRequestRepository.save(req)).thenReturn(req);

        ShiftEditRequest res = service.save(req);
        assertSame(req, res);
        verify(shiftEditRequestRepository, times(1)).save(req);
    }

    @Test
    void findById_returnsOptional() {
        UUID id = UUID.randomUUID();
        ShiftEditRequest req = new ShiftEditRequest();
        req.setId(id);

        when(shiftEditRequestRepository.findById(id)).thenReturn(Optional.of(req));

        Optional<ShiftEditRequest> res = service.findById(id);
        assertTrue(res.isPresent());
        assertEquals(id, res.get().getId());
    }

    @Test
    void findByShiftId_returnsPage() {
        UUID shiftId = UUID.randomUUID();
        ShiftEditRequest r1 = new ShiftEditRequest();
        ShiftEditRequest r2 = new ShiftEditRequest();
        Page<ShiftEditRequest> page = new PageImpl<>(List.of(r1, r2));

        PageRequest pageRequest = PageRequest.of(0, 10);
        when(shiftEditRequestRepository.findByShiftId(shiftId, pageRequest)).thenReturn(page);

        Page<ShiftEditRequest> res = service.findByShiftId(shiftId, pageRequest);
        assertEquals(2, res.getNumberOfElements());
        assertEquals(2, res.getContent().size());
    }

    @Test
    void update_whenAccepted_updatesOriginalShiftAndSavesBoth() {
        // przygotowanie istniejącej prośby
        Shift originalShift = new Shift();
        originalShift.setId(UUID.randomUUID());
        originalShift.setStartTime(LocalDateTime.of(2025,1,1,8,0));
        originalShift.setEndTime(LocalDateTime.of(2025,1,1,16,0));
        originalShift.setDescription("old");

        ShiftEditRequest editRequest = new ShiftEditRequest();
        editRequest.setId(UUID.randomUUID());
        editRequest.setShift(originalShift);
        editRequest.setStartTime(originalShift.getStartTime());
        editRequest.setEndTime(originalShift.getEndTime());
        editRequest.setDescription("old edit");
        editRequest.setStatus("PENDING");

        // żądanie aktualizacji
        PatchShiftEditRequest patch = new PatchShiftEditRequest();
        patch.setStatus("accepted");
        LocalDateTime newStart = LocalDateTime.of(2025,1,1,9,0);
        LocalDateTime newEnd = LocalDateTime.of(2025,1,1,17,0);
        patch.setStartTime(newStart);
        patch.setEndTime(newEnd);

        // mockowanie zapisów
        when(shiftRepository.save(any(Shift.class))).thenAnswer(invocation -> invocation.getArgument(0));
        when(shiftEditRequestRepository.save(any(ShiftEditRequest.class))).thenAnswer(invocation -> invocation.getArgument(0));

        ShiftEditRequest updated = service.update(editRequest, patch);

        // sprawdzenia: status i czasy w samej prośbie
        assertEquals("accepted", updated.getStatus());
        assertEquals(newStart, updated.getStartTime());
        assertEquals(newEnd, updated.getEndTime());

        // sprawdzenie, że oryginalny shift został zmieniony i przekazany do zapisu
        assertEquals(newStart, originalShift.getStartTime());
        assertEquals(newEnd, originalShift.getEndTime());

        verify(shiftRepository, times(1)).save(originalShift);
        verify(shiftEditRequestRepository, times(1)).save(editRequest);
    }
}