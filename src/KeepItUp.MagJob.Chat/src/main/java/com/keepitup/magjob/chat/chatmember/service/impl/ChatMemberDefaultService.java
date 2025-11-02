package com.keepitup.magjob.chat.chatmember.service.impl;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import com.keepitup.magjob.chat.chatmessage.entity.ChatMessage;
import com.keepitup.magjob.chat.chatmessage.repository.api.ChatMessageRepository;
import com.keepitup.magjob.chat.chatmember.repository.api.ChatMemberRepository;
import com.keepitup.magjob.chat.chatmember.service.api.ChatMemberService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.Optional;
import java.util.UUID;

@Service
public class ChatMemberDefaultService implements ChatMemberService {
    private final ChatMemberRepository chatMemberRepository;
    private final ChatMessageRepository chatMessageRepository;

    @Autowired
    public ChatMemberDefaultService(
            ChatMemberRepository chatMemberRepository,
            ChatMessageRepository chatMessageRepository
    ) {
        this.chatMemberRepository = chatMemberRepository;
        this.chatMessageRepository = chatMessageRepository;
    }

    @Override
    public Page<ChatMember> findAllByMemberId(UUID memberId, Pageable pageable) {
        return chatMemberRepository.findAllByMemberId(memberId, pageable);
    }

    @Override
    public Page<ChatMember> findAllByChat(Chat chat, Pageable pageable) {
        return chatMemberRepository.findAllByChat(chat, pageable);
    }

    @Override
    public Page<Chat> findAllChatsByMemberId(UUID memberId, Pageable pageable) {
        return chatMemberRepository.findAllByMemberId(memberId, pageable).map(ChatMember::getChat);
    }

    @Override
    public Optional<ChatMember> find(UUID id) {
        return chatMemberRepository.findById(id);
    }

    @Override
    public Optional<ChatMember> findByMemberIdAndChat(UUID memberId, Chat chat) {
        return chatMemberRepository.findByMemberIdAndChat(memberId, chat);
    }

    @Override
    public void create(ChatMember chatMember) {
        chatMemberRepository.save(chatMember);
    }

    @Override
    public void delete(UUID id) {
        chatMemberRepository.findById(id).ifPresent(chatMember -> {
            if (chatMember.getChatMessages() != null && !chatMember.getChatMessages().isEmpty()) {
                for (ChatMessage message : chatMember.getChatMessages()) {
                    message.setChatMember(null);
                }
                chatMessageRepository.saveAll(chatMember.getChatMessages());
            }
            chatMemberRepository.delete(chatMember);
        });
    }

    @Override
    public void update(ChatMember chatMember) {
        chatMemberRepository.save(chatMember);
    }
}
