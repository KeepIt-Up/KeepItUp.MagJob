package com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity.ChatMember;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.entity.ChatMessage;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotNull;
import lombok.*;
import lombok.experimental.SuperBuilder;

import java.time.LocalDate;
import java.util.List;
import java.util.UUID;

@Getter
@Setter
@SuperBuilder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Entity
public class Chat {
    @Id
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "chatSequenceGenerator")
    @SequenceGenerator(name = "chatSequenceGenerator")
    private UUID id;

    @Column(name = "date_of_creation")
    private LocalDate dateOfCreation;

    @NotNull
    @Column(name = "title", nullable = false)
    private String title;

    @Column(name = "organizationId", nullable = false)
    private UUID organizationId;

    @OneToMany(mappedBy = "chat")
    private List<ChatMessage> chatMessages;

    @OneToMany(mappedBy = "chat", cascade = CascadeType.REMOVE)
    private List<ChatMember> chatMembers;

    @OneToMany(mappedBy = "chat", cascade = CascadeType.REMOVE)
    private List<ChatMember> chatAdministrators;
}
