package com.keepitup.calendar.api.Calendar.API.timeentrymember.entity;

import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import com.keepitup.calendar.api.Calendar.API.member.entity.Member;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotNull;
import lombok.*;
import lombok.experimental.SuperBuilder;

import java.math.BigInteger;
import java.util.UUID;

@Getter
@Setter
@SuperBuilder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Entity
@Table(name = "time_entry_members")
public class TimeEntryMember {
    @Id
    @Column(length = 254, unique = true, nullable = false, updatable = false)
    private UUID id;

    @NotNull
    @Column(name = "status", nullable = false)
    private String status;

    // Many-to-One relationship with Member
    @ManyToOne
    @JoinColumn(name = "member")
    private Member member;

    // Many-to-One relationship with TimeEntry
    @ManyToOne
    @JoinColumn(name = "time_entry")
    private TimeEntry timeEntry;

    // Getters and setters
}
