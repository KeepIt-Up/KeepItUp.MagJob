package com.keepitup.magjob.chat.chatmember.controller.impl;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chat.service.api.ChatService;
import com.keepitup.magjob.chat.chatmember.controller.api.ChatMemberController;
import com.keepitup.magjob.chat.chatmember.dto.*;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import com.keepitup.magjob.chat.chatmember.function.ChatMemberToResponseFunction;
import com.keepitup.magjob.chat.chatmember.function.ChatMembersToResponseFunction;
import com.keepitup.magjob.chat.chatmember.function.RequestToChatMemberFunction;
import com.keepitup.magjob.chat.chatmember.function.UpdateChatMemberWithRequestFunction;
import com.keepitup.magjob.chat.chatmember.service.api.ChatMemberService;
import com.keepitup.magjob.chat.configuration.Constants;
//import com.keepitup.magjob.chat.configuration.SecurityService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Controller;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigInteger;
import java.util.UUID;

@Controller
@Log
public class ChatMemberDefaultController implements ChatMemberController {
    private final ChatMemberService chatMemberService;
    //private final SecurityService securityService;
    private final ChatService chatService;
    private final SimpMessagingTemplate messagingTemplate;

    private final ChatMemberToResponseFunction chatMemberToResponseFunction;
    private final ChatMembersToResponseFunction chatMembersToResponseFunction;
    private final RequestToChatMemberFunction requestToChatMemberFunction;
    private final UpdateChatMemberWithRequestFunction updateChatMemberWithRequestFunction;

    @Autowired
    public ChatMemberDefaultController(
            ChatMemberService chatMemberService,
            //SecurityService securityService,
            ChatService chatService,
            SimpMessagingTemplate messagingTemplate,
            ChatMemberToResponseFunction chatMemberToResponseFunction,
            ChatMembersToResponseFunction chatMembersToResponseFunction,
            RequestToChatMemberFunction requestToChatMemberFunction,
            UpdateChatMemberWithRequestFunction updateChatMemberWithRequestFunction
    ) {
        this.chatMemberService = chatMemberService;
        //this.securityService = securityService;
        this.chatService = chatService;
        this.messagingTemplate = messagingTemplate;
        this.chatMemberToResponseFunction = chatMemberToResponseFunction;
        this.chatMembersToResponseFunction = chatMembersToResponseFunction;
        this.requestToChatMemberFunction = requestToChatMemberFunction;
        this.updateChatMemberWithRequestFunction = updateChatMemberWithRequestFunction;
    }

    @Override
    public GetChatMembersResponse getChatMembersByMember(int page, int size, UUID memberId) {
        PageRequest pageRequest = PageRequest.of(page, size);

//        Member member = memberService.find(memberId)
//                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
//
//        if (!member.equals(securityService.getCurrentMember(member.getOrganization()))) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }
//
//        if (!securityService.isOwner(member.getOrganization())) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        Integer count = chatMemberService.findAllByMemberId(memberId, Pageable.unpaged()).getNumberOfElements();

        return chatMembersToResponseFunction.apply(chatMemberService.findAllByMemberId(memberId, pageRequest), count);
    }

    @Override
    public GetChatMembersResponse getChatMembersByChat(int page, int size, UUID chatId) {
        PageRequest pageRequest = PageRequest.of(page, size);

        Chat chat = chatService.find(chatId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

//        if (!securityService.belongsToChat(chat, chat.getOrganization())) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        Integer count = chatMemberService.findAllByChat(chat, Pageable.unpaged()).getNumberOfElements();

        return chatMembersToResponseFunction.apply(chatMemberService.findAllByChat(chat, pageRequest), count);
    }

    @Override
    public GetChatMemberResponse createChatMember(PostChatMemberRequest postChatMemberRequest) {
        Chat chat = chatService.find(postChatMemberRequest.getChatId())
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

//        Member member = memberService.find(postChatMemberRequest.getMember())
//                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
//
//        if (!securityService.belongsToChat(chat, chat.getOrganization())) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }
//
//        if (!securityService.isChatAdmin(chat)) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

//        if (chatMemberService.findByMemberIdAndChat(member, chat).isPresent()) {
//            throw new ResponseStatusException(HttpStatus.CONFLICT);
//        }
//
        chatMemberService.create(requestToChatMemberFunction.apply(postChatMemberRequest));

        return chatMemberService.findByMemberIdAndChat(postChatMemberRequest.getMemberId(), chat)
                .map(chatMemberToResponseFunction)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public GetChatMemberResponse setNickname(UUID id, PatchChatMemberRequest patchChatMemberRequest) {
        ChatMember chatMember = chatMemberService.find(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

//        if (!chatMember.getMember().equals(securityService.getCurrentMember(chatMember.getChat().getOrganization()))) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        chatMemberService.update(updateChatMemberWithRequestFunction.apply(chatMember, patchChatMemberRequest));
        ChatMember chatMemberAfterUpdate = chatMemberService.find(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        return chatMemberToResponseFunction.apply(chatMemberAfterUpdate);
    }

    @Override
    public void deleteChatMember(UUID id) {
        ChatMember chatMember = chatMemberService.find(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        // Usuwamy członka z czatu
        chatMemberService.delete(id);
    }
}
