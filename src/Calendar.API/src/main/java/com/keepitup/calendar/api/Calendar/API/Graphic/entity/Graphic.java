package com.keepitup.calendar.api.Calendar.API.Graphic.entity;

import com.fasterxml.jackson.core.JsonToken;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import jakarta.persistence.*;
import lombok.*;

import java.math.BigInteger;
import java.util.List;
import java.util.UUID;

@Entity
@Data
@NoArgsConstructor
@AllArgsConstructor
@RequiredArgsConstructor
@Builder
public class Graphic {
    @Id
    @Column(length = 254, unique = true, nullable = false, updatable = false)
    private UUID id;

    @NonNull
    @Column(name = "name", nullable = false)
    private String name;

    @NonNull
    @Column(name = "managerId", nullable = false)
    private BigInteger managerId;

    @OneToMany
    private List<TimeEntryMember> timeEntryMembers;
}
