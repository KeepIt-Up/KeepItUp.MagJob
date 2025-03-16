package com.keepitup.chat.notification.api.ChatAndNotification.API.chat.controller.impl;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.controller.api.ChatController;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto.GetChatResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto.GetChatsResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto.PatchChatRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto.PostChatRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.function.ChatToResponseFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.function.ChatsToResponseFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.function.RequestToChatFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.function.UpdateChatWithRequestFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.service.impl.ChatDefaultService;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity.ChatMember;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.service.api.ChatMemberService;
//import com.keepitup.chat.notification.api.ChatAndNotification.API.configuration.SecurityService;
import com.keepitup.chat.notification.api.ChatAndNotification.API.jwt.CustomJwt;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Controller;
import org.springframework.web.server.ResponseStatusException;

import java.util.Optional;
import java.util.UUID;

@Controller
@Log
public class ChatDefaultController implements ChatController {
    private final ChatDefaultService chatService;
    //private final SecurityService securityService;
    private final ChatMemberService chatMemberService;

    private final ChatToResponseFunction chatToResponseFunction;
    private final ChatsToResponseFunction chatsToResponseFunction;
    private final RequestToChatFunction requestToChatFunction;
    private final UpdateChatWithRequestFunction updateChatWithRequestFunction;

    @Autowired
    public ChatDefaultController(
            ChatDefaultService chatService,
            //SecurityService securityService,
            ChatMemberService chatMemberService,
            ChatToResponseFunction chatToResponseFunction,
            ChatsToResponseFunction chatsToResponseFunction,
            RequestToChatFunction requestToChatFunction,
            UpdateChatWithRequestFunction updateChatWithRequestFunction
    ) {
        this.chatService = chatService;
        //this.securityService = securityService;
        this.chatMemberService = chatMemberService;
        this.chatToResponseFunction = chatToResponseFunction;
        this.chatsToResponseFunction = chatsToResponseFunction;
        this.requestToChatFunction = requestToChatFunction;
        this.updateChatWithRequestFunction = updateChatWithRequestFunction;
    }

    @Override
    public GetChatsResponse getChats(int page, int size) {
//        if (!securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        PageRequest pageRequest = PageRequest.of(page, size);
        Integer count = chatService.findAll().size();
        return chatsToResponseFunction.apply(chatService.findAll(pageRequest), count);
    }

    @Override
    public GetChatResponse getChat(UUID id) {
        Chat chat = chatService.find(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

//        Member member = securityService.getCurrentMember(chat.getOrganization());
//
//        boolean isChatMember = chat.getChatMembers().stream()
//                .anyMatch(chatMember -> chatMember.getMember().equals(member));
//
//        if (!isChatMember) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        return chatToResponseFunction.apply(chat);
    }

    @Override
    public GetChatsResponse getChatsByOrganization(int page, int size, UUID organizationId) {
        PageRequest pageRequest = PageRequest.of(page, size);

//        Optional<Organization> organizationOptional = organizationService.find(organizationId);
//
//        Organization organization = organizationOptional
//                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
//
//        if (!securityService.isOwner(organization)) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        Integer count = chatService.findAllByOrganizationId(organizationId, Pageable.unpaged()).getNumberOfElements();

        return chatsToResponseFunction.apply(chatService.findAllByOrganizationId(organizationId, pageRequest), count);
    }

    @Override
    public GetChatsResponse getChatsByMember(int page, int size, UUID memberId) {
//        Member member = memberService.find(memberId)
//                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
//
//        if (!securityService.getCurrentMember(member.getOrganization()).getId().equals(memberId)) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Integer count = chatMemberService.findAllChatsByMemberId(memberId, Pageable.unpaged()).getNumberOfElements();
        Page<Chat> chats = chatMemberService.findAllChatsByMemberId(memberId, pageRequest);

        return chatsToResponseFunction.apply(chats, count);
    }

    @Override
    public GetChatResponse createChat(PostChatRequest postChatRequest) {
//        Optional<Organization> organizationOptional = organizationService.find(postChatRequest.getOrganization());
//
//        Organization organization = organizationOptional
//                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
//
//        if(!securityService.belongsToOrganization(organization)) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }
//
//        Member member = securityService.getCurrentMember(organization);

        chatService.create(requestToChatFunction.apply(postChatRequest));
        Optional<Chat> createdChat = chatService.findByTitle(postChatRequest.getTitle());

        if (createdChat.isPresent()) {
            chatMemberService.acceptInvitation(ChatMember.builder()
                    .chat(createdChat.get())
                    .nickname("Test")
                    .memberId(postChatRequest.getMemberId())
                    .build());

            ChatMember adminChatMember = chatMemberService.findByMemberIdAndChat(postChatRequest.getMemberId(), createdChat.get())
                    .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

            chatService.addAdmin(createdChat.get(), adminChatMember);
        }

        return chatService.findByTitle(postChatRequest.getTitle())
                .map(chatToResponseFunction)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public GetChatResponse updateChat(UUID id, PatchChatRequest patchChatRequest) {
        Chat chat = chatService.find(id).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

//        if (!securityService.isChatAdmin(chat)) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        chatService.update(updateChatWithRequestFunction.apply(chat, patchChatRequest));

        return chatService.find(id)
                .map(chatToResponseFunction)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public void deleteChat(UUID id) {
        Chat chat = chatService.find(id).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

//        if (!securityService.isChatAdmin(chat)) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        chatService.delete(id);
    }
}
