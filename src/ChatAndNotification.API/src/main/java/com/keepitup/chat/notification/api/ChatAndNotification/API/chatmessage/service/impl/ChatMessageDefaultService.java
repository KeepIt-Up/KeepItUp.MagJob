package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.service.impl;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.entity.ChatMessage;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.repository.api.ChatMessageRepository;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.service.api.ChatMessageService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.math.BigInteger;
import java.time.LocalDate;
import java.util.Optional;
import java.util.UUID;

@Service
public class ChatMessageDefaultService implements ChatMessageService {
    private final ChatMessageRepository chatMessageRepository;

    @Autowired
    public ChatMessageDefaultService(ChatMessageRepository chatMessageRepository) {
        this.chatMessageRepository = chatMessageRepository;
    }

    @Override
    public Optional<ChatMessage> find(UUID id) {
        return chatMessageRepository.findById(id);
    }

    @Override
    public Page<ChatMessage> findAllByChat(Chat chat, Pageable pageable) {
        return chatMessageRepository.findAllByChat(chat, pageable);
    }

    @Override
    public ChatMessage create(ChatMessage chatMessage) {
//        String firstName = chatMessage.getChatMember().getMember().getUser().getFirstname();
//        String lastName = chatMessage.getChatMember().getMember().getUser().getLastname();
//
//        chatMessage.setDateOfCreation(LocalDate.now());
//        chatMessage.setFirstAndLastName(String.join(" ", firstName, lastName));
//        chatMessageRepository.save(chatMessage);
//        return chatMessage;
        return null;
    }

    @Override
    public void update(ChatMessage chatMessage) {
        chatMessageRepository.save(chatMessage);
    }
}
