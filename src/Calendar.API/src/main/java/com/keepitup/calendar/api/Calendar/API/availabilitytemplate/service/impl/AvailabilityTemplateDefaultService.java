package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.service.impl;

import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.repository.api.AvailabilityTemplateRepository;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.service.api.AvailabilityTemplateService;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class AvailabilityTemplateDefaultService implements AvailabilityTemplateService {
    private final AvailabilityTemplateRepository timeEntryTemplateRepository;

    @Autowired
    public AvailabilityTemplateDefaultService(AvailabilityTemplateRepository timeEntryTemplateRepository) {
        this.timeEntryTemplateRepository = timeEntryTemplateRepository;
    }

    @Override
    public Optional<Page<AvailabilityTemplate>> findAllAvailabilityTemplatesByUser(UUID userId, PageRequest pageRequest) {
        return Optional.empty();
    }

    @Override
    public List<AvailabilityTemplate> findAll() {
        return timeEntryTemplateRepository.findAll();
    }

    @Override
    public Page<AvailabilityTemplate> findAll(Pageable pageable) {
        return timeEntryTemplateRepository.findAll(pageable);
    }

    @Override
    public Optional<AvailabilityTemplate> find(UUID id) {
        return timeEntryTemplateRepository.findById(id);
    }

    @Override
    public void create(AvailabilityTemplate timeEntryTemplate) {
        timeEntryTemplateRepository.save(timeEntryTemplate);
    }

    @Override
    public void delete(UUID id) {
        timeEntryTemplateRepository.findById(id).ifPresent(timeEntryTemplateRepository::delete);
    }

    @Override
    public void update(AvailabilityTemplate timeEntryTemplate) {
        timeEntryTemplateRepository.save(timeEntryTemplate);
    }
}
