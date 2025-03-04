package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.entity;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotNull;
import lombok.*;
import lombok.experimental.SuperBuilder;

import java.math.BigInteger;
import java.time.LocalDateTime;
import java.util.UUID;

@Getter
@Setter
@SuperBuilder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Table(name = "shiftEditRequests")
@Entity
public class ShiftEditRequest {

    @Id
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "memberSequenceGenerator")
    @SequenceGenerator(name = "memberSequenceGenerator")
    private BigInteger id;
    @NotNull
    @Column(name = "status", nullable = false)
    private String status;

    @NotNull
    @Column(name = "startTime", nullable = false)
    private LocalDateTime startTime;

    @NotNull
    @Column(name = "endTime", nullable = false)
    private LocalDateTime endTime;

    @ManyToOne
    @JoinColumn(name="Shift")
    private Shift shift;

}
