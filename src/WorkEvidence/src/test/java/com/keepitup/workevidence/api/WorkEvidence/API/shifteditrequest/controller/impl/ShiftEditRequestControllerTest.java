package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.controller.impl;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.repository.api.ShiftRepository;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.GetShiftEditRequestResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.GetShiftEditRequestsResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.PatchShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.PostShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.entity.ShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.function.RequestToShiftEditRequestFunction;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.function.ShiftEditRequestToResponseFunction;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.function.ShiftEditRequestsToResponseFunction;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.function.UpdateShiftEditRequestFunction;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.service.impl.ShiftEditRequestDefaultService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.ArgumentMatchers;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
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
class ShiftEditRequestControllerTest {

    @Mock
    private ShiftEditRequestDefaultService service;

    @Mock
    private ShiftEditRequestToResponseFunction shiftEditRequestToResponse;

    @Mock
    private ShiftEditRequestsToResponseFunction shiftEditRequestsToResponse;

    @Mock
    private RequestToShiftEditRequestFunction requestToShiftEditRequest;

    @Mock
    private UpdateShiftEditRequestFunction updateShiftEditRequestWithRequest;

    @Mock
    private ShiftRepository shiftRepository;

    private ShiftEditRequestDefaultConroller controller;

    @BeforeEach
    void setUp() {
        controller = new ShiftEditRequestDefaultConroller(
                service,
                shiftEditRequestToResponse,
                shiftEditRequestsToResponse,
                requestToShiftEditRequest,
                updateShiftEditRequestWithRequest,
                shiftRepository
        );
    }

    @Test
    void getShiftEditRequest_found_returnsDto() {
        UUID id = UUID.randomUUID();
        ShiftEditRequest entity = new ShiftEditRequest();
        entity.setId(id);
        entity.setStatus("PENDING");
        entity.setStartTime(LocalDateTime.of(2025,1,1,8,0));
        entity.setEndTime(LocalDateTime.of(2025,1,1,16,0));
        entity.setDescription("reason");

        GetShiftEditRequestResponse dto = GetShiftEditRequestResponse.builder()
                .id(id)
                .status(entity.getStatus())
                .startTime(entity.getStartTime())
                .endTime(entity.getEndTime())
                .description(entity.getDescription())
                .build();

        when(service.findById(id)).thenReturn(Optional.of(entity));
        when(shiftEditRequestToResponse.apply(entity)).thenReturn(dto);

        GetShiftEditRequestResponse res = controller.getShiftEditRequest(id);

        assertNotNull(res);
        assertEquals(id, res.getId());
        assertEquals("PENDING", res.getStatus());
        verify(service, times(1)).findById(id);
        verify(shiftEditRequestToResponse, times(1)).apply(entity);
    }

    @Test
    void getShiftEditRequest_notFound_throws404() {
        UUID id = UUID.randomUUID();
        when(service.findById(id)).thenReturn(Optional.empty());

        ResponseStatusException ex = assertThrows(ResponseStatusException.class, () -> controller.getShiftEditRequest(id));
    }

    @Test
    void getEditRequests_returnsListResponse() {
        UUID shiftId = UUID.randomUUID();
        ShiftEditRequest r1 = new ShiftEditRequest();
        ShiftEditRequest r2 = new ShiftEditRequest();
        PageImpl<ShiftEditRequest> page = new PageImpl<>(List.of(r1, r2), PageRequest.of(0, 10), 2);

        GetShiftEditRequestsResponse response = GetShiftEditRequestsResponse.builder()
                .shiftEditRequest(GetShiftEditRequestsResponse.ShiftEditRequest.builder().description("d1").build())
                .shiftEditRequest(GetShiftEditRequestsResponse.ShiftEditRequest.builder().description("d2").build())
                .count(2)
                .build();

        when(service.findByShiftId(eq(shiftId), any(PageRequest.class))).thenReturn(page);
        when(shiftEditRequestsToResponse.apply(any(), anyInt())).thenReturn(response);

        GetShiftEditRequestsResponse res = controller.getEditRequests(shiftId, 0, 10);

        assertNotNull(res);
        assertEquals(2, res.getCount());
        verify(service, times(1)).findByShiftId(eq(shiftId), any(PageRequest.class));
        verify(shiftEditRequestsToResponse, times(1)).apply(any(), eq(page.getNumberOfElements()));
    }

