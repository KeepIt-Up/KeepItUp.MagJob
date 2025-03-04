package com.keepitup.calendar.api.Calendar.API.timeentrymember.service.impl;

import com.keepitup.calendar.api.Calendar.API.timeentrymember.service.api.TimeEntryMemberService;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.repository.api.TimeEntryMemberRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class TimeEntryMemberDefaultService implements TimeEntryMemberService {
    private final TimeEntryMemberRepository timeEntryMemberRepository;

    @Autowired
    public TimeEntryMemberDefaultService(TimeEntryMemberRepository timeEntryMemberRepository) {
        this.timeEntryMemberRepository = timeEntryMemberRepository;
    }

    @Override
    public Optional<Page<TimeEntryMember>> findAllTimeEntryMembersByUser(UUID userId, PageRequest pageRequest) {
        return Optional.empty();
    }

    @Override
    public List<TimeEntryMember> findAll() {
        return timeEntryMemberRepository.findAll();
    }

    @Override
    public Page<TimeEntryMember> findAll(Pageable pageable) {
        return timeEntryMemberRepository.findAll(pageable);
    }

    @Override
    public Optional<TimeEntryMember> find(UUID id) {
        return timeEntryMemberRepository.findById(id);
    }

    @Override
    public void create(TimeEntryMember timeEntryMember) {
        timeEntryMemberRepository.save(timeEntryMember);
    }

    @Override
    public void delete(UUID id) {
        timeEntryMemberRepository.findById(id).ifPresent(timeEntryMemberRepository::delete);
    }

    @Override
    public void update(TimeEntryMember timeEntryMember) {
        timeEntryMemberRepository.save(timeEntryMember);
    }
}
