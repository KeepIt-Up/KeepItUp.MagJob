package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.entity;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity.ChatMember;
import jakarta.persistence.*;
import lombok.*;
import lombok.experimental.SuperBuilder;

import java.math.BigInteger;
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
@Table(name = "chat_messages")
public class ChatMessage {
    @Id
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "chatMessageSequenceGenerator")
    @SequenceGenerator(name = "chatMessageSequenceGenerator")
    private UUID id;

    @Column(name = "content")
    private String content;

    @Column(name = "date_of_creation")
    private LocalDate dateOfCreation;

    @ElementCollection
    private List<String> viewedBy;

    @Lob
    private byte[] attachment;

    @ManyToOne
    @JoinColumn(name = "chat_id")
    private Chat chat;

    @ManyToOne
    @JoinColumn(name = "chat_member")
    private ChatMember chatMember;

    @Column(name = "first_and_last_name")
    private String firstAndLastName;
}
