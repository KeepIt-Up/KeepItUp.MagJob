package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.entity;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity.ChatMember;
import jakarta.persistence.*;
import lombok.*;
import lombok.experimental.SuperBuilder;
import org.hibernate.annotations.GenericGenerator;

import java.time.LocalDateTime;
import java.util.ArrayList;
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
@Table(name = "ChatMessages")
public class ChatMessage {
    @Id
    @GeneratedValue(generator = "UUID")
    @GenericGenerator(
        name = "UUID",
        strategy = "org.hibernate.id.UUIDGenerator"
    )
    @Column(name = "id", updatable = false, nullable = false)
    private UUID id;

    @Column(name = "content")
    private String content;

    @Column(name = "date_of_creation")
    private LocalDateTime dateOfCreation;

    @ElementCollection
    private List<String> viewedBy;

    @Lob
    @Column(name = "attachment")
    private byte[] attachment;

    @ManyToOne(optional = true)
    @JoinColumn(name = "chat_id", nullable = true)
    private Chat chat;

    @ManyToOne(optional = true)
    @JoinColumn(name = "chat_member", nullable = true)
    private ChatMember chatMember;

    @Column(name = "first_and_last_name")
    private String firstAndLastName;
}
