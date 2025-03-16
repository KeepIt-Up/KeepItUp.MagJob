package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.entity;

import jakarta.persistence.*;
import lombok.*;
import lombok.experimental.SuperBuilder;

import java.time.LocalDateTime;
import java.util.UUID;

@Getter
@Setter
@SuperBuilder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Entity
@Table(name = "notifications")
public class Notification {
    @Id
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "notificationSequenceGenerator")
    private UUID id;

    @Column(name = "date_of_creation")
    private LocalDateTime dateOfCreation;

    @Column(name = "content")
    private String content;

    @Column(name = "seen")
    private boolean seen;

    @Column(name = "sent")
    private boolean sent;

    @Column(name = "userId")
    private UUID userId;

    @Column(name = "memberId")
    private UUID memberId;

    @Column(name = "organizationId")
    private UUID organizationId;
}

