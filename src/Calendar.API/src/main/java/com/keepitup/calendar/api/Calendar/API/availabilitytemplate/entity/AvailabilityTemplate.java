package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotNull;
import lombok.*;
import lombok.experimental.SuperBuilder;

import java.math.BigInteger;


@Getter
@Setter
@SuperBuilder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Table(name = "availability_templates")
@Entity
public class AvailabilityTemplate {

    @Id
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "memberSequenceGenerator")
    @SequenceGenerator(name = "memberSequenceGenerator")
    private BigInteger id;

    @NotNull
    @Column(name = "name", nullable = false)
    private String name;

    @NotNull
    @Column(name = "organizationId", nullable = false)
    private BigInteger organizationId;

    @NotNull
    @Column(name = "startDayOfWeek", nullable = false)
    private String startDayOfWeek;

    @NotNull
    @Column(name = "numberOfDays", nullable = false)
    private Integer numberOfDays;

    // Getters and setters
}