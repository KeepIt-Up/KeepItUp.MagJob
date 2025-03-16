package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.entity.ChatMessage;
import jakarta.persistence.*;
import lombok.*;
import lombok.experimental.SuperBuilder;

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
@Entity
@Table(name = "chat_members")
public class ChatMember {
    @Id
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "chatMemberSequenceGenerator")
    @SequenceGenerator(name = "chatMemberSequenceGenerator")
    private UUID id;

    @Column(name = "nickname")
    private String nickname;

    @Column(name = "is_invitation_accepted")
    private Boolean isInvitationAccepted;

    @Column(name = "memberId")
    private UUID memberId;

    @OneToMany(mappedBy = "chatMember")
    private List<ChatMessage> chatMessages;

    @ManyToOne
    @JoinColumn(name = "chat_id")
    private Chat chat;
}
