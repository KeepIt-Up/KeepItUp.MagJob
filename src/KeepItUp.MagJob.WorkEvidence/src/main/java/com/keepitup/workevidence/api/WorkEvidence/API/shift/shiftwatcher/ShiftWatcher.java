package com.keepitup.workevidence.api.WorkEvidence.API.shift.shiftwatcher;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.*;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.service.impl.ShiftDefaultService;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import lombok.extern.java.Log;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ArrayNode;
import org.springframework.http.ResponseEntity;
import org.springframework.http.HttpMethod;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.client.RestTemplate;
import org.springframework.boot.web.client.RestTemplateBuilder;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Set;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;
import java.util.Optional;

@Component
@Log
public class ShiftWatcher {
    private final RestTemplate restTemplate;
    private final String CALENDAR_API = "http://apigateway:80/api/calendar/api/timeentrymembers/upcoming";
    private final ShiftDefaultService shiftService;
    public ShiftWatcher(RestTemplateBuilder builder, ShiftDefaultService shiftService) {
        this.restTemplate = builder.build();
        this.shiftService = shiftService;
    }

    @Scheduled(fixedRate = 300000) // Check every 5 minutes
    public void checkShifts() {
        try {
            String json = restTemplate.getForObject(CALENDAR_API, String.class);
            if (json == null || json.isBlank()) {
                log.info("ShiftWatcher: No upcoming shifts returned (empty body).");
                return;
            }
            log.info("ShiftWatcher: Fetched upcoming shifts JSON: " + json);
            
            ObjectMapper mapper = new ObjectMapper();
            JsonNode root = mapper.readTree(json);

            JsonNode timeEntryMemberListNode = root.get("timeEntryMemberList");
            if (timeEntryMemberListNode == null || !timeEntryMemberListNode.isArray()) {
                log.info("ShiftWatcher: No 'timeEntryMemberList' array found in response.");
                return;
            }

            for (JsonNode entryNode : timeEntryMemberListNode) {
                UUID memberId = entryNode.get("memberId").isNull() ? null : UUID.fromString(entryNode.get("memberId").asText());
                JsonNode timeEntryNode = entryNode.get("timeEntry");
                if (timeEntryNode == null || timeEntryNode.isNull()) {
                    log.warning("ShiftWatcher: Missing 'timeEntry' object for memberId: " + memberId);
                    continue;
                }

                String startDateTime = timeEntryNode.get("startDateTime").asText();
                String endDateTime = timeEntryNode.get("endDateTime").asText();
                if (memberId == null || startDateTime == null || endDateTime == null) {
                    log.warning("ShiftWatcher: Incomplete shift data for memberId: " + memberId);
                    continue;
                }
                Optional<Shift> activeShift = shiftService.getActiveShifts(memberId);
                if (activeShift.isPresent()) {
                    log.info("ShiftWatcher: Shift already active for memberId: " + memberId);
                    continue;
                }
                Shift newShift = new Shift();
                newShift.setMemberId(memberId);
                newShift.setStartTime(LocalDateTime.parse(startDateTime));
                newShift.setEndTime(LocalDateTime.parse(endDateTime));
                newShift.setDescription("Auto-generated shift");
                shiftService.startShift(newShift);
            }

        } catch (Exception e) {
            log.warning("ShiftWatcher: Error fetching or parsing upcoming shifts - " + e.getMessage());
            e.printStackTrace();
        }
    }
}