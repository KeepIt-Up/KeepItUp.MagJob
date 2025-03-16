package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.dto.PatchChatMemberRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity.ChatMember;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class UpdateChatMemberWithRequestFunction implements BiFunction<ChatMember, PatchChatMemberRequest, ChatMember> {
    @Override
    public ChatMember apply(ChatMember chatMember, PatchChatMemberRequest patchChatMemberRequest) {
        return ChatMember.builder()
                .id(chatMember.getId())
                .nickname(patchChatMemberRequest.getNickname())
                .isInvitationAccepted(chatMember.getIsInvitationAccepted())
                .memberId(chatMember.getMemberId())
                .chat(chatMember.getChat())
                .build();
    }
}
