package com.keepitup.calendar.api.Calendar.API.timeentrymember.service.api;

import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface TimeEntryMemberService {
    Optional<Page<TimeEntryMember>> findAllTimeEntryMembersByUser(UUID userId, PageRequest pageRequest);

    List<TimeEntryMember> findAll();

    Page<TimeEntryMember> findAll(Pageable pageable);

    Optional<TimeEntryMember> find(UUID id);

    void create(TimeEntryMember timeEntryTemplate);

    void delete(UUID id);

    void update(TimeEntryMember timeEntryTemplate);
}
