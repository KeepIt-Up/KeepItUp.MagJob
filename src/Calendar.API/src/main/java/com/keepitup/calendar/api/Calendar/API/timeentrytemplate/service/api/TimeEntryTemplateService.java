package com.keepitup.calendar.api.Calendar.API.timeentrytemplate.service.api;

import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;

import java.time.ZonedDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface TimeEntryTemplateService {
    Optional<Page<TimeEntryTemplate>> findAllTimeEntryTemplatesByUser(UUID userId, PageRequest pageRequest);

    List<TimeEntryTemplate> findAll();

    Page<TimeEntryTemplate> findAll(Pageable pageable);

    Optional<TimeEntryTemplate> find(UUID id);

    void create(TimeEntryTemplate timeEntryTemplate);

    void delete(UUID id);

    void update(TimeEntryTemplate timeEntryTemplate);
}
