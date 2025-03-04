package com.keepitup.calendar.api.Calendar.API.timeentrymember.repository.api;

import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface TimeEntryMemberRepository extends JpaRepository<TimeEntryMember, BigInteger> {
    Optional<TimeEntryMember> findById(UUID uuid);
}
