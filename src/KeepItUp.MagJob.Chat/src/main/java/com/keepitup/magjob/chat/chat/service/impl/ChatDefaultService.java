package com.keepitup.magjob.chat.chat.service.impl;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chat.repository.api.ChatRepository;
import com.keepitup.magjob.chat.chat.service.api.ChatService;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.math.BigInteger;
import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class ChatDefaultService implements ChatService {
    private final ChatRepository chatRepository;

    @Autowired
    public ChatDefaultService(ChatRepository chatRepository) {
        this.chatRepository = chatRepository;
    }

    @Override
    public Optional<Chat> find(UUID id) {
        return chatRepository.findById(id);
    }

    @Override
    public Optional<Chat> findByTitle(String title) {
        return chatRepository.findByTitle(title);
    }

    @Override
    public List<Chat> findAll() {
        return chatRepository.findAll();
    }

    @Override
    public Page<Chat> findAll(Pageable pageable) {
        return chatRepository.findAll(pageable);
    }

    @Override
    public Page<Chat> findAllByOrganizationId(UUID organizationId, Pageable pageable) {
        return chatRepository.findAllByOrganizationId(organizationId, pageable);
    }

    @Override
    public void create(Chat chat) {
        chat.setDateOfCreation(LocalDate.now());
        chatRepository.save(chat);
    }

    @Override
    public void delete(UUID id) {
        chatRepository.findById(id).ifPresent(chatRepository::delete);
    }

    @Override
    public void update(Chat chat) {
        chatRepository.save(chat);
    }
}
