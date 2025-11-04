package com.keepitup.magjob.chat.chatmember.entity;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chatmessage.entity.ChatMessage;
import jakarta.persistence.*;
import lombok.*;
import lombok.experimental.SuperBuilder;
import org.hibernate.annotations.GenericGenerator;

import java.math.BigInteger;
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
@Table(name = "ChatMembers")
public class ChatMember {
    @Id
    @GeneratedValue(generator = "UUID")
    @GenericGenerator(
        name = "UUID",
        strategy = "org.hibernate.id.UUIDGenerator"
    )
    @Column(name = "id", updatable = false, nullable = false)
    private UUID id;

    @Column(name = "nickname")
    private String nickname;


    @Column(name = "memberId")
    private UUID memberId;

    @OneToMany(mappedBy = "chatMember")
    private List<ChatMessage> chatMessages;

    @ManyToOne
    @JoinColumn(name = "chat_id")
    private Chat chat;
}
