package com.keepitup.calendar.api.Calendar.API.timeentry.repository.api;

import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.UUID;

@Repository
public interface TimeEntryRepository extends JpaRepository<TimeEntry, UUID> {
}
