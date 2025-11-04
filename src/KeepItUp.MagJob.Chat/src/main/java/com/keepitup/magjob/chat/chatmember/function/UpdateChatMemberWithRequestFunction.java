package com.keepitup.magjob.chat.chatmember.function;

import com.keepitup.magjob.chat.chatmember.dto.PatchChatMemberRequest;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class UpdateChatMemberWithRequestFunction implements BiFunction<ChatMember, PatchChatMemberRequest, ChatMember> {
    @Override
    public ChatMember apply(ChatMember chatMember, PatchChatMemberRequest patchChatMemberRequest) {
        return ChatMember.builder()
                .id(chatMember.getId())
                .nickname(patchChatMemberRequest.getNickname())
                .memberId(chatMember.getMemberId())
                .chat(chatMember.getChat())
                .build();
    }
}
