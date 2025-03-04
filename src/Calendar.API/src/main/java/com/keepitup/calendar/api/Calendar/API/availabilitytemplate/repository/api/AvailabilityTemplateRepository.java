package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.repository.api;

import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface AvailabilityTemplateRepository extends JpaRepository<AvailabilityTemplate, BigInteger> {
    Optional<AvailabilityTemplate> findById(UUID uuid);
}
