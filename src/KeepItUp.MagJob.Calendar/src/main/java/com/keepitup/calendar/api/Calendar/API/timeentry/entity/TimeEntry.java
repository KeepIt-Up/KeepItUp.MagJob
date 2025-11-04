package com.keepitup.calendar.api.Calendar.API.timeentry.entity;
import com.fasterxml.jackson.annotation.JsonBackReference;
import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotNull;
import lombok.*;
import lombok.experimental.SuperBuilder;

import java.math.BigInteger;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.UUID;

@Getter
@Setter
@SuperBuilder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Table(name = "time_entrys")
@Entity
public class TimeEntry {
    @Id
    @Column(length = 254, unique = true, nullable = false, updatable = false)
    @GeneratedValue(generator = "UUID")
    private UUID id;

    @NotNull
    @Column(name = "start_date_time", nullable = false)
    private LocalDateTime startDateTime;

    @NotNull
    @Column(name = "end_date_time", nullable = false)
    private LocalDateTime endDateTime;

    @JsonBackReference
    @ManyToOne
    @JoinColumn(name = "graphic", referencedColumnName = "id", columnDefinition = "uuid")
    private Graphic graphic;
}
