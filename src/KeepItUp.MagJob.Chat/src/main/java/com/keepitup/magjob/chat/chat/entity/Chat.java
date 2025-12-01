package com.keepitup.magjob.chat.chat.entity;

import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import com.keepitup.magjob.chat.chatmessage.entity.ChatMessage;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotNull;
import lombok.*;
import lombok.experimental.SuperBuilder;
import org.hibernate.annotations.GenericGenerator;

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
@Table(name = "Chats")
public class Chat {
    @Id
    @GeneratedValue(generator = "UUID")
    @GenericGenerator(
        name = "UUID",
        strategy = "org.hibernate.id.UUIDGenerator"
    )
    @Column(name = "id", updatable = false, nullable = false)
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
}
