package com.keepitup.calendar.api.Calendar.API.timeentry.service.api;

import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface TimeEntryService {
    Optional<Page<TimeEntry>> findAllTimeEntrysByUser(UUID userId, PageRequest pageRequest);

    List<TimeEntry> findAll();

    Page<TimeEntry> findAll(Pageable pageable);

    Optional<TimeEntry> find(UUID id);

    void create(TimeEntry timeEntry);

    void delete(UUID id);

    void update(TimeEntry timeEntry);
}
