package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.service.impl;

import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.repository.api.AvailabilityTemplateRepository;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.service.api.AvailabilityTemplateService;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
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
    private final AvailabilityTemplateRepository availabilityTemplateRepository;

    @Autowired
    public AvailabilityTemplateDefaultService(AvailabilityTemplateRepository availabilityTemplateRepository) {
        this.availabilityTemplateRepository = availabilityTemplateRepository;
    }

    @Override
    public Optional<Page<AvailabilityTemplate>> findAllAvailabilityTemplatesByUser(UUID userId, PageRequest pageRequest) {
        return Optional.empty();
    }

    @Override
    public List<AvailabilityTemplate> findAll() {
        return availabilityTemplateRepository.findAll();
    }

    @Override
    public Page<AvailabilityTemplate> findAll(Pageable pageable) {
        return availabilityTemplateRepository.findAll(pageable);
    }

    @Override
    public Optional<AvailabilityTemplate> find(UUID id) {
        System.out.println("there there");
      System.out.println(availabilityTemplateRepository.findById(id).isPresent());
      System.out.println(availabilityTemplateRepository.findById(id).get().getName());
        return availabilityTemplateRepository.findById(id);
    }

  @Override
  public AvailabilityTemplate create(AvailabilityTemplate availabilityTemplate) {
    return availabilityTemplateRepository.save(availabilityTemplate);
  }

  @Override
    public void delete(UUID id) {
        availabilityTemplateRepository.findById(id).ifPresent(availabilityTemplateRepository::delete);
    }

    @Override
    public void update(AvailabilityTemplate availabilityTemplate) {
        availabilityTemplateRepository.save(availabilityTemplate);
    }
}
