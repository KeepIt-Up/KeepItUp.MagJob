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
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "memberSequenceGenerator")
    @SequenceGenerator(name = "memberSequenceGenerator")
    private BigInteger id;

    @NotNull
    @Column(name = "startTime", nullable = false)
    private LocalDateTime startTime;

    @NotNull
    @Column(name = "endTime", nullable = false)
    private LocalDateTime endTime;

    @NotNull
    @Column(name="description")
    private String description;

    @ManyToOne
    @JoinColumn(name="Member")
    private Member member;

    @OneToMany(mappedBy = "shift")
    private List<ShiftEditRequest> shiftEditRequests;

}
