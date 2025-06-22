package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity;
import com.fasterxml.jackson.annotation.JsonManagedReference;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotNull;
import lombok.*;
import lombok.experimental.SuperBuilder;
import org.hibernate.annotations.GenericGenerator;

import java.math.BigInteger;
import java.util.List;
import java.util.UUID;


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
    @Column(length = 254, unique = true, nullable = false, updatable = false)
    @GeneratedValue(generator = "UUID")
    private UUID id;

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

    @OneToMany(
            mappedBy = "availabilityTemplate",
            cascade = CascadeType.ALL, // Cascade ALL operations to children
            orphanRemoval = true
    )
    @JsonManagedReference
    @ToString.Exclude
    private List<TimeEntryTemplate> timeEntryTemplates;
}
