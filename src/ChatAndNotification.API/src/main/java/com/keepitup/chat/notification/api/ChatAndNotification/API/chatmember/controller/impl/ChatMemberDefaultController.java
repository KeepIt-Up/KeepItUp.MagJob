package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.controller.impl;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.service.api.ChatService;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.controller.api.ChatMemberController;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.dto.*;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity.ChatMember;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.function.ChatMemberToResponseFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.function.ChatMembersToResponseFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.function.RequestToChatMemberFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.function.UpdateChatMemberWithRequestFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.service.api.ChatMemberService;
import com.keepitup.chat.notification.api.ChatAndNotification.API.configuration.Constants;
//import com.keepitup.chat.notification.api.ChatAndNotification.API.configuration.SecurityService;
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

        Integer count = chatMemberService.findAllAcceptedMembers(chat, Pageable.unpaged()).getNumberOfElements();

        return chatMembersToResponseFunction.apply(chatMemberService.findAllAcceptedMembers(chat, pageRequest), count);
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
    public void acceptInvitation(AcceptInvitationToChatRequest request) {
        Chat chat = chatService.find(request.getChat())
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

//            Member member = memberService.find(request.getMember())
//                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

//        if (!securityService.getCurrentMember(chat.getOrganization()).equals(member)) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        ChatMember invitation = chatMemberService.findByMemberIdAndChat(request.getMember(), chat)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        messagingTemplate.convertAndSend(
                Constants.CHAT_DEFAULT_WEBSOCKET_ENDPOINT + chat.getId(),
                String.join(Constants.CHAT_JOIN_MESSAGE, invitation.getNickname())
        );

        chatMemberService.acceptInvitation(invitation);
    }

    @Override
    public void rejectInvitation(AcceptInvitationToChatRequest request) {
        Chat chat = chatService.find(request.getChat())
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

//        Member member = memberService.find(request.getMember())
//                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

//        if (!securityService.getCurrentMember(chat.getOrganization()).equals(member)) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        ChatMember invitation = chatMemberService.findByMemberIdAndChat(request.getMember(), chat)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        messagingTemplate.convertAndSend(
                Constants.CHAT_LEAVE_MESSAGE + invitation.getChat().getId(),
                String.join(Constants.CHAT_DELETE_ADMIN_MESSAGE, invitation.getNickname())
        );

        chatMemberService.delete(invitation.getId());
    }

    @Override
    public void removeAdminAccess(UUID id) {
        ChatMember chatMember = chatMemberService.find(id).orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        if (!chatMember.getChat().getChatAdministrators().contains(chatMember)) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }

//        if (!securityService.isChatAdmin(chatMember.getChat())) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        messagingTemplate.convertAndSend(
                Constants.CHAT_DEFAULT_WEBSOCKET_ENDPOINT + chatMember.getChat().getId(),
                String.join(Constants.CHAT_DELETE_ADMIN_MESSAGE, chatMember.getNickname())
        );

        chatService.removeAdmin(chatMember.getChat(), chatMember);
    }

    @Override
    public void giveAdminAccess(UUID id) {
        ChatMember chatMember = chatMemberService.find(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        if (chatMember.getChat().getChatAdministrators().contains(chatMember)) {
            throw new ResponseStatusException(HttpStatus.CONFLICT);
        }

//        if (!securityService.isChatAdmin(chatMember.getChat())) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        messagingTemplate.convertAndSend(
                Constants.CHAT_DEFAULT_WEBSOCKET_ENDPOINT + chatMember.getChat().getId(),
                String.join(Constants.CHAT_ADD_ADMIN_MESSAGE, chatMember.getNickname())
        );

        chatService.addAdmin(chatMember.getChat(), chatMember);
    }
}
