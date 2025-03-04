package com.keepitup.calendar.api.Calendar.API.timeentry.service.impl;

import com.keepitup.calendar.api.Calendar.API.timeentry.service.api.TimeEntryService;
import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import com.keepitup.calendar.api.Calendar.API.timeentry.repository.api.TimeEntryRepository;
import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class TimeEntryDefaultService implements TimeEntryService {
    private final TimeEntryRepository timeEntryRepository;

    @Autowired
    public TimeEntryDefaultService(TimeEntryRepository timeEntryRepository) {
        this.timeEntryRepository = timeEntryRepository;
    }


    @Override
    public Optional<Page<TimeEntry>> findAllTimeEntrysByUser(UUID userId, PageRequest pageRequest) {
        return Optional.empty();
    }

    @Override
    public List<TimeEntry> findAll() {
        return timeEntryRepository.findAll();
    }

    @Override
    public Page<TimeEntry> findAll(Pageable pageable) {
        return timeEntryRepository.findAll(pageable);
    }

    @Override
    public Optional<TimeEntry> find(UUID id) {
        return timeEntryRepository.findById(id);
    }

    @Override
    public void create(TimeEntry timeEntry) {
        timeEntryRepository.save(timeEntry);
    }

    @Override
    public void delete(UUID id) {
        timeEntryRepository.findById(id).ifPresent(timeEntryRepository::delete);
    }

    @Override
    public void update(TimeEntry timeEntry) {
        timeEntryRepository.save(timeEntry);
    }
}
