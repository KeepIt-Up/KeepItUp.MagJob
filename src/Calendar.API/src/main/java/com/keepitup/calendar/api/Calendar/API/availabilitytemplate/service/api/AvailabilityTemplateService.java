package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.service.api;

import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface AvailabilityTemplateService {
    Optional<Page<AvailabilityTemplate>> findAllAvailabilityTemplatesByUser(UUID userId, PageRequest pageRequest);

    List<AvailabilityTemplate> findAll();

    Page<AvailabilityTemplate> findAll(Pageable pageable);

    Optional<AvailabilityTemplate> find(UUID id);

    void create(AvailabilityTemplate availabilityTemplate);

    void delete(UUID id);

    void update(AvailabilityTemplate availabilityTemplate);
}
