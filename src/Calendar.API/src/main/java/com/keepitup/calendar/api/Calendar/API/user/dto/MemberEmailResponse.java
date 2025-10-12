package com.keepitup.calendar.api.Calendar.API.user.dto;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class MemberEmailResponse {
    @JsonProperty("email")
    private String email;
}