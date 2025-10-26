package com.keepitup.calendar.api.Calendar.API.timeentrymember.repository.api;

import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.math.BigInteger;
import java.time.LocalDateTime;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface TimeEntryMemberRepository extends JpaRepository<TimeEntryMember, BigInteger> {
    Optional<TimeEntryMember> findById(UUID uuid);
    
    @Query("SELECT tem FROM TimeEntryMember tem " +
           "JOIN tem.timeEntry te " +
           "WHERE tem.status = :status " +
           "AND te.startDateTime BETWEEN :startTime AND :endTime")
    Page<TimeEntryMember> findByStatusAndTimeEntryStartDateTimeBetween(
        @Param("status") String status,
        @Param("startTime") LocalDateTime startTime,
        @Param("endTime") LocalDateTime endTime,
        Pageable pageable
    );
    
    @Query("SELECT tem FROM TimeEntryMember tem " +
           "WHERE tem.memberId = :userId " +
           "AND tem.status IN ('Pending', 'Confirmed')")
    Page<TimeEntryMember> findByUserId(
        @Param("userId") UUID userId,
        Pageable pageable
    );
    
    @Query("SELECT tem FROM TimeEntryMember tem " +
           "JOIN tem.timeEntry te " +
           "WHERE te.graphic.id = :graphicId")
    Page<TimeEntryMember> findByGraphicId(
        @Param("graphicId") UUID graphicId,
        Pageable pageable
    );
}
