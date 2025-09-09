package com.keepitup.workevidence.api.WorkEvidence.API.shift.entity;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.entity.ShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.Member.entity.Member;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotNull;
import lombok.*;
import lombok.experimental.SuperBuilder;

import java.math.BigInteger;
import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;

@Getter
@Setter
@SuperBuilder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Table(name = "shifts")
@Entity
public class Shift {

    @Id
    @Column(length = 254, unique = true, nullable = false, updatable = false)
    @GeneratedValue(generator = "UUID")
    private UUID id;

    @NotNull
    @Column(name = "startTime", nullable = false)
    private LocalDateTime startTime;

    @NotNull
    @Column(name = "endTime", nullable = false)
    private LocalDateTime endTime;

    @NotNull
    @Column(name="status", nullable = false)
    private boolean status;

    @NotNull
    @Column(name="description")
    private String description;

    @Column(name="MemberID",nullable=false)
    private UUID memberId;

    @OneToMany(mappedBy = "shift")
    private List<ShiftEditRequest> shiftEditRequests;

}
