package com.keepitup.calendar.api.Calendar.API.timeentrytemplate.repository.api;

import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.*;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.math.BigInteger;
import java.time.ZonedDateTime;
import java.util.Optional;

@Repository
public interface TimeEntryTemplateRepository extends JpaRepository<TimeEntryTemplate, BigInteger> {
    Optional<TimeEntryTemplate> findByName(String name);

    Page<TimeEntryTemplate> findAllByDateOfCreation(ZonedDateTime dateOfCreation, Pageable pageable);
}
