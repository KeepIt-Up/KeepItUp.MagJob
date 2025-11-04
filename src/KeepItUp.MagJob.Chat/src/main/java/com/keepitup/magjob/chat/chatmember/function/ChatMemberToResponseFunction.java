package com.keepitup.magjob.chat.chatmember.function;

import com.keepitup.magjob.chat.chatmember.dto.GetChatMemberResponse;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class ChatMemberToResponseFunction implements Function<ChatMember, GetChatMemberResponse> {
    @Override
    public GetChatMemberResponse apply(ChatMember chatMember) {
        return GetChatMemberResponse.builder()
                .id(chatMember.getId())
                .nickname(chatMember.getNickname())
                .memberId(chatMember.getMemberId())
                .chat(GetChatMemberResponse.Chat.builder()
                        .id(chatMember.getChat().getId())
                        .title(chatMember.getChat().getTitle())
                        .build())
                .build();
    }
}
