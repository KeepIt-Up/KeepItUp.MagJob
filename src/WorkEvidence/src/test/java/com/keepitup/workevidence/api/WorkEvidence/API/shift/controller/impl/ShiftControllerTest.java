package com.keepitup.workevidence.api.WorkEvidence.API.shift.controller.impl;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.SerializationFeature;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.PostStartShiftRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.function.RequestToShiftFunction;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.function.ShiftToResponseFunction;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.function.ShiftsToResponseFunction;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.function.UpdateShiftWithRequestFunction;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.service.impl.ShiftDefaultService;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.put;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.delete;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mockito;
import org.springframework.http.MediaType;
import org.springframework.http.converter.json.MappingJackson2HttpMessageConverter;
import org.springframework.test.web.servlet.MockMvc;
import org.springframework.test.web.servlet.setup.MockMvcBuilders;

import java.time.LocalDateTime;
import java.util.Collections;
import java.util.Optional;
import java.util.UUID;
import java.util.List;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.doNothing;
import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;

class ShiftControllerTest {

    private MockMvc mockMvc;
    private ShiftDefaultService mockService;
    private ObjectMapper objectMapper;

    @BeforeEach
    void setup() {
        mockService = Mockito.mock(ShiftDefaultService.class);

        ShiftDefaultController controller = new ShiftDefaultController(
                mockService,
                new ShiftToResponseFunction(),
                new ShiftsToResponseFunction(),
                new RequestToShiftFunction(),
                new UpdateShiftWithRequestFunction()
        );

        objectMapper = new ObjectMapper();
        objectMapper.registerModule(new JavaTimeModule()); // obsługa LocalDateTime
        objectMapper.disable(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS);

        mockMvc = MockMvcBuilders.standaloneSetup(controller)
                .setMessageConverters(new MappingJackson2HttpMessageConverter(objectMapper))
                .build();
    }

    @Test
    void testStartShiftAndReturnHttp201() throws Exception {
        // given
        UUID memberId = UUID.randomUUID();

        PostStartShiftRequest request = new PostStartShiftRequest();
        request.setMemberId(memberId);
        request.setDescription("Test shift");
        request.setStartTime(LocalDateTime.of(2025, 1, 16, 8, 0));


        Shift savedShift = new Shift();
        savedShift.setId(UUID.randomUUID());
        savedShift.setStartTime(request.getStartTime());
        savedShift.setEndTime(request.getStartTime().plusHours(8));
        savedShift.setDescription(request.getDescription());
        savedShift.setMemberId(memberId);
        savedShift.setStatus(true);

        Mockito.when(mockService.startShift(any(Shift.class)))
                .thenReturn(Optional.of(savedShift));

        mockMvc.perform(post("/shifts/start")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(objectMapper.writeValueAsString(request)))
                .andExpect(status().isCreated())
                .andExpect(jsonPath("$.description").value("Test shift"))
                .andExpect(jsonPath("$.startTime").value("2025-01-16T08:00:00"))
                .andExpect(jsonPath("$.endTime").value("2025-01-16T16:00:00"))
                .andExpect(jsonPath("$.memberId").value(savedShift.getMemberId().toString()));
    }

    @Test
    void testGetShiftSuccess() throws Exception {
        UUID shiftId = UUID.randomUUID();
        UUID memberId = UUID.randomUUID();
        Shift shift = new Shift();
        shift.setId(shiftId);
        shift.setStartTime(LocalDateTime.of(2025, 1, 16, 8, 0));
        shift.setEndTime(LocalDateTime.of(2025, 1, 16, 16, 0));
        shift.setDescription("Existing shift");
        shift.setMemberId(memberId);
        shift.setStatus(true);
        shift.setShiftEditRequests(Collections.emptyList());

        Mockito.when(mockService.findById(shiftId)).thenReturn(Optional.of(shift));

        mockMvc.perform(get("/shifts/{id}", shiftId)
                        .contentType(MediaType.APPLICATION_JSON))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.id").value(shiftId.toString()))
                .andExpect(jsonPath("$.description").value("Existing shift"))
                .andExpect(jsonPath("$.startTime").value("2025-01-16T08:00:00"))
                .andExpect(jsonPath("$.endTime").value("2025-01-16T16:00:00"))
                .andExpect(jsonPath("$.memberId").value(memberId.toString()))
                .andExpect(jsonPath("$.status").value(true));
    }

    @Test
    void testGetShiftError() throws Exception {
        UUID shiftId = UUID.randomUUID();

        Mockito.when(mockService.findById(shiftId)).thenReturn(Optional.empty());

        mockMvc.perform(get("/shifts/{id}", shiftId)
                        .contentType(MediaType.APPLICATION_JSON))
                .andExpect(status().isNotFound());
    }

