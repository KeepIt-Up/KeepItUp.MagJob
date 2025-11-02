package com.keepitup.magjob.chat.chat.controller.impl;

import com.keepitup.magjob.chat.chat.controller.api.ChatController;
import com.keepitup.magjob.chat.chat.dto.GetChatResponse;
import com.keepitup.magjob.chat.chat.dto.GetChatsResponse;
import com.keepitup.magjob.chat.chat.dto.PatchChatRequest;
import com.keepitup.magjob.chat.chat.dto.PostChatRequest;
import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chat.function.ChatToResponseFunction;
import com.keepitup.magjob.chat.chat.function.ChatsToResponseFunction;
import com.keepitup.magjob.chat.chat.function.RequestToChatFunction;
import com.keepitup.magjob.chat.chat.function.UpdateChatWithRequestFunction;
import com.keepitup.magjob.chat.chat.service.impl.ChatDefaultService;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import com.keepitup.magjob.chat.chatmember.service.api.ChatMemberService;
//import com.keepitup.magjob.chat.configuration.SecurityService;
import com.keepitup.magjob.chat.jwt.CustomJwt;
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
            chatMemberService.create(ChatMember.builder()
                    .chat(createdChat.get())
                    .nickname(postChatRequest.getNickname())
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