    @Test
    void updateShiftEditRequest_found_updatesAndReturns() {
        UUID id = UUID.randomUUID();
        ShiftEditRequest existing = new ShiftEditRequest();
        existing.setId(id);
        existing.setStatus("PENDING");
        existing.setStartTime(LocalDateTime.of(2025,1,1,8,0));
        existing.setEndTime(LocalDateTime.of(2025,1,1,16,0));

        PatchShiftEditRequest patch = new PatchShiftEditRequest();
        patch.setStatus("accepted");
        patch.setStartTime(LocalDateTime.of(2025,1,1,9,0));
        patch.setEndTime(LocalDateTime.of(2025,1,1,17,0));

        ShiftEditRequest updatedEntity = new ShiftEditRequest();
        updatedEntity.setId(id);
        updatedEntity.setStatus("accepted");
        updatedEntity.setStartTime(patch.getStartTime());
        updatedEntity.setEndTime(patch.getEndTime());

        GetShiftEditRequestResponse dto = GetShiftEditRequestResponse.builder()
                .id(id)
                .status("accepted")
                .startTime(patch.getStartTime())
                .endTime(patch.getEndTime())
                .build();

        when(service.findById(id)).thenReturn(Optional.of(existing));
        when(service.update(existing, patch)).thenReturn(updatedEntity);
        when(shiftEditRequestToResponse.apply(updatedEntity)).thenReturn(dto);

        GetShiftEditRequestResponse res = controller.updateShiftEditRequest(id, patch);

        assertNotNull(res);
        assertEquals("accepted", res.getStatus());
        verify(service, times(1)).findById(id);
        verify(service, times(1)).update(existing, patch);
        verify(shiftEditRequestToResponse, times(1)).apply(updatedEntity);
    }

    @Test
    void updateShiftEditRequest_notFound_throws404() {
        UUID id = UUID.randomUUID();
        PatchShiftEditRequest patch = new PatchShiftEditRequest();
        when(service.findById(id)).thenReturn(Optional.empty());

        ResponseStatusException ex = assertThrows(ResponseStatusException.class, () -> controller.updateShiftEditRequest(id, patch));
    }

    @Test
    void createShiftEditRequest_success_returnsDto() {
        UUID shiftId = UUID.randomUUID();
        PostShiftEditRequest req = new PostShiftEditRequest();
        req.setShiftId(shiftId);
        req.setDescription("reason");

        ShiftEditRequest toSave = new ShiftEditRequest();
        toSave.setDescription(req.getDescription());

        Shift shift = new Shift();
        shift.setId(shiftId);

        ShiftEditRequest saved = new ShiftEditRequest();
        saved.setId(UUID.randomUUID());
        saved.setShift(shift);
        saved.setDescription(req.getDescription());

        GetShiftEditRequestResponse dto = GetShiftEditRequestResponse.builder()
                .id(saved.getId())
                .description(saved.getDescription())
                .build();

        when(requestToShiftEditRequest.apply(req)).thenReturn(toSave);
        when(shiftRepository.findById(shiftId)).thenReturn(Optional.of(shift));
        when(service.save(ArgumentMatchers.any(ShiftEditRequest.class))).thenReturn(saved);
        when(shiftEditRequestToResponse.apply(saved)).thenReturn(dto);

        GetShiftEditRequestResponse res = controller.createShiftEditRequest(req);

        assertNotNull(res);
        assertEquals(saved.getId(), res.getId());
        verify(requestToShiftEditRequest, times(1)).apply(req);
        verify(shiftRepository, times(1)).findById(shiftId);
        verify(service, times(1)).save(any(ShiftEditRequest.class));
    }

    @Test
    void createShiftEditRequest_shiftNotFound_throws404() {
        UUID shiftId = UUID.randomUUID();
        PostShiftEditRequest req = new PostShiftEditRequest();
        req.setShiftId(shiftId);

        when(requestToShiftEditRequest.apply(req)).thenReturn(new ShiftEditRequest());
        when(shiftRepository.findById(shiftId)).thenReturn(Optional.empty());

        ResponseStatusException ex = assertThrows(ResponseStatusException.class, () -> controller.createShiftEditRequest(req));
        verify(service, never()).save(any());
    }

    @Test
    void deleteShiftEditRequest_delegatesToService() {
        UUID id = UUID.randomUUID();
        doNothing().when(service).delete(id);

        controller.deleteShiftEditRequest(id);

        verify(service, times(1)).delete(id);
    }
}