    @Test
    void testEndShiftSuccess() throws Exception {
        UUID shiftId = UUID.randomUUID();
        UUID memberId = UUID.randomUUID();

        Shift existing = new Shift();
        existing.setId(shiftId);
        existing.setMemberId(memberId);
        existing.setStartTime(LocalDateTime.of(2025,1,16,8,0));
        existing.setEndTime(LocalDateTime.of(2025,1,16,16,0));
        existing.setStatus(true);

        Shift ended = new Shift();
        ended.setId(shiftId);
        ended.setMemberId(memberId);
        ended.setStartTime(existing.getStartTime());
        ended.setEndTime(LocalDateTime.of(2025,1,16,16,0));
        ended.setStatus(false);

        when(mockService.findById(shiftId)).thenReturn(Optional.of(existing));
        when(mockService.endShift(shiftId)).thenReturn(Optional.of(ended));

        mockMvc.perform(put("/shifts/end/{id}", shiftId)
                        .contentType(MediaType.APPLICATION_JSON))
                .andExpect(status().isOk());
    }

    @Test
    void testEndShiftNotFound() throws Exception {
        UUID shiftId = UUID.randomUUID();

        when(mockService.findById(shiftId)).thenReturn(Optional.empty());

        mockMvc.perform(put("/shifts/end/{id}", shiftId)
                        .contentType(MediaType.APPLICATION_JSON))
                .andExpect(status().isNotFound());
    }

    @Test
    void testDeleteShiftSuccess() throws Exception {
        UUID shiftId = UUID.randomUUID();
        Shift existing = new Shift();
        existing.setId(shiftId);

        when(mockService.findById(shiftId)).thenReturn(Optional.of(existing));
        doNothing().when(mockService).deleteShift(shiftId);

        mockMvc.perform(delete("/api/shifts/{id}", shiftId)
                        .contentType(MediaType.APPLICATION_JSON))
                .andExpect(status().isNoContent());
    }

    @Test
    void testDeleteShiftNotFound() throws Exception {
        UUID shiftId = UUID.randomUUID();

        when(mockService.findById(shiftId)).thenReturn(Optional.empty());

        mockMvc.perform(delete("/api/shifts/{id}", shiftId)
                        .contentType(MediaType.APPLICATION_JSON))
                .andExpect(status().isNotFound());
    }

    @Test
    void testGetActiveShiftsSuccess() throws Exception {
        UUID memberId = UUID.randomUUID();
        Shift s = new Shift();
        s.setMemberId(memberId);
        s.setStatus(true);
        s.setStartTime(LocalDateTime.of(2025,1,16,8,0));
        s.setEndTime(LocalDateTime.of(2025,1,16,16,0));

        when(mockService.getActiveShifts(memberId)).thenReturn(Optional.of(s));

        mockMvc.perform(get("/shifts/active/{memberId}", memberId)
                        .contentType(MediaType.APPLICATION_JSON))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.memberId").value(memberId.toString()))
                .andExpect(jsonPath("$.status").value(true));
    }

    @Test
    void testGetActiveShiftsNotFound() throws Exception {
        UUID memberId = UUID.randomUUID();

        when(mockService.getActiveShifts(memberId)).thenReturn(Optional.empty());

        mockMvc.perform(get("/shifts/active/{memberId}", memberId)
                        .contentType(MediaType.APPLICATION_JSON))
                .andExpect(status().isNotFound());
    }

    @Test
    void testGetAllShiftsSuccess() throws Exception {
        UUID memberId = UUID.randomUUID();
        Shift s1 = new Shift();
        s1.setMemberId(memberId);
        s1.setStartTime(LocalDateTime.of(2025,1,16,8,0));
        s1.setEndTime(LocalDateTime.of(2025,1,16,16,0));

        Shift s2 = new Shift();
        s2.setMemberId(memberId);
        s2.setStartTime(LocalDateTime.of(2025,1,17,8,0));
        s2.setEndTime(LocalDateTime.of(2025,1,17,16,0));

        List<Shift> list = List.of(s1, s2);

        when(mockService.getAllShifts(memberId)).thenReturn(Optional.of(list));

        mockMvc.perform(get("/shifts/all/{memberId}", memberId)
                        .contentType(MediaType.APPLICATION_JSON))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.shifts.length()").value(2))
                .andExpect(jsonPath("$.shifts[0].memberId").value(memberId.toString()));
    }

    @Test
    void testGetAllShiftsNotFound() throws Exception {
        UUID memberId = UUID.randomUUID();

        when(mockService.getAllShifts(memberId)).thenReturn(Optional.empty());

        mockMvc.perform(get("/shifts/all/{memberId}", memberId)
                        .contentType(MediaType.APPLICATION_JSON))
                .andExpect(status().isNotFound());
    }
}