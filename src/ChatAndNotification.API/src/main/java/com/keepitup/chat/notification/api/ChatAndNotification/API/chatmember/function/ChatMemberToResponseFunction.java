package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.dto.GetChatMemberResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity.ChatMember;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class ChatMemberToResponseFunction implements Function<ChatMember, GetChatMemberResponse> {
    @Override
    public GetChatMemberResponse apply(ChatMember chatMember) {
        return GetChatMemberResponse.builder()
                .id(chatMember.getId())
                .nickname(chatMember.getNickname())
                .isInvitationAccepted(chatMember.getIsInvitationAccepted())
                .memberId(chatMember.getMemberId())
                .chat(GetChatMemberResponse.Chat.builder()
                        .id(chatMember.getChat().getId())
                        .title(chatMember.getChat().getTitle())
                        .build())
                .build();
    }
}